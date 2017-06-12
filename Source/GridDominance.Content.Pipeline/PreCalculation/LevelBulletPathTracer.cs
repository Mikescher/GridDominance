using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath.Geometry;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;

namespace GridDominance.Content.Pipeline.PreCalculation
{
	internal static class LevelBulletPathTracer
	{
		public const int MAX_COUNT_RECAST  = 8;
		public const int MAX_COUNT_REFLECT = 16;

		private const int RESOLUTION = 3600;

		private const float HITBOX_ENLARGE = 32;

		private static object objRefract = new object();

		public static void Precalc(LevelBlueprint lvl)
		{
			foreach (var cannon in lvl.BlueprintCannons)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);

			foreach (var cannon in lvl.BlueprintLaserCannons)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);
		}

		public static BulletPathBlueprint[] Precalc(LevelBlueprint lvl, CannonBlueprint cannon)
		{
			var worldNormal = CreateRayWorld(lvl, 0, 1);
			var worldExtend = CreateRayWorld(lvl, HITBOX_ENLARGE, 1.5f);

			var rayClock = new List<Tuple<BulletPathBlueprint, float>>[RESOLUTION];

			for (int ideg = 0; ideg < RESOLUTION; ideg++)
			{
				float deg = ideg * (360f / RESOLUTION);
				var rays = FindBulletPaths(lvl, worldNormal, worldExtend, cannon, deg);

				rayClock[ideg] = rays;
			}

			List<BulletPathBlueprint> resultRays = new List<BulletPathBlueprint>();
			for (;;)
			{
				for (int ideg = 0; ideg < RESOLUTION; ideg++)
				{
					if (rayClock[ideg].Any()) resultRays.Add(ExtractBestRay(rayClock, ideg, rayClock[ideg].First().Item1.TargetCannonID));
				}
				break;
			}

			return resultRays.ToArray();
		}

		public static BulletPathBlueprint[] Precalc(LevelBlueprint lvl, LaserCannonBlueprint cannon)
		{
			var worldNormal = CreateRayWorld(lvl, 0, 1);
			var worldExtend = CreateRayWorld(lvl, HITBOX_ENLARGE, 1.5f);

			var rayClock = new List<Tuple<BulletPathBlueprint, float>>[RESOLUTION];

			for (int ideg = 0; ideg < RESOLUTION; ideg++)
			{
				float deg = ideg * (360f / RESOLUTION);
				var rays = FindLaserPaths(lvl, worldNormal, worldExtend, cannon, deg);

				rayClock[ideg] = rays;
			}

			List<BulletPathBlueprint> resultRays = new List<BulletPathBlueprint>();
			for (;;)
			{
				for (int ideg = 0; ideg < RESOLUTION; ideg++)
				{
					if (rayClock[ideg].Any()) resultRays.Add(ExtractBestRay(rayClock, ideg, rayClock[ideg].First().Item1.TargetCannonID));
				}
				break;
			}

			return resultRays.ToArray();
		}

		private static BulletPathBlueprint ExtractBestRay(List<Tuple<BulletPathBlueprint, float>>[] rayClock, int iStart, int cid)
		{
			float bestQuality = rayClock[iStart].First(p => p.Item1.TargetCannonID == cid).Item2;
			BulletPathBlueprint bestRay = rayClock[iStart].First(p => p.Item1.TargetCannonID == cid).Item1;
			
			for (int delta = 0; delta < RESOLUTION; delta++)
			{
				var ideg = (iStart + delta + RESOLUTION) % RESOLUTION;

				var clockrays = rayClock[ideg].Where(p => p.Item1.TargetCannonID == cid).ToList();
				if (!clockrays.Any()) break;

				foreach (var ray in clockrays)
				{
					if (ray.Item2 < bestQuality) { bestQuality = ray.Item2; bestRay = ray.Item1; }
					rayClock[ideg].Remove(ray);
				}
			}

			for (int delta = 1; delta < RESOLUTION; delta++)
			{
				var ideg = (iStart - delta + RESOLUTION) % RESOLUTION;

				var clockrays = rayClock[ideg].Where(p => p.Item1.TargetCannonID == cid).ToList();
				if (!clockrays.Any()) break;

				foreach (var ray in clockrays)
				{
					if (ray.Item2 < bestQuality) { bestQuality = ray.Item2; bestRay = ray.Item1; }
					rayClock[ideg].Remove(ray);
				}
			}

			return bestRay;
		}

		private static List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World wBase, World wCollision, CannonBlueprint cannon, float deg)
		{
			return FindBulletPaths(lvl, wBase, wCollision, cannon.CannonID, new Vector2(cannon.X, cannon.Y), new List<Tuple<Vector2, Vector2>>(), deg * FloatMath.DegRad, deg * FloatMath.DegRad, MAX_COUNT_RECAST);
		}

		private static List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World wBase, World wCollision, int sourceID, Vector2 rcStart, List<Tuple<Vector2, Vector2>> sourcerays, float startRadians, float cannonRadians, int remainingRecasts)
		{
			var none = new List<Tuple<BulletPathBlueprint, float>>();
			if (remainingRecasts <= 0) return none;

			var rays = sourcerays.ToList();
			
			var rcEnd = rcStart + new Vector2(2048, 0).Rotate(startRadians);

			var traceResult  = RayCastBullet(wBase, rcStart, rcEnd);
			var traceResult2 = RayCastBullet(wCollision, rcStart, rcEnd);

			if (traceResult2 != null && traceResult != null && traceResult2.Item1.UserData != traceResult.Item1.UserData)
			{
				// Dirty hit
				return none;
			}

			if (traceResult == null)
			{
				return none;
			}

			var fCannon = traceResult.Item1.UserData as ICannonBlueprint;
			if (fCannon != null)
			{
				if (fCannon.CannonID == sourceID) return none;

				var quality = FloatMath.LinePointDistance(rcStart, traceResult.Item2, new Vector2(fCannon.X, fCannon.Y));
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));
				var path = new BulletPathBlueprint(fCannon.CannonID, cannonRadians, rays.ToArray());
				return new List<Tuple<BulletPathBlueprint, float>> { Tuple.Create(path, quality) };
			}

			var fGlassBlock = traceResult.Item1.UserData as GlassBlockBlueprint;
			if (fGlassBlock != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;
				var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

				return FindBulletPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1);
			}

			var fMirrorBlock = traceResult.Item1.UserData as MirrorBlockBlueprint;
			if (fMirrorBlock != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;
				var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

				return FindBulletPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1);
			}

			var fMirrorCircle = traceResult.Item1.UserData as MirrorCircleBlueprint;
			if (fMirrorCircle != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;
				var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

				return FindBulletPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1);
			}

			var fVoidWall = traceResult.Item1.UserData as VoidWallBlueprint;
			if (fVoidWall != null)
			{
				return none;
			}

			var fVoidCircle = traceResult.Item1.UserData as VoidCircleBlueprint;
			if (fVoidCircle != null)
			{
				return none;
			}

			var fBlackhole = traceResult.Item1.UserData as BlackHoleBlueprint;
			if (fBlackhole != null)
			{
				return none; // Black holes are _not_ correctly calculated in this preprocessor
			}

			var fPortal = traceResult.Item1.UserData as PortalBlueprint;
			if (fPortal != null)
			{
				bool hit = FloatMath.DiffRadiansAbs(traceResult.Item3.ToAngle(), FloatMath.ToRadians(fPortal.Normal)) < FloatMath.RAD_POS_005;

				if (!hit) return none;

				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var dat = new List<Tuple<BulletPathBlueprint, float>>();
				foreach (var outportal in lvl.BlueprintPortals.Where(p => p.Side != fPortal.Side && p.Group == fPortal.Group))
				{
					var cIn  = new Vector2(fPortal.X, fPortal.Y);
					var cOut = new Vector2(outportal.X, outportal.Y);

					var rot = FloatMath.ToRadians(outportal.Normal - fPortal.Normal + 180);
					var stretch = outportal.Length / fPortal.Length;

					var newAngle = FloatMath.NormalizeAngle(startRadians + rot);
					var newStart = cOut + stretch * (traceResult.Item2 - cIn).Rotate(rot);

					newStart = newStart.MirrorAtNormal(cOut, Vector2.UnitX.Rotate(FloatMath.ToRadians(outportal.Normal)));

					var sub = FindBulletPaths(lvl, wBase, wCollision, sourceID, newStart, rays, newAngle, cannonRadians, remainingRecasts - 1);
					dat.AddRange(sub);
				}
				return dat;
			}

			throw new Exception("Unknown rayTrace resturn ficture: " + traceResult.Item1.UserData);
		}

		private static List<Tuple<BulletPathBlueprint, float>> FindLaserPaths(LevelBlueprint lvl, World wBase, World wCollision, LaserCannonBlueprint cannon, float deg)
		{
			return FindLaserPaths(lvl, wBase, wCollision, cannon.CannonID, new Vector2(cannon.X, cannon.Y), new List<Tuple<Vector2, Vector2>>(), deg * FloatMath.DegRad, deg * FloatMath.DegRad, MAX_COUNT_REFLECT, false);
		}

		private static List<Tuple<BulletPathBlueprint, float>> FindLaserPaths(LevelBlueprint lvl, World wBase, World wCollision, int sourceID, Vector2 rcStart, List<Tuple<Vector2, Vector2>> sourcerays, float startRadians, float cannonRadians, int remainingRecasts, bool inGlassBlock)
		{
			var none = new List<Tuple<BulletPathBlueprint, float>>();
			if (remainingRecasts <= 0) return none;

			var rays = sourcerays.ToList();

			var rcEnd = rcStart + new Vector2(2048, 0).Rotate(startRadians);

			var traceResult  = RayCastLaser(wBase, rcStart, rcEnd);
			var traceResult2 = RayCastLaser(wCollision, rcStart, rcEnd);

			if (traceResult2 != null && traceResult != null && traceResult2.Item1.UserData != traceResult.Item1.UserData)
			{
				// Dirty hit
				return none;
			}

			if (traceResult == null)
			{
				return none;
			}

			var fCannon = traceResult.Item1.UserData as ICannonBlueprint;
			if (fCannon != null)
			{
				if (fCannon.CannonID == sourceID) return none;

				var quality = FloatMath.LinePointDistance(rcStart, traceResult.Item2, new Vector2(fCannon.X, fCannon.Y));
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));
				var path = new BulletPathBlueprint(fCannon.CannonID, cannonRadians, rays.ToArray());
				return new List<Tuple<BulletPathBlueprint, float>> { Tuple.Create(path, quality) };
			}

			var fGlassBlock = traceResult.Item1.UserData as GlassBlockBlueprint;
			if (fGlassBlock != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;

				var normal = traceResult.Item3;
				var aIn = (rcEnd - rcStart).ToAngle() - normal.ToAngle();

				// sin(aIn) / sin(aOut) = currRefractIdx / Glass.RefractIdx

				var n = inGlassBlock ? (GlassBlockBlueprint.REFRACTION_INDEX / 1f) : (1f / GlassBlockBlueprint.REFRACTION_INDEX);

				var sinaOut = FloatMath.Sin(aIn) / n;

				var dat = new List<Tuple<BulletPathBlueprint, float>>();

				if (sinaOut < 1 && sinaOut > -1)
				{
					// refraction

					var aOut = FloatMath.Asin(sinaOut);

					var pRefractAngle = normal.ToAngle() + aOut;

					var sub = FindLaserPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pRefractAngle, cannonRadians, remainingRecasts - 1, !inGlassBlock);
					dat.AddRange(sub);
				}

				if (! inGlassBlock)
				{
					// reflection

					var pReflectVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

					var sub = FindLaserPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pReflectVec.ToAngle(), cannonRadians, remainingRecasts - 1, !inGlassBlock);
					dat.AddRange(sub);
				}

				return dat;
			}

			var fMirrorBlock = traceResult.Item1.UserData as MirrorBlockBlueprint;
			if (fMirrorBlock != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;
				var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

				return FindLaserPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1, inGlassBlock);
			}

			var fMirrorCircle = traceResult.Item1.UserData as MirrorCircleBlueprint;
			if (fMirrorCircle != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;
				var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

				return FindLaserPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1, inGlassBlock);
			}

			var fVoidWall = traceResult.Item1.UserData as VoidWallBlueprint;
			if (fVoidWall != null)
			{
				return none;
			}

			var fVoidCircle = traceResult.Item1.UserData as VoidCircleBlueprint;
			if (fVoidCircle != null)
			{
				return none;
			}

			var fPortal = traceResult.Item1.UserData as PortalBlueprint;
			if (fPortal != null)
			{
				bool hit = FloatMath.DiffRadiansAbs(traceResult.Item3.ToAngle(), FloatMath.ToRadians(fPortal.Normal)) < FloatMath.RAD_POS_005;

				if (!hit) return none;

				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var dat = new List<Tuple<BulletPathBlueprint, float>>();
				foreach (var outportal in lvl.BlueprintPortals.Where(p => p.Side != fPortal.Side && p.Group == fPortal.Group))
				{
					var cIn = new Vector2(fPortal.X, fPortal.Y);
					var cOut = new Vector2(outportal.X, outportal.Y);

					var rot = FloatMath.ToRadians(outportal.Normal - fPortal.Normal + 180);
					var stretch = outportal.Length / fPortal.Length;

					var newAngle = FloatMath.NormalizeAngle(startRadians + rot);
					var newStart = cOut + stretch * (traceResult.Item2 - cIn).Rotate(rot);

					newStart = newStart.MirrorAtNormal(cOut, Vector2.UnitX.Rotate(FloatMath.ToRadians(outportal.Normal)));

					var sub = FindLaserPaths(lvl, wBase, wCollision, sourceID, newStart, rays, newAngle, cannonRadians, remainingRecasts - 1, inGlassBlock);
					dat.AddRange(sub);
				}
				return dat;
			}

			var fBorder = traceResult.Item1.UserData as MarkerCollisionBorder;
			if (fBorder != null)
			{
				if (lvl.WrapMode == LevelBlueprint.WRAPMODE_DONUT)
				{
					rays.Add(Tuple.Create(rcStart, traceResult.Item2));

					var pNewStartX = traceResult.Item2.X;
					var pNewStartY = traceResult.Item2.Y;
					switch (fBorder.Side)
					{
						case FlatAlign4.NN: pNewStartY += lvl.LevelHeight; break;
						case FlatAlign4.EE: pNewStartX -= lvl.LevelWidth; break;
						case FlatAlign4.SS: pNewStartY -= lvl.LevelHeight; break;
						case FlatAlign4.WW: pNewStartX += lvl.LevelWidth; break;
					}
					var pVec = rcEnd - rcStart;
					var pNewStart = new Vector2(pNewStartX, pNewStartY);

					return FindLaserPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1, inGlassBlock);
				}
				else if (lvl.WrapMode == LevelBlueprint.WRAPMODE_SOLID)
				{
					rays.Add(Tuple.Create(rcStart, traceResult.Item2));

					var pNewStart = traceResult.Item2;
					var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

					return FindLaserPaths(lvl, wBase, wCollision, sourceID, pNewStart, rays, pVec.ToAngle(), cannonRadians, remainingRecasts - 1, inGlassBlock);
				}
				throw new Exception("Unsupported WrapMode: " + lvl.WrapMode);
			}

			throw new Exception("Unknown rayTrace resturn ficture: " + traceResult.Item1.UserData);
		}

		private static Tuple<Fixture, Vector2, Vector2> RayCastBullet(World w, Vector2 start, Vector2 end)
		{
			Tuple<Fixture, Vector2, Vector2> result = null;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == objRefract) return -1; // ignore

				result = Tuple.Create(f, pos, normal);

				return frac; // limit
			};

			w.RayCast(callback, start, end);

			return result;
		}

		private static Tuple<Fixture, Vector2, Vector2> RayCastLaser(World w, Vector2 start, Vector2 end)
		{
			Tuple<Fixture, Vector2, Vector2> result = null;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData is GlassBlockBlueprint) return -1; // ignore
				if (f.UserData is BlackHoleBlueprint) return -1; // ignore

				result = Tuple.Create(f, pos, normal);

				return frac; // limit
			};

			w.RayCast(callback, start, end);

			return result;
		}

		private static World CreateRayWorld(LevelBlueprint lvl, float extend, float cannonEnlarge)
		{
			var world = new World(Vector2.Zero);

			ConvertUnits.SetDisplayUnitToSimUnitRatio(1);

			foreach (var elem in lvl.BlueprintCannons)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(cannonEnlarge * elem.Diameter / 2f + extend, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintGlassBlocks)
			{
				if (elem.Width < 0.01f) throw new Exception("Invalid Physics");
				if (elem.Height < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), FloatMath.ToRadians(elem.Rotation), BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Width + extend, elem.Height + extend, 1, Vector2.Zero, body, elem);


				var bodyN = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), FloatMath.ToRadians(elem.Rotation), BodyType.Static, objRefract);
				FixtureFactory.AttachRectangle(elem.Width, 0.1f, 1, new Vector2(+elem.Height / 2f, 0), bodyN, objRefract);

				var bodyE = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), FloatMath.ToRadians(elem.Rotation), BodyType.Static, objRefract);
				FixtureFactory.AttachRectangle(0.1f, elem.Height, 1, new Vector2(+elem.Width / 2f, 0), bodyE, objRefract);

				var bodyS = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), FloatMath.ToRadians(elem.Rotation), BodyType.Static, objRefract);
				FixtureFactory.AttachRectangle(elem.Width, 0.1f, 1, new Vector2(-elem.Height / 2f, 0), bodyS, objRefract);

				var bodyW = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), FloatMath.ToRadians(elem.Rotation), BodyType.Static, objRefract);
				FixtureFactory.AttachRectangle(0.1f, elem.Height, 1, new Vector2(-elem.Width / 2f, 0), bodyW, objRefract);
			}

			foreach (var elem in lvl.BlueprintVoidCircles)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(elem.Diameter / 2f + extend, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintVoidWalls)
			{
				if (elem.Length < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Length + extend, VoidWallBlueprint.DEFAULT_WIDTH + extend, 1, Vector2.Zero, body, elem);
				body.Rotation = FloatMath.DegRad * elem.Rotation;
			}

			foreach (var elem in lvl.BlueprintBlackHoles)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(elem.Diameter * 0.5f * 0.3f, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintPortals)
			{
				if (elem.Length < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Length + extend, PortalBlueprint.DEFAULT_WIDTH + extend, 1, Vector2.Zero, body, elem);
				body.Rotation = FloatMath.DegRad * (elem.Normal + 90);
			}

			foreach (var elem in lvl.BlueprintMirrorBlocks)
			{
				if (elem.Width < 0.01f) throw new Exception("Invalid Physics");
				if (elem.Height < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), FloatMath.ToRadians(elem.Rotation), BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Width + extend, elem.Height + extend, 1, Vector2.Zero, body, elem);
			}

			foreach (var elem in lvl.BlueprintMirrorCircles)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(elem.Diameter / 2f + extend, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintLaserCannons)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(cannonEnlarge * elem.Diameter / 2f + extend, 1, body, Vector2.Zero, elem);
			}

			if (lvl.WrapMode == LevelBlueprint.WRAPMODE_SOLID || lvl.WrapMode == LevelBlueprint.WRAPMODE_DONUT)
			{
				var mw = lvl.LevelWidth;
				var mh = lvl.LevelHeight;
				var ex = 2 * GDConstants.TILE_WIDTH;

				var rn = new FRectangle(-ex, -ex, mw + 2 * ex, ex);
				var re = new FRectangle(+mw, -ex, ex, mh + 2 * ex);
				var rs = new FRectangle(-ex, +mh, mw + 2 * ex, ex);
				var rw = new FRectangle(-ex, -ex, ex, mh + 2 * ex);

				var dn = new MarkerCollisionBorder { Side = FlatAlign4.NN };
				var de = new MarkerCollisionBorder { Side = FlatAlign4.EE };
				var ds = new MarkerCollisionBorder { Side = FlatAlign4.SS };
				var dw = new MarkerCollisionBorder { Side = FlatAlign4.WW };

				var bn = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rn.Center), 0, BodyType.Static);
				var be = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rn.Center), 0, BodyType.Static);
				var bs = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rn.Center), 0, BodyType.Static);
				var bw = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rn.Center), 0, BodyType.Static);

				var fn = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rn.Width), ConvertUnits.ToSimUnits(rn.Height), 1, Vector2.Zero, bn, dn);
				var fe = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(re.Width), ConvertUnits.ToSimUnits(re.Height), 1, Vector2.Zero, be, de);
				var fs = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rs.Width), ConvertUnits.ToSimUnits(rs.Height), 1, Vector2.Zero, bs, ds);
				var fw = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rw.Width), ConvertUnits.ToSimUnits(rw.Height), 1, Vector2.Zero, bw, dw);
			}

			return world;
		}
	}
}
