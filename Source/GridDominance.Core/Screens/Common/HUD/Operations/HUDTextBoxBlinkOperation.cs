using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class HUDTextBoxBlinkOperation : FixTimeOperation<HUDTextBox>
	{
		public const int BLINK_COUNT = 4;
		public const float BLINK_LENGTH = 0.3f;

		private Color baseColor1;
		private Color baseColor2;
		private readonly Color highlightColor;

		public override string Name => "HUDTextBoxBlink";

		public HUDTextBoxBlinkOperation(Color highlight) : base(BLINK_COUNT * BLINK_LENGTH)
		{
			highlightColor = highlight;

		}

		protected override void OnStart(HUDTextBox element)
		{
			baseColor1 = element.ColorBackground;
			baseColor2 = element.ColorFocused;
		}

		protected override void OnEnd(HUDTextBox element)
		{
			element.ColorBackground = baseColor1;
			element.ColorFocused = baseColor2;
		}

		protected override void OnProgress(HUDTextBox owner, float progress, SAMTime gameTime, InputState istate)
		{
			var p = FloatMath.AbsSin(progress * (BLINK_COUNT / 2f) * FloatMath.TAU);
			owner.ColorBackground = ColorMath.Blend(baseColor1, highlightColor, p);
			owner.ColorFocused = ColorMath.Blend(baseColor2, highlightColor, p);
		}

		protected override void OnAbort(HUDTextBox element)
		{
			OnEnd(element);
		}
	}
}
