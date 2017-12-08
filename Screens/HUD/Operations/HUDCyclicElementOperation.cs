using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDCyclicElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		public bool Finished = false;

		private readonly bool _reliable;
		private readonly float _cyleTime;

		private int _cycleCounter = 0;
		private float _timeSinceLastCycle = 0f;

		protected HUDCyclicElementOperation(float cycleTime, bool reliable)
		{
			_cyleTime = cycleTime;
			_reliable = reliable;
		}

		public override bool Update(TElement element, SAMTime gameTime, InputState istate)
		{
			if (Finished) return false;


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

			return true;
		}

		protected abstract void OnCycle(TElement element, int counter);
	}
}