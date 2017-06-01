using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;

namespace GridDominance.Content.Pipeline.PreCalculation
{
	internal static class LevelBulletPathSimulator
	{
		private const int RESOLUTION = 720;

		public static void Precalc(LevelBlueprint lvl)
		{
			foreach (var cannon in lvl.BlueprintCannons) cannon.PrecalculatedPaths = Precalc(lvl, cannon);
		}

		private static BulletPathBlueprint[] Precalc(LevelBlueprint lvl, CannonBlueprint cannon)
		{
			List<BulletPathBlueprint> resultRays = new List<BulletPathBlueprint>();

			BulletPathBlueprint bestRay = null;
			float bestQuality = float.MaxValue;

			var worldNormal = CreateRayWorld(lvl);

			bool rayAtStart = true;
			float rayAtStartQuality = float.MaxValue;
			for (int ideg = 0; ideg < RESOLUTION; ideg++)
			{
				float deg = ideg * (360f / RESOLUTION);

				var rays = FindBulletPaths(lvl, worldNormal, cannon, deg);

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

		private static List<Tuple<BulletPathBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World world, CannonBlueprint cannon, float deg)
		{
			float startRadians = deg * FloatMath.DegRad;
			var scale = cannon.Diameter / Cannon.CANNON_DIAMETER;
			var spawnPoint = new Vector2(cannon.X, cannon.Y) + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(startRadians);
			var spawnVeloc = new Vector2(1, 0).Rotate(startRadians) * Cannon.BULLET_INITIAL_SPEED;

			var fbpresult = FindBulletPaths(lvl, world, spawnPoint, spawnVeloc, new List<Vector2>(), scale, 0);

			var result = new List<Tuple<BulletPathBlueprint, float>>();
			foreach (var r in fbpresult)
			{
				var tgray = new BulletPathBlueprint(r.Item2.CannonID, startRadians, SimplifyRays(new Vector2(cannon.X, cannon.Y), r.Item1), r.Item1);
				result.Add(Tuple.Create(tgray, r.Item3));
			}

			return result;
		}

		private static List<Tuple<List<Vector2>, CannonBlueprint, float>> FindBulletPaths(LevelBlueprint lvl, World world, Vector2 spawnPoint, Vector2 spawnVeloc, List<Vector2> fullpath, float scale, float lifetime)
		{
			var none = new List<Tuple<List<Vector2>, CannonBlueprint, float>>();

			fullpath = fullpath.ToList();

			object collisionUserObject = null;
			Contact collisionContact = null;

			var farseerBullet = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(scale * Bullet.BULLET_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(spawnPoint), BodyType.Dynamic, null);
			farseerBullet.LinearVelocity = ConvertUnits.ToSimUnits(spawnVeloc);
			farseerBullet.CollidesWith = Category.All;
			farseerBullet.IsBullet = true;               // Use CCD solver (prevents tunelling) - do we really need this / how much perf soes this cost ??
			farseerBullet.Restitution = 1f;              // Bouncability, 1=bounce always elastic
			farseerBullet.AngularDamping = 1f;           // Practically no angular rotation
			farseerBullet.Friction = 0f;
			farseerBullet.LinearDamping = 0f;            // no slowing down
			farseerBullet.OnCollision += (fa, fb, cobj) =>
			{
				collisionUserObject = fb.UserData;
				collisionContact    = cobj;
				if (fb.UserData is GlassBlockBlueprint) return true;
				return false;
			};
			farseerBullet.AngularVelocity = 0;

			fullpath.Add(spawnPoint);

			for (;;)
			{
				if (lifetime >= Bullet.MAXIMUM_LIEFTIME) { world.RemoveBody(farseerBullet); return none; }

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

				if (collisionUserObject is CannonBlueprint)
				{
					world.RemoveBody(farseerBullet);
					var tgcannon = (CannonBlueprint)collisionUserObject;

					var quality = FloatMath.LinePointDistance(ConvertUnits.ToDisplayUnits(farseerBullet.Position), ConvertUnits.ToDisplayUnits(farseerBullet.Position) + ConvertUnits.ToDisplayUnits(farseerBullet.LinearVelocity), new Vector2(tgcannon.X, tgcannon.Y));
					
					return new List<Tuple<List<Vector2>, CannonBlueprint, float>> { Tuple.Create(fullpath, tgcannon, quality) };
				}

				if (collisionUserObject is PortalBlueprint)
				{
					var veloc = farseerBullet.LinearVelocity;

					world.RemoveBody(farseerBullet);
					var fPortal = (PortalBlueprint)collisionUserObject;

					//TODO or evtl GetWorldManifold ?? (https://gamedev.stackexchange.com/questions/29254/how-do-i-determine-which-side-of-the-player-has-collided-with-an-object)
					var normal = collisionContact.Manifold.LocalNormal;

					bool hit = FloatMath.DiffRadiansAbs(normal.ToAngle(), FloatMath.ToRadians(fPortal.Normal)) < FloatMath.RAD_POS_005;

					if (!hit) return none;

					var dat = new List<Tuple<List<Vector2>, CannonBlueprint, float>>();
					foreach (var outportal in lvl.BlueprintPortals.Where(p => p.Side != fPortal.Side && p.Group == fPortal.Group))
					{
						var cIn = new Vector2(fPortal.X, fPortal.Y);
						var cOut = new Vector2(outportal.X, outportal.Y);

						var rot = FloatMath.ToRadians(outportal.Normal - fPortal.Normal + 180);
						var stretch = outportal.Length / fPortal.Length;

						var newVelocity = veloc.Rotate(rot);
						var newStart = cOut + stretch * (farseerBullet.Position - cIn).Rotate(rot); //TODO this starts behind portal??

						var sub = FindBulletPaths(lvl, world, newStart, newVelocity, fullpath, scale, lifetime);
						dat.AddRange(sub);
					}
					return dat;
				}

				if (collisionUserObject is VoidWallBlueprint)   { world.RemoveBody(farseerBullet); return none; }
				if (collisionUserObject is VoidCircleBlueprint) { world.RemoveBody(farseerBullet); return none; }
				if (collisionUserObject is BlackHoleBlueprint)  { world.RemoveBody(farseerBullet); return none; }
				
				if (farseerBullet.Position.X < 0    - 64) { world.RemoveBody(farseerBullet); return none; }
				if (farseerBullet.Position.Y < 0    - 64) { world.RemoveBody(farseerBullet); return none; }
				if (farseerBullet.Position.X > 1024 + 64) { world.RemoveBody(farseerBullet); return none; }
				if (farseerBullet.Position.Y > 640  + 64) { world.RemoveBody(farseerBullet); return none; }
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

			return world;
		}

		private static Tuple<Vector2, Vector2>[] SimplifyRays(Vector2 cannonpos, List<Vector2> fullpath)
		{
			//return fullpath.Skip(1).Select(o => Tuple.Create(o.X, o.Y)).ToArray();

			List<Vector2> outl = new List<Vector2>();

			Vector2 vStart = cannonpos;
			int start = 0;
			for (int i = 2; i < fullpath.Count; i++)
			{
				if (CanSimplify(vStart, fullpath[i], fullpath.Skip(start+1).Take(i - start - 1)) && !IsSplit(fullpath[i-1], fullpath[i])) continue;

				outl.Add(vStart);
				start = i - 1;
				vStart = fullpath[i - 1];
				i++;
			}
			outl.Add(fullpath.Last());
			
			Tuple<Vector2, Vector2>[] funres = new Tuple<Vector2, Vector2>[outl.Count - 1];
			for (int i = 0; i < outl.Count-1; i++)
			{
				funres[i] = Tuple.Create(outl[i], outl[i + 1]);
			}
			return funres;
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
			return (p2 - p1).LengthSquared() > 0.1f;
		}
	}
}
