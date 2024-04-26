using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard
{
	public class KeyboardLayout
	{
		private readonly HUDKeyboard _target;
		private readonly float _virtualWidth;
		private readonly float _padKey;
		private readonly float _padNS;
		private readonly float _padEW;
		private readonly float _rowHeight;

		private float currentVirtualX = 0f;
		private int currentRow = 0;
		
		public KeyboardLayout(HUDKeyboard target, float virtWidth, float padKey, float padTopBottom, float padRightLeft, float rowHeight)
		{
			_target = target;
			_virtualWidth = virtWidth;
			_padKey = padKey;
			_padNS = padTopBottom;
			_padEW = padRightLeft;
			_rowHeight = rowHeight;

			currentVirtualX = padRightLeft;
		}

		public void Offset(float width)
		{
			currentVirtualX += width;
		}

		public HUDLabel AddPreviewLine()
		{
			var scale = _target.HUD.SafeWidth / _virtualWidth;

			var x = currentVirtualX;
			var y = _padNS + currentRow * _rowHeight + currentRow * _padKey;

			var w = _virtualWidth - x - _padEW;
			var h = _rowHeight;

			var pnl = new HUDRectangle(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(_target.HUD.SafetyMargins.MarginLeft + x * scale, y * scale),
				Size = new FSize(w * scale, h * scale),

				Definition = HUDBackgroundDefinition.CreateRounded(HUDKeyboard.COLOR_PREVIEW_BG, scale * 0.1f),

			};
			_target.AddElement(pnl);

			var padH = h * 0.25f;
			var padV = h * 0.1f;

			var lbl = new HUDLabel(2)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(_target.HUD.SafetyMargins.MarginLeft + (x+padH) * scale, (y+padV) * scale),
				Size = new FSize((w-2*padH) * scale, (h-2*padV) * scale),
				TextColor = HUDKeyboard.COLOR_PREVIEW_TEXT,
				FontSize = (h - 2 * padV) * scale,
				TextAlignment = HUDAlignment.CENTERLEFT,
			};
			_target.AddElement(lbl);

			NextRow();

			return lbl;
		}

		public void Add(char? keyNormal, char? keyCaps, char? keyAlt, float? width, HUDKeyboardButtonType t)
		{
			var scale = _target.HUD.SafeWidth / _virtualWidth;

			var x = currentVirtualX;
			var y = _padNS + currentRow * _rowHeight + currentRow * _padKey;

			var w = width ?? (_virtualWidth - x - _padEW);
			var h = _rowHeight;

			var key = new HUDKeyboardButton(_target);

			key.Alignment = HUDAlignment.TOPLEFT;
			key.RelativePosition = new FPoint(_target.HUD.SafetyMargins.MarginLeft + x * scale, y * scale);
			key.Size = new FSize(w * scale, h * scale);
			key.TextSize = scale * h * 0.65f;
			key.CornerRadius = scale * 0.1f;

			switch (t)
			{
				case HUDKeyboardButtonType.Normal:
					key.Text = keyNormal.ToString();
					if (!keyNormal.HasValue) throw new ArgumentException(nameof(keyNormal));
					key.Event = () => PressKeyNormal(keyNormal.Value, keyCaps, keyAlt);
					break;
				case HUDKeyboardButtonType.Caps:
					key.Icon = StaticTextures.KeyboardCaps;
					key.IconPadding = scale * 0.2f;
					key.CapsMarker = true;
					key.HighlightOnCapsLock = true;
					key.Event = PressKeyCaps;
					break;
				case HUDKeyboardButtonType.Enter:
					key.Icon = StaticTextures.KeyboardEnter;
					key.IconPadding = scale * 0.2f;
					key.Event = PressKeyEnter;
					break;
				case HUDKeyboardButtonType.Backspace:
					key.Icon = StaticTextures.KeyboardBackspace;
					key.IconPadding = scale * 0.25f;
					key.Event = PressKeyBackspace;
					break;
				case HUDKeyboardButtonType.Alt:
					key.Text = ".?123";
					key.TextSize = scale * _rowHeight * 0.55f;
					key.HighlightOnAlt = true;
					key.Event = PressKeyAlt;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}

			if (keyAlt != null)
			{
				key.TextAlt = keyAlt.ToString();
				key.TextSizeAlt = scale * _rowHeight * 0.32f;
				key.TextAltPadding = scale * _rowHeight * 0.1f;
			}
			if (keyCaps != null)
			{
				key.TextCaps = keyCaps.ToString();
			}

			_target.AddElement(key);

			currentVirtualX += w + _padKey;
		}

		public void NextRow()
		{
			currentVirtualX = _padEW;
			currentRow++;
		}

		public void Finish()
		{
			var scale = _target.HUD.SafeWidth / _virtualWidth;

			_target.Size = new FSize(_target.HUD.RealWidth, (2 * _padNS + (currentRow + 1) * _rowHeight + currentRow * _padKey) * scale + _target.HUD.SafetyMargins.MarginBottom);
			_target.Alignment = HUDAlignment.BOTTOMLEFT;
		}

		private void PressKeyCaps()
		{
			switch (_target.KeyMode)
			{
				case HUDKeyboard.HUDKeyboardKeyMode.Normal:
					_target.KeyMode = HUDKeyboard.HUDKeyboardKeyMode.Caps;
					break;
				case HUDKeyboard.HUDKeyboardKeyMode.Caps:
					_target.KeyMode = HUDKeyboard.HUDKeyboardKeyMode.CapsLock;
					break;
				case HUDKeyboard.HUDKeyboardKeyMode.CapsLock:
					_target.KeyMode = HUDKeyboard.HUDKeyboardKeyMode.Normal;
					break;
				case HUDKeyboard.HUDKeyboardKeyMode.Alt:
					_target.KeyMode = HUDKeyboard.HUDKeyboardKeyMode.Caps;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void PressKeyAlt()
		{
			_target.KeyMode = (_target.KeyMode == HUDKeyboard.HUDKeyboardKeyMode.Alt) ? HUDKeyboard.HUDKeyboardKeyMode.Normal : HUDKeyboard.HUDKeyboardKeyMode.Alt;
		}

		private void PressKeyEnter()
		{
			_target.Return();
		}

		private void PressKeyBackspace()
		{
			_target.Backspace();
		}

		private void PressKeyNormal(char keyNormal, char? keyCaps, char? keyAlt)
		{
			switch (_target.KeyMode)
			{
				case HUDKeyboard.HUDKeyboardKeyMode.Normal:
					_target.AppendChar(keyNormal);
					break;
				case HUDKeyboard.HUDKeyboardKeyMode.Caps:
					_target.AppendChar(keyCaps ?? keyNormal);
					_target.KeyMode = HUDKeyboard.HUDKeyboardKeyMode.Normal;
					break;
				case HUDKeyboard.HUDKeyboardKeyMode.CapsLock:
					_target.AppendChar(keyCaps ?? keyNormal);
					break;
				case HUDKeyboard.HUDKeyboardKeyMode.Alt:
					if (keyAlt != null) _target.AppendChar(keyAlt.Value);
					break;
				default:
					SAMLog.Error("HUDKB::EnumSwitch_PKN", "KeyMode = " + _target.KeyMode);
					break;
			}
		}
	}
}
