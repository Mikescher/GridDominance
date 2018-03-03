using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD;
using GridDominance.Shared.Screens.Common.HUD.Elements;
using GridDominance.Shared.Screens.EndGameScreen;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public class GDGameEndHUD : GameHUD, ISettingsOwnerHUD
	{
		public readonly SettingsButton Settings;
		public readonly ScoreDisplayManager ScoreDispMan;

		public GDGameEndHUD(GDEndGameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(Settings = new SettingsButton());
			
			ScoreDispMan = new ScoreDisplayManager(this, false);
		}

		public void ShowAccountPanel()
		{
			var profile = MainGame.Inst.Profile;

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

		public void ShowHighscorePanel()
		{
			Settings.Close();
			AddModal(new HighscorePanel(null, HighscoreCategory.GlobalPoints), true);
		}

		public void ShowAboutPanel()
		{
			Settings.Close();

			AddModal(new InfoPanel(), true);
		}

		private async Task CreateUserAndShowAnonPanel()
		{
			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_GLOB_WAITFORSERVER,
				TextColor = FlatColors.TextHUD,
				Background = HUDBackgroundDefinition.CreateRounded(FlatColors.BelizeHole, 16),

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCogBig,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			MainGame.Inst.DispatchBeginInvoke(() => { AddModal(waitDialog, false, 0.7f); });

			await MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile);

			waitDialog.Remove();

			MainGame.Inst.DispatchBeginInvoke(() =>
			{
				if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous)
				{
					AddModal(new AnonymousAccountPanel(), true);
				}
			});
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			ScoreDispMan.Update();

#if DEBUG
			root.IsVisible = !DebugSettings.Get("HideHUD");
#endif
		}
	}
}
