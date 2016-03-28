using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.GameScreen
{
	abstract class GDEntityOperation<TEntity> : IGDEntityOperation where TEntity : GDEntity
	{
		private readonly float length;
		private float time;

		public float Progress => time/length;

		public GDEntityOperation(float operationlength)
		{
			length = operationlength;
			time = 0;
		}

		public bool Update(GDEntity entity, GameTime gameTime, InputState istate)
		{
			return Update((TEntity)entity, gameTime, istate);
		}

		public void OnStart(GDEntity entity)
		{
			OnStart((TEntity)entity);
		}

		public void OnEnd(GDEntity entity)
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
