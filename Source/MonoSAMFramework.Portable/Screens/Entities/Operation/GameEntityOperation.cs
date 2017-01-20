using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public abstract class GameEntityOperation<TEntity> : IGameEntityOperation where TEntity : GameEntity
	{
		private readonly float length;
		private float time;

		public float Progress => time / length;

		public string Name { get; }

		protected GameEntityOperation(string name, float operationlength)
		{
			length = operationlength;
			time = 0;
			Name = name;
		}

		public bool Update(GameEntity entity, SAMTime gameTime, InputState istate)
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

		public void OnAbort(GameEntity entity)
		{
			OnAbort((TEntity)entity);
		}

		public bool Update(TEntity entity, SAMTime gameTime, InputState istate)
		{
			time += gameTime.ElapsedSeconds;

			if (time >= length) return false;

			OnProgress(entity, time / length, istate);
			return true;
		}

		public void ForceSetProgress(float p)
		{
			time = FloatMath.Clamp(p, 0f, 1f) * length;
		}

		protected abstract void OnStart(TEntity entity);
		protected abstract void OnProgress(TEntity entity, float progress, InputState istate);
		protected abstract void OnEnd(TEntity entity);
		protected abstract void OnAbort(TEntity entity);

	}
}
