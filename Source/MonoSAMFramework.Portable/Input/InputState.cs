using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable ImpureMethodCallOnReadonlyValueField
namespace MonoSAMFramework.Portable.Input
{
	public class InputState
	{
		public readonly MouseState Mouse;
		public readonly KeyboardState Keyboard;
		public readonly TouchCollection TouchPanel;
		public readonly GamePadState GamePad;

		private readonly bool isDown;
		private readonly bool isJustDown;
		private readonly bool isJustUp;
		private bool isUpDownSwallowed = false;

		public bool IsExclusiveDown => isDown && !isUpDownSwallowed;
		public bool IsExclusiveUp => !isDown && !isUpDownSwallowed;
		public bool IsExclusiveJustDown => isJustDown && !isUpDownSwallowed;
		public bool IsExclusiveJustUp => isJustUp && !isUpDownSwallowed;
		public bool IsRealDown => isDown;
		public bool IsRealUp => !isDown;
		public bool IsRealJustDown => isJustDown;
		public bool IsRealJustUp => isJustUp;
		public void Swallow() => isUpDownSwallowed = true;
		
		public readonly FPoint PointerPosition;
		public readonly FPoint PointerPositionOnMap;

		private readonly Dictionary<Keys, bool> lastKeyState;
		private readonly Dictionary<Keys, bool> currentKeyState;


		private InputState(SAMViewportAdapter adapter, KeyboardState ks, MouseState ms, TouchCollection ts, GamePadState gs, InputState prev, float mox, float moy)
		{
			Mouse = ms;
			Keyboard = ks;
			TouchPanel = ts;
			GamePad = gs;

			if (Mouse.LeftButton == ButtonState.Pressed)
			{
				isDown = true;
				PointerPosition = adapter.PointToScreen(Mouse.Position);
			}
			else if (TouchPanel.Count > 0)
			{
				isDown = true;
				PointerPosition = adapter.PointToScreen(TouchPanel[0].Position.ToPoint());
			}
			else
			{
				isDown = false;
				PointerPosition = prev.PointerPosition;
			}

			PointerPositionOnMap = PointerPosition.RelativeTo(mox, moy);

			isJustDown = isDown && !prev.isDown;
			isJustUp = !isDown && prev.isDown;

			lastKeyState = prev.currentKeyState;
			currentKeyState = lastKeyState.ToDictionary(p => p.Key, p => ks.IsKeyDown(p.Key));
		}

		public InputState(SAMViewportAdapter adapter, KeyboardState ks, MouseState ms, TouchCollection ts, GamePadState gs, float mox, float moy)
		{
			Mouse = ms;
			Keyboard = ks;
			TouchPanel = ts;
			GamePad = gs;

			if (Mouse.LeftButton == ButtonState.Pressed)
			{
				isDown = true;
				PointerPosition = adapter.PointToScreen(Mouse.Position);
			}
			else if (TouchPanel.Count > 0)
			{
				isDown = true;
				PointerPosition = adapter.PointToScreen(TouchPanel[0].Position.ToPoint());
			}
			else
			{
				isDown = false;
				PointerPosition = FPoint.Zero;
			}

			PointerPositionOnMap = PointerPosition.RelativeTo(mox, moy);

			currentKeyState = new Dictionary<Keys, bool>(0);
		}

		public static InputState GetState(SAMViewportAdapter adapter, InputState previous, float mox, float moy)
		{
			var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
			var ts = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var gs = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

			return new InputState(adapter, ks, ms, ts, gs, previous, mox, moy);
		}

		public static InputState GetInitialState(SAMViewportAdapter adapter, float mox, float moy)
		{
			var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
			var ts = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
			var gs = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

			return new InputState(adapter, ks, ms, ts, gs, mox, moy);
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

		public bool IsModifierDown(KeyModifier mod)
		{
			bool needsCtrl  = (mod & KeyModifier.Control) == KeyModifier.Control;
			bool needsShift = (mod & KeyModifier.Shift)   == KeyModifier.Shift;
			bool needsAlt   = (mod & KeyModifier.Alt)     == KeyModifier.Alt;

			bool down = true;

			down &= ! needsCtrl  || IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
			down &= ! needsShift || IsKeyDown(Keys.LeftShift)   || IsKeyDown(Keys.RightShift);
			down &= ! needsAlt   || IsKeyDown(Keys.LeftAlt)     || IsKeyDown(Keys.RightAlt);
			
			return down;
		}

		public bool IsShortcutJustPressed(KeyModifier mod, Keys key)
		{
			return IsModifierDown(mod) && IsKeyJustDown(key);
		}

		public bool IsShortcutPressed(KeyModifier mod, Keys key)
		{
			return IsModifierDown(mod) && IsKeyDown(key);
		}
	}
}
