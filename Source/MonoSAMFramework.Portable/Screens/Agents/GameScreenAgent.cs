using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.Agents
{
	public abstract class GameScreenAgent : ISAMUpdateable
	{
		protected readonly GameScreen Screen;

		protected GameScreenAgent(GameScreen scrn)
		{
			Screen = scrn;
		}

		public abstract void Update(GameTime gameTime, InputState istate);
	}
}
