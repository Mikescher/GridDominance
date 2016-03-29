using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.hud
{
	class GDGameHUD : GameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Owner;

		public override int Top  => -(int)GDOwner.GDViewport.VirtualOffsetY;
		public override int Left => -(int)GDOwner.GDViewport.VirtualOffsetX;

		public override int Bottom => (int)(GDOwner.GDViewport.VirtualHeight + GDOwner.GDViewport.VirtualOffsetY);
		public override int Right => (int)(GDOwner.GDViewport.VirtualWidth + GDOwner.GDViewport.VirtualOffsetX);

		public GDGameHUD(GDGameScreen scrn) : base(scrn)
		{
			AddElement(new GameHUDButton
			{
				RelativePosition = new Point(16, 16),
				Size = new Size(48, 48),
				Alignment = HUDAlignment.TOPRIGHT,
			});

			AddElement(new GameHUDButton
			{
				RelativePosition = new Point(16, 16),
				Size = new Size(64, 64),
				Alignment = HUDAlignment.BOTTOMLEFT,
			});
		}
	}
}
