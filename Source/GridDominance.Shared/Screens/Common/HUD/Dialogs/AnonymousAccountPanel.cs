using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD.HUDOperations;
using GridDominance.Shared.Screens.OverworldScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class AnonymousAccountPanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private HUDTextBox editUsername;
		private HUDTextBox editPassword;

		public AnonymousAccountPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDRoundedRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT),

				Color = FlatColors.BackgroundHUD2,
				RoundCornerTL = false,
				RoundCornerTR = false,
				RoundCornerBL = true,
				RoundCornerBR = true,
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 100),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_AAP_HEADER,
				TextColor = FlatColors.Clouds,
			});

			AddElement(editUsername = new HUDIconTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 100),
				Size = new FSize(WIDTH - 40, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 40,

				Placeholder = L10N.T(L10NImpl.STR_AAP_USERNAME),
				MaxLength = 16,

				BackgoundCornerSize = 8,
				ColorPadLeft = FlatColors.ControlHighlight,
				WidthPadLeft = 80,
				IconColor = FlatColors.Asbestos,
				Icon = Textures.TexHUDIconUser,
				IconSize = new FSize(50, 50),
			});
			
			AddElement(editPassword = new HUDIconTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 200),
				Size = new FSize(WIDTH - 40, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 40,

				Placeholder = L10N.T(L10NImpl.STR_AAP_PASSWORD),
				IsPassword = true,

				BackgoundCornerSize = 8,
				ColorPadLeft = FlatColors.ControlHighlight,
				WidthPadLeft = 80,
				IconColor = FlatColors.Asbestos,
				Icon = Textures.TexHUDIconPassword,
				IconSize = new FSize(50, 50),
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 3 * 24) / 2, 64),

				L10NText = L10NImpl.STR_AAP_CREATEACCOUNT,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgroundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,

				Click = OnCreateAccount,
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 3 * 24) / 2, 64),

				L10NText = L10NImpl.STR_AAP_LOGIN,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgroundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,

				Click = OnLogin,
			});
		}

		private void OnLogin(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (editUsername.Text == "")
			{
				editUsername.AddHUDOperation(new HUDTextBoxBlinkOperation(Color.Red));
				return;
			}

			if (editPassword.Text == "")
			{
				editPassword.AddHUDOperation(new HUDTextBoxBlinkOperation(Color.Red));
				return;
			}

			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_AAP_LOGGINGIN,
				TextColor = FlatColors.TextHUD,
				ColorBackground = FlatColors.BelizeHole,

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCog,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			HUD.AddModal(waitDialog, false, 0.7f);

			DoLogin(waitDialog, editUsername.Text, editPassword.Text).RunAsync();
		}

		private async Task DoLogin(HUDElement spinner, string username, string password)
		{
			try
			{
				var profile = MainGame.Inst.Profile;

				var r = await MainGame.Inst.Backend.MergeLogin(profile, username, password);
				var verifyResult = r.Item1;

				if (verifyResult != VerifyResult.Success)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						var text = "???";
						if (verifyResult == VerifyResult.InternalError) text = r.Item2;
						if (verifyResult == VerifyResult.WrongPassword) text = L10N.T(L10NImpl.STR_AAP_WRONGPW);
						if (verifyResult == VerifyResult.WrongUsername) text = L10N.T(L10NImpl.STR_AAP_USERNOTFOUND);
						if (verifyResult == VerifyResult.NoConnection)  text = L10N.T(L10NImpl.STR_AAP_NOCOM);

						spinner.Remove();
						HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
						{
							Text = text,
							TextColor = FlatColors.Clouds,
							ColorBackground = FlatColors.Alizarin,

							CloseOnClick = true,

						}, true);

						Remove();
					});
					return;
				}
				
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();

					MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
					var screen = MainGame.Inst.GetCurrentScreen() as GameScreen;
					screen?.HUD?.AddModal(new HUDFadeOutInfoBox(3, 1, 0.3f)
					{
						L10NText = L10NImpl.STR_AAP_LOGINSUCCESS,
						TextColor = FlatColors.TextHUD,
						ColorBackground = FlatColors.Nephritis,

						CloseOnClick = true,

					}, true);

					Remove();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Login", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						L10NText = L10NImpl.STR_AAP_NOLOGIN,
						TextColor = FlatColors.Clouds,
						ColorBackground = FlatColors.Alizarin,

						CloseOnClick = true,

					}, true);

					Remove();
				});
			}
		}

		private void OnCreateAccount(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (editUsername.Text == "" || editPassword.Text == "") return;

			if (MainGame.Inst.Profile.AccountType != AccountType.Anonymous) return;

			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_AAP_ACCCREATING,
				TextColor = FlatColors.TextHUD,
				ColorBackground = FlatColors.BelizeHole,

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCog,
				RotationSpeed = 1f,

				CloseOnClick = false,

			};

			HUD.AddModal(waitDialog, false, 0.7f);

			DoSignup(waitDialog, editUsername.Text, editPassword.Text).RunAsync();
		}

		private async Task DoSignup(HUDElement spinner, string username, string password)
		{
			try
			{
				var profile = MainGame.Inst.Profile;

				var r = await MainGame.Inst.Backend.UpgradeUser(profile, username, password);

				switch (r.Item1)
				{
					case UpgradeResult.Success:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(3, 1, 0.3f)
							{
								L10NText = L10NImpl.STR_AAP_ACCCREATED,
								TextColor = FlatColors.TextHUD,
								ColorBackground = FlatColors.Nephritis,
								CloseOnClick = true,
							}, true);

							Remove();
						});
						break;
					case UpgradeResult.UsernameTaken:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_AAP_USERTAKEN,
								TextColor = FlatColors.Clouds,
								ColorBackground = FlatColors.Alizarin,
								CloseOnClick = true,
							}, true);
						});
						break;
					case UpgradeResult.AlreadyFullAcc:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_AAP_ALREADYCREATED,
								TextColor = FlatColors.Clouds,
								ColorBackground = FlatColors.Alizarin,
								CloseOnClick = true,
							}, true);
							Remove();
						});
						break;
					case UpgradeResult.InternalError:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(10, 2, 0.3f)
							{
								Text = r.Item2,
								TextColor = FlatColors.Clouds,
								ColorBackground = FlatColors.Alizarin,
								CloseOnClick = true,
							}, true);
							Remove();
						});
						break;
					case UpgradeResult.NoConnection:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_AAP_NOCOM,
								TextColor = FlatColors.Clouds,
								ColorBackground = FlatColors.Alizarin,
								CloseOnClick = true,
							}, true);
							Remove();
						});
						break;
					case UpgradeResult.AuthError:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_AAP_AUTHERROR,
								TextColor = FlatColors.Clouds,
								ColorBackground = FlatColors.Alizarin,
								CloseOnClick = true,
							}, true);
							Remove();
						});
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("CreateAccount", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						L10NText = L10NImpl.STR_AAP_COULDNOTCREATE,
						TextColor = FlatColors.Clouds,
						ColorBackground = FlatColors.Alizarin,
						CloseOnClick = true,
					}, true);

					Remove();
				});
			}
		}
	}
}
