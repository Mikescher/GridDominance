using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.InputListeners;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.GameMath.Geometry;

// ReSharper disable ImpureMethodCallOnReadonlyValueField
namespace MonoSAMFramework.Portable.Input
{
	public class InputState
	{
		public readonly MouseState Mouse;
		public readonly KeyboardState Keyboard;
		public readonly TouchCollection TouchPanel;
		public readonly GamePadState GamePad;

		public readonly bool IsDown;
		public readonly bool IsJustDown;
		public readonly bool IsJustUp;
		public readonly FPoint PointerPosition;

		private readonly Dictionary<Keys, bool> lastKeyState;
		private readonly Dictionary<Keys, bool> currentKeyState;

		private InputState(SAMViewportAdapter adapter, KeyboardState ks, MouseState ms, TouchCollection ts, GamePadState gs, InputState prev)
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

			lastKeyState = prev.currentKeyState;
			currentKeyState = lastKeyState.ToDictionary(p => p.Key, p => ks.IsKeyDown(p.Key));
		}

		public InputState(SAMViewportAdapter adapter, KeyboardState ks, MouseState ms, TouchCollection ts, GamePadState gs)
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
				PointerPosition = FPoint.Zero;
			}

			currentKeyState = new Dictionary<Keys, bool>(0);
		}

		public static InputState GetState(SAMViewportAdapter adapter, InputState previous)
		{
			var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
			var ts = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var gs = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

			return new InputState(adapter, ks, ms, ts, gs, previous);
		}

		public static InputState GetInitialState(SAMViewportAdapter adapter)
		{
			var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
			var ts = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var gs = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

			return new InputState(adapter, ks, ms, ts, gs);
		}

		public bool IsExit()
		{

#if !__IOS__
			return GamePad.Buttons.Back == ButtonState.Pressed || IsKeyDown(Keys.Escape);
#else
			return false;
#endif

		}

		public bool IsKeyDown(Keys key)
		{
			return Keyboard.IsKeyDown(key);
		}

		public bool IsKeyJustDown(Keys key)
		{
			bool v;

			if (currentKeyState.TryGetValue(key, out v))
			{
				return v && !lastKeyState[key];
			}
			else
			{
				v = IsKeyDown(key);
				currentKeyState.Add(key, v);
				return v;
			}
		}

		public bool IsModifierDown(KeyboardModifiers mod)
		{
			bool needsCtrl  = (mod & KeyboardModifiers.Control) == KeyboardModifiers.Control;
			bool needsShift = (mod & KeyboardModifiers.Shift)   == KeyboardModifiers.Shift;
			bool needsAlt   = (mod & KeyboardModifiers.Alt)     == KeyboardModifiers.Alt;

			bool down = true;

			down &= ! needsCtrl  || IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
			down &= ! needsShift || IsKeyDown(Keys.LeftShift)   || IsKeyDown(Keys.RightShift);
			down &= ! needsAlt   || IsKeyDown(Keys.LeftAlt)     || IsKeyDown(Keys.RightAlt);
			
			return down;
		}

		public bool IsShortcutJustPressed(KeyboardModifiers mod, Keys key)
		{
			return IsModifierDown(mod) && IsKeyJustDown(key);
		}

		public bool IsShortcutPressed(KeyboardModifiers mod, Keys key)
		{
			return IsModifierDown(mod) && IsKeyDown(key);
		}
	}
}
