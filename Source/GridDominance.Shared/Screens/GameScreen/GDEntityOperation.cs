using GridDominance.Shared.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GridDominance.Shared.Screens.GameScreen
{
	abstract class GDEntityOperation<TEntity> : IGDEntityOperation where TEntity : GDEntity
	{
		private readonly float length;
		private float time;

		public GDEntityOperation(float operationlength)
		{
			length = operationlength;
			time = 0;
		}

		public bool Update(GDEntity entity, GameTime gameTime, InputState istate)
		{
			return Update((TEntity)entity, gameTime, istate);
		}

		public bool Update(TEntity entity, GameTime gameTime, InputState istate)
		{
			if (time == 0) OnStart(entity, istate);

			time += gameTime.GetElapsedSeconds();

			if (time >= length)
			{
				OnEnd(entity, istate);
				return false;
			}
			else
			{
				OnProgress(entity, time / length, istate);
				return true;
			}
		}

		protected abstract void OnStart(TEntity entity, InputState istate);
		protected abstract void OnProgress(TEntity entity, float progress, InputState istate);
		protected abstract void OnEnd(TEntity entity, InputState istate);

	}
}
