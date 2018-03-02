using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.OverworldScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System.Threading.Tasks;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class FullAccountPanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private int _logOutCounter = 0;
		private float _lastClickLogout;

		private HUDImageButton btnReload;
		private bool isUploading = false;

		public FullAccountPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateRounded(FlatColors.BackgroundHUD2, 16, false, false, true, true),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 100),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_FAP_ACCOUNT,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 100),
				Size = new FSize((WIDTH - 60) / 2, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 64,
				TextColor = FlatColors.Clouds,

				L10NText = L10NImpl.STR_FAP_USERNAME,
			});

			AddElement(new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20 + (WIDTH - 60) / 2 + 20, 100),
				Size = new FSize((WIDTH - 60) / 2, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 64,
				TextColor = FlatColors.Clouds,

				Text = MainGame.Inst.Profile.OnlineUsername,
			});

			AddElement(new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 200),
				Size = new FSize((WIDTH - 60) / 2, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 64,
				TextColor = FlatColors.Clouds,

				L10NText = L10NImpl.STR_FAP_SCORE,
			});

			AddElement(new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20 + (WIDTH - 60) / 2 + 20, 200),
				Size = new FSize((WIDTH - 60) / 2, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 64,
				TextColor = FlatColors.Clouds,

				Text = MainGame.Inst.Profile.TotalPoints.ToString(),
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize(4 * GDConstants.TILE_WIDTH, 64),

				L10NText = L10NImpl.STR_FAP_LOGOUT,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ButtonPressedHUD, 16),

				Click = OnLogout,
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 64),

				L10NText = L10NImpl.STR_FAP_CHANGEPW,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ButtonPressedHUD, 16),

				Click = OnChangePassword,
			});

			AddElement(btnReload = new HUDImageButton(2)
			{
				Alignment = HUDAlignment.TOPRIGHT,
				RelativePosition = new FPoint(6, 6),
				Size = new FSize(64, 64),

				BackgroundNormal = HUDBackgroundDefinition.NONE,
				BackgroundPressed = HUDBackgroundDefinition.NONE,

				ImageAlignment = HUDImageAlignmentAlgorithm.CENTER,
				ImageScale     = HUDImageScaleAlgorithm.UNDERSCALE,
				Image = Textures.TexHUDIconReload,
				ImageColor = MainGame.Inst.Profile.NeedsReupload ? FlatColors.Carrot : FlatColors.Clouds,
				ImageRotationSpeed = 0f,

				ClickMode = HUDButton.HUDButtonClickMode.Single,
				Click = OnReuploadProfile,
			});
		}

		private void OnReuploadProfile(HUDImageButton sender, HUDButtonEventArgs e)
		{
			if (isUploading) return;
			isUploading = true;

			btnReload.ImageRotationSpeed = 0.5f;
			MainGame.Inst.ShowToast(null, L10N.T(L10NImpl.STR_PROFILESYNC_START), 40, FlatColors.Silver, FlatColors.Foreground, 2f);
			MainGame.Inst.Profile.NeedsReupload = true;

			DoReuploadProfile(MainGame.Inst.Profile).RunAsync();
		}

		private async Task DoReuploadProfile(PlayerProfile p)
		{
			try
			{
				// ======== STEP 1 -- UPLOAD ========

				bool sucess = await MainGame.Inst.Backend.Reupload(p);
				if (!sucess)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						btnReload.ImageRotationSpeed = 0f;
						btnReload.ImageColor = FlatColors.Carrot;

						MainGame.Inst.ShowToast(null, L10N.T(L10NImpl.STR_PROFILESYNC_ERROR), 40, FlatColors.Orange, FlatColors.Foreground, 2f);
					});

					return;
				}

				// ======== STEP 2 -- DOWNLOAD ========

				sucess = await MainGame.Inst.Backend.DownloadData(p);
				if (!sucess)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						btnReload.ImageRotationSpeed = 0f;
						btnReload.ImageColor = FlatColors.Carrot;

						MainGame.Inst.ShowToast(null, L10N.T(L10NImpl.STR_PROFILESYNC_ERROR), 40, FlatColors.Orange, FlatColors.Foreground, 2f);
					});

					return;
				}

				// ======== FINISHED ========

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					btnReload.ImageRotationSpeed = 0f;
					btnReload.ImageColor = FlatColors.Clouds;

					MainGame.Inst.ShowToast(null, L10N.T(L10NImpl.STR_PROFILESYNC_SUCCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2f);
				});
			}
			finally
			{
				isUploading = false;
			}
		}

		private void OnChangePassword(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new ChangePasswordPanel(), true);
		}

		private void OnLogout(HUDTextButton sender, HUDButtonEventArgs e)
		{
			var delta = MonoSAMGame.CurrentTime.TotalElapsedSeconds - _lastClickLogout;

			if (delta > 5f) _logOutCounter = 0;

			if (_logOutCounter == 0)
			{
				HUD.ShowToast(null, L10N.T(L10NImpl.STR_FAP_WARN1), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				_lastClickLogout = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
				_logOutCounter++;
				return;
			}
			else if (_logOutCounter == 1)
			{
				if (delta < 0.5f) return;
				
				HUD.ShowToast(null, L10N.T(L10NImpl.STR_FAP_WARN2), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				_lastClickLogout = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
				_logOutCounter++;
				return;
			}
			else if (_logOutCounter == 2)
			{
				MainGame.Inst.Profile.LevelData.Clear();
				
				MainGame.Inst.Profile.AccountType = AccountType.Local;
				MainGame.Inst.Profile.OnlineUserID = -1;
				MainGame.Inst.Profile.OnlineUsername = "anonymous";
				MainGame.Inst.Profile.OnlinePasswordHash = string.Empty;
				MainGame.Inst.Profile.OnlineRevisionID = -1;
				MainGame.Inst.Profile.NeedsReupload = false;

				MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile).ContinueWith(t => MainGame.Inst.Backend.DownloadHighscores(MainGame.Inst.Profile)).EnsureNoError();

				HUD.ShowToast(null, L10N.T(L10NImpl.STR_FAP_LOGOUT_SUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 1.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}
		}
	}
}
