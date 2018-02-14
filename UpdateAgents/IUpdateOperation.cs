using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents
{
	public interface IUpdateOperation
	{
		bool Alive { get; }
		string Name { get; }

		void InitUnchecked(IUpdateOperationOwner owner);
		bool UpdateUnchecked(IUpdateOperationOwner owner, SAMTime gameTime, InputState istate);

		void ManualOnAbort(IUpdateOperationOwner owner);

		void Finish();
		void Abort();
	}
}
