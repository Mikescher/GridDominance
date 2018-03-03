using System;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class TetrisShrinkOperation : FixTimeOperation<OverworldNode_SCCM>
	{
		public override string Name => "Tetris::Shrink";

		public TetrisShrinkOperation(float duration) : base(duration)
		{
			//
		}

		protected override void OnStart(OverworldNode_SCCM node)
		{
			while (node.Blocks.Count>1) node.Blocks.RemoveAt(node.Blocks.Count-1);
			node.Blocks[0].Item1 = node.NonTranslatedBounds;
		}
		
		protected override void OnProgress(OverworldNode_SCCM node, float progress, SAMTime gameTime, InputState istate)
		{
			var a = CustomEase(progress) * (node.NonTranslatedBounds.Width/10f);
			node.Blocks[0].Item1 = new FRectangle(node.NonTranslatedBounds.X, node.NonTranslatedBounds.Bottom - a, a, a);
		}

		protected override void OnEnd(OverworldNode_SCCM node)
		{
			var a = node.NonTranslatedBounds.Width / 5f;
			node.Blocks[0].Item1 = new FRectangle(node.NonTranslatedBounds.X, node.NonTranslatedBounds.Bottom - a, a, a);
		}

		protected override void OnAbort(OverworldNode_SCCM node)
		{
			OnEnd(node);
		}

		private float CustomEase(float t)
		{
			var vstart = 8;
			var vend = 1;
			var power = 0.4f;
			var bounces = 2;
			var sbfm = 0.175f * bounces + 0.0875f - power / 4;
			var sbm = FloatMath.Sin(2 * sbfm * FloatMath.PI / power);
			var max = sbm * FloatMath.Pow(2, -10 * (0.175f * bounces + 0.0875f)) + 1;
			var tt = t * (0.175f * bounces + 0.0875f);
			var sb = FloatMath.Sin((tt - power / 4) * (2 * FloatMath.PI) / power);
			var r = (1 - (sb * FloatMath.Pow(2, -10 * tt) + 1) / max) * (vstart - vend) + vend + 2;

			r /= 1 + 0.5f * t;

			return r;
		}
	}
}
