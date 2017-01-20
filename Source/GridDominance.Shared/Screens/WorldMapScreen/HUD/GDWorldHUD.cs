using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class GDWorldHUD : GameHUD
	{
		public GDWorldMapScreen GDOwner => (GDWorldMapScreen)Screen;

		private HUDWorldSettingsButton btnSettings;
		private HUDLabel lblPoints;

		public GDWorldHUD(GDWorldMapScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			btnSettings = new HUDWorldSettingsButton();
			AddElement(btnSettings);

			lblPoints = new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTER,
				Text = "Points: ???",
				TextColor = FlatColors.Clouds,
				BackgroundColor = Color.Black * 0.5f,
				Alignment = HUDAlignment.TOPRIGHT,
				RelativePosition = new FPoint(10, 10),
				FontSize = 60.0f,
				Size = new FSize(300, 60)
			};
			AddElement(lblPoints);
		}
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			lblPoints.Text = "Points: " + MainGame.Inst.Profile.TotalPoints;
		}
	}
}
