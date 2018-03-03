using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
	internal class LevelKISimulator
	{
		private int RESOLUTION = 720;
		private float TRUSTANGLE = 0;
		private float SIMULATION_UPS = 24;
		private float LIFETIME_FAC = 0.75f;

		public void Precalc(LevelBlueprint lvl)
		{
			lvl.GetConfig(LevelBlueprint.KI_CONFIG_SIMULATION_RESOLUTION, ref RESOLUTION);
			lvl.GetConfig(LevelBlueprint.KI_CONFIG_SIMULATION_SCATTERTRUST, ref TRUSTANGLE);
			lvl.GetConfig(LevelBlueprint.KI_CONFIG_SIMULATION_UPS, ref SIMULATION_UPS);
			lvl.GetConfig(LevelBlueprint.KI_CONFIG_SIMULATION_LIFETIME_FAC, ref LIFETIME_FAC);

			foreach (var cannon in lvl.BlueprintCannons)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);

			foreach (var cannon in lvl.BlueprintRelayCannon)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);

			foreach (var cannon in lvl.BlueprintMinigun)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);

			foreach (var cannon in lvl.BlueprintTrishotCannon)
				cannon.PrecalculatedPaths = Precalc(lvl, cannon);

			foreach (var cannon in lvl.BlueprintLaserCannons)
				cannon.PrecalculatedPaths = new LevelKITracer().PrecalcLaser(lvl, cannon);

			foreach (var cannon in lvl.BlueprintShieldProjector)
				cannon.PrecalculatedPaths = new LevelKITracer().PrecalcLaser(lvl, cannon);
		}

		private BulletPathBlueprint[] Precalc(LevelBlueprint lvl, ICannonBlueprint cannon)
		{
			var worldNormal = CreateRayWorld(lvl);

			var rayClock = new List<Tuple<BulletPathBlueprint, float>>[RESOLUTION];

			for (int ideg = 0; ideg < RESOLUTION; ideg++)
			{
				float deg = ideg * (360f / RESOLUTION);
				var rays = FindBulletPaths(lvl, worldNormal, cannon, deg);

				rayClock[ideg] = rays;
			}

			RemoveUntrusted(rayClock);
			
			List<BulletPathBlueprint> resultRays = new List<BulletPathBlueprint>();
			for (;;)
			{
				for (int ideg = 0; ideg < RESOLUTION; ideg++)
				{
					if (rayClock[ideg].Any()) resultRays.Add(ExtractBestRay(rayClock, ideg, rayClock[ideg].First().Item1.TargetCannonID));
				}
				break;
			}

			for (int i = 0; i < resultRays.Count; i++) resultRays[i] = resultRays[i].AsCleaned();

			return resultRays.ToArray();
		}

		private void RemoveUntrusted(List<Tuple<BulletPathBlueprint, float>>[] rayClock)
		{
			List<Tuple<int, Tuple<BulletPathBlueprint, float>>> rems = new List<Tuple<int, Tuple<BulletPathBlueprint, float>>>(); 
			
			for (int ideg = 0; ideg < RESOLUTION; ideg++)
			{
				var ddeg = (ideg * 1f / RESOLUTION) * 360;
				
				foreach (var ray in rayClock[ideg])
				{
					bool fault = false;
					for (int ideg2 = 0; ideg2 < RESOLUTION; ideg2++)
					{
						var ddeg2 = (ideg2 * 1f / RESOLUTION) * 360;

						if (ideg2 == ideg || FloatMath.Abs(FloatMath.DiffModulo(ddeg, ddeg2, 360)) > TRUSTANGLE) continue;

						if (rayClock[ideg2].All(r => r.Item1.TargetCannonID != ray.Item1.TargetCannonID)) fault = true;
					}
					if (fault) rems.Add(Tuple.Create(ideg, ray));
				}
			}

			foreach (var rem in rems)
			{
				rayClock[rem.Item1].Remove(rem.Item2);
			}
		}

		private BulletPathBlueprint ExtractBestRay(List<Tuple<BulletPathBlueprint, float>>[] rayClock, int iStart, int cid)
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

		private List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World world, ICannonBlueprint cannon, float deg)
		{
			float startRadians = deg * FloatMath.DegRad;
			var scale = cannon.Diameter / Cannon.CANNON_DIAMETER;
			var spawnPoint = new FPoint(cannon.X, cannon.Y) + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(startRadians);
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

		private List<Tuple<List<Vector2>, ICannonBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World world, int sourceID, FPoint spawnPoint, Vector2 spawnVeloc, List<Vector2> fullpath, float scale, float lifetime)
		{
			var none = new List<Tuple<List<Vector2>, ICannonBlueprint, float>>(); // path, cannon, quality

			fullpath = fullpath.ToList();

			object collisionUserObject = null;
			Contact collisionContact = null;

			var farseerBullet = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(scale * Bullet.BULLET_DIAMETER / 2), 1, ConvertUnits2.ToSimUnits(spawnPoint), BodyType.Dynamic, null);
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

			fullpath.Add(spawnPoint.ToVec2D());

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

				world.Step(1 / SIMULATION_UPS); // x UPS
				lifetime += 1 / SIMULATION_UPS;
				fullpath.Add(ConvertUnits2.ToDisplayUnitsPoint(farseerBullet.Position).ToVec2D());

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

					var dat = new List<Tuple<List<Vector2>, ICannonBlueprint, float>>();
					foreach (var outportal in lvl.BlueprintPortals.Where(p => p.Side != fPortal.Side && p.Group == fPortal.Group))
					{
						var stretch = outportal.Length / fPortal.Length;

						var cIn = new FPoint(fPortal.X, fPortal.Y);
						var cOut = new FPoint(outportal.X, outportal.Y);
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

				if (collisionUserObject is ICannonBlueprint)
				{
					world.RemoveBody(farseerBullet);
					var tgcannon = (ICannonBlueprint)collisionUserObject;

					if (tgcannon.CannonID == sourceID) return none;

					var quality = Math2D.LinePointDistance(ConvertUnits2.ToDisplayUnitsPoint(farseerBullet.Position), ConvertUnits2.ToDisplayUnitsPoint(farseerBullet.Position) + ConvertUnits.ToDisplayUnits(farseerBullet.LinearVelocity), new FPoint(tgcannon.X, tgcannon.Y));

					return new List<Tuple<List<Vector2>, ICannonBlueprint, float>> { Tuple.Create(fullpath, tgcannon, quality) };
				}


				bool oow = (farseerBullet.Position.X < 0 - 64) || (farseerBullet.Position.Y < 0 - 64) || (farseerBullet.Position.X > ConvertUnits.ToSimUnits(lvl.LevelWidth) + 64) || (farseerBullet.Position.Y > ConvertUnits.ToSimUnits(lvl.LevelHeight) + 64);
				bool ool = (lifetime >= Bullet.MAXIMUM_LIFETIME * LIFETIME_FAC);

				//if (collisionUserObject != null || oow || ool)
				//{
				//	world.RemoveBody(farseerBullet);
				//	return new List<Tuple<List<Vector2>, CannonBlueprint, float>> { Tuple.Create(fullpath, new CannonBlueprint(farseerBullet.Position.X, farseerBullet.Position.Y, 64, 1, 0, FloatMath.GetRangedIntRandom(0, 9999999)), 1f) };
				//}

				if (collisionUserObject is VoidWallBlueprint)   { world.RemoveBody(farseerBullet); return none; }
				if (collisionUserObject is VoidCircleBlueprint) { world.RemoveBody(farseerBullet); return none; }
				if (collisionUserObject is BlackHoleBlueprint)  { world.RemoveBody(farseerBullet); return none; }
				
				if (oow || ool)                                 { world.RemoveBody(farseerBullet); return none; }

				if (lvl.WrapMode == LevelBlueprint.WRAPMODE_DONUT)
				{
					var pbullet = ConvertUnits.ToDisplayUnits(farseerBullet.Position);

					pbullet.X = (pbullet.X + lvl.LevelWidth) % lvl.LevelWidth;
					pbullet.Y = (pbullet.Y + lvl.LevelHeight) % lvl.LevelHeight;

					farseerBullet.Position = ConvertUnits.ToSimUnits(pbullet);
				}
				
			}
		}
		
		private World CreateRayWorld(LevelBlueprint lvl)
		{
			var world = new World(Vector2.Zero);

			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDConstants.PHYSICS_CONVERSION_FACTOR);

			foreach (var elem in lvl.AllCannons)
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

			foreach (var elem in lvl.BlueprintMirrorBlocks)
			{
				if (elem.Width < 0.01f) throw new Exception("Invalid Physics");
				if (elem.Height < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), FloatMath.ToRadians(elem.Rotation), BodyType.Static, elem);
				FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(elem.Width), ConvertUnits.ToSimUnits(elem.Height), 1, Vector2.Zero, body, elem);
			}

			foreach (var elem in lvl.BlueprintMirrorCircles)
			{
				if (elem.Diameter < 0.01f) throw new Exception("Invalid Physics");

				var body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(new Vector2(elem.X, elem.Y)), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(ConvertUnits.ToSimUnits(elem.Diameter / 2f), 1, body, Vector2.Zero, elem);
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

				var bn = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rn.VecCenter), 0, BodyType.Static, dn);
				var be = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(re.VecCenter), 0, BodyType.Static, de);
				var bs = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rs.VecCenter), 0, BodyType.Static, ds);
				var bw = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rw.VecCenter), 0, BodyType.Static, dw);

				var fn = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rn.Width), ConvertUnits.ToSimUnits(rn.Height), 1, Vector2.Zero, bn, dn);
				var fe = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(re.Width), ConvertUnits.ToSimUnits(re.Height), 1, Vector2.Zero, be, de);
				var fs = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rs.Width), ConvertUnits.ToSimUnits(rs.Height), 1, Vector2.Zero, bs, ds);
				var fw = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rw.Width), ConvertUnits.ToSimUnits(rw.Height), 1, Vector2.Zero, bw, dw);
			}

			return world;
		}

		private Tuple<Vector2, Vector2>[] SimplifyRays(Vector2 cannonpos, List<Vector2> fullpath)
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

		private bool CanSimplify(Vector2 p1, Vector2 p2, IEnumerable<Vector2> test)
		{
			foreach (var elem in test)
			{
				if (Math2D.LinePointDistance(p1.ToFPoint(), p2.ToFPoint(), elem.ToFPoint()) > Bullet.BULLET_DIAMETER / 3f) return false;
			}

			return true;
		}

		private bool IsSplit(Vector2 p1, Vector2 p2)
		{
			return (p2 - p1).LengthSquared() > 16f * 16f;
		}
	}
}
