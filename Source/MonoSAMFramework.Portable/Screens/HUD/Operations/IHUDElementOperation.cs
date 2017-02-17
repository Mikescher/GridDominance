using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public interface IHUDElementOperation
	{
		bool Update(HUDElement element, SAMTime gameTime, InputState istate);

		void OnStart(HUDElement element);
		void OnEnd(HUDElement element);
	}
}
