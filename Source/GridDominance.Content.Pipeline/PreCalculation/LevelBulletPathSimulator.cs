using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using FarseerPhysics.Common;
using MonoSAMFramework.Portable.GameMath.Geometry;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;

namespace GridDominance.Content.Pipeline.PreCalculation
{
	internal static class LevelBulletPathSimulator
	{
		private const int RESOLUTION = 720;

		public static void Precalc(LevelBlueprint lvl)
		{
			foreach (var cannon in lvl.BlueprintCannons)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);

			foreach (var cannon in lvl.BlueprintLaserCannons)
				cannon.PrecalculatedPaths = LevelBulletPathTracer.Precalc(lvl, cannon);
		}

		public static BulletPathBlueprint[] Precalc(LevelBlueprint lvl, CannonBlueprint cannon)
		{
			var worldNormal = CreateRayWorld(lvl);

			var rayClock = new List<Tuple<BulletPathBlueprint, float>>[RESOLUTION];

			for (int ideg = 0; ideg < RESOLUTION; ideg++)
			{
				float deg = ideg * (360f / RESOLUTION);
				var rays = FindBulletPaths(lvl, worldNormal, cannon, deg);

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

		private static List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World world, CannonBlueprint cannon, float deg)
		{
			float startRadians = deg * FloatMath.DegRad;
			var scale = cannon.Diameter / Cannon.CANNON_DIAMETER;
			var spawnPoint = new Vector2(cannon.X, cannon.Y) + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(startRadians);
			var spawnVeloc = new Vector2(1, 0).Rotate(startRadians) * Cannon.BULLET_INITIAL_SPEED;

			var fbpresult = FindBulletPaths(lvl, world, cannon.CannonID, spawnPoint, spawnVeloc, new List<Vector2>(), scale, 0);

			var result = new List<Tuple<BulletPathBlueprint, float>>();
			foreach (var r in fbpresult)
			{
				var tgray = new BulletPathBlueprint(r.Item2.CannonID, startRadians, SimplifyRays(new Vector2(cannon.X, cannon.Y), r.Item1), r.Item1);
				result.Add(Tuple.Create(tgray, r.Item3));
			}

			return result;
		}

		private static List<Tuple<List<Vector2>, CannonBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World world, int sourceID, Vector2 spawnPoint, Vector2 spawnVeloc, List<Vector2> fullpath, float scale, float lifetime)
		{
			var none = new List<Tuple<List<Vector2>, CannonBlueprint, float>>();

			fullpath = fullpath.ToList();

			object collisionUserObject = null;
			Contact collisionContact = null;

			var farseerBullet = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(scale * Bullet.BULLET_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(spawnPoint), BodyType.Dynamic, null);
			farseerBullet.LinearVelocity = ConvertUnits.ToSimUnits(spawnVeloc);
			farseerBullet.CollidesWith = Category.All;
			farseerBullet.Restitution = 1f;              // Bouncability, 1=bounce always elastic
			farseerBullet.AngularDamping = 1f;           // Practically no angular rotation
			farseerBullet.Friction = 0f;
			farseerBullet.LinearDamping = 0f;            // no slowing down
			farseerBullet.OnCollision += (fa, fb, cobj) =>
			{
				collisionUserObject = fb.UserData;
				collisionContact    = cobj; 
				if (fb.UserData is GlassBlockBlueprint)   return true;
				if (fb.UserData is MirrorBlockBlueprint)  return true;
				if (fb.UserData is MirrorCircleBlueprint) return true;
				if (fb.UserData is MarkerCollisionBorder) return true;
				return false;
			};
			farseerBullet.AngularVelocity = 0;

			fullpath.Add(spawnPoint);

			for (;;)
			{
				collisionUserObject = null;

				foreach (var bh in lvl.BlueprintBlackHoles)
				{
					var pp = ConvertUnits.ToDisplayUnits(farseerBullet.Position) - new Vector2(bh.X, bh.Y);
					var force = bh.Power / pp.LengthSquared();
					var vecForce = pp.WithLength(force);
					farseerBullet.ApplyForce(ConvertUnits.ToSimUnits(vecForce));
				}

				world.Step(1 / 24f); // 24 UPS
				lifetime += 1 / 24f;
				fullpath.Add(ConvertUnits.ToDisplayUnits(farseerBullet.Position));

				if (collisionUserObject is PortalBlueprint)
				{
					var veloc = ConvertUnits.ToDisplayUnits(farseerBullet.LinearVelocity);

					world.RemoveBody(farseerBullet);
					var fPortal = (PortalBlueprint)collisionUserObject;

					Vector2 normal;
					FixedArray2<Vector2> t;
					collisionContact.GetWorldManifold(out normal, out t);

					bool hit = FloatMath.DiffRadiansAbs(normal.ToAngle(), FloatMath.ToRadians(fPortal.Normal)) < FloatMath.RAD_POS_001;

					if (!hit) return none;

					var dat = new List<Tuple<List<Vector2>, CannonBlueprint, float>>();
					foreach (var outportal in lvl.BlueprintPortals.Where(p => p.Side != fPortal.Side && p.Group == fPortal.Group))
					{
						var stretch = outportal.Length / fPortal.Length;

						var cIn = new Vector2(fPortal.X, fPortal.Y);
						var cOut = new Vector2(outportal.X, outportal.Y);
						var cInVecNormal  = Vector2.UnitX.RotateDeg(fPortal.Normal);
						var cInVecDirection = cInVecNormal.RotateWithLength(FloatMath.RAD_POS_090, fPortal.Length / 2f);
						var cOutVecNormal = Vector2.UnitX.RotateDeg(outportal.Normal);
						var cOutVecDirection = cOutVecNormal.RotateWithLength(FloatMath.RAD_POS_090, outportal.Length / 2f);

						var rot = FloatMath.ToRadians(outportal.Normal - fPortal.Normal) + FloatMath.RAD_POS_180;
						var projec = ConvertUnits.ToDisplayUnits(farseerBullet.Position).ProjectOntoLine(cIn, cInVecDirection);

						var newVelocity = ConvertUnits.ToDisplayUnits(farseerBullet.LinearVelocity).Rotate(rot);
						var newStart = cOut + cOutVecDirection * (-projec) + cOutVecNormal * (Portal.WIDTH / 2f) + newVelocity.WithLength(scale * Bullet.BULLET_DIAMETER / 2 + 4);

						var sub = FindBulletPaths(lvl, world, sourceID, newStart, newVelocity, fullpath, scale * stretch, lifetime);
						dat.AddRange(sub);

					}
					return dat;
				}

				if (collisionUserObject is CannonBlueprint)
				{
					world.RemoveBody(farseerBullet);
					var tgcannon = (CannonBlueprint)collisionUserObject;

					if (tgcannon.CannonID == sourceID) return none;

					var quality = FloatMath.LinePointDistance(ConvertUnits.ToDisplayUnits(farseerBullet.Position), ConvertUnits.ToDisplayUnits(farseerBullet.Position) + ConvertUnits.ToDisplayUnits(farseerBullet.LinearVelocity), new Vector2(tgcannon.X, tgcannon.Y));

					return new List<Tuple<List<Vector2>, CannonBlueprint, float>> { Tuple.Create(fullpath, tgcannon, quality) };
				}
				
				bool oow = (farseerBullet.Position.X < 0 - 64) || (farseerBullet.Position.Y < 0 - 64) || (farseerBullet.Position.X > ConvertUnits.ToSimUnits(lvl.LevelWidth) + 64) || (farseerBullet.Position.Y > ConvertUnits.ToSimUnits(lvl.LevelHeight) + 64);
				bool don = false;
				if (lvl.WrapMode == LevelBlueprint.WRAPMODE_DONUT) { oow = false; don = true; }
				bool ool = (lifetime >= Bullet.MAXIMUM_LIFETIME);

				//if (collisionUserObject != null || oow || ool)
				//{
				//	world.RemoveBody(farseerBullet);
				//	return new List<Tuple<List<Vector2>, CannonBlueprint, float>> { Tuple.Create(fullpath, new CannonBlueprint(farseerBullet.Position.X, farseerBullet.Position.Y, 64, 1, 0, FloatMath.GetRangedIntRandom(0, 9999999)), 1f) };
				//}

				if (collisionUserObject is VoidWallBlueprint)   { world.RemoveBody(farseerBullet); return none; }
				if (collisionUserObject is VoidCircleBlueprint) { world.RemoveBody(farseerBullet); return none; }
				if (collisionUserObject is BlackHoleBlueprint)  { world.RemoveBody(farseerBullet); return none; }
				
				if (oow || ool)                                 { world.RemoveBody(farseerBullet); return none; }

				if (don)
				{
					var nx = (farseerBullet.Position.X + ConvertUnits.ToSimUnits(lvl.LevelWidth)) % ConvertUnits.ToSimUnits(lvl.LevelWidth);
					var ny = (farseerBullet.Position.Y + ConvertUnits.ToSimUnits(lvl.LevelHeight)) % ConvertUnits.ToSimUnits(lvl.LevelHeight);
					farseerBullet.Position = new Vector2(nx, ny);
				}
			}
		}
		
