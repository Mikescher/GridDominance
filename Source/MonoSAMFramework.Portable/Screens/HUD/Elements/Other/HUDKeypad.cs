using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDKeypad : HUDWrapperContainer
	{
		public class HUDKeypadEventArgs : EventArgs
		{
			public readonly char Character;
			public HUDKeypadEventArgs(char c) { Character = c; }
		}

		public delegate void KeypadEventHandler(HUDKeypad sender, HUDKeypadEventArgs e);

		public event KeypadEventHandler PadClick;

		private HUDAlignment _buttonTextAlignment = HUDAlignment.CENTER;
		public HUDAlignment ButtonTextAlignment
		{
			get => _buttonTextAlignment;
			set { if (_buttonTextAlignment != value) { _buttonTextAlignment = value; InvalidateButtonState(); } }
		}

		private Color _textColor = Color.Black;
		public Color ButtonTextColor
		{
			get => _textColor;
			set { if (_textColor != value) { _textColor = value; InvalidateButtonState(); } }
		}

		private SpriteFont _font = null;
		public SpriteFont ButtonFont
		{
			get => _font;
			set { if (_font != value) { _font = value; InvalidateButtonState(); } }
		}

		private float _fontSize = 16;
		public float ButtonFontSize
		{
			get => _fontSize;
			set { if (_fontSize != value) { _fontSize = value; InvalidateButtonState(); } }
		}

		private Color _color = Color.White;
		public Color ButtonColor
		{
			get => _color;
			set { if (_color != value) { _color = value; InvalidateButtonState(); } }
		}

		private Color _colorPressed = Color.Magenta;
		public Color ButtonColorPressed
		{
			get => _colorPressed;
			set { if (_colorPressed != value) { _colorPressed = value; InvalidateButtonState(); } }
		}

		private HUDBackgroundType _backgroundType = HUDBackgroundType.Simple;
		public HUDBackgroundType ButtonBackgroundType
		{
			get => _backgroundType;
			set { if (_backgroundType != value) { _backgroundType = value; InvalidateButtonState(); } }
		}

		private float _backgoundCornerSize = 16f;
		public float ButtonBackgoundCornerSize
		{
			get => _backgoundCornerSize;
			set { if (_backgoundCornerSize != value) { _backgoundCornerSize = value; InvalidateButtonState(); } }
		}

		private readonly int _rows;
		private readonly int _cols;
		private readonly float _size;
		private readonly float _pad;

		private readonly HUDFixedUniformGrid _grid;

		private bool _buttonsInvalidated = false;

		public HUDKeypad(int columns, int rows, float btnSize, float pad, int depth = 0)
		{
			_rows = rows;
			_cols = columns;
			_size = btnSize;
			_pad = pad;

			SetElement(_grid = new HUDFixedUniformGrid
			{
				GridWidth = _cols,
				GridHeight = _rows,
				ColumnWidth = _size,
				RowHeight = _size,
				Padding = _pad,
			});
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			if (ButtonFont == null) ButtonFont = HUD.DefaultFont;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			if (_buttonsInvalidated)
			{
				SetButtonProperties();
			}
		}

		private void SetButtonProperties()
		{
			foreach (var btn in _grid.Children.OfType<HUDTextButton>())
			{
				btn.TextAlignment = ButtonTextAlignment;

				btn.Font = ButtonFont;
				btn.FontSize = ButtonFontSize;

				btn.TextColor = ButtonTextColor;

				btn.Color = ButtonColor;
				btn.ColorPressed = ButtonColorPressed;
				btn.BackgroundType = ButtonBackgroundType;
				btn.BackgroundCornerSize = ButtonBackgoundCornerSize;
			}

			_buttonsInvalidated = false;
		}

		public void AddKey(char c, int x, int y)
		{
			_grid.AddElement(x, y, new HUDTextButton
			{
				TextAlignment = ButtonTextAlignment,

				Font = ButtonFont,
				FontSize = ButtonFontSize,

				Text = c.ToString(),
				TextColor = ButtonTextColor,

				Color = ButtonColor,
				ColorPressed = ButtonColorPressed,
				BackgroundType = ButtonBackgroundType,
				BackgroundCornerSize = ButtonBackgoundCornerSize,

				Click = (s, e) => OnClickEvt(c),
			});
		}

		private void OnClickEvt(char c)
		{
			PadClick?.Invoke(this, new HUDKeypadEventArgs(c));
		}

		private void InvalidateButtonState()
		{
			_buttonsInvalidated = true;
		}
	}
}
