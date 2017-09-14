using System;
using System.Collections.Generic;
using System.Text;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations
{
	class HUDScorePanelTimeChangeOperation : HUDTimedElementOperation<HUDScorePanel>
	{
		public override string Name => "HUDScorePanelTimeChangeOperation";
		private const float DURATION = 1.1f;

		private readonly int _txt1;
		private readonly string _txt2;

		public HUDScorePanelTimeChangeOperation(int newtext1, string newtext2) : base(DURATION)
		{
			_txt1 = newtext1;
			_txt2 = newtext2;
		}

		protected override void OnProgress(HUDScorePanel element, float progress, InputState istate)
		{
			if (progress < 0.5f)
			{
				var p = (FloatMath.Cos(progress * 2 * FloatMath.PI)+1)/2;
				element.LabelTime1.Alpha = p;
				element.LabelTime2.Alpha = p;
			}
			else
			{
				var p = (FloatMath.Cos(progress * 2 * FloatMath.PI)+1)/2;

				element.LabelTime1.L10NText = _txt1;
				element.LabelTime2.Text = _txt2;

				element.LabelTime1.Alpha = p;
				element.LabelTime2.Alpha = p;
			}
		}

		protected override void OnStart(HUDScorePanel element)
		{

		}

		protected override void OnEnd(HUDScorePanel element)
		{
			element.LabelTime1.L10NText = _txt1;
			element.LabelTime2.Text = _txt2;

			element.LabelTime1.Alpha = 1.0f;
			element.LabelTime2.Alpha = 1.0f;
		}
	}
}
