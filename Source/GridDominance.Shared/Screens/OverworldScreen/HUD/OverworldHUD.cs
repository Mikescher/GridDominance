using System.Threading.Tasks;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.RenderHelper;
using System;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
	public class OverworldHUD : GameHUD, ISettingsOwnerHUD
	{
		public GDOverworldScreen GDOwner => (GDOverworldScreen) Screen;

		public readonly SettingsButton Settings;
		public readonly ScoreDisplay ScoreDisplay;
		public readonly MultiplayerScoreDisplay MPScoreDisplay;

		public OverworldHUD(GDOverworldScreen scrn, bool firstShow) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(Settings       = new SettingsButton());
			AddElement(ScoreDisplay   = new ScoreDisplay(firstShow));
			AddElement(MPScoreDisplay = new MultiplayerScoreDisplay(ScoreDisplay, firstShow));


			AddElement(new HUDLabel
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, -200),

				AutoSize = true,

				Background = HUDBackgroundDefinition.CreateSimple(Color.LightBlue),
				TextColor = Color.Red,

				FontSize = 64,
				Text = "INTERNAL ALPHA VERSION 4",
			});
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
			AddModal(new HighscorePanel(null, false), true);
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
				Text = "Contacting server",
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


#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}