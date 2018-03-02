using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Input
{
	public abstract class HUDTextBox : HUDElement, IKeyboardListener
	{
		private const float CURSOR_BLINK_TIME = 0.5f;
		private const float PASSWORD_FADEOUT = 0.7f;

		public override int Depth { get; }

		#region Properties

		public string Placeholder = "";

		public string Text = "";
		public SpriteFont Font = null;
		public float FontSize = 12;
		public int Padding = 4;

		public Color ColorText = Color.Black;
		public Color ColorPlaceholder = Color.DimGray;

		public Color ColorBackground = Color.White;
		public Color ColorFocused = Color.White;

		public bool IsPassword = false;
		public int MaxLength = -1;

		public float CursorWidth = 2;

		#endregion

		public delegate void TextboxEventHandler(HUDTextBox sender, EventArgs e);

		public event TextboxEventHandler TextboxChanged;
		public event TextboxEventHandler TextboxEnterKeyPressed;

		public TextboxEventHandler Changed { set { TextboxChanged += value; } }
		public TextboxEventHandler EnterKey { set { TextboxEnterKeyPressed += value; } }

		private float _cursorBlinkTimer = 0;
		private float _lastCharAdd = 0;
		private bool _forceShowLastChar = false;

		protected HUDTextBox(int depth)
		{
			Depth = depth;
		}

		protected void DrawText(IBatchRenderer sbatch, FRectangle bounds, float leftOffset, float rightOffset)
		{
			var maxWidth = bounds.Width - leftOffset  - rightOffset - CursorWidth - Font.Spacing;
			var dispText = Text;

			if (IsPassword)
			{
				if (_forceShowLastChar && dispText.Length > 0)
				{
					dispText = new string('*', dispText.Length - 1) + dispText[dispText.Length - 1];
				}
				else
				{
					dispText = new string('*', dispText.Length);
				}
			}

			var textBounds = FontRenderHelper.MeasureStringCached(Font, dispText, FontSize);

			while (dispText.Length > 0 && textBounds.Width > maxWidth)
			{
				dispText = dispText.Substring(1);
				textBounds = FontRenderHelper.MeasureStringCached(Font, dispText, FontSize);
			}

			FontRenderHelper.DrawTextVerticallyCentered(
				sbatch, 
				Font, 
				FontSize, 
				dispText, 
				ColorText, 
				new FPoint(bounds.X + leftOffset, bounds.Y + bounds.Height / 2));

			if (IsFocused && (int) (_cursorBlinkTimer / CURSOR_BLINK_TIME) % 2 == 0)
				SimpleRenderHelper.DrawSimpleRect(
					sbatch, 
					new FRectangle(
						bounds.X + leftOffset + textBounds.Width + Font.Spacing, 
						bounds.Y + bounds.Height / 2 - FontSize / 2, 
						CursorWidth, 
						FontSize), 
					ColorText);
		}

		protected void DrawPlaceholder(IBatchRenderer sbatch, FRectangle bounds, float leftOffset, float rightOffset)
		{
			FontRenderHelper.DrawTextVerticallyCentered(
				sbatch, 
				Font, 
				FontSize, 
				Placeholder, 
				ColorPlaceholder,
				new FPoint(bounds.X + leftOffset, bounds.Y + bounds.Height / 2));
		}

		public override void OnInitialize()
		{
			if (Font == null) Font = HUD.DefaultFont;
		}

		public override void OnRemove()
		{
			//
		}
		
		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_cursorBlinkTimer += gameTime.ElapsedSeconds;

			if (IsFocused)
			{
				if (istate.IsKeyExclusiveJustDown(SKeys.Delete)) { istate.SwallowKey(SKeys.Delete, InputConsumer.HUDElement); PressBackspace(); }
				if (istate.IsKeyExclusiveJustDown(SKeys.Backspace)) { istate.SwallowKey(SKeys.Delete, InputConsumer.HUDElement); PressBackspace(); }
				if (istate.IsKeyExclusiveJustDown(SKeys.Enter)) { istate.SwallowKey(SKeys.Enter, InputConsumer.HUDElement); PressEnter(); }

				char? c = istate.GetCharExclusiveJustDownAndSwallow(InputConsumer.HUDElement);
				if (c != null) PressChar(c.Value);
			}

			_forceShowLastChar = IsFocused && (gameTime.TotalElapsedSeconds - _lastCharAdd) < PASSWORD_FADEOUT;
		}

		public override void FocusGain()
		{
			HUD.ShowKeyboard(this);
		}

		public override void FocusLoose()
		{
			HUD.HideKeyboard();
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			return true;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			return true;
		}

		public void PressChar(char chr)
		{
			if (MaxLength == -1 || Text.Length < MaxLength)
			{
				Text += chr;
				_lastCharAdd = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
				TextboxChanged?.Invoke(this, new EventArgs());
			}
		}

		public void PressBackspace()
		{
			if (Text.Length > 0)
			{
				Text = Text.Substring(0, Text.Length - 1);
				TextboxChanged?.Invoke(this, new EventArgs());
			}
		}

		public void PressEnter()
		{
			TextboxEnterKeyPressed?.Invoke(this, new EventArgs());
		}

		public void KeyboardClosed()
		{
			//
		}

		public string GetPreviewText() => IsPassword ? new string('*', Text.Length) : Text;
	}
}
