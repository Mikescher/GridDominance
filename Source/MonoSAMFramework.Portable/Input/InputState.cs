using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		private readonly Dictionary<SKeys, bool> lastKeyState;
		private readonly Dictionary<SKeys, bool> currentKeyState;


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
			currentKeyState = lastKeyState.ToDictionary(p => p.Key, p => IsKeyDown(p.Key, ks, gs));
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

			currentKeyState = new Dictionary<SKeys, bool>(0);
		}

		private static bool IsKeyDown(SKeys key, KeyboardState ks, GamePadState gs)
		{
			switch (key)
			{
				case SKeys.ShiftAny:
					return ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);
				case SKeys.ControlAny:
					return ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl);
				case SKeys.AltAny:
					return ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt);
				case SKeys.AndroidBack:
					return gs.Buttons.Back == ButtonState.Pressed;
				default:
					return ks.IsKeyDown((Keys) key);
			}
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

		public bool IsKeyDown(SKeys key)
		{
			bool v;

			if (currentKeyState.TryGetValue(key, out v))
			{
				return v;
			}
			else
			{
				v = IsKeyDown(key, Keyboard, GamePad);
				currentKeyState.Add(key, v);
				return v;
			}
		}

		public bool IsKeyJustDown(SKeys key)
		{
			bool v;

			if (currentKeyState.TryGetValue(key, out v))
			{
				return v && !lastKeyState[key];
			}
			else
			{
				v = IsKeyDown(key, Keyboard, GamePad);
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

			down &= ! needsCtrl  || IsKeyDown(SKeys.ControlAny);
			down &= ! needsShift || IsKeyDown(SKeys.ShiftAny)  ;
			down &= ! needsAlt   || IsKeyDown(SKeys.AltAny);
			
			return down;
		}

		public bool IsShortcutJustPressed(KeyModifier mod, SKeys key)
		{
			return IsModifierDown(mod) && IsKeyJustDown(key);
		}

		public bool IsShortcutJustPressed(KeyCombination key)
		{
			return IsModifierDown(key.Mod) && IsKeyJustDown(key.Key);
		}

		public bool IsShortcutPressed(KeyModifier mod, SKeys key)
		{
			return IsModifierDown(mod) && IsKeyDown(key);
		}

		public bool IsShortcutPressed(KeyCombination key)
		{
			return IsModifierDown(key.Mod) && IsKeyDown(key.Key);
		}

		public string GetFullDebugSummary()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Keys k in Enum.GetValues(typeof(Keys)))
			{
				if (IsKeyDown((SKeys)k, Keyboard, GamePad)) sb.AppendLine("[X] Keys." + k);
			}
			foreach (SKeys k in Enum.GetValues(typeof(SKeys)))
			{
				if (IsKeyDown(k, Keyboard, GamePad)) sb.AppendLine("[X] SKeys." + k);
			}

			foreach (Buttons b in Enum.GetValues(typeof(Buttons)))
			{
				if (GamePad.IsButtonDown(b)) sb.AppendLine("[X] Buttons." + b);
			}

			return sb.ToString().Trim();

		}
	}
}
