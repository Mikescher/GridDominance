using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class GameEntityOperation<TEntity> : IGameEntityOperation where TEntity : GameEntity
	{
		private readonly float length;
		private float time;

		public float Progress => time / length;

		public GameEntityOperation(float operationlength)
		{
			length = operationlength;
			time = 0;
		}

		public bool Update(GameEntity entity, GameTime gameTime, InputState istate)
		{
			return Update((TEntity)entity, gameTime, istate);
		}

		public void OnStart(GameEntity entity)
		{
			OnStart((TEntity)entity);
		}

		public void OnEnd(GameEntity entity)
		{
			OnEnd((TEntity)entity);
		}

		public bool Update(TEntity entity, GameTime gameTime, InputState istate)
		{
			time += gameTime.GetElapsedSeconds();

			if (time >= length) return false;

			OnProgress(entity, time / length, istate);
			return true;
		}

		protected abstract void OnStart(TEntity entity);
		protected abstract void OnProgress(TEntity entity, float progress, InputState istate);
		protected abstract void OnEnd(TEntity entity);

	}
}
