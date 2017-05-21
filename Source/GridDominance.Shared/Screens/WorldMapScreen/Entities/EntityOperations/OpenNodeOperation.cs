using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations
{
	class OpenNodeOperation : GameEntityOperation<LevelNode>
	{
		private readonly FractionDifficulty _diff;

		public OpenNodeOperation(FractionDifficulty extender) : base("LevelNode::Open::"+(int)extender, LevelNode.EXPANSION_TIME)
		{
			_diff = extender;
		}

		protected override void OnStart(LevelNode node)
		{
			node.State[(int)_diff] = BistateProgress.Opening;
		}

		protected override void OnProgress(LevelNode node, float progress, SAMTime gameTime, InputState istate)
		{
			node.ExpansionProgress[(int)_diff] = progress;
		}

		protected override void OnEnd(LevelNode node)
		{
			node.ExpansionProgress[(int) _diff] = 1f;
			node.State[(int)_diff] = BistateProgress.Open;
		}

		protected override void OnAbort(LevelNode node)
		{
			//
		}
	}
}
