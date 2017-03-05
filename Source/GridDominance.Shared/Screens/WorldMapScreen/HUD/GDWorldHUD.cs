using System.Threading.Tasks;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
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

			ShowKeyboard();//TODO DEUBG
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

			SelectNode(null);
			Settings.Close();

			if (profile.AccountType == AccountType.Local)
			{
				CreateUserAndShowAnonPanel().EnsureNoError();
			}
			else if (profile.AccountType == AccountType.Anonymous)
			{
				AddModal(new AnonymousAccountPanel(), true);
			}
			else if (profile.AccountType == AccountType.Full)
			{
				AddModal(new FullAccountPanel(), true);
			}
		}

		private async Task CreateUserAndShowAnonPanel()
		{
			var waitDialog = new HUDIconMessageBox
			{
				Text = "Contacting server",
				TextColor = FlatColors.TextHUD,
				ColorBackground = FlatColors.BelizeHole,

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCog,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			MainGame.Inst.DispatchBeginInvoke(() => { AddModal(waitDialog, false, 0.7f); });

			await MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile);

			if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous)
			{
				MainGame.Inst.DispatchBeginInvoke(() => { AddModal(new AnonymousAccountPanel(), true); });
			}
		}
	}
}
