using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.Common.HUD.HUDOperations
{
    class HUDTextBoxBlinkOperation : HUDTimedElementOperation<HUDTextBox>
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

	    protected override void OnProgress(HUDTextBox element, float progress, InputState istate)
	    {
		    var p = FloatMath.AbsSin(progress * (BLINK_COUNT / 2f) * FloatMath.TAU);
		    element.ColorBackground = ColorMath.Blend(baseColor1, highlightColor, p);
		    element.ColorFocused = ColorMath.Blend(baseColor2, highlightColor, p);
		}
    }
}
