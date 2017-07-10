using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;

namespace MonoSAMFramework.Portable.DebugTools
{
	[DebuggerDisplay("[{ParentIdentifier} -> {Identifier}] {Active} ({_active})")]
	public class DebugListener
	{
		public enum DebugListenerType
		{
			Trigger,
			Switch,
			Push,
			Constant,
		}

		public readonly string Identifier;
		public readonly string ParentIdentifier;
		public readonly ILifetimeObject Owner;
		public readonly DebugListenerType Type;

		private readonly KeyCombinationList keys;

		private Action<DebugListener> triggerEvent;

		private bool _active;

		public bool Active => _active && (ParentIdentifier == null || DebugSettings.Get(ParentIdentifier));
		public bool SelfActive => _active;

		public DebugListener(string parentIdent, string ident, ILifetimeObject owner, SKeys actionkey, KeyModifier mod, DebugListenerType debugtype)
		{
			ParentIdentifier = parentIdent;
			Identifier = ident;
			keys = new KeyCombinationList(new KeyCombination(actionkey, mod));
			Type = debugtype;
			Owner = owner;
		}

		public DebugListener(string parentIdent, string ident, ILifetimeObject owner, KeyCombinationList src, DebugListenerType debugtype)
		{
			ParentIdentifier = parentIdent;
			Identifier = ident;
			keys = src;
			Type = debugtype;
			Owner = owner;
		}

		public void SetEvent(Action<DebugListener> e) => triggerEvent = e;

		public void Set(bool value) => _active = value;

		public void Update(InputState istate)
		{
			switch (Type)
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
				case DebugListenerType.Constant:
					// Do nothing
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateSwitch(InputState istate)
		{
			foreach (var key in keys)
			{
				if (istate.IsShortcutExclusiveJustPressed(key))
				{
					istate.SwallowKey(key, InputConsumer.DebugDisplay);

					_active = !Active;

					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => triggerEvent?.Invoke(this));
				}
			}
		}

		private void UpdatePush(InputState istate)
		{
			foreach (var key in keys)
			{
				_active = istate.IsShortcutPressed(key);

				if (Active)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => triggerEvent?.Invoke(this));
				}
			}
		}

		private void UpdateTrigger(InputState istate)
		{
			foreach (var key in keys)
			{
				if (istate.IsShortcutExclusiveJustPressed(key))
				{
					istate.SwallowKey(key, InputConsumer.DebugDisplay);

					_active = true;

					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => triggerEvent?.Invoke(this));
				}
				else
				{
					_active = false;
				}
			}
		}

		public string GetSummary()
		{
			string keycombination = string.Join(" || ", keys.Select(p => p.GetMmemonic()));

			return string.Format("[{0}] {1} => {2}", Active ? "X" : " ", keycombination.PadRight(12), Identifier);
		}
	}
}
