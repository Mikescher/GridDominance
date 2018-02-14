using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public abstract class FixTimeOperation<TOwner> : SAMUpdateOp<TOwner>, IProgressableOperation where TOwner : IUpdateOperationOwner
	{
		private readonly float length;
		public float Progress => FloatMath.Min(Lifetime / length, 1f);

		public float TotalDuration => length;

		protected FixTimeOperation(float operationlength)
		{
			length = operationlength;
		}
		
		protected override void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate)
		{
			if (Lifetime >= length) { Finish(); return; }

			OnProgress(owner, Progress, gameTime, istate);
		}

		public void ForceSetProgress(float p)
		{
			ManipulateLifetime(FloatMath.Clamp(p, 0f, 1f) * length);
		}

		protected abstract void OnProgress(TOwner owner, float progress, SAMTime gameTime, InputState istate);
	}
}
