using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class DummyDebugTextDisplay : IDebugTextDisplay
	{
		private readonly DebugTextDisplayLine dummy = new DebugTextDisplayLine(() => string.Empty);

		public bool IsEnabled { get { return false; } set { } }
		public float Scale { get { return 1f; } set { } }

		public int LastRenderSpriteCount => 0;
		public int LastRenderTextCount => 0;

		public DummyDebugTextDisplay() { /* DUMMY */ }
		
		public DebugTextDisplayLine AddLine(DebugTextDisplayLine l) => dummy;
		public DebugTextDisplayLine AddLine(Func<string> text) => dummy;
		public DebugTextDisplayLine AddLine(Func<string> text, Color b, Color f) => dummy;
		public DebugTextDisplayLine AddLine(string debugSettingsKey, Func<string> text) => dummy;
		public DebugTextDisplayLine AddLine(string text) => dummy;
		public DebugTextDisplayLine AddDecayLine(string text, float lifetime = 2, float decaytime = 0.75f, float spawntime = 0.25f) => dummy;
		public DebugTextDisplayLine AddErrorDecayLine(string text, float lifetime = 2, float decaytime = 0.75f, float spawntime = 0.25f) => dummy;

		public void Update(GameTime gameTime, InputState istate) { /* DUMMY */ }
		public void Draw() { /* DUMMY */ }
	}
}
