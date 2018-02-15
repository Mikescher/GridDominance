using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents
{
	public abstract class SAMUpdateOp<TOwner> : IUpdateOperation where TOwner : IUpdateOperationOwner
	{
		public float Lifetime { get; private set; } = 0f;

		private bool _alive = false;
		public bool Alive   { get => _alive; set => _alive = value; }

		public abstract string Name { get; }

		private bool _isStarted = false;
		private bool _isFinished = false;

		private TOwner _initElem;

		public SAMUpdateOp()
		{
			//
		}

		public void Update(TOwner owner, SAMTime gameTime, InputState istate)
		{
			if (_isFinished) return;

			Lifetime += gameTime.ElapsedSeconds;

			if (!_isStarted) OnStart(owner);
			_isStarted = true;

			
			if (!Alive)  { OnEnd(owner);   _isFinished = true; return; }

			OnUpdate(owner, gameTime, istate);
			if (_isFinished) return;

			if (!Alive)  { OnEnd(owner);   _isFinished = true; return; }
		}

		public void Init(TOwner owner)
		{
			Alive = true;

			_initElem = owner;
			OnInit(owner);
		}

		public void Finish()
		{
			Alive = false;
		}

		public void Abort()
		{
			if (_isFinished) return;

			OnAbort(_initElem);

			Alive = false;
			_isFinished = true;
		}

		public void ManualOnAbort(IUpdateOperationOwner owner)
		{
			if (_isFinished) return;

			OnAbort((TOwner)owner);
			_isFinished = true;
		}

		protected void ManipulateLifetime(float f)
		{
			Lifetime = f;
		}

		public void InitUnchecked(IUpdateOperationOwner owner)
		{
			Init((TOwner)owner);
		}

		public bool UpdateUnchecked(IUpdateOperationOwner owner, SAMTime gameTime, InputState istate)
		{
			Update((TOwner)owner, gameTime, istate);
			return !_isFinished;
		}

		public void FullReset()
		{
			_isStarted  = false;
			_isFinished = false;
			_alive = false;
			Lifetime = 0;
			_initElem = default(TOwner);
		}

		/// Called on append to owner
		protected virtual void OnInit(TOwner owner) { }

		/// Called before first OnUpdate
		protected virtual void OnStart(TOwner owner) { }

		/// Called every cycle
		protected abstract void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate);

		/// Called after last update (regular end)
		protected virtual void OnEnd(TOwner owner) { }

		/// Called after last update (problematic end)
		protected virtual void OnAbort(TOwner owner) { }

	}
}
