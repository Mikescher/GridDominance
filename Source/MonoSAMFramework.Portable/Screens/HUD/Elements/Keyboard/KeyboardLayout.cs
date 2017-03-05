using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using MonoSAMFramework.Portable.RenderHelper;

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

		public void Add(char? keyNormal, char? keyCaps, char? keyAlt, float? width, HUDKeyboardButtonType t)
		{
			var scale = _target.HUD.Width / _virtualWidth;

			var x = currentVirtualX;
			var y = _padNS + currentRow * _rowHeight + currentRow * _padKey;

			var w = width ?? (_virtualWidth - x - _padEW);
			var h = _rowHeight;

			var key = new HUDKeyboardButton();

			key.Alignment = HUDAlignment.TOPLEFT;
			key.RelativePosition = new FPoint(x * scale, y * scale);
			key.Size = new FSize(w * scale, h * scale);
			key.TextSize = scale * _rowHeight * 0.65f;
			key.CornerRadius = scale * 0.1f;

			switch (t)
			{
				case HUDKeyboardButtonType.Normal:
					key.Text = keyNormal.ToString();
					break;
				case HUDKeyboardButtonType.Caps:
					key.Icon = StaticTextures.KeyboardCaps;
					key.IconPadding = scale * 0.2f;
					break;
				case HUDKeyboardButtonType.Enter:
					key.Icon = StaticTextures.KeyboardEnter;
					key.IconPadding = scale * 0.2f;
					break;
				case HUDKeyboardButtonType.Backspace:
					key.Icon = StaticTextures.KeyboardBackspace;
					key.IconPadding = scale * 0.25f;
					break;
				case HUDKeyboardButtonType.Alt:
					key.Text = ".?123";
					key.TextSize = scale * _rowHeight * 0.55f;
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
			var scale = _target.HUD.Width / _virtualWidth;

			_target.Size = new FSize(_target.HUD.Width, (2 * _padNS + (currentRow + 1) * _rowHeight + currentRow * _padKey) * scale);
			_target.Alignment = HUDAlignment.BOTTOMLEFT;
		}
	}
}
