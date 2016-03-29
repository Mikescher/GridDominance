using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable
{
	interface ISAMUpdateable
	{
		void Update(GameTime gameTime, InputState istate);
	}
}
