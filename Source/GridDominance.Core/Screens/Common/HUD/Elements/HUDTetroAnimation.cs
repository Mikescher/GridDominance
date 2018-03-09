using System;
using System.Linq;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Tetromino;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
{
	class HUDTetroAnimation : HUDElement
	{
		public override int Depth { get; }

		private const float SLEEP_TIME      = 1.00f;
		private const float TRANSFORM1_TIME = 0.25f;
		private const float MINIMIZED_TIME  = 0.10f;
		private const float TRANSFORM2_TIME = 0.35f;
		private const float CYCLE_TIME     = SLEEP_TIME + TRANSFORM1_TIME + MINIMIZED_TIME + TRANSFORM2_TIME;
		
		private static readonly Vector2 VEC_HALF = new Vector2(0.5f, 0.5f);

		private readonly int[] _indexShuffle;
		private readonly float _seed;
		
		private float _iconrotation = 0;
		private readonly FPoint[] _tetroCenters = { FPoint.Zero, FPoint.Zero, FPoint.Zero, FPoint.Zero };

		public Color Foreground = FlatColors.Alizarin;

		public HUDTetroAnimation(float seed = 0, int depth = 0)
		{
			Depth = depth;
			_seed = seed;
			_indexShuffle = Enumerable.Range(0, TetroPieces.DISTINCT_ALPHABET.Length).Shuffle(new Random(seed.GetHashCode())).ToArray();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			int cycles = (int)((gameTime.TotalElapsedSeconds + _seed) / CYCLE_TIME);
			float cycletime =  (gameTime.TotalElapsedSeconds + _seed) - (CYCLE_TIME * cycles);

			var pieceCurr = TetroPieces.DISTINCT_ALPHABET[_indexShuffle[(cycles + 0) % _indexShuffle.Length]];
			var pieceNext = TetroPieces.DISTINCT_ALPHABET[_indexShuffle[(cycles + 1) % _indexShuffle.Length]];

			if (cycletime < (SLEEP_TIME))
			{
				// show curr
				_tetroCenters[0] = (pieceCurr.P1 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
				_tetroCenters[1] = (pieceCurr.P2 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
				_tetroCenters[2] = (pieceCurr.P3 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
				_tetroCenters[3] = (pieceCurr.P4 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
			}
			else if (cycletime < (SLEEP_TIME + TRANSFORM1_TIME))
			{
				// shrink curr
				var progress = FloatMath.Clamp((cycletime-SLEEP_TIME) / TRANSFORM1_TIME, 0, 1);
				
				_tetroCenters[0] = (pieceCurr.P1 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
				_tetroCenters[1] = (pieceCurr.P2 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
				_tetroCenters[2] = (pieceCurr.P3 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
				_tetroCenters[3] = (pieceCurr.P4 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
			}
			else if (cycletime < (SLEEP_TIME + TRANSFORM1_TIME + MINIMIZED_TIME))
			{
				// show dot
				_tetroCenters[0] = FPoint.Zero;
				_tetroCenters[1] = FPoint.Zero;
				_tetroCenters[2] = FPoint.Zero;
				_tetroCenters[3] = FPoint.Zero;
			}
			else
			{
				// grow next
				var progress = FloatMath.Clamp((cycletime-SLEEP_TIME-TRANSFORM1_TIME-MINIMIZED_TIME) / TRANSFORM2_TIME, 0, 1);
				
				_tetroCenters[0] = (pieceNext.P1 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
				_tetroCenters[1] = (pieceNext.P2 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
				_tetroCenters[2] = (pieceNext.P3 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
				_tetroCenters[3] = (pieceNext.P4 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
			}
			
			_iconrotation = 0.05f * gameTime.TotalElapsedSeconds * FloatMath.TAU;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var iconcenter = bounds.Center;

			for (int i = 0; i < 4; i++)
			{
				var pos = _tetroCenters[i].AsScaled(bounds.Width/4).WithOrigin(iconcenter).RotateAround(iconcenter, _iconrotation);

				sbatch.DrawCentered(Textures.TexPixel, pos, bounds.Width/4, bounds.Width/4, Foreground, _iconrotation);
			}
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}
	}
}
