using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace MonoSAMFramework.Portable.Screens.Background
{
	public class SolidColorBackground : GameBackground
	{
		private readonly Color color;

		protected readonly SAMViewportAdapter VAdapter;

		public SolidColorBackground(GameScreen scrn, Color clr) : base(scrn)
		{
			VAdapter = scrn.VAdapter;
			color = clr;
		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			//
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			sbatch.FillRectangle(VAdapter.VirtualGuaranteedBoundingBox, color);
		}
	}
}
