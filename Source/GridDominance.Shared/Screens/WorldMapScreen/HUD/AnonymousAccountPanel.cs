using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
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

				Text = "Sign up / Log in",
				TextColor = FlatColors.Clouds,
			});

			AddElement(editUsername = new HUDIconTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 100),
				Size = new FSize(WIDTH - 40, 80),

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


			AddElement(editPassword = new HUDIconTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 200),
				Size = new FSize(WIDTH - 40, 80),

				Font = Textures.HUDFontRegular,
				FontSize = 40,

				Placeholder = "Password",
				IsPassword = true,

				BackgoundCornerSize = 8,
				ColorPadLeft = FlatColors.ControlHighlight,
				WidthPadLeft = 80,
				IconColor = FlatColors.Asbestos,
				Icon = Textures.TexHUDIconPassword,
				IconSize = new FSize(50, 50),
			});

			AddElement(new HUDTextButton(1, OnCreateAccount)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 3 * 24) / 2, 64),

				Text = "Create Account",
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,
			});

			AddElement(new HUDTextButton(1, OnLogin)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 3 * 24) / 2, 64),

				Text = "Login",
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

		private void OnLogin(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (editUsername.Text == "" || editPassword.Text == "") return;

			var waitDialog = new HUDIconMessageBox
			{
				Text = "Logging in",
				TextColor = FlatColors.Clouds,
				ColorBackground = FlatColors.MidnightBlue,

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCog,
				RotationSpeed = 1f,

				CloseOnClick = false,

			};

			HUD.AddModal(waitDialog, false, 0.7f);

			DoLogin(waitDialog, editUsername.Text, editPassword.Text).EnsureNoError();
		}

		private async Task DoLogin(HUDElement spinner, string username, string password)
		{
			try
			{
				var profile = MainGame.Inst.Profile;

				var r = await MainGame.Inst.Backend.Verify(username, password);
				var verifyResult = r.Item1;
				var verifyUserID = r.Item2;

				if (verifyResult != VerifyResult.Success)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						var text = "???";
						if (verifyResult == VerifyResult.InternalError) text = "Internal server error";
						if (verifyResult == VerifyResult.WrongPassword) text = "Wrong password";
						if (verifyResult == VerifyResult.WrongUsername) text = "User not found";
						if (verifyResult == VerifyResult.NoConnection) text = "Could not communicate with server";

						spinner.Remove();
						HUD.AddModal(new HUDMessageBox
						{
							Text = text,
							TextColor = FlatColors.Clouds,
							ColorBackground = FlatColors.Alizarin,

							CloseOnClick = true,

						}, true, 0.7f);

						Remove();
					});
					return;
				}

				await MonoSAMGame.CurrentInst.DispatchInvoke(() =>
				{
					profile.OnlineUserID = verifyUserID;
					profile.AccountType = AccountType.Full;
					profile.OnlinePasswordHash = MainGame.Inst.Bridge.DoSHA256(password);
					profile.OnlineUsername = username;
				});

				await MainGame.Inst.Backend.Reupload(profile);
				await MainGame.Inst.Backend.DownloadData(profile);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					var text = "???";
					if (verifyResult == VerifyResult.InternalError) text = "Internal server error";
					if (verifyResult == VerifyResult.WrongPassword) text = "Wrong password";
					if (verifyResult == VerifyResult.WrongUsername) text = "User not found";
					if (verifyResult == VerifyResult.NoConnection) text = "Could not communicate with server";

					spinner.Remove();
					HUD.AddModal(new HUDMessageBox
					{
						Text = "Successfully logged in",
						TextColor = FlatColors.TextHUD,
						ColorBackground = FlatColors.Nephritis,

						CloseOnClick = true,

					}, true, 0.7f);

					Remove();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Login", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					HUD.AddModal(new HUDMessageBox
					{
						Text = "Could not login",
						TextColor = FlatColors.Clouds,
						ColorBackground = FlatColors.Alizarin,

						CloseOnClick = true,

					}, true, 0.7f);

					Remove();
				});
			}
		}

		private void OnCreateAccount(HUDTextButton sender, HUDButtonEventArgs e)
		{
			throw new System.NotImplementedException();
		}
	}
}
