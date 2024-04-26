using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Interfaces
{
	public interface ISAMUpdateable
	{
		void Update(SAMTime gameTime, InputState istate);
	}
}
