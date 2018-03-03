using System.Linq.Expressions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations
{
	class HUDSCCMScorePanelTimeChangeOperation : FixTimeOperation<HUDSCCMScorePanel_Won>
	{
		public override string Name => "HUDSCCMScorePanelTimeChange";
		private const float DURATION = 1.1f;

		private bool switched = false;

		public HUDSCCMScorePanelTimeChangeOperation() : base(DURATION)
		{

		}

		protected override void OnProgress(HUDSCCMScorePanel_Won element, float progress, SAMTime gameTime, InputState istate)
		{
			if (progress < 0.5f)
			{
				var p = (FloatMath.Cos(progress * 2 * FloatMath.PI)+1)/2;
				element.LabelTimeHeader.Alpha = p;
				element.LabelTimeValue.Alpha = p;
			}
			else
			{
				if (!switched) Switch(element);

				var p = (FloatMath.Cos(progress * 2 * FloatMath.PI)+1)/2;

				element.LabelTimeHeader.Alpha = p;
				element.LabelTimeValue.Alpha = p;
			}
		}

		protected override void OnStart(HUDSCCMScorePanel_Won element)
		{

		}

		protected override void OnEnd(HUDSCCMScorePanel_Won element)
		{
			if (!switched) Switch(element);

			element.LabelTimeHeader.Alpha = 1.0f;
			element.LabelTimeValue.Alpha = 1.0f;
		}

		private void Switch(HUDSCCMScorePanel_Won element)
		{
			switched = true;

			if (element.LevelDifficulty == element.SelectedDifficulty)
			{
				element.TimeDisplayMode = (element.TimeDisplayMode + 1) % 3;
			}
			else
			{
				element.TimeDisplayMode = (element.TimeDisplayMode + 1) % 2;
			}

			element.UpdateTDMLabels();
		}
	}
}
