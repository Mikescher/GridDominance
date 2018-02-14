using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public abstract class DecayOperation<TOwner> : SAMUpdateOp<TOwner> where TOwner : IUpdateOperationOwner
	{
		private readonly float length;
		public float DecayProgress => 1 - FloatMath.Min(Lifetime / length, 1f);

		public float TotalDuration => length;

		protected DecayOperation(float operationlength)
		{
			length = operationlength;
		}

		protected override void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate)
		{
			if (Lifetime >= length) { Finish(); return; }

			OnDecayProgress(owner, DecayProgress, gameTime, istate);
		}

		protected abstract void OnDecayProgress(TOwner owner, float progress, SAMTime gameTime, InputState istate);
	}
}
