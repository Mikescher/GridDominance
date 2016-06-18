using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Background
{
	public class SolidColorBackground : GameBackground
	{
		private readonly Color color;

		protected readonly GraphicsDevice Graphics;

		public SolidColorBackground(GameScreen scrn, Color clr) : base(scrn)
		{
			Graphics = scrn.Game.GraphicsDevice;
			color = clr;
		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			//
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			var size = new Vector2(Graphics.Viewport.Width, Graphics.Viewport.Height);

			sbatch.DrawRectangle(Vector2.Zero, size, color);
		}
	}
}
