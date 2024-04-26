using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public abstract class SingleTimeOperation<TOwner>  : SAMUpdateOp<TOwner>, IProgressableOperation where TOwner : IUpdateOperationOwner
	{
		public float Progress => 0;

		protected SingleTimeOperation()
		{
			//
		}
		
		protected override void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate)
		{
			OnExecute(owner, gameTime, istate);
			Finish();
		}
		
		protected abstract void OnExecute(TOwner owner, SAMTime gameTime, InputState istate);
	}
}
