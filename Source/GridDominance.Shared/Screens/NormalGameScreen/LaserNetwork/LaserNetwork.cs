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
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserNetwork : ISAMUpdateable, ISAMDrawable
	{
		public const int MAX_LASER_RAYCOUNT   = 16;
		public const int MAX_LASER_PER_SOURCE = 128; // 8 * 16
		public const int MAX_BACKTRACE        = 4;

		public bool Dirty = false;
		
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
			if (!DebugSettings.Get("ContinoousLasers") && !Dirty) return;
#else
			if (!Dirty) return;
#endif

			
#if DEBUG
			DebugUtils.TIMING_LASER.Start();
#endif

			foreach (LaserSource src in Sources) src.Lasers.Clear();

			foreach (LaserSource src in Sources) RecalcSource(src);

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
							RecalcFromRay(src, ray.Start, ray.Start + (ray.End - ray.Start).WithLength(_rayLength), ray.Depth, ray.InGlass, ray.StartIgnoreObj, ray.Source, i+1 >= MAX_BACKTRACE);
						}

					}
				}
			}
			
			Dirty = false;
#if DEBUG
			DebugUtils.TIMING_LASER.Stop();
#endif
		}

		public void Draw(IBatchRenderer sbatch)
		{
			// TODO Draw Laz0rs
		}

		public void DrawDebug(IBatchRenderer sbatch)
		{
			foreach (var src in Sources)
			{
//				sbatch.FillCircle(src.Position, 64, 128, Color.LimeGreen);
//				sbatch.FillCircle(src.Position, 48, 128, src.LaserFraction.Color);

				foreach (var ray in src.Lasers)
				{
					sbatch.DrawLine(ray.Start, ray.End, Color.LimeGreen, 4);
					
					if (ray.Terminator == LaserRayTerminator.LaserDoubleTerm) sbatch.FillRectangle(ray.End - new Vector2(4, 4), new Vector2(8, 8), Color.Salmon);
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

			Vector2 istart = src.Position;
			Vector2 iend   = src.Position + new Vector2(_rayLength, 0).Rotate(src.LaserRotation);

			RecalcFromRay(src, istart, iend, 0, false, src.UserData, null, false);
		}

		private void RecalcFromRay(LaserSource src, Vector2 istart, Vector2 iend, int idepth, bool iinglass, object iignore, LaserRay isrc, bool nofault)
		{
			Stack<Tuple<Vector2, Vector2, int, bool, object, LaserRay>> remaining = new Stack<Tuple<Vector2, Vector2, int, bool, object, LaserRay>>();

			remaining.Push(Tuple.Create(istart, iend, idepth, iinglass, iignore, isrc));

			while (remaining.Any())
			{
				var pop = remaining.Pop();
				
				var start   = pop.Item1;
				var end     = pop.Item2;
				var depth   = pop.Item3;
				var inglass = pop.Item4;
				var ignore  = pop.Item5;
				var source  = pop.Item6;

				if (src.Lasers.Count + 1 >= MAX_LASER_PER_SOURCE) continue;
				if (depth >= MAX_LASER_RAYCOUNT) continue;

				var result = RayCast(_world, start, end, ignore);

				#region OOB
				if (result == null)
				{
					var ray = new LaserRay(start, end, source, LaserRayTerminator.OOB, depth, inglass, ignore, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region Cannon
				var resultCannon = result.Item1.UserData as Cannon;
				if (resultCannon != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Target, depth, inglass, ignore, resultCannon, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region GlassBlockRefraction
				var resultGlassBlockRefrac = result.Item1.UserData as MarkerRefractionEdge;
				if (resultGlassBlockRefrac != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Glass, depth, inglass, ignore, null, null);
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

						remaining.Push(Tuple.Create(result.Item2, result.Item2 + new Vector2(_rayLength, 0).Rotate(pRefractAngle), depth + 1, !inglass, (object) resultGlassBlockRefrac, ray));
					}

					if (!inglass)
					{
						var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
						remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultGlassBlockRefrac, ray));
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
							remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultGlassBlockRefrac, ray));
							continue;
						}
					}
				}
				#endregion

				#region MirrorBlock
				var resultMirrorBlock = result.Item1.UserData as MirrorBlock;
				if (resultMirrorBlock != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Mirror, depth, inglass, ignore, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
					remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultMirrorBlock, ray));
					continue;
				}
				#endregion

				#region MirrorCircle
				var resultMirrorCircle = result.Item1.UserData as MirrorCircle;
				if (resultMirrorCircle != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Mirror, depth, inglass, ignore, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					var reflect_end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(_rayLength);
					remaining.Push(Tuple.Create(result.Item2, reflect_end, depth + 1, inglass, (object) resultMirrorCircle, ray));
					continue;
				}
				#endregion

				#region VoidWall
				var resultVoidWall = result.Item1.UserData as VoidWall;
				if (resultVoidWall != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.VoidObject, depth, inglass, ignore, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region VoidCircle
				var resultVoidCircle = result.Item1.UserData as VoidCircle;
				if (resultVoidCircle != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.VoidObject, depth, inglass, ignore, null, null);
					src.Lasers.Add(ray);
					if (TestForLaserCollision(src, ray, nofault)) continue;

					continue;
				}
				#endregion

				#region Portal
				var resultPortal = result.Item1.UserData as Portal;
				if (resultPortal != null)
				{
					var ray = new LaserRay(start, result.Item2, source, LaserRayTerminator.Portal, depth, inglass, ignore, null, null);
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

						remaining.Push(Tuple.Create(newStart, newEnd, depth + 1, false, (object) outportal, ray));
					}
					continue;
				}
				#endregion

				// wud ???
				SAMLog.Error("LaserNetwork", string.Format("Ray collided with unkown fixture: {0}", result?.Item1?.UserData ?? "<NULL>"));
			}
		}

		private Tuple<Fixture, Vector2, Vector2> RayCast(World w, Vector2 start, Vector2 end, object udIgnore)
		{
			Tuple<Fixture, Vector2, Vector2> result = null;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			float rcCallback(Fixture f, Vector2 pos, Vector2 normal, float frac)
			{
				if (f.UserData is GlassBlock) return -1; // ignore;
				
				if (udIgnore != null && udIgnore == f.UserData) return -1; // ignore

				if (_wrapMode == GameWrapMode.Death && f.UserData is MarkerCollisionBorder) return -1; // ignore

				result = Tuple.Create(f, ConvertUnits.ToDisplayUnits(pos), normal);

				return frac; // limit
			}

			w.RayCast(rcCallback, ConvertUnits.ToSimUnits(start), ConvertUnits.ToSimUnits(end));

			return result;
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
					FPoint intersect;
					float u;
					float tmp;
					if (Math2D.LineIntersectionExt(ray1.Start, ray1.End, ray2.Start, ray2.End, -0.1f, out intersect, out u, out tmp))
					{
						if (src1 == minSrc2 && ray1 == minRay2) continue;

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
				if (src1 == minSrc2 || nofault)
				{
					ray1.SetSelfLaserIntersect(minP, minRay2, minSrc2);

					return true;
				}
				else
				{
					ray1.SetLaserIntersect(minP, minRay2, minSrc2);

					CutRayAndUpdate(minSrc2, minRay2, minP, src1, ray1);

					return true;
				}
			}
			
			
			return false;
		}

		private void CutRayAndUpdate(LaserSource src, LaserRay ray, FPoint intersect, LaserSource srcOther, LaserRay rayOther)
		{
			if (ray.Terminator == LaserRayTerminator.LaserDoubleTerm)
			{
				_faultRays.Add(ray.TerminatorRay);
			}

			ray.SetLaserIntersect(intersect, rayOther, srcOther);

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

					rem.Push(src.Lasers[i]);
					src.Lasers.RemoveAt(i);
					
				}
			}
			
		}
	}
}