		private static World CreateRayWorld(LevelBlueprint lvl)
		{
			var world = new World(Vector2.Zero);

			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDConstants.PHYSICS_CONVERSION_FACTOR);

			foreach (var elem in lvl.BlueprintCannons)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(ConvertUnits.ToSimUnits(elem.Diameter / 2f), 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintGlassBlocks)
			{
				if (elem.Width < 0.01f) throw new Exception("Invalid Physics");
				if (elem.Height < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(elem.Width), ConvertUnits.ToSimUnits(elem.Height), 1, Vector2.Zero, body, elem);
			}

			foreach (var elem in lvl.BlueprintVoidCircles)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(ConvertUnits.ToSimUnits(elem.Diameter / 2f), 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintVoidWalls)
			{
				if (elem.Length < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(elem.Length), ConvertUnits.ToSimUnits(VoidWallBlueprint.DEFAULT_WIDTH), 1, Vector2.Zero, body, elem);
				body.Rotation = FloatMath.DegRad * elem.Rotation;
			}

			foreach (var elem in lvl.BlueprintBlackHoles)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(ConvertUnits.ToSimUnits(elem.Diameter * 0.5f * 0.3f), 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintPortals)
			{
				if (elem.Length < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(elem.Length), ConvertUnits.ToSimUnits(PortalBlueprint.DEFAULT_WIDTH), 1, Vector2.Zero, body, elem);
				body.Rotation = FloatMath.DegRad * (elem.Normal+90);
			}

