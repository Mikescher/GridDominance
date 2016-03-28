using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.GameScreen
{
	interface IGDEntityOperation
	{
		float Progress { get; }

		bool Update(GDEntity entity, GameTime gameTime, InputState istate);

		void OnStart(GDEntity gdEntity);
		void OnEnd(GDEntity gdEntity);
	}
}