using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.External;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Background
{
	public abstract class GameBackground : ISAMDrawable, ISAMUpdateable
	{
		protected readonly Screen Owner;

		protected GameBackground(Screen scrn)
		{
			Owner = scrn;
		}
		
		public abstract void Update(GameTime gameTime, InputState istate);
		public abstract void Draw(SpriteBatch sbatch);
	}
}
