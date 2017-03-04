using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Input
{
	public abstract class HUDTextBox : HUDElement
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

		public float CursorWidth = 2;

		#endregion

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

			while (dispText.Length > 0 && textBounds.X > maxWidth)
			{
				dispText = Text.Substring(1);
				textBounds = FontRenderHelper.MeasureStringCached(Font, dispText, FontSize);
			}

			FontRenderHelper.DrawTextVerticallyCentered(
				sbatch, 
				Font, 
				FontSize, 
				dispText, 
				ColorText, 
				new Vector2(bounds.X + leftOffset, bounds.Y + bounds.Height / 2));

			if (IsFocused && (int) (_cursorBlinkTimer / CURSOR_BLINK_TIME) % 2 == 0)
				SimpleRenderHelper.DrawSimpleRect(
					sbatch, 
					new FRectangle(
						bounds.X + leftOffset + textBounds.X + Font.Spacing, 
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
				new Vector2(bounds.X + leftOffset, bounds.Y + bounds.Height / 2));
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
				if ((istate.IsKeyJustDown(SKeys.Delete) || istate.IsKeyJustDown(SKeys.Backspace)) && Text.Length > 0) Text = Text.Substring(0, Text.Length - 1);

				char? c = istate.GetCharJustDown();
				if (c != null)
				{
					Text += c;
					_lastCharAdd = gameTime.TotalElapsedSeconds;
				}
			}

			_forceShowLastChar = IsFocused && (gameTime.TotalElapsedSeconds - _lastCharAdd) < PASSWORD_FADEOUT;
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			return true;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			return true;
		}
	}
}
