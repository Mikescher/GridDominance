using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Content.Pipeline.PreCalculation
{
	internal static partial class LevelBulletPathSimulator
	{
		public const int MAX_PATH_SIZE  = 64;
		public const int MIN_RAY_LENGTH = 32;

		private const float HITBOX_ENLARGE = 8f;

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
			for (int ideg = 0; ideg < 3600; ideg ++)
			{
				float deg = ideg / 10f;

				float quality;
				var ray = FindBulletPaths(worldNormal, cannon, deg, out quality);

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

		private static BulletPathBlueprint FindBulletPaths(GDGameScreen world, CannonBlueprint cannon, float deg, out float quality)
		{
			var wcannon = world.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == cannon.CannonID);

			wcannon.Rotation.ForceSet(FloatMath.ToRadians(deg));
			var bullet = wcannon.ShootForSimulation();

			List<Vector2> positionList = new List<Vector2>();

			for (int i = 0;; i++)
			{
				world.Update(new SAMTime(1/32f, i/32f));

				if (!bullet.Alive)
				{
					bullet.OnRemove();

					switch (bullet.LastCollision)
					{
						case Bullet.BulletCollisionType.VoidObject:
							quality = float.MaxValue;
							return null;

						case Bullet.BulletCollisionType.FriendlyCannon:
						{
							var tcannon = (Cannon) bullet.LastCollisionObject;
							quality = DistLinePoint(positionList[positionList.Count - 2], positionList[positionList.Count - 1], wcannon.Position);
							return CreateBulletPath(tcannon, positionList);
						}

						case Bullet.BulletCollisionType.NeutralCannon:
						{
							var tcannon = (Cannon)bullet.LastCollisionObject;
							quality = DistLinePoint(positionList[positionList.Count - 2], positionList[positionList.Count - 1], wcannon.Position);
							return CreateBulletPath(tcannon, positionList);
						}

						case Bullet.BulletCollisionType.EnemyCannon:
						{
							var tcannon = (Cannon)bullet.LastCollisionObject;
							quality = DistLinePoint(positionList[positionList.Count - 2], positionList[positionList.Count - 1], wcannon.Position);
							return CreateBulletPath(tcannon, positionList);
						}

						case Bullet.BulletCollisionType.None:
						{
							quality = float.MaxValue;
							return null;
						}

						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		private static BulletPathBlueprint CreateBulletPath(Cannon c, List<Vector2> positions)
		{
			List<Vector2> rays = new List<Vector2>();

			Vector2 lastpos = positions.First();

			float len = 0f;
			foreach (var p in positions.Skip(1))
			{
				len += (p - lastpos).Length();
				if (len > MIN_RAY_LENGTH)
				{
					rays.Add(p);
					len = 0;
				}
				lastpos = p;
			}
			if (len > 0) rays.Add(positions.Last());

			if (rays.Count < MAX_PATH_SIZE)
			{
				return new BulletPathBlueprint(c.BlueprintCannonID, c.Rotation.ActualValue, rays.Select(p => Tuple.Create(p.X, p.Y)).ToArray());
			}

			return null;
		}

		private static GDGameScreen CreateRayWorld(LevelBlueprint lvl)
		{
			var g = new MainGame();

			g.SetLevelScreen(lvl, FractionDifficulty.DIFF_0, new GraphBlueprint());

			return (GDGameScreen) g.GetCurrentScreen();
		}

		private static float CrossProduct(Vector2 pointA, Vector2 pointB, Vector2 pointC)
		{
			var AB = new Vector2();
			var AC = new Vector2();
			AB.X = pointB.X - pointA.X;
			AB.Y = pointB.Y - pointA.Y;
			AC.X = pointC.X - pointA.X;
			AC.Y = pointC.Y - pointA.Y;
			float cross = AB.X * AC.Y - AB.Y * AC.X;

			return cross;
		}

		private static float DistLinePoint(Vector2 p1, Vector2 p2, Vector2 point)
		{
			double dist = CrossProduct(p1, p2, point) / (p2 - p1).Length();
			return (float)Math.Abs(dist);
		}
	}
}
