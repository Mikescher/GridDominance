using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace GridDominance.Shared.Framework.DebugDisplay
{
	class DummyDebugTextDisplay : IDebugTextDisplay
	{
		private readonly Func<string> dummyFunc = () => string.Empty; 

		public DummyDebugTextDisplay()
		{
			// DUMMY
		}

		public DebugTextDisplayLine AddLine(DebugTextDisplayLine l)
		{
			return new DebugTextDisplayLine(dummyFunc);
		}

		public DebugTextDisplayLine AddLine(Func<string> text)
		{
			return new DebugTextDisplayLine(dummyFunc);
		}

		public DebugTextDisplayLine AddLine(string text)
		{
			return new DebugTextDisplayLine(dummyFunc);
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			// DUMMY
		}

		public void Draw()
		{
			// DUMMY
		}
	}
}
