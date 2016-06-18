using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MonoSAMFramework.Portable.DebugTools
{
	public static class DebugSettings
	{
		private static readonly Dictionary<string, DebugListener> listeners = new Dictionary<string, DebugListener>();

		public static void Update(InputState istate)
		{
			foreach (var listener in listeners.ToList())
			{
				if (listener.Value.Owner.Alive)
					listener.Value.Update(istate);
				else
					listeners.Remove(listener.Key);
			}
		}

		[Conditional("DEBUG")]
		public static void AddSwitch(string ident, ILifetimeObject owner, Keys actionkey, KeyboardModifiers mod, bool initial)
		{
			var upperIdent = ident.ToUpper();

			if (listeners.ContainsKey(upperIdent))
				listeners.Remove(upperIdent);

			var result = new DebugListener(ident, owner, actionkey, mod, DebugListener.DebugListenerType.Switch);
			result.Set(initial);
			listeners.Add(upperIdent, result);
		}

		[Conditional("DEBUG")]
		public static void AddTrigger(string ident, ILifetimeObject owner, Keys actionkey, KeyboardModifiers mod, Action<DebugListener> listenerEvent = null)
		{
			var upperIdent = ident.ToUpper();

			if (listeners.ContainsKey(upperIdent))
				listeners.Remove(upperIdent);

			var result = new DebugListener(ident, owner, actionkey, mod, DebugListener.DebugListenerType.Trigger);
			if (listenerEvent != null) result.SetEvent(listenerEvent);
			listeners.Add(upperIdent, result);
		}

		[Conditional("DEBUG")]
		public static void AddPush(string ident, ILifetimeObject owner, Keys actionkey, KeyboardModifiers mod, Action<DebugListener> listenerEvent = null)
		{
			var upperIdent = ident.ToUpper();

			if (listeners.ContainsKey(upperIdent))
				listeners.Remove(upperIdent);

			var result = new DebugListener(ident, owner, actionkey, mod, DebugListener.DebugListenerType.Push);
			if (listenerEvent != null) result.SetEvent(listenerEvent);
			listeners.Add(upperIdent, result);
		}

		[Conditional("DEBUG")]
		public static void Remove(string ident)
		{
			ident = ident.ToUpper();

			if (listeners.ContainsKey(ident))
				listeners.Remove(ident);
		}

		public static bool Get(string ident)
		{
			ident = ident.ToUpper();

			DebugListener l;
			if (listeners.TryGetValue(ident, out l))
				return l.Active;

			return false;
		}

		public static string GetSummary()
		{
			StringBuilder builder = new StringBuilder();

			foreach (var listener in listeners)
			{
				builder.AppendLine(listener.Value.GetSummary());
			}

			return builder.ToString().TrimEnd();
		}
	}
}
