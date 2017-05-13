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

				float quality;
				var ray = FindBulletPaths(lvl, worldNormal, cannon, deg, out quality);

				if (ideg == 0) rayAtStart = (ray != null);

				if (ray == null)
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
					if (bestRay == null)
					{
						bestRay = ray;
						bestQuality = quality;
					}
					else if (bestRay.TargetCannonID != ray.TargetCannonID)
					{
						if (!resultRays.Any()) rayAtStartQuality = bestQuality;
						resultRays.Add(bestRay);
						bestRay = ray;
						bestQuality = quality;
					}
					else if (bestQuality > quality)
					{
						bestRay = ray;
						bestQuality = quality;
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

		private static BulletPathBlueprint FindBulletPaths(LevelBlueprint lvl, World world, CannonBlueprint cannon, float deg, out float quality)
		{
			float startRadians = deg * FloatMath.DegRad;

			var scale = cannon.Diameter / Cannon.CANNON_DIAMETER;

			var spawnPoint = new Vector2(cannon.X, cannon.Y) + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(startRadians);
			var spawnVeloc = new Vector2(1, 0).Rotate(startRadians) * Cannon.BULLET_INITIAL_SPEED;

			object collisionUserObject = null;

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
				if (fb.UserData is GlassBlockBlueprint) return true;
				return false;
			};
			farseerBullet.AngularVelocity = 0;

			List<Vector2> fullpath = new List<Vector2>();
			fullpath.Add(spawnPoint);

			for (int i = 0; i < 15 * Bullet.MAXIMUM_LIEFTIME; i++)
			{
				collisionUserObject = null;

				foreach (var bh in lvl.BlueprintBlackHoles)
				{
					var pp = ConvertUnits.ToDisplayUnits(farseerBullet.Position) - new Vector2(bh.X, bh.Y);
					var force = bh.Power / pp.LengthSquared();
					var vecForce = pp.WithLength(force);
					farseerBullet.ApplyForce(ConvertUnits.ToSimUnits(vecForce));
				}

				world.Step(1/24f); // 24 UPS
				fullpath.Add(ConvertUnits.ToDisplayUnits(farseerBullet.Position));

				if (collisionUserObject is CannonBlueprint)
				{
					world.RemoveBody(farseerBullet);
					var tgcannon = (CannonBlueprint)collisionUserObject;
					quality = FloatMath.LinePointDistance(ConvertUnits.ToDisplayUnits(farseerBullet.Position), ConvertUnits.ToDisplayUnits(farseerBullet.Position) + ConvertUnits.ToDisplayUnits(farseerBullet.LinearVelocity), new Vector2(tgcannon.X, tgcannon.Y));
					return new BulletPathBlueprint(tgcannon.CannonID, startRadians, SimplifyRays(new Vector2(cannon.X, cannon.Y), fullpath), fullpath);
				}

				if (collisionUserObject is VoidWallBlueprint)   { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
				if (collisionUserObject is VoidCircleBlueprint) { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
				if (collisionUserObject is BlackHoleBlueprint)  { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
				
				if (farseerBullet.Position.X < 0    - 64) { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
				if (farseerBullet.Position.Y < 0    - 64) { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
				if (farseerBullet.Position.X > 1024 + 64) { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
				if (farseerBullet.Position.Y > 640  + 64) { world.RemoveBody(farseerBullet); quality = float.MaxValue; return null; }
			}

			// Timeout
			world.RemoveBody(farseerBullet);
			quality = float.MaxValue;
			return null;
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

			return world;
		}

		private static Tuple<float, float>[] SimplifyRays(Vector2 cannonpos, List<Vector2> fullpath)
		{
			//return fullpath.Skip(1).Select(o => Tuple.Create(o.X, o.Y)).ToArray();

			List<Vector2> outl = new List<Vector2>();

			Vector2 vStart = cannonpos;
			int start = 0;
			for (int i = 2; i < fullpath.Count; i++)
			{
				if (CanSimplify(vStart, fullpath[i], fullpath.Skip(start+1).Take(i - start - 1))) continue;

				outl.Add(vStart);
				start = i - 1;
				vStart = fullpath[i - 1];
				i++;
			}
			outl.Add(fullpath.Last());

			return outl.Select(o => Tuple.Create(o.X, o.Y)).ToArray();
		}

		private static bool CanSimplify(Vector2 p1, Vector2 p2, IEnumerable<Vector2> test)
		{
			foreach (var elem in test)
			{
				if (FloatMath.LinePointDistance(p1, p2, elem) > Bullet.BULLET_DIAMETER / 3f) return false;
			}

			return true;
		}
	}
}
