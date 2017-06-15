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

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserNetwork : ISAMUpdateable, ISAMDrawable
	{
		public const int MAX_LASER_RAYCOUNT   = 16;
		public const int MAX_LASER_PER_SOURCE = 128; // 8 * 16

		public bool Dirty = false;
		
		private readonly GDGameScreen _screen;
		private readonly World _world;
		private readonly float _worldWidth;
		private readonly float _worldHeight;
		private readonly GameWrapMode _wrapMode;

		public List<LaserSource> Sources = new List<LaserSource>();

		public LaserNetwork(World b2dworld, GDGameScreen scrn, GameWrapMode wrap)
		{
			_screen = scrn;
			_world = b2dworld;

			_worldWidth  = scrn.Blueprint.LevelWidth;
			_worldHeight = scrn.Blueprint.LevelHeight;
			_wrapMode = wrap;
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			if (!Dirty) return;

#if DEBUG
			DebugUtils.TIMING_LASER.Start();
#endif

			for (int i = 0; i < Sources.Count; i++)
			{
				Sources[i].LaserCount = 0;
			}

			for (int i = 0; i < Sources.Count; i++)
			{
				RecalcSource(Sources[i]);
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

				for (int i = 0; i < src.LaserCount; i++)
				{
					sbatch.DrawLine(src.Lasers[i].Start, src.Lasers[i].End, Color.LimeGreen, 4);
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
			if (!src.LaserActive) { src.LaserCount = 0; return; }

			var raylen = _worldWidth + _worldHeight;

			Vector2 istart = src.Position;
			Vector2 iend   = src.Position + new Vector2(raylen,0).Rotate(src.LaserRotation);

			Stack<Tuple<Vector2, Vector2, int, bool, object>> remaining = new Stack<Tuple<Vector2, Vector2, int, bool, object>>();

			remaining.Push(Tuple.Create(istart, iend, 0, false, src.UserData));

			int arridx = 0;
			while (remaining.Any())
			{
				var pop     = remaining.Pop();
				var start   = pop.Item1;
				var end     = pop.Item2;
				var depth   = pop.Item3;
				var inglass = pop.Item4;
				var ignore  = pop.Item5;

				for (int dd = depth; dd < MAX_LASER_RAYCOUNT; dd++)
				{
					if (arridx + 1 >= MAX_LASER_PER_SOURCE) break;

					var result = RayCast(_world, start, end, ignore);

					#region OOB
					if (result == null)
					{
						src.LaserCount = arridx+1;
						src.Lasers[arridx].Start  = start;
						src.Lasers[arridx].End    = end;
						src.Lasers[arridx].Target = null;
						arridx++;

						break;
					}
					#endregion

					#region Cannon
					var resultCannon = result.Item1.UserData as Cannon;
					if (resultCannon != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start  = start;
						src.Lasers[arridx].End    = result.Item2;
						src.Lasers[arridx].Target = resultCannon;
						arridx++;

						break;
					}
					#endregion

					#region GlassBlockRefraction
					var resultGlassBlockRefrac = result.Item1.UserData as MarkerRefractionEdge;
					if (resultGlassBlockRefrac != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start = start;
						src.Lasers[arridx].End = result.Item2;
						src.Lasers[arridx].Target = null;
						arridx++;

						// sin(aIn) / sin(aOut) = currRefractIdx / Glass.RefractIdx
						var aIn = (start - end).ToAngle() - result.Item3.ToAngle();
						var n = inglass ? (GlassBlock.REFRACTION_INDEX / 1f) : (1f / GlassBlock.REFRACTION_INDEX);

						var sinaOut = FloatMath.Sin(aIn) * n;

						var isRefracting = sinaOut < 1 && sinaOut > -1;
						if (isRefracting) // refraction
						{
							var aOut = FloatMath.Asin(sinaOut);
							var pRefractAngle = (-result.Item3).ToAngle() + aOut;

							remaining.Push(Tuple.Create(result.Item2, result.Item2 + new Vector2(raylen, 0).Rotate(pRefractAngle), dd+1, !inglass, (object)resultGlassBlockRefrac));
						}

						if (!inglass) 
						{
							start = result.Item2;
							end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(raylen);
							ignore = resultGlassBlockRefrac;

							continue; // reflection
						}
						else
						{
							if (isRefracting)
							{
								break; // no reflection in glass
							}
							else
							{
								start = result.Item2;
								end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(raylen);
								ignore = resultGlassBlockRefrac;

								continue; // reflection
							}
						}
					}
					#endregion
					
					#region MirrorBlock
					var resultMirrorBlock = result.Item1.UserData as MirrorBlock;
					if (resultMirrorBlock != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start = start;
						src.Lasers[arridx].End = result.Item2;
						src.Lasers[arridx].Target = null;
						arridx++;

						start = result.Item2;
						end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(raylen);
						ignore = resultMirrorBlock;

						continue;
					}
					#endregion

					#region MirrorCircle
					var resultMirrorCircle = result.Item1.UserData as MirrorCircle;
					if (resultMirrorCircle != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start = start;
						src.Lasers[arridx].End = result.Item2;
						src.Lasers[arridx].Target = null;
						arridx++;

						start = result.Item2;
						end = result.Item2 + Vector2.Reflect(end - start, result.Item3).WithLength(raylen);
						ignore = resultMirrorCircle;

						continue;
					}
					#endregion

					#region VoidWall
					var resultVoidWall = result.Item1.UserData as VoidWall;
					if (resultVoidWall != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start = start;
						src.Lasers[arridx].End = result.Item2;
						src.Lasers[arridx].Target = null;
						arridx++;

						break;
					}
					#endregion

					#region VoidCircle
					var resultVoidCircle = result.Item1.UserData as VoidCircle;
					if (resultVoidCircle != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start = start;
						src.Lasers[arridx].End = result.Item2;
						src.Lasers[arridx].Target = null;
						arridx++;

						break;
					}
					#endregion

					#region Portal
					var resultPortal = result.Item1.UserData as Portal;
					if (resultPortal != null)
					{
						src.LaserCount = arridx + 1;
						src.Lasers[arridx].Start = start;
						src.Lasers[arridx].End = result.Item2;
						src.Lasers[arridx].Target = null;
						arridx++;

						
						var inPortal = resultPortal;
						var normal = result.Item3;
						bool hit = FloatMath.DiffRadiansAbs(normal.ToAngle(), inPortal.Normal) < FloatMath.RAD_POS_001;

						if (!hit) break; // back-side hit

						if (inPortal.Links.Count == 0) break; // void portal

						foreach (var outportal in inPortal.Links)
						{
							var rot = outportal.Normal - inPortal.Normal + FloatMath.RAD_POS_180;
							var projec = result.Item2.ProjectOntoLine(inPortal.Position, inPortal.VecDirection);

							var newVelocity = (end-start).Rotate(rot);
							var newStart = outportal.Position + outportal.VecDirection * (-projec) + outportal.VecNormal * (Portal.WIDTH / 2f);

							var newEnd = newStart + newVelocity.WithLength(raylen);


							remaining.Push(Tuple.Create(newStart, newEnd, dd+1, false, (object)outportal));
						}
						break;
					}
					#endregion

					// wud ???
					SAMLog.Error("LaserNetwork", string.Format("Ray collided with unkown fixture: {0}", result?.Item1?.UserData ?? "<NULL>"));
					break;
				}
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
	}
}
