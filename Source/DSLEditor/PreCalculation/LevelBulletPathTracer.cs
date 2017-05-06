using System;
using System.Collections.Generic;
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

		public static void Precalc(LevelBlueprint lvl)
		{
			foreach (var cannon in lvl.BlueprintCannons) cannon.PrecalculatedPaths = Precalc(lvl, cannon);
		}

		private static BulletPathBlueprint[] Precalc(LevelBlueprint lvl, CannonBlueprint cannon)
		{
			List<BulletPathBlueprint> resultRays = new List<BulletPathBlueprint>();

			BulletPathBlueprint bestRay = null;
			float bestQuality = float.MaxValue;

			var world = CreateRayWorld(lvl);

			for (float deg = 0; deg < 360; deg += 0.1f)
			{
				float quality;
				var ray = RayTrace(world, cannon, deg, out quality);

				if (ray == null)
				{
					if (bestRay != null)
					{
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

			if (bestRay != null) resultRays.Add(bestRay);

			return resultRays.ToArray();
		}

		private static World CreateRayWorld(LevelBlueprint lvl)
		{
			var world = new World(Vector2.Zero);

			ConvertUnits.SetDisplayUnitToSimUnitRatio(1);

			foreach (var elem in lvl.BlueprintCannons)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(elem.Diameter/2f, 1,body,Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintGlassBlocks)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Width, elem.Height, 1, Vector2.Zero, body, elem);
			}

			foreach (var elem in lvl.BlueprintVoidCircles)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachCircle(elem.Diameter / 2f, 1, body, Vector2.Zero, elem);
			}

			foreach (var elem in lvl.BlueprintVoidWalls)
			{
				var body = BodyFactory.CreateBody(world, new Vector2(elem.X, elem.Y), 0, BodyType.Static, elem);
				FixtureFactory.AttachRectangle(elem.Length, VoidWallBlueprint.DEFAULT_WIDTH, 1, Vector2.Zero, body, elem);
				body.Rotation = elem.Rotation;
			}

			return world;
		}

		private static BulletPathBlueprint RayTrace(World w, CannonBlueprint cannon, float deg, out float quality)
		{
			float startRadians = (float)(180 * deg / Math.PI);
			Vector2 pStart = new Vector2(cannon.X, cannon.Y) + Rotate(new Vector2(cannon.Diameter/2f + 2, 0), startRadians);
			Vector2 pEnd = new Vector2(cannon.X, cannon.Y) + Rotate(new Vector2(2048, 0), startRadians);

			int cb_cannonID;
			Vector2 cb_collPos;
			float cb_quality;

			bool reflect;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == cannon) return 0; // terminate

				var cann = f.UserData as CannonBlueprint;
				if (cann != null)
				{
					cb_cannonID = cann.CannonID;
					cb_collPos = pos;
					cb_quality = DistLinePoint(pStart, pEnd, new Vector2(cann.X, cann.Y));

					return frac; // limit
				}

				var reflectBlock = f.UserData as GlassBlockBlueprint;
				if (reflectBlock != null)
				{
					var pNewStart = pos;
					var pVec = Vector2.Reflect(pEnd - pStart, normal);
					pVec *= 2048 * pVec.Length();
					var pNewEnd = pNewStart + pVec;

					pStart = pNewStart;
					pEnd = pNewEnd;

					reflect = true;
					return frac; // limit
				}

				return 0; // terminate
			};

			cb_cannonID = -1;
			cb_collPos = Vector2.Zero;
			cb_quality = -1;
			reflect = false;

			for (int i = 0; i < MAX_COUNT_REFLECT; i++)
			{
				w.RayCast(callback, pStart, pEnd);

				if (cb_cannonID >= 0)
				{
					quality = cb_quality;
					return new BulletPathBlueprint(cb_cannonID, startRadians, new[] { Tuple.Create(cb_collPos.X, cb_collPos.Y) });
				}

				if (reflect)
				{
					cb_cannonID = -1;
					cb_collPos = Vector2.Zero;
					cb_quality = -1;
					reflect = false;

					continue;
				}

				quality = float.MaxValue;
				return null;
			}

			quality = float.MaxValue;
			return null;
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
