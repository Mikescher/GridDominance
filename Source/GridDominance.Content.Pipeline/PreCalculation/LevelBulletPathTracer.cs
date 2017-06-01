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

namespace GridDominance.Content.Pipeline.PreCalculation
{
	internal static class LevelBulletPathTracer
	{
		public const int MAX_COUNT_RECAST = 8;

		private const int RESOLUTION = 3600;

		private const float HITBOX_ENLARGE = 32;

		public static void Precalc(LevelBlueprint lvl)
		{
			foreach (var cannon in lvl.BlueprintCannons) cannon.PrecalculatedPaths = Precalc(lvl, cannon);
		}

		private static BulletPathBlueprint[] Precalc(LevelBlueprint lvl, CannonBlueprint cannon)
		{
			List<BulletPathBlueprint> resultRays = new List<BulletPathBlueprint>();

			BulletPathBlueprint bestRay = null;
			float bestQuality = float.MaxValue;

			var worldNormal = CreateRayWorld(lvl, 0, 1);
			var worldExtend = CreateRayWorld(lvl, HITBOX_ENLARGE, 1.5f);

			bool rayAtStart = true;
			float rayAtStartQuality = float.MaxValue;
			for (int ideg = 0; ideg < RESOLUTION; ideg ++)
			{
				float deg = ideg * (360f / RESOLUTION);

				var rays = FindBulletPaths(lvl, worldNormal, worldExtend, cannon, deg);

				if (ideg == 0) rayAtStart = (rays.Any());

				if (rays.Count == 0)
				{
					if (bestRay != null)
					{
						if (!resultRays.Any()) rayAtStartQuality = bestQuality;
						resultRays.Add(bestRay);
						bestRay = null;
						bestQuality = float.MaxValue;
					}
				}
				else
				{
					foreach (var ray in rays)
					{
						if (bestRay == null)
						{
							bestRay = ray.Item1;
							bestQuality = ray.Item2;
						}
						else if (bestRay.TargetCannonID != ray.Item1.TargetCannonID)
						{
							if (!resultRays.Any()) rayAtStartQuality = bestQuality;
							resultRays.Add(bestRay);
							bestRay = ray.Item1;
							bestQuality = ray.Item2;
						}
						else if (bestQuality > ray.Item2)
						{
							bestRay = ray.Item1;
							bestQuality = ray.Item2;
						}
					}
				}
			}

			if (bestRay != null)
			{
				if (resultRays.Any() && rayAtStart && resultRays.First().TargetCannonID == bestRay.TargetCannonID)
				{
					if (rayAtStartQuality > bestQuality)
					{
						resultRays.RemoveAt(0);
						resultRays.Add(bestRay);
					}
					else
					{
						// keep first
					}
				}
				else
				{
					resultRays.Add(bestRay);
				}
			}

			return resultRays.ToArray();
		}

		private static List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World wBase, World wCollision, CannonBlueprint cannon, float deg)
		{
			return FindBulletPaths(lvl, wBase, wCollision, new Vector2(cannon.X, cannon.Y), new List<Tuple<Vector2, Vector2>>(), deg * FloatMath.DegRad, MAX_COUNT_RECAST);
		}

		private static List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World wBase, World wCollision, Vector2 rcStart, List<Tuple<Vector2, Vector2>> sourcerays, float startRadians, int remainingRecasts)
		{
			var none = new List<Tuple<BulletPathBlueprint, float>>();
			if (remainingRecasts <= 0) return none;

			var rays = sourcerays.ToList();
			
			var rcEnd = rcStart + new Vector2(2048, 0).Rotate(startRadians);

			var traceResult = RayCast(wBase, rcStart, rcEnd);
			var traceResult2 = RayCast(wCollision, rcStart, rcEnd);

			if (traceResult2 != null && traceResult != null && traceResult2.Item1.UserData != traceResult.Item1.UserData)
			{
				// Dirty hit
				return none;
			}

			if (traceResult == null)
			{
				return none;
			}

			var fCannon = traceResult.Item1.UserData as CannonBlueprint;
			if (fCannon != null)
			{
				var quality = FloatMath.LinePointDistance(rcStart, traceResult.Item2, new Vector2(fCannon.X, fCannon.Y));
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));
				var path = new BulletPathBlueprint(fCannon.CannonID, startRadians, rays.ToArray());
				return new List<Tuple<BulletPathBlueprint, float>> { Tuple.Create(path, quality) };
			}

			var fGlassBlock = traceResult.Item1.UserData as GlassBlockBlueprint;
			if (fGlassBlock != null)
			{
				rays.Add(Tuple.Create(rcStart, traceResult.Item2));

				var pNewStart = traceResult.Item2;
				var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);

				return FindBulletPaths(lvl, wBase, wCollision, pNewStart, rays, pVec.ToAngle(), remainingRecasts - 1);
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

					newStart = Mirror(newStart, cOut, Vector2.UnitX.Rotate(FloatMath.ToRadians(outportal.Normal)));


					var sub = FindBulletPaths(lvl, wBase, wCollision, newStart, rays, newAngle, remainingRecasts - 1);
					dat.AddRange(sub);
				}
				return dat;
			}

			throw new Exception("Unknown rayTrace resturn ficture: " + traceResult.Item1.UserData);
		}

		private static Tuple<Fixture, Vector2, Vector2> RayCast(World w, Vector2 start, Vector2 end)
		{
			Tuple<Fixture, Vector2, Vector2> result = null;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
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

				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Width + extend, elem.Height + extend, 1, Vector2.Zero, body, elem);
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

			return world;
		}

		private static Vector2 Mirror(Vector2 p, Vector2 center, Vector2 normal)
		{
			//https://stackoverflow.com/a/6177788/1761622

			float x1 = center.X - normal.Y;
			float y1 = center.Y + normal.X;

			float x2 = center.X + normal.Y;
			float y2 = center.Y - normal.X;

			float x3 = p.X;
			float y3 = p.Y;

			float u = ((x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			float xu = x1 + u * (x2 - x1);
			float yu = y1 + u * (y2 - y1);
			
			float dx = xu - p.X;
			float dy = yu - p.Y;

			float rx = p.X + 2 * dx;
			float ry = p.Y + 2 * dy;

			return new Vector2(rx, ry);
		}
	}
}
