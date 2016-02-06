using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.ViewportAdapters;

namespace GridDominance.Shared.Framework
{
	class InputState
	{
		public readonly MouseState Mouse;
		public readonly KeyboardState Keyboard;
		public readonly TouchCollection TouchPanel;
		public readonly GamePadState GamePad;

		public readonly bool IsDown;
		public readonly bool IsJustDown;
		public readonly bool IsJustUp;
		public readonly Point PointerPosition;

		private InputState(ViewportAdapter adapter, KeyboardState ks, MouseState ms, TouchCollection ts, GamePadState gs, InputState prev)
		{
			Mouse = ms;
			Keyboard = ks;
			TouchPanel = ts;
			GamePad = gs;

			if (Mouse.LeftButton == ButtonState.Pressed)
			{
				IsDown = true;
				PointerPosition = adapter.PointToScreen(Mouse.Position);
			}
			else if (TouchPanel.Count > 0)
			{
				IsDown = true;
				PointerPosition = adapter.PointToScreen(TouchPanel[0].Position.ToPoint());
			}
			else
			{
				IsDown = false;
				PointerPosition = prev.PointerPosition;
			}

			IsJustDown = IsDown && !prev.IsDown;
			IsJustUp = !IsDown && prev.IsDown;
		}

		public InputState(KeyboardState ks, MouseState ms, TouchCollection ts, GamePadState gs)
		{
			Mouse = ms;
			Keyboard = ks;
			TouchPanel = ts;
			GamePad = gs;

			PointerPosition = Point.Zero;

			IsDown = false;
			IsJustDown = false;
			IsJustUp = false;
		}

		public static InputState GetState(ViewportAdapter adapter, InputState previous)
		{
			var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
			var ts = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var gs = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

			return new InputState(adapter, ks, ms, ts, gs, previous);
		}

		public static InputState GetInitialState()
		{
			var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
			var ts = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var gs = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

			return new InputState(ks, ms, ts, gs);
		}

		public bool IsExit()
		{

#if !__IOS__
			return GamePad.Buttons.Back == ButtonState.Pressed || Keyboard.IsKeyDown(Keys.Escape);
#else
			return false;
#endif

		}
	}
}
