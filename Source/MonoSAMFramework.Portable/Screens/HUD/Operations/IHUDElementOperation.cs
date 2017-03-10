using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public interface IHUDElementOperation
	{
		string Name { get; }

		bool Update(HUDElement element, SAMTime gameTime, InputState istate);

		void OnStart(HUDElement element);
		void OnEnd(HUDElement element);
	}
}
