using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public interface IGameEntityOperation
	{
		float Progress { get; }

		bool Update(GameEntity entity, GameTime gameTime, InputState istate);

		void OnStart(GameEntity gameEntity);
		void OnEnd(GameEntity gameEntity);
	}
}
