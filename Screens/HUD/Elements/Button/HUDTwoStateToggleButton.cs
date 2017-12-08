using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDTwoStateToggleButton : HUDElement
	{
		private const float TRANSITION_TIME = 0.2f;
		private const float KNOB_SCALE = 0.75f;

		public override int Depth { get; }

		#region Properties

		public bool ToggleState = false;

		public Color ColorBackground = Color.White;

		public string TextStateOn  = string.Empty;
		public string TextStateOff = string.Empty;

		public Color ColorStateOn  = Color.Magenta;
		public Color ColorStateOff = Color.Magenta;

		public Color FontColor = Color.Black;
		public SpriteFont Font = null;

		#endregion

		#region Events

		public delegate void ToggleButtonEventHandler(HUDTwoStateToggleButton sender, HUDToggleButtonEventArgs e);

		public event ToggleButtonEventHandler ButtonClick;

		public ToggleButtonEventHandler Click { set { ButtonClick += value; } }

		#endregion

		private float _knobPosition = 0f;

		public HUDTwoStateToggleButton(int depth = 0)
		{
			Depth = depth;
		}

		public void ForceStateWithoutAnimation(bool newState)
		{
			ToggleState = newState;
			_knobPosition = ToggleState ? 1f : 0f;
		}

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (!IsEnabled) return;

			ToggleState = !ToggleState;
			ButtonClick?.Invoke(this, new HUDToggleButtonEventArgs(ToggleState));

			HUD.Screen.Game.Sound.TryPlayButtonClickEffect();
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			return IsVisible;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			return IsVisible;
		}

		public override void OnInitialize()
		{
			if (Font == null) Font = HUD.DefaultFont;
			_knobPosition = ToggleState ? 1f : 0f;
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var radiusOuter = Height / 2f;
			var radiusInner = radiusOuter * KNOB_SCALE;

			var knobPosition = new FPoint(bounds.Left + radiusOuter + (Width - 2 * radiusOuter) * _knobPosition, bounds.CenterY);
			var knobColor = ColorMath.Blend(ColorStateOff, ColorStateOn, _knobPosition);

			var textHeight = Height / 2;

			// Background
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, ColorBackground, radiusOuter);

			// Text[On]
			if (!string.IsNullOrWhiteSpace(TextStateOn) && _knobPosition > 0)
			{
				var pos = new FPoint(bounds.Left + radiusOuter/2, bounds.CenterY);
				var alpha = FloatMath.Clamp(1.5f * _knobPosition, 0f, 1f);
				if (alpha > 0) FontRenderHelper.DrawTextVerticallyCentered(sbatch, Font, textHeight, TextStateOn, FontColor * alpha, pos);
			}

			// Text[Off]
			if (!string.IsNullOrWhiteSpace(TextStateOff) && _knobPosition < 1)
			{
				var pos = new FPoint(bounds.Right - radiusOuter / 2, bounds.CenterY);
				var alpha = FloatMath.Clamp(1 - 1.5f * _knobPosition, 0f, 1f);
				if (alpha > 0) FontRenderHelper.DrawTextVerticallyCenteredRightAligned(sbatch, Font, textHeight, TextStateOff, FontColor * alpha, pos);
			}

			// Knob
			sbatch.DrawCentered(StaticTextures.MonoCircle, knobPosition, radiusInner * 2, radiusInner * 2, knobColor);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (ToggleState == false && FloatMath.IsNotZero(_knobPosition))
			{
				_knobPosition -= gameTime.ElapsedSeconds / TRANSITION_TIME;
				if (_knobPosition <= 0f) _knobPosition = 0f;
			}
			else if (ToggleState == true && FloatMath.IsNotOne(_knobPosition))
			{
				_knobPosition += gameTime.ElapsedSeconds / TRANSITION_TIME;
				if (_knobPosition >= 1f) _knobPosition = 1f;
			}
		}
	}
}
