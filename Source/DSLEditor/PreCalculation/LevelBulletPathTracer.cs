using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework;

namespace GridDominance.DSLEditor.PreCalculation
{
	public static class LevelBulletPathTracer
	{
		public const int MAX_COUNT_REFLECT = 8;

		private const float DEG2RAD = (float)(Math.PI / 180f);
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

			var world_normal = CreateRayWorld(lvl, 0, 1);
			var world_extend = CreateRayWorld(lvl, HITBOX_ENLARGE, 1.5f);

			bool rayAtStart = true;
			float rayAtStartQuality = float.MaxValue;
			for (int ideg = 0; ideg < 3600; ideg ++)
			{
				float deg = ideg / 10f;

				float quality;
				var ray = FindBulletPaths(world_normal, world_extend, cannon, deg, out quality);

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

		private static BulletPathBlueprint FindBulletPaths(World wBase, World wCollision, CannonBlueprint cannon, float deg, out float quality)
		{
			float startRadians = deg * DEG2RAD;

			var rays = new List<Tuple<float, float>>();
			
			var rcStart = new Vector2(cannon.X, cannon.Y);
			var rcEnd   = new Vector2(cannon.X, cannon.Y) + Rotate(new Vector2(2048, 0), startRadians);

			for (int i = 0; i < MAX_COUNT_REFLECT; i++)
			{
				var traceResult = RayCast(wBase, rcStart, rcEnd);
				var traceResult2 = RayCast(wCollision, rcStart, rcEnd);

				if (traceResult2 != null && traceResult != null && traceResult2.Item1.UserData != traceResult.Item1.UserData)
				{
					// Dirty hit
					quality = float.MaxValue;
					return null;
				}

				if (traceResult == null)
				{
					quality = float.MaxValue;
					return null;
				}

				var fCannon = traceResult.Item1.UserData as CannonBlueprint;
				if (fCannon != null)
				{
					quality = DistLinePoint(rcStart, traceResult.Item2, new Vector2(fCannon.X, fCannon.Y));
					rays.Add(Tuple.Create(traceResult.Item2.X, traceResult.Item2.Y));
					return new BulletPathBlueprint(fCannon.CannonID, startRadians, rays.ToArray());
				}

				var fGlassBlock = traceResult.Item1.UserData as GlassBlockBlueprint;
				if (fGlassBlock != null)
				{
					rays.Add(Tuple.Create(traceResult.Item2.X, traceResult.Item2.Y));

					var pNewStart = traceResult.Item2;
					var pVec = Vector2.Reflect(rcEnd - rcStart, traceResult.Item3);
					pVec *= 2048 * pVec.Length();
					var pNewEnd = pNewStart + pVec;

					rcStart = pNewStart;
					rcEnd = pNewEnd;

					continue;
				}

				var fVoidWall = traceResult.Item1.UserData as VoidWallBlueprint;
				if (fVoidWall != null)
				{
					quality = float.MaxValue;
					return null;
				}

				var fVoidCircle = traceResult.Item1.UserData as VoidCircleBlueprint;
				if (fVoidCircle != null)
				{
					quality = float.MaxValue;
					return null;
				}

				throw new Exception("Unknown rayTrace resturn ficture: " + traceResult.Item1.UserData);
			}

			// Too many hops
			quality = float.MaxValue;
			return null;
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
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(cannonEnlarge * elem.Diameter / 2f + extend, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintGlassBlocks)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Width + extend, elem.Height + extend, 1, Vector2.Zero, body, elem);
			}

			foreach (var elem in lvl.BlueprintVoidCircles)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(elem.Diameter / 2f + extend, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintVoidWalls)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Length + extend, VoidWallBlueprint.DEFAULT_WIDTH + HITBOX_ENLARGE, 1, Vector2.Zero, body, elem);
				body.Rotation = DEG2RAD * elem.Rotation;
			}

			return world;
		}

		public static Vector2 Rotate(Vector2 vector2, double radians)
		{
			var cos = (float)Math.Cos(radians);
			var sin = (float)Math.Sin(radians);

			return new Vector2(vector2.X * cos - vector2.Y * sin, vector2.X * sin + vector2.Y * cos);
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

		public static float DistLinePoint(Vector2 p1, Vector2 p2, Vector2 point)
		{
			double dist = CrossProduct(p1, p2, point) / (p2 - p1).Length();
			return (float)Math.Abs(dist);
		}
	}
}
