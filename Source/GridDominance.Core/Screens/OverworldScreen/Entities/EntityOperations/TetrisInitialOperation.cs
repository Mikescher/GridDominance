using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class TetrisInitialOperation : FixTimeOperation<OverworldNode_SCCM>
	{
		public override string Name => "Tetris::Initial";

		public TetrisInitialOperation(float duration) : base(duration)
		{

		}

		protected override void OnStart(OverworldNode_SCCM node)
		{
			node.Blocks.Add(new MutableTuple<FRectangle, Color>(new FRectangle(
				node.NonTranslatedBounds.X,
				node.NonTranslatedBounds.Bottom - (node.NonTranslatedBounds.Width / 5f),
				node.NonTranslatedBounds.Width / 5f,
				node.NonTranslatedBounds.Width / 5f
			), Color.White));

			node.Blocks[0].Item1 = FRectangle.CreateByCenter(node.Blocks[0].Item1.Center, 0, 0);
		}
		
		protected override void OnProgress(OverworldNode_SCCM node, float progress, SAMTime gameTime, InputState istate)
		{
			node.Blocks[0].Item1 = FRectangle.CreateByCenter(
				node.Blocks[0].Item1.Center, 
				FloatMath.FunctionEaseInOutCubic(progress) * node.NonTranslatedBounds.Width / 5f,
				FloatMath.FunctionEaseInOutCubic(progress) * node.NonTranslatedBounds.Width / 5f);
		}

		protected override void OnEnd(OverworldNode_SCCM node)
		{
			node.Blocks.Add(new MutableTuple<FRectangle, Color>(new FRectangle(
				node.NonTranslatedBounds.X,
				node.NonTranslatedBounds.Bottom - (node.NonTranslatedBounds.Width / 5f),
				node.NonTranslatedBounds.Width / 5f,
				node.NonTranslatedBounds.Width / 5f
			), Color.White));
		}

		protected override void OnAbort(OverworldNode_SCCM node)
		{
			OnEnd(node);
		}
	}
}
