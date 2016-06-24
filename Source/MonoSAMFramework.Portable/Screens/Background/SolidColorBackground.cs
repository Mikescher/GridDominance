using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Background
{
	public class SolidColorBackground : GameBackground
	{
		private readonly Color color;

		protected readonly ViewportAdapter VAdapter;

		public SolidColorBackground(GameScreen scrn, Color clr) : base(scrn)
		{
			VAdapter = scrn.Viewport;
			color = clr;
		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			//
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			var size = new Vector2(VAdapter.VirtualWidth, VAdapter.VirtualHeight);
			sbatch.FillRectangle(Vector2.Zero, size, color);
		}
	}
}
