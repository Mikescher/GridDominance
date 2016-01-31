using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Framework.DebugDisplay
{
	class DebugTextDisplayLine
	{
		private static readonly Func<bool> ActionTrue = () => true;

		public readonly Func<string> DisplayText;
		public readonly Func<bool> Active;

		public bool IsAlive { get; private set; }
		public Color Color { get; private set; }

		public DebugTextDisplayLine(Func<string> text)
			: this(text, ActionTrue)
		{
			// 
		}

		public DebugTextDisplayLine(Func<string> text, Func<bool> active)
		{
			DisplayText = text;
			Active = active;

			IsAlive = true;
			Color = Color.Black;
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			//
		}

		public DebugTextDisplayLine SetColor(Color c)
		{
			Color = c;
			return this;
		}
	}
}
