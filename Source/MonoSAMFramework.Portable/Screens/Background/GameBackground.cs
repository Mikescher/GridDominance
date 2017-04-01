using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace MonoSAMFramework.Portable.Screens.Background
{
	public abstract class GameBackground : ISAMDrawable, ISAMUpdateable
	{
		protected readonly GameScreen Owner;
		protected readonly SAMViewportAdapter VAdapter;

		protected GameBackground(GameScreen scrn)
		{
			Owner = scrn;
			VAdapter = Owner.VAdapterGame;
		}
		
		public abstract void Update(SAMTime gameTime, InputState istate);
		public abstract void Draw(IBatchRenderer sbatch);
	}
}
