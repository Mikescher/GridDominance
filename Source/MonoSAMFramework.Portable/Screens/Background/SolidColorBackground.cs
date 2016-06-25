using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Background
{
	public class SolidColorBackground : GameBackground
	{
		private readonly Color color;

		public SolidColorBackground(GameScreen scrn, Color clr) : base(scrn)
		{
			color = clr;
		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			//
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			sbatch.FillRectangle(VAdapter.VirtualTotalBoundingBox, color);
		}
	}
}
