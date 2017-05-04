using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations
{
	class CloseNodeOperation : GameEntityOperation<LevelNode>
	{
		private readonly FractionDifficulty _diff;

		public CloseNodeOperation(FractionDifficulty extender) : base("LevelNode::Close::"+(int)extender, LevelNode.CLOSING_TIME)
		{
			_diff = extender;
		}

		protected override void OnStart(LevelNode node)
		{
			node.State[(int)_diff] = BistateProgress.Closing;
		}

		protected override void OnProgress(LevelNode node, float progress, SAMTime gameTime, InputState istate)
		{
			node.ExpansionProgress[(int)_diff] = 1-progress;
		}

		protected override void OnEnd(LevelNode node)
		{
			node.State[(int)_diff] = BistateProgress.Closed;
		}

		protected override void OnAbort(LevelNode node)
		{
			//
		}
	}
}
