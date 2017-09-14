using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations
{
	class HUDScorePanelTimeDisplayOperation : HUDCyclicElementOperation<HUDScorePanel>
	{
		private const float CYCLE_TIME = 3.0f;

		private bool _mode = true;
		private readonly int    _text1_1;
		private readonly string _text1_2;
		private readonly int    _text2_1;
		private readonly string _text2_2;

		public override string Name => "HUDScorePanelTimeDisplayOperation";

		public HUDScorePanelTimeDisplayOperation(int txt11, string txt12, int txt21, string txt22) : base(CYCLE_TIME, false)
		{
			_text1_1 = txt11;
			_text1_2 = txt12;
			_text2_1 = txt21;
			_text2_2 = txt22;
		}

		protected override void OnStart(HUDScorePanel element)
		{
			//
		}

		protected override void OnEnd(HUDScorePanel element)
		{
			//
		}

		protected override void OnCycle(HUDScorePanel element, int counter)
		{
			if (_mode)
			{
				_mode = !_mode;
				element.AddHUDOperation(new HUDScorePanelTimeChangeOperation(_text2_1, _text2_2));
			}
			else
			{
				_mode = !_mode;
				element.AddHUDOperation(new HUDScorePanelTimeChangeOperation(_text1_1, _text1_2));
			}
		}
	}
}
