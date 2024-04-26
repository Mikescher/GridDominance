using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class SleepOperation<TElement> : FixTimeOperation<TElement> where TElement : IUpdateOperationOwner
	{
		public SleepOperation(float duration) : base(duration)
		{
			// nothing
		}

		public override string Name => $"Sleep({TotalDuration})";

		protected override void OnProgress(TElement owner, float progress, SAMTime gameTime, InputState istate)
		{
			// nothing
		}
	}
}
