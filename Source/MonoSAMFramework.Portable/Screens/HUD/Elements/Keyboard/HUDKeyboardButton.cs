using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using System;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard
{
	class HUDKeyboardButton : HUDElement
	{
		public override int Depth { get; } = 0;

		private readonly HUDKeyboard _owner;

		public float CornerRadius = 0f;
		public float TextSize = 1f;

		public string Text = "";
		public string TextAlt = null;
		public string TextCaps = null;

		public float IconPadding = 0f;
		public TextureRegion2D Icon = null;

		public float TextSizeAlt = 1f;
		public float TextAltPadding = 1f;
		public bool CapsMarker = false;
		public bool HighlightOnAlt = false;
		public bool HighlightOnCapsLock = false;

		public Action Event = null;

		public HUDKeyboardButton(HUDKeyboard owner)
		{
			Focusable = false;
			_owner = owner;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var hl =
				IsPointerDownOnElement ||
				(HighlightOnAlt && _owner.KeyMode == HUDKeyboard.HUDKeyboardKeyMode.Alt) ||
				(HighlightOnCapsLock && _owner.KeyMode == HUDKeyboard.HUDKeyboardKeyMode.CapsLock);

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, hl ? HUDKeyboard.COLOR_KEY_PRESSED : HUDKeyboard.COLOR_KEY, CornerRadius);

			switch (_owner.KeyMode)
			{
				case HUDKeyboard.HUDKeyboardKeyMode.Normal:
					InternalDraw(sbatch, bounds, Text, TextAlt);
					break;

				case HUDKeyboard.HUDKeyboardKeyMode.Caps:
				case HUDKeyboard.HUDKeyboardKeyMode.CapsLock:
					InternalDraw(sbatch, bounds, TextCaps ?? Text, TextAlt);
					break;

				case HUDKeyboard.HUDKeyboardKeyMode.Alt:
					InternalDraw(sbatch, bounds, TextAlt ?? Text, null);
					break;

				default:
					SAMLog.Error("HUDKB::EnumSwitch_DD", "KeyMode = " + _owner.KeyMode);
					break;
			}

			if (CapsMarker)
			{
				var cps = (_owner.KeyMode == HUDKeyboard.HUDKeyboardKeyMode.Caps) || (_owner.KeyMode == HUDKeyboard.HUDKeyboardKeyMode.CapsLock);

				sbatch.DrawCentered(
					StaticTextures.KeyboardCircle,
					bounds.TopLeft + new Vector2(CornerRadius, CornerRadius) * 1.2f,
					CornerRadius, CornerRadius,
					cps ? HUDKeyboard.COLOR_SHIFT_ON : HUDKeyboard.COLOR_SHIFT_OFF);
			}
		}

		private void InternalDraw(IBatchRenderer sbatch, FRectangle bounds, string textBase, string textAlt)
		{
			if (Icon != null)
			{
				var b = Icon.Size().Underfit(bounds.Size, IconPadding);
				sbatch.DrawCentered(Icon, bounds.Center, b.Width, b.Height, HUDKeyboard.COLOR_TEXT);
			}
			else
			{
				if (!string.IsNullOrEmpty(textBase))
				{
					FontRenderHelper.DrawTextCentered(sbatch, HUD.DefaultFont, TextSize, textBase, HUDKeyboard.COLOR_TEXT, bounds.Center);
				}
				if (!string.IsNullOrEmpty(textAlt))
				{
					FontRenderHelper.DrawTextTopRight(sbatch, HUD.DefaultFont, TextSizeAlt, textAlt, HUDKeyboard.COLOR_ALT, bounds.TopRight + new Vector2(-TextAltPadding, 0));
				}
			}
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			HUD.Screen.Game.Sound.TryPlayButtonKeyboardClickEffect();

			Event?.Invoke();
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
