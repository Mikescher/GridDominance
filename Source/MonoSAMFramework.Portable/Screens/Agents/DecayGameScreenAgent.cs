using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Agents
{
	public abstract class DecayGameScreenAgent : GameScreenAgent
	{
		private float lifetime;
		private readonly float maxlifetime;
		private bool finished = false;
		private bool started = false;

		protected DecayGameScreenAgent(GameScreen scrn, float agentlifetime) : base(scrn)
		{
			lifetime = 0;
			maxlifetime = agentlifetime;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			if (lifetime > maxlifetime)
			{
				if (!finished)
				{
					End();
					finished = true;
				}

				return;
			}

			if (!started)
			{
				Start();
				started = true;
			}

			lifetime += gameTime.ElapsedSeconds;

			Run(lifetime / maxlifetime);
		}

		public override bool Alive => !finished;

		protected abstract void Start();
		protected abstract void Run(float perc);
		protected abstract void End();
	}
}
