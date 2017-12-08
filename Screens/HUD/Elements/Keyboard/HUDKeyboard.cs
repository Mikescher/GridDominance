using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard
{
	public class HUDKeyboard : HUDContainer
	{
		public enum HUDKeyboardKeyMode { Normal, Caps, CapsLock, Alt }

		public static readonly Color COLOR_BACKGROUND   = new Color(  0,   0,   0);
		public static readonly Color COLOR_TEXT         = new Color(255, 255, 255);
		public static readonly Color COLOR_KEY          = new Color( 48,  47,  55);
		public static readonly Color COLOR_ALT          = new Color(128, 128, 128);
		public static readonly Color COLOR_SHIFT_ON     = new Color(  0, 255,  59);
		public static readonly Color COLOR_SHIFT_OFF    = new Color(192, 192, 192);
		public static readonly Color COLOR_KEY_PRESSED  = new Color( 51,  93, 135);
		public static readonly Color COLOR_PREVIEW_BG   = new Color(201, 201, 201);
		public static readonly Color COLOR_PREVIEW_TEXT = new Color(  0,   0,   0);

		public override int Depth { get; }

		private readonly IKeyboardListener _owner;

		private HUDLabel _prevLabel;

		public HUDKeyboardKeyMode KeyMode = HUDKeyboardKeyMode.Normal;

		public HUDKeyboard(IKeyboardListener owner, int depth = 0)
		{
			Focusable = false;
			Depth = depth;
			_owner = owner;
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

			_prevLabel = builder.AddPreviewLine();

			builder.Add('q',  'Q',  '1',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('w',  'W',  '2',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('e',  'E',  '3',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('r',  'R',  '4',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('t',  'T',  '5',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('y',  'Y',  '6',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('u',  'U',  '7',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('i',  'I',  '8',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('o',  'O',  '9',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('p',  'P',  '0',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Backspace);

			builder.NextRow();

			builder.Offset(0.5f);
			builder.Add('a',  'A',  '@',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('s',  'S',  '#',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('d',  'D',  '&',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('f',  'F',  '*',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('g',  'G',  '-',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('h',  'H',  '+',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('j',  'J',  '=',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('k',  'K',  '(',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('l',  'L',  ')',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Enter);

			builder.NextRow();

			builder.Add(null, null, null, 1f,   HUDKeyboardButtonType.Caps);
			builder.Add('z',  'Z',  '_',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('x',  'X',  '$',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('c',  'C',  '"',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('v',  'V',  '\'', 1f,   HUDKeyboardButtonType.Normal);
			builder.Add('b',  'B',  '/',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add('n',  'N',  '\\', 1f,   HUDKeyboardButtonType.Normal);
			builder.Add('m',  'M',  '%',  1f,   HUDKeyboardButtonType.Normal);
			builder.Add(',',  null,  '.', 1f,   HUDKeyboardButtonType.Normal);
			builder.Add('!',  null,  '?', 1f,   HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Caps);

			builder.NextRow();

			builder.Add(null, null, null, 3.2f, HUDKeyboardButtonType.Alt);
			builder.Add(' ',  null, null, 6.5f, HUDKeyboardButtonType.Normal);
			builder.Add(null, null, null, null, HUDKeyboardButtonType.Alt);

			builder.Finish();
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		
		public override void OnRemove()
		{
			_owner.KeyboardClosed();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (istate.IsKeyExclusiveJustDown(SKeys.AndroidBack))
			{
				istate.SwallowKey(SKeys.AndroidBack, InputConsumer.HUDElement);
				Remove();
			}

			if (_prevLabel.Text != _owner.GetPreviewText()) _prevLabel.Text = _owner.GetPreviewText();
		}

		public void AppendChar(char chr)
		{
			_owner.PressChar(chr);
		}

		public void Backspace()
		{
			_owner.PressBackspace();
		}

		public void Return()
		{
			Remove();
		}
	}
}
