using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class GDWorldHUD : GameHUD
	{
		public GDWorldMapScreen GDOwner => (GDWorldMapScreen)Screen;

		public LevelNode SelectedNode = null;

		public readonly TopLevelDisplay TopLevelDisplay;
		public readonly InformationDisplay InfoDisplay;
		public readonly SettingsButton Settings;

		public GDWorldHUD(GDWorldMapScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(Settings = new SettingsButton());
			AddElement(new ScoreDisplay());
			AddElement(TopLevelDisplay = new TopLevelDisplay());
			AddElement(InfoDisplay = new InformationDisplay());


			AddElement(new HUDIconTextBox(0)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(450, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 40,

				Placeholder = "Username",
				
				BackgoundCornerSize = 8,
				ColorPadLeft = FlatColors.ControlHighlight,
				WidthPadLeft = 80,
				IconColor = FlatColors.Asbestos,
				Icon = Textures.TexHUDIconUser,
				IconSize = new FSize(50, 50),
			});
		}

		public void SelectNode(LevelNode n)
		{
			SelectedNode = n;
			InfoDisplay.ResetCycle();
			
			foreach (var node in GDOwner.GetEntities<LevelNode>())
			{
				if (node != n && (node.IsOpening || node.IsOpened)) node.CloseNode();
			}
		}

		public void ShowAccountPanel()
		{
			var profile = MainGame.Inst.Profile;

			if (profile.AccountType == AccountType.Local)
			{
				return; //TODO What do?
			}
			else if (profile.AccountType == AccountType.Anonymous)
			{
				SelectNode(null);
				Settings.Close();

				AddElement(new HUDModalDialog(new AnonymousAccountPanel()));
			}
			else if (profile.AccountType == AccountType.Full)
			{
				//TODO full acc panel
			}
		}
	}
}
