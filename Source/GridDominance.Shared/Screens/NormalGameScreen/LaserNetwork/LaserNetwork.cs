using MonoSAMFramework.Portable.Interfaces;
using System.Collections.Generic;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Extensions;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.GameMath;
using FarseerPhysics;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserNetwork : ISAMUpdateable
	{
		public const int MAX_LASER_RAYCOUNT        = 16;
		public const int MAX_LASER_PER_SOURCE      = 128; // 8 * 16
		public const int MAX_BACKTRACE             = 4;
		public const float RAY_WIDTH               = 6f;
		public const float PARALLEL_ANGLE_EPSILON  = FloatMath.RAD_POS_004;

		public bool Dirty = false;     // calc everything new
		public bool SemiDirty = false; // could be that something crossed a ray


		private readonly GDGameScreen _screen;
		private readonly World _world;
		private readonly float _worldWidth;
		private readonly float _worldHeight;
		private readonly float _rayLength;
		private readonly GameWrapMode _wrapMode;

		public List<LaserSource> Sources = new List<LaserSource>();

		private readonly List<Tuple<LaserRay, LaserSource>> _faultRays = new List<Tuple<LaserRay, LaserSource>>();
		
		public LaserNetwork(World b2dworld, GDGameScreen scrn, GameWrapMode wrap)
		{
			_screen = scrn;
			_world = b2dworld;

			_worldWidth  = scrn.Blueprint.LevelWidth;
			_worldHeight = scrn.Blueprint.LevelHeight;
			_rayLength = _worldWidth + _worldHeight;
			_wrapMode = wrap;
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			if (DebugSettings.Get("ContinoousLasers")) Dirty = true;
#endif


#if DEBUG
			DebugUtils.TIMING_LASER.Start();
#endif
			if (SemiDirty && !Dirty)
			{
				TestSemiDirty();
				SemiDirty = false;
			}

			if (Dirty)
			{
				foreach (LaserSource src in Sources) src.Lasers.Clear();
				foreach (LaserSource src in Sources) RecalcSource(src);
			}

			for (int i = 0; i < MAX_BACKTRACE; i++)
			{
				if (_faultRays.Any())
				{
					var r = _faultRays.ToList();
					_faultRays.Clear();
					foreach (var tup in r)
					{
						var src = tup.Item2;
						var ray = tup.Item1;

						if (src.Lasers.Contains(ray))
						{
							src.Lasers.Remove(ray);
							RecalcFromRay(src, ray.Start, ray.Start + (ray.End - ray.Start).WithLength(_rayLength), ray.Depth, ray.InGlass, ray.StartIgnoreObj, ray.Source, ray.SourceDistance, i+1 >= MAX_BACKTRACE);
						}

					}
				}
			}
			
			Dirty = false;
			SemiDirty = false;
#if DEBUG
			DebugUtils.TIMING_LASER.Stop();
#endif
		}

		private void TestSemiDirty()
		{
			foreach (var src in Sources)
			{
				foreach (var ray in src.Lasers)
				{
					if (TestCast(ray)) { Dirty = true; return; }
				}
			}
		}

		public LaserSource AddSource(GameEntity e)
		{
			var src = new LaserSource(this, e.Position, _screen.GetNeutralFraction(), e);
			Sources.Add(src);
			return src;
		}
		
		private void RecalcSource(LaserSource src)
		{
			if (!src.LaserActive) { src.Lasers.Clear(); return; }

			FPoint istart = src.Position;
			FPoint iend   = src.Position + new Vector2(_rayLength, 0).Rotate(src.LaserRotation);

			RecalcFromRay(src, istart, iend, 0, false, src.UserData, null, 0, false);
		}

		private void RecalcFromRay(LaserSource src, FPoint istart, FPoint iend, int idepth, bool iinglass, object iignore, LaserRay isrc, float idist, bool nofault)
		{
			Stack<Tuple<FPoint, FPoint, int, bool, object, LaserRay, float>> remaining = new Stack<Tuple<FPoint, FPoint, int, bool, object, LaserRay, float>>();

			remaining.Push(Tuple.Create(istart, iend, idepth, iinglass, iignore, isrc, idist));

			while (remaining.Any())
			{
				var pop = remaining.Pop();
				
				var start     = pop.Item1;
				var end       = pop.Item2;
				var depth     = pop.Item3;
				var inglass   = pop.Item4;
				var ignore    = pop.Item5;
				var source    = pop.Item6;
				var startdist = pop.Item7;

				if (src.Lasers.Count + 1 >= MAX_LASER_PER_SOURCE) continue;
				if (depth >= MAX_LASER_RAYCOUNT) continue;
				if (!start.IsValid) continue;
				if (!end.IsValid) continue;
				if (start.EpsilonEquals(end, FloatMath.EPSILON6)) continue;

				var result = RayCast(start, end, ignore);

				#region OOB
				if (result == null)
				{
					var ray = new LaserRay(start, end, source, LaserRayTerminator.OOB, depth, inglass, ignore, null, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region Cannon
				var resultCannon = result.Item1.UserData as Cannon;
				if (resultCannon != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Target, depth, inglass, ignore, resultCannon, startdist, resultCannon, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region GlassBlockRefraction
				var resultGlassBlockRefrac = result.Item1.UserData as MarkerRefractionEdge;
				if (resultGlassBlockRefrac != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Glass, depth, inglass, ignore, resultGlassBlockRefrac, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					// sin(aIn) / sin(aOut) = currRefractIdx / Glass.RefractIdx
					var aIn = (start - end).ToAngle() - result.Item3.ToAngle();
					var n = inglass ? (GlassBlock.REFRACTION_INDEX / 1f) : (1f / GlassBlock.REFRACTION_INDEX);

					var sinaOut = FloatMath.Sin(aIn) * n;

					var isRefracting = sinaOut < 1 && sinaOut > -1;
					if (isRefracting) // refraction
					{
						var aOut = FloatMath.Asin(sinaOut);
						var pRefractAngle = (-result.Item3).ToAngle() + aOut;

						remaining.Push(Tuple.Create(result.Item2, result.Item2 + new Vector2(_rayLength, 0).Rotate(pRefractAngle), depth + 1, !inglass, (object) resultGlassBlockRefrac, ray, startdist + ray.Length));
					}

					if (!inglass)
					{
						var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
						remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultGlassBlockRefrac, ray, startdist + ray.Length));
						continue;
					}
					else
					{
						if (isRefracting)
						{
							continue; // no reflection in glass
						}
						else
						{
							var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
							remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultGlassBlockRefrac, ray, startdist + ray.Length));
							continue;
						}
					}
				}
				#endregion

				#region MirrorBlock
				var resultMirrorBlock = result.Item1.UserData as MirrorBlock;
				if (resultMirrorBlock != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Mirror, depth, inglass, ignore, resultMirrorBlock, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
					remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultMirrorBlock, ray, startdist + ray.Length));
					continue;
				}
				#endregion

				#region MirrorCircle
				var resultMirrorCircle = result.Item1.UserData as MirrorCircle;
				if (resultMirrorCircle != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Mirror, depth, inglass, ignore, resultMirrorCircle, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
					remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultMirrorCircle, ray, startdist + ray.Length));
					continue;
				}
				#endregion

				#region VoidWall
				var resultVoidWall = result.Item1.UserData as VoidWall;
				if (resultVoidWall != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.VoidObject, depth, inglass, ignore, resultVoidWall, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region VoidCircle
				var resultVoidCircle = result.Item1.UserData as VoidCircle;
				if (resultVoidCircle != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.VoidObject, depth, inglass, ignore, resultVoidCircle, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region Portal
				var resultPortal = result.Item1.UserData as Portal;
				if (resultPortal != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Portal, depth, inglass, ignore, resultPortal, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					var inPortal = resultPortal;
					var normal = result.Item3;
					bool hit = FloatMath.DiffRadiansAbs(normal.ToAngle(), inPortal.Normal) < FloatMath.RAD_POS_001;

					if (!hit) continue; // back-side hit

					if (inPortal.Links.Count == 0) continue; // void portal

					foreach (var outportal in inPortal.Links)
					{
						var rot = outportal.Normal - inPortal.Normal + FloatMath.RAD_POS_180;
						var projec = result.Item2.ProjectOntoLine(inPortal.Position, inPortal.VecDirection);

						var newVelocity = (end - start).Rotate(rot);
						var newStart = outportal.Position + outportal.VecDirection * (-projec) + outportal.VecNormal * (Portal.WIDTH / 2f);

						var newEnd = newStart + newVelocity.WithLength(_rayLength);

						remaining.Push(Tuple.Create(newStart, newEnd, depth + 1, false, (object) outportal, ray, startdist + ray.Length));
					}
					continue;
				}
				#endregion

				#region Border
				var resultBorderMarker = result.Item1.UserData as MarkerCollisionBorder;
				if (resultBorderMarker != null)
				{
					if (_wrapMode == GameWrapMode.Reflect)
					{
						var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Mirror, depth, inglass, ignore, resultBorderMarker, startdist, null, null);
						src.Lasers.Add(ray);
						if (TestForLaserCollision(src, ray, nofault)) continue;

						var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
						remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object)resultBorderMarker, ray, startdist + ray.Length));
						continue;
					}
					else if (_wrapMode == GameWrapMode.Donut)
					{
						var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Portal, depth, inglass, ignore, resultBorderMarker, startdist, null, null);
						src.Lasers.Add(ray);
						if (TestForLaserCollision(src, ray, nofault)) continue;

						var pNewStartX = result.Item2.X;
						var pNewStartY = result.Item2.Y;
						switch (resultBorderMarker.Side)
						{
							case FlatAlign4.NN: pNewStartY += _screen.Blueprint.LevelHeight; break;
							case FlatAlign4.EE: pNewStartX -= _screen.Blueprint.LevelWidth; break;
							case FlatAlign4.SS: pNewStartY -= _screen.Blueprint.LevelHeight; break;
							case FlatAlign4.WW: pNewStartX += _screen.Blueprint.LevelWidth; break;
						}

						var newStart = new FPoint(pNewStartX, pNewStartY);
						var newEnd = newStart + (end - start);
						
						remaining.Push(Tuple.Create(newStart, newEnd, depth + 1, inglass, (object)null, ray, startdist + ray.Length));
						continue;
					}
					else
					{
						SAMLog.Error("LSR-NTW", "Unknown wrapmode: " + _wrapMode);
					}

					continue;
				}
				#endregion

				#region Bullet
				var resultBullet = result.Item1.UserData as Bullet;
				if (resultBullet != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.VoidObject, depth, inglass, ignore, resultBullet, startdist, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				// wud ???
				SAMLog.Error("LaserNetwork", string.Format("Ray collided with unkown fixture: {0}", result?.Item1?.UserData ?? "<NULL>"));
			}
		}

		private Tuple<Fixture, FPoint, Vector2> RayCast(FPoint start, FPoint end, object udIgnore)
		{
			Tuple<Fixture, FPoint, Vector2> result = null;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			float rcCallback(Fixture f, Vector2 pos, Vector2 normal, float frac)
			{
				if (f.UserData is GlassBlock) return -1; // ignore;

				if (udIgnore != null && udIgnore == f.UserData) return -1; // ignore

				if (_wrapMode == GameWrapMode.Death && f.UserData is MarkerCollisionBorder) return -1; // ignore

				result = Tuple.Create(f, ConvertUnits.ToDisplayUnits(pos).ToFPoint(), normal);

				return frac; // limit
			}

			_world.RayCast(rcCallback, ConvertUnits.ToSimUnits(start.ToVec2D()), ConvertUnits.ToSimUnits(end.ToVec2D()));

			return result;
		}

		private bool TestCast(LaserRay ray)
		{
			bool newobstaclefound = false;
			bool correctendfound  = false;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			float rcCallback(Fixture f, Vector2 pos, Vector2 normal, float frac)
			{
				if (f.UserData is GlassBlock) return -1; // ignore;

				if (_wrapMode == GameWrapMode.Death && f.UserData is MarkerCollisionBorder) return -1; // ignore

				if (ray.StartIgnoreObj == f.UserData) return -1; // ignore

				if (ray.EndIgnoreObj == f.UserData)
				{
					var p = ConvertUnits.ToDisplayUnits(pos).ToFPoint();
					if (p.EpsilonEquals(ray.End, 0.5f))
					{
						correctendfound = true;
						return frac; // limit;
					}
					else
					{
						correctendfound = false;
						return frac; // limit;
					}
				}
				else
				{
					if (correctendfound)
					{
						newobstaclefound = true;
						return 0; // terminate
					}
					else
					{
						return frac; // limit;
					}
				}
			}

			var ss = ConvertUnits.ToSimUnits(ray.Start.ToVec2D());
			var ee = ConvertUnits.ToSimUnits(ray.End.ToVec2D());

			ee = ee + (ee-ss).WithLength(1f);

			_world.RayCast(rcCallback, ss, ee);

			if (!correctendfound) return true;
			if (newobstaclefound) return true;
			return false;
		}

		private bool TestForLaserCollision(LaserSource src1, LaserRay ray1, bool nofault)
		{
			float minU = float.MaxValue;
			FPoint minP = FPoint.Zero;
			LaserRay minRay2 = null;
			LaserSource minSrc2 = null;

			foreach (var src2 in Sources)
			{
				foreach (var ray2 in src2.Lasers)
				{
					if (src1 == src2 && ray1 == ray2) continue;
					
					FPoint intersect;
					float u;
					if (LineParallality(ray1.Start, ray1.End, ray1.SourceDistance, ray2.Start, ray2.End, ray2.SourceDistance, out intersect, out u))
					{
						if (u < minU)
						{
							minU = u;
							minP = intersect;
							minRay2 = ray2;
							minSrc2 = src2;
						}
					}
					else if (Math2D.LineIntersectionExt(ray1.Start, ray1.End, ray2.Start, ray2.End, -0.1f, out intersect, out u, out _))
					{
						if (u < minU)
						{
							minU = u;
							minP = intersect;
							minRay2 = ray2;
							minSrc2 = src2;
						}
					}
				}
			}

			if (minRay2 != null)
			{
				if (nofault)
				{
					ray1.SetLaserIntersect(minP, minRay2, minSrc2, LaserRayTerminator.LaserFaultTerm);

					return true;
				}
				else if (src1 == minSrc2)
				{
					ray1.SetLaserIntersect(minP, minRay2, minSrc2, LaserRayTerminator.LaserSelfTerm);
					minRay2.SelfCollRays.Add(ray1);
					foreach (var r in ray1.SelfCollRays) _faultRays.Add(Tuple.Create(r, src1));
					ray1.SelfCollRays.Clear();

					return true;
				}
				else
				{
					ray1.SetLaserIntersect(minP, minRay2, minSrc2, LaserRayTerminator.LaserDoubleTerm);
					foreach (var r in ray1.SelfCollRays) _faultRays.Add(Tuple.Create(r, src1));
					ray1.SelfCollRays.Clear();

					CutRayAndUpdate(minSrc2, minRay2, minP, src1, ray1);

					return true;
				}
			}
			
			
			return false;
		}

		private bool LineParallality(FPoint v1s, FPoint v1e, float v1d, FPoint v2s, FPoint v2e, float v2d, out FPoint intersect, out float u)
		{
			var dsq = Math2D.SegmentSegmentDistanceSquared(v1s, v1e, v2s, v2e, out _);
			
			if (dsq > RAY_WIDTH*RAY_WIDTH) { intersect = FPoint.Zero; u = float.NaN; return false; }

			var a1 = (v1e - v1s).ToAngle();
			var a2 = (v2e - v2s).ToAngle();

			if (FloatMath.DiffRadiansAbs(a1, a2) < PARALLEL_ANGLE_EPSILON)
			{
				// same direction

				var u1 = v2s.ProjectOntoLine(v1s, v1e);
				var u2 = v1s.ProjectOntoLine(v2s, v2e);

				if (FloatMath.Abs(u1) < FloatMath.Abs(u2))
				{
					intersect = v2s;
					u = u1;
					return true;
				}
				else
				{
					intersect = v1s;
					u = 0;
					return true;
				}
			}
			else if (FloatMath.Abs(FloatMath.DiffModulo(a1, a2, FloatMath.RAD_POS_180)) < PARALLEL_ANGLE_EPSILON)
			{
				var pc = FPoint.MiddlePoint(v1s, v2s);

				pc = pc + (v1s - v2s).WithLength((v1d - v2d)/2f);

				var pcu1 = pc.ProjectOntoLine(v1s, v1e);
				if (pcu1 < 0) pc = v1s;
				if (pcu1 > 1) pc = v1e;

				var pcu2 = pc.ProjectOntoLine(v2s, v2e);
				if (pcu2 < 0) pc = v2s;
				if (pcu2 > 1) pc = v2e;

				intersect = pc;
				u = pc.ProjectOntoLine(v1s, v1e);
				return true;
			}

			intersect = FPoint.Zero;
			u = float.NaN;
			
			return false;
		}

		private void CutRayAndUpdate(LaserSource src, LaserRay ray, FPoint intersect, LaserSource srcOther, LaserRay rayOther)
		{
			if (ray.Terminator == LaserRayTerminator.LaserDoubleTerm)
			{
				_faultRays.Add(ray.TerminatorRay);
			}


			ray.SetLaserIntersect(intersect, rayOther, srcOther, LaserRayTerminator.LaserDoubleTerm);
			foreach (var r in ray.SelfCollRays) _faultRays.Add(Tuple.Create(r, src));
			ray.SelfCollRays.Clear();

			Stack<LaserRay> rem = new Stack<LaserRay>();
			rem.Push(ray);

			while (rem.Any())
			{
				var elim = rem.Pop();

				for (int i = src.Lasers.Count - 1; i >= 0; i--)
				{
					if (src.Lasers[i].Source != elim) continue;

					if (src.Lasers[i].Terminator == LaserRayTerminator.LaserDoubleTerm)
					{
						_faultRays.Add(src.Lasers[i].TerminatorRay);
					}

					foreach (var r in src.Lasers[i].SelfCollRays) _faultRays.Add(Tuple.Create(r, src));

					rem.Push(src.Lasers[i]);
					src.Lasers.RemoveAt(i);
					
				}
			}
			
		}
	}
}
