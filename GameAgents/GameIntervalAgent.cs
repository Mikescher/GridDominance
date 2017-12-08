using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.GameAgents
{
	public abstract class GameIntervalAgent : MonoSAMGameAgent
	{
		private bool _alive = true;
		private readonly float _initDelay;
		private readonly float _interval;
		private float _time = 0;
		
		protected GameIntervalAgent(string name, float initialDelay, float interval) : base(name)
		{
			_initDelay = initialDelay;
			_interval = interval;
		}

		public override void Update(SAMTime gameTime)
		{
			_time += gameTime.ElapsedSeconds;

			if (_time > _initDelay + _interval)
			{
				_time -= _interval;

				OnEvent(gameTime);
			}
		}

		protected abstract void OnEvent(SAMTime gameTime);
		
		public void Terminate()
		{
			_alive = false;
		}

		public override bool Alive => _alive;
	}
}
