using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.Agents
{
	public abstract class GameScreenAgent : ISAMUpdateable
	{
		protected readonly GameScreen Screen;

		public GameScreenAgent(GameScreen scrn)
		{
			Screen = scrn;
		}

		public abstract void Update(GameTime gameTime, InputState istate);
	}
}
