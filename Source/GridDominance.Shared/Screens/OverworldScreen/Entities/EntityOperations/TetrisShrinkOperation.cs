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

		}

		protected override void OnStart(OverworldNode_SCCM node)
		{
			while (node.Blocks.Count>1) node.Blocks.RemoveAt(node.Blocks.Count-1);
			node.Blocks[0].Item1 = node.LastInnerBounds;
		}
		
		protected override void OnProgress(OverworldNode_SCCM node, float progress, SAMTime gameTime, InputState istate)
		{
			var a = node.LastInnerBounds.Width * (4 / 5f) * progress;

			node.Blocks[0].Item1 = node.LastInnerBounds.AsDeflated(a, a, 0, 0);
		}

		protected override void OnEnd(OverworldNode_SCCM node)
		{
			//
		}

		protected override void OnAbort(OverworldNode_SCCM node)
		{
			OnEnd(node);
		}
	}
}
