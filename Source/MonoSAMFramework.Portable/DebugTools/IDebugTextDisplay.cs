using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public interface IDebugTextDisplay
	{
		bool IsEnabled { get; set; }
		float Scale { get; set; }

		DebugTextDisplayLine AddLine(DebugTextDisplayLine l);
		DebugTextDisplayLine AddLine(Func<string> text);
		DebugTextDisplayLine AddLine(string debugSettingsKey, Func<string> text);
		DebugTextDisplayLine AddLine(string text);
		DebugTextDisplayLine AddDecayLine(string text, float lifetime = 2f, float decaytime = 0.15f, float spawntime = 0.25f);
		DebugTextDisplayLine AddErrorDecayLine(string text, float lifetime = 2f, float decaytime = 0.15f, float spawntime = 0.25f);

		void Update(GameTime gameTime, InputState istate);
		void Draw();
	}
}
