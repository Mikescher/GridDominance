using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations
{
	class HUDSCCMScorePanelTimeDisplayOperation : CyclicOperation<HUDSCCMScorePanel_Won>
	{
		private const float CYCLE_TIME = 3.0f;

		public override string Name => "HUDSCCMScorePanelTimeDisplayOperation";

		public HUDSCCMScorePanelTimeDisplayOperation() : base(CYCLE_TIME, false)
		{

		}

		protected override void OnCycle(HUDSCCMScorePanel_Won element, int counter)
		{
			element.AddOperation(new HUDSCCMScorePanelTimeChangeOperation());
		}
	}
}
