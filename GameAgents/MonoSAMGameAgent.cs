using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.GameAgents
{
	public abstract class MonoSAMGameAgent
	{
		public readonly string Name;
		
		protected MonoSAMGameAgent(string name)
		{
			Name = name;
		}

		public abstract void Update(SAMTime gameTime);
		public abstract bool Alive { get; }
	}
}
