using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public interface IGameEntityOperation
	{
		float Progress { get; }
		string Name { get; }

		bool Update(GameEntity entity, SAMTime gameTime, InputState istate);

		void OnStart(GameEntity gameEntity);
		void OnEnd(GameEntity gameEntity);
		void OnAbort(GameEntity gameEntity);

		void ForceSetProgress(float p);
	}
}
