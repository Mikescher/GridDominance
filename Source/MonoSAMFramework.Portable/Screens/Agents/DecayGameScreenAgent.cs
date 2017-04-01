using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Agents
{
	public abstract class DecayGameScreenAgent : GameScreenAgent
	{
		private float lifetime;
		private readonly float maxlifetime;

		protected DecayGameScreenAgent(GameScreen scrn, float agentlifetime) : base(scrn)
		{
			lifetime = 0;
			maxlifetime = agentlifetime;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			if (lifetime > maxlifetime) return;

			lifetime += gameTime.ElapsedSeconds;

			Run(lifetime / maxlifetime);
		}

		public override bool Alive => lifetime < maxlifetime;

		protected abstract void Run(float perc);
	}
}
