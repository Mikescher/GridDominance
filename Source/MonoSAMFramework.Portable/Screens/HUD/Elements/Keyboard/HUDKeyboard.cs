using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard
{
	public class HUDKeyboard : HUDContainer
	{
		public static readonly Color COLOR_KEY = new Color(48, 47, 55);
		public static readonly Color COLOR_BACKGROUND = Color.Black;
		public static readonly Color COLOR_TEXT = Color.White;
		public static readonly Color COLOR_ALT = new Color(128, 128, 128);

		public const int VIRTUAL_WIDTH = 1366;

		public const int ROW_HEIGHT    = 84;
		public const int COL_WIDTH     = 100;
		public const int PADDING       = 10;
		public const int PADDING_OUTER = 28;


		public override int Depth { get; }

		public HUDKeyboard(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.FillRectangle(bounds, COLOR_BACKGROUND);
		}

		public override void OnInitialize()
		{
			CreateKeys();
		}

		private void CreateKeys()
		{
			var builder = new KeyboardLayout(this, 12.5f, 0.1f, 0.1f, 0.28f, 0.84f);

			builder.Add('q', 'Q', '1', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('w', 'W', '2', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('e', 'E', '3', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('r', 'R', '4', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('t', 'T', '5', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('y', 'Y', '6', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('u', 'U', '7', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('i', 'I', '8', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('o', 'O', '9', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('p', 'P', '0', 1f, HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Backspace);

			builder.NextRow();

			builder.Offset(0.5f);
			builder.Add('a', 'A', '@', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('s', 'S', '#', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('d', 'D', '&', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('f', 'F', '*', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('g', 'G', '-', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('h', 'H', '+', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('j', 'J', '=', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('k', 'K', '(', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('l', 'L', ')', 1f, HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Enter);

			builder.NextRow();

			builder.Add(null, null, null,  1f, HUDKeyboardButtonType.Caps);
			builder.Add('z', 'Z', '_',  1f, HUDKeyboardButtonType.Normal);
			builder.Add('x', 'X', '$',  1f, HUDKeyboardButtonType.Normal);
			builder.Add('c', 'C', '"',  1f, HUDKeyboardButtonType.Normal);
			builder.Add('v', 'V', '\'', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('b', 'B', '/',  1f, HUDKeyboardButtonType.Normal);
			builder.Add('n', 'N', '\\', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('m', 'M', '%',  1f, HUDKeyboardButtonType.Normal);
			builder.Add(',', null, '.', 1f, HUDKeyboardButtonType.Normal);
			builder.Add('!', null, '?', 1f, HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Caps);

			builder.NextRow();

			builder.Add(null, null, null, 3.2f, HUDKeyboardButtonType.Alt);
			builder.Add(' ', null, null, 6.5f, HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Alt);

			builder.Finish();
		}

		private void AddKey(char chr, int col, int row, float widthFactor)
		{
			var scale = HUD.Width / VIRTUAL_WIDTH;

			var x = PADDING_OUTER + col * (COL_WIDTH) + (col - 1) * PADDING;
			var y = PADDING + row * (PADDING + ROW_HEIGHT);

			var key = new HUDKeyboardButton();

			key.Alignment = HUDAlignment.TOPLEFT;
			key.RelativePosition = new FPoint(x * scale, y * scale);
			key.Size = new FSize(COL_WIDTH * widthFactor * scale, ROW_HEIGHT * scale);

			AddElement(key);
		}

		public override void OnRemove()
		{

		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{

		}
	}
}
