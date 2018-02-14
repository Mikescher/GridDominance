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
			//
		}
		
		protected override void OnProgress(OverworldNode_SCCM owner, float progress, SAMTime gameTime, InputState istate)
		{
			//
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
