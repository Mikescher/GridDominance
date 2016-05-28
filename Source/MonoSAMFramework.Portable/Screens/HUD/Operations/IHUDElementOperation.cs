using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public interface IHUDElementOperation
	{
		bool Update(HUDElement element, GameTime gameTime, InputState istate);

		void OnStart(HUDElement element);
		void OnEnd(HUDElement element);
	}
}
