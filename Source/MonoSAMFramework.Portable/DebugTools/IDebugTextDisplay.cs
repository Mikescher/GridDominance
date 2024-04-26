using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using System;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.DebugTools
{
	public interface IDebugTextDisplay
	{
		bool IsEnabled { get; set; }
		float Scale { get; set; }

#if DEBUG
		int LastRenderSpriteCount { get; }
		int LastRenderTextCount { get; }
#endif

		DebugTextDisplayLine AddLine(DebugTextDisplayLine l);
		DebugTextDisplayLine AddLine(Func<string> text);
		DebugTextDisplayLine AddLine(Func<string> text, Color background, Color foreground);
		DebugTextDisplayLine AddLine(string debugSettingsKey, Func<string> text);
		DebugTextDisplayLine AddLine(string debugSettingsKey, Func<string> text, ILifetimeObject owner);
		DebugTextDisplayLine AddLine(string text);
		DebugTextDisplayLine AddDecayLine(string text, float lifetime = 2f, float decaytime = 0.15f, float spawntime = 0.25f);
		DebugTextDisplayLine AddLine(string text, Color background, Color foreground);

		DebugTextDisplayLine AddLineFromAsync(Func<string> text);
		DebugTextDisplayLine AddLineFromAsync(Func<string> text, Color background, Color foreground);
		DebugTextDisplayLine AddLineFromAsync(string text, Color background, Color foreground);
		DebugTextDisplayLine AddLineFromAsync(DebugTextDisplayLine l);

		void AddLogLines();

		void Update(SAMTime gameTime, InputState istate);
		void Draw();

		void Clear();
	}
}
