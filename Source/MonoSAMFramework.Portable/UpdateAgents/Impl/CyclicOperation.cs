using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public abstract class CyclicOperation<TElement> : SAMUpdateOp<TElement> where TElement : IUpdateOperationOwner
	{
		private readonly bool _reliable;
		private readonly float _cyleTime;

		private int _cycleCounter = 0;
		private float _timeSinceLastCycle = 0f;

		protected CyclicOperation(float cycleTime, bool reliable)
		{
			_cyleTime = cycleTime;
			_reliable = reliable;
		}

		protected override void OnUpdate(TElement element, SAMTime gameTime, InputState istate)
		{
			_timeSinceLastCycle += gameTime.ElapsedSeconds;

			while (_timeSinceLastCycle > _cyleTime)
			{
				OnCycle(element, _cycleCounter);

				if (_reliable)
					_timeSinceLastCycle -= _cyleTime;
				else
					_timeSinceLastCycle = 0;

				_cycleCounter++;

				if (_cyleTime < float.Epsilon) break; // prevent inf loop
			}
		}

		protected abstract void OnCycle(TElement element, int counter);
	}
}