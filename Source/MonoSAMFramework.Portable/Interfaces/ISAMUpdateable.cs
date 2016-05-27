using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Interfaces
{
	interface ISAMUpdateable
	{
		void Update(GameTime gameTime, InputState istate);
	}
}
