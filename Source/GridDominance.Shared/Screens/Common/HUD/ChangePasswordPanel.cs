using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD.HUDOperations;
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

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class ChangePasswordPanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private HUDTextBox editPassword;

		public ChangePasswordPanel()
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

				Text = "Change Password",
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

				Text = "Username:",
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

			AddElement(editPassword = new HUDIconTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 200),
				Size = new FSize(WIDTH - 40, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 40,

				Placeholder = "New Password",
				IsPassword = true,

				BackgoundCornerSize = 8,
				ColorPadLeft = FlatColors.ControlHighlight,
				WidthPadLeft = 80,
				IconColor = FlatColors.Asbestos,
				Icon = Textures.TexHUDIconPassword,
				IconSize = new FSize(50, 50),
			});
			
			AddElement(new HUDTextButton(1, OnChangePassword)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 2 * 24), 64),

				Text = "Change",
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,
			});
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		private void OnChangePassword(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (editPassword.Text == "")
			{
				editPassword.AddHUDOperation(new HUDTextBoxBlinkOperation(Color.Red));
				return;
			}

			if (MainGame.Inst.Profile.AccountType != AccountType.Full) return;

			var waitDialog = new HUDIconMessageBox
			{
				Text = "Changing password",
				TextColor = FlatColors.TextHUD,
				ColorBackground = FlatColors.BelizeHole,

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCog,
				RotationSpeed = 1f,

				CloseOnClick = false,

			};

			HUD.AddModal(waitDialog, false, 0.7f);

			DoChangePassword(waitDialog, editPassword.Text).RunAsync();
		}


		private async Task DoChangePassword(HUDElement spinner, string newPassword)
		{
			try
			{
				var profile = MainGame.Inst.Profile;

				var r = await MainGame.Inst.Backend.ChangePassword(profile, newPassword);

				switch (r.Item1)
				{
					case ChangePasswordResult.Success:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(3, 1, 0.3f)
							{
								Text = "Password changed",
								TextColor = FlatColors.TextHUD,
								ColorBackground = FlatColors.Nephritis,
								CloseOnClick = true,
							}, true);

							Remove();
						});
						break;
					case ChangePasswordResult.InternalError:
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
					case ChangePasswordResult.NoConnection:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								Text = "Could not communicate with server",
								TextColor = FlatColors.Clouds,
								ColorBackground = FlatColors.Alizarin,
								CloseOnClick = true,
							}, true);
							Remove();
						});
						break;
					case ChangePasswordResult.AuthError:
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							spinner.Remove();
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								Text = "Authentication error",
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
				SAMLog.Error("ChangePassword", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						Text = "Could not change password",
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
