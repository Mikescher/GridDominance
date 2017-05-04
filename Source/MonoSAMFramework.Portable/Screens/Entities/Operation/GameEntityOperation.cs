using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public abstract class GameEntityOperation<TEntity> : IGameEntityOperation where TEntity : GameEntity
	{
		private readonly float? length;
		private float time;

		public float Progress => length.HasValue ? FloatMath.Min(time / length.Value, 1f) : 0;

		public string Name { get; }

		protected GameEntityOperation(string name, float? operationlength)
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

			if (length.HasValue && time >= length) return false;

			OnProgress(entity, Progress, gameTime, istate);
			return true;
		}

		public void ForceSetProgress(float p)
		{
			if (length.HasValue) time = FloatMath.Clamp(p, 0f, 1f) * length.Value;
		}

		protected abstract void OnStart(TEntity entity);
		protected abstract void OnProgress(TEntity entity, float progress, SAMTime gameTime, InputState istate);
		protected abstract void OnEnd(TEntity entity);
		protected abstract void OnAbort(TEntity entity);

	}
}
