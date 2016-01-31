using System;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Framework.DebugDisplay
{
    interface IDebugTextDisplay
    {
	    DebugTextDisplayLine AddLine(DebugTextDisplayLine l);
	    DebugTextDisplayLine AddLine(Func<string> text);
	    DebugTextDisplayLine AddLine(string text);

	    void Update(GameTime gameTime, InputState istate);
	    void Draw();
    }
}
