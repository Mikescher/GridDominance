using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDScorePanel : HUDPanel
	{
		public const int WIDTH = 12 * GDGameScreen.TILE_WIDTH;
		public const int HEIGHT = 8 * GDGameScreen.TILE_WIDTH;

		public override int Depth => 0;

		public HUDScorePanel()
		{
			RelativePosition = Point.Zero;
			Size = new Size(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
		}
	}
}
