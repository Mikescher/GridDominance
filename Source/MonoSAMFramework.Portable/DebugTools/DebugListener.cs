using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class DebugListener
	{
		public enum DebugListenerType
		{
			Trigger,
			Switch,
			Push,
		}

		public readonly string Identifier;
		public readonly ILifetimeObject Owner;

		private readonly Keys key;
		private readonly KeyboardModifiers modifiers;
		private readonly DebugListenerType type;

		private Action<DebugListener> triggerEvent;

		public bool Active { get; private set; }

		public DebugListener(string ident, ILifetimeObject owner,  Keys actionkey, KeyboardModifiers mod, DebugListenerType debugtype)
		{
			Identifier = ident;
			key = actionkey;
			modifiers = mod;
			type = debugtype;
			Owner = owner;
		}

		public void SetEvent(Action<DebugListener> e) => triggerEvent = e;

		public void Set(bool value) => Active = value;

		public void Update(InputState istate)
		{
			switch (type)
			{
				case DebugListenerType.Trigger:
					UpdateTrigger(istate);
					break;
				case DebugListenerType.Switch:
					UpdateSwitch(istate);
					break;
				case DebugListenerType.Push:
					UpdatePush(istate);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateSwitch(InputState istate)
		{
			if (istate.IsShortcutJustPressed(modifiers, key))
			{
				Active = !Active;

				triggerEvent?.Invoke(this);
			}
		}

		private void UpdatePush(InputState istate)
		{
			Active = istate.IsShortcutPressed(modifiers, key);

			if (Active)
			{
				triggerEvent?.Invoke(this);
			}
		}

		private void UpdateTrigger(InputState istate)
		{
			if (istate.IsShortcutJustPressed(modifiers, key))
			{
				Active = true;

				triggerEvent?.Invoke(this);
			}
			else
			{
				Active = false;
			}
		}

		public string GetSummary()
		{
			string keycombination;

			if (modifiers == KeyboardModifiers.None)
			{
				keycombination = key.ToString();
			}
			else if (modifiers == KeyboardModifiers.Alt || modifiers == KeyboardModifiers.Control || modifiers == KeyboardModifiers.Shift)
			{
				keycombination = string.Format("{0} + {1}", modifiers, key);
			}
			else
			{
				keycombination = string.Format("[{0}] + {1}", modifiers, key);
			}

			return string.Format("[{0}] {1} => {2}", Active ? "X" : " ", keycombination.PadRight(12), Identifier);
		}
	}
}
