using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class DummyDebugTextDisplay : IDebugTextDisplay
	{
		private readonly DebugTextDisplayLine dummy = new DebugTextDisplayLine(() => string.Empty);

		public bool IsEnabled { get { return false; } set { } }

		public DummyDebugTextDisplay()
		{
			// DUMMY
		}

		public DebugTextDisplayLine AddLine(DebugTextDisplayLine l)
		{
			return dummy;
		}

		public DebugTextDisplayLine AddLine(Func<string> text)
		{
			return dummy;
		}

		public DebugTextDisplayLine AddLine(string text)
		{
			return dummy;
		}

		public DebugTextDisplayLine AddDecayLine(string text, float lifetime = 2, float decaytime = 0.75f, float spawntime = 0.25f)
		{
			return dummy;
		}

		public DebugTextDisplayLine AddErrorDecayLine(string text, float lifetime = 2, float decaytime = 0.75f, float spawntime = 0.25f)
		{
			return dummy;
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
