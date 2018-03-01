using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Tetromino;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents;
using System.Linq;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class SCCMDisplayAnimationOperation : SAMUpdateOp<SCCMScoreDisplay>
	{
		private static readonly int[] INDEX_SHUFFLE = Enumerable.Range(0, TetroPieces.DISTINCT_ALPHABET.Length).Shuffle().ToArray();

		private const float SLEEP_TIME      = 1.00f;
		private const float TRANSFORM1_TIME = 0.25f;
		private const float MINIMIZED_TIME  = 0.10f;
		private const float TRANSFORM2_TIME = 0.35f;
		private const float CYCLE_TIME     = SLEEP_TIME + TRANSFORM1_TIME + MINIMIZED_TIME + TRANSFORM2_TIME;

		private readonly static Vector2 VEC_HALF = new Vector2(0.5f, 0.5f);

		public override string Name => "SCCMDisplayAnimation";

		public SCCMDisplayAnimationOperation() : base()
		{
		}

		protected override void OnUpdate(SCCMScoreDisplay owner, SAMTime gameTime, InputState istate)
		{
			int cycles = (int)(gameTime.TotalElapsedSeconds / CYCLE_TIME);
			float cycletime =  gameTime.TotalElapsedSeconds - (CYCLE_TIME * cycles);

			var pieceCurr = TetroPieces.DISTINCT_ALPHABET[INDEX_SHUFFLE[(cycles + 0) % INDEX_SHUFFLE.Length]];
			var pieceNext = TetroPieces.DISTINCT_ALPHABET[INDEX_SHUFFLE[(cycles + 1) % INDEX_SHUFFLE.Length]];

			if (cycletime < (SLEEP_TIME))
			{
				// show curr
				owner.TetroCenters[0] = (pieceCurr.P1 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
				owner.TetroCenters[1] = (pieceCurr.P2 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
				owner.TetroCenters[2] = (pieceCurr.P3 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
				owner.TetroCenters[3] = (pieceCurr.P4 + VEC_HALF).RelativeTo(pieceCurr.RealCenter);
			}
			else if (cycletime < (SLEEP_TIME + TRANSFORM1_TIME))
			{
				// shrink curr
				var progress = FloatMath.Clamp((cycletime-SLEEP_TIME) / TRANSFORM1_TIME, 0, 1);
				
				owner.TetroCenters[0] = (pieceCurr.P1 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
				owner.TetroCenters[1] = (pieceCurr.P2 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
				owner.TetroCenters[2] = (pieceCurr.P3 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
				owner.TetroCenters[3] = (pieceCurr.P4 + VEC_HALF).RelativeTo(pieceCurr.RealCenter).AsScaled(1-progress);
			}
			else if (cycletime < (SLEEP_TIME + TRANSFORM1_TIME + MINIMIZED_TIME))
			{
				// show dot
				owner.TetroCenters[0] = FPoint.Zero;
				owner.TetroCenters[1] = FPoint.Zero;
				owner.TetroCenters[2] = FPoint.Zero;
				owner.TetroCenters[3] = FPoint.Zero;
			}
			else
			{
				// grow next
				var progress = FloatMath.Clamp((cycletime-SLEEP_TIME-TRANSFORM1_TIME-MINIMIZED_TIME) / TRANSFORM2_TIME, 0, 1);
				
				owner.TetroCenters[0] = (pieceNext.P1 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
				owner.TetroCenters[1] = (pieceNext.P2 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
				owner.TetroCenters[2] = (pieceNext.P3 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
				owner.TetroCenters[3] = (pieceNext.P4 + VEC_HALF).RelativeTo(pieceNext.RealCenter).AsScaled(progress);
			}


		}
	}
}
