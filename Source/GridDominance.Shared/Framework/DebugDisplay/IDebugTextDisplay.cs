using System;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Framework.DebugDisplay
{
	interface IDebugTextDisplay
	{
		DebugTextDisplayLine AddLine(DebugTextDisplayLine l);
		DebugTextDisplayLine AddLine(Func<string> text);
		DebugTextDisplayLine AddLine(string text);
		DebugTextDisplayLine AddDecayLine(string text, float lifetime = 2f, float decaytime = 0.75f, float spawntime = 0.25f);
		DebugTextDisplayLine AddErrorDecayLine(string text, float lifetime = 2f, float decaytime = 0.75f, float spawntime = 0.25f);


		void Update(GameTime gameTime, InputState istate);
		void Draw(GameTime gameTime);
	}
}