			if (lvl.WrapMode == LevelBlueprint.WRAPMODE_SOLID)
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
				var be = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(re.Center), 0, BodyType.Static);
				var bs = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rs.Center), 0, BodyType.Static);
				var bw = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rw.Center), 0, BodyType.Static);

				var fn = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rn.Width), ConvertUnits.ToSimUnits(rn.Height), 1, Vector2.Zero, bn, dn);
				var fe = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(re.Width), ConvertUnits.ToSimUnits(re.Height), 1, Vector2.Zero, be, de);
				var fs = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rs.Width), ConvertUnits.ToSimUnits(rs.Height), 1, Vector2.Zero, bs, ds);
				var fw = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rw.Width), ConvertUnits.ToSimUnits(rw.Height), 1, Vector2.Zero, bw, dw);
			}

			return world;
		}

		private static Tuple<Vector2, Vector2>[] SimplifyRays(Vector2 cannonpos, List<Vector2> fullpath)
		{
			//return fullpath.Skip(1).Select(o => Tuple.Create(o.X, o.Y)).ToArray();

			List<Tuple<Vector2, Vector2>> outl = new List<Tuple<Vector2, Vector2>>();

			Vector2 vStart = cannonpos;
			int start = 0;
			for (int i = 2; i < fullpath.Count; i++)
			{
				if (IsSplit(fullpath[i - 1], fullpath[i]))
				{
					outl.Add(Tuple.Create(vStart, fullpath[i - 1]));
					start = i;
					vStart = fullpath[i];
					continue;
				}

				if (CanSimplify(vStart, fullpath[i], fullpath.Skip(start+1).Take(i - start - 1))) continue;

				outl.Add(Tuple.Create(vStart, fullpath[i - 1]));
				start = i - 1;
				vStart = fullpath[i - 1];
				i++;
			}
			outl.Add(Tuple.Create(vStart, fullpath.Last()));
			
			return outl.ToArray();
		}

		private static bool CanSimplify(Vector2 p1, Vector2 p2, IEnumerable<Vector2> test)
		{
			foreach (var elem in test)
			{
				if (FloatMath.LinePointDistance(p1, p2, elem) > Bullet.BULLET_DIAMETER / 3f) return false;
			}

			return true;
		}

		private static bool IsSplit(Vector2 p1, Vector2 p2)
		{
			return (p2 - p1).LengthSquared() > 16f * 16f;
		}
	}
}
