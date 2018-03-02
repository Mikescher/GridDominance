using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.RenderHelper;
using GridDominance.Shared.Network.Multiplayer;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDSCCMUploadScorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;
		public const float FOOTER_COLBAR_HEIGHT = GDConstants.TILE_WIDTH / 4f;
		public const float ICON_MARGIN = GDConstants.TILE_WIDTH * (3/8f);
		public const float ICON_SIZE = GDConstants.TILE_WIDTH * 2;

		private GDGameScreen GDScreen => (GDGameScreen)HUD.Screen;

		public override int Depth => 0;

		private readonly LevelBlueprint Level;
		private readonly SCCMLevelData SCCMData;
		private readonly FractionDifficulty Diff;
		private readonly GameSpeedModes Speed;
		private readonly bool CanUpload;

		public HUDSCCMUploadScorePanel(LevelBlueprint lvl, SCCMLevelData dat, FractionDifficulty d, GameSpeedModes s, bool playerWon)
		{
			Level = lvl;
			SCCMData = dat;
			Diff = d;
			Speed = s;
			CanUpload = playerWon;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			#region Footer

			AddElement(new HUDRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT - 10),

				Definition = HUDBackgroundDefinition.CreateRounded(FlatColors.BackgroundHUD2, 16, false, false, true, true),
			});


			AddElement(new HUDRectangle(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateSimple(FlatColors.Nephritis),
			});

			AddElement(new HUDRectangle(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 2f, FOOTER_COLBAR_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateSimple(FlatColors.PeterRiver),
			});

			AddElement(new HUDRectangle(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateSimple(FlatColors.Pomegranate),
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(WIDTH / 3f, 0),
				Size = new FSize(1, FOOTER_HEIGHT),

				Color = FlatColors.SeperatorHUD,
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(WIDTH / 3f, 0),
				Size = new FSize(1, FOOTER_HEIGHT),

				Color = FlatColors.SeperatorHUD,
			});

			#endregion

			#region Buttons

			AddElement(new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

				L10NText = L10NImpl.STR_HSP_BACK,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconBack,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				Click = (s, a) => { MainGame.Inst.SetLevelEditorScreen(SCCMData); },
			});

			if (CanUpload)
			{
				var w = L10N.LANGUAGE == L10N.LANG_EN_US ? 3.5f : 5.0f;
				AddElement(new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
					Size = new FSize(w * GDConstants.TILE_WIDTH, 60),

					L10NText = L10NImpl.STR_LVLED_BTN_UPLOAD,
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconUpload,

					BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Emerald, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Nephritis, 16),

					Click = DoUpload,
				});
			}
			else
			{
				var w = L10N.LANGUAGE == L10N.LANG_EN_US ? 3.5f : 5.0f;
				AddElement(new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
					Size = new FSize(w * GDConstants.TILE_WIDTH, 60),

					L10NText = L10NImpl.STR_HSP_AGAIN,
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconRedo,

					BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Orange, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.SunFlower, 16),

					Click = (s, a) => ((GDGameScreen)HUD.Screen).RestartLevel(false),
				});
			}

			#endregion

			#region Icons

			AddElement(new HUDSCCMUploadDifficultyButton(2, CanUpload, 0)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDSCCMUploadDifficultyButton(2, CanUpload, 1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDSCCMUploadDifficultyButton(2, CanUpload, 2)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDSCCMUploadDifficultyButton(2, CanUpload, 3)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(7 * ICON_MARGIN + 3 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			#endregion
		}

		private void DoUpload(HUDIconTextButton sender, HUDButtonEventArgs e)
		{
			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_LVLED_UPLOADING,
				TextColor = FlatColors.TextHUD,
				Background = HUDBackgroundDefinition.CreateRounded(FlatColors.BelizeHole, 16),

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCogBig,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			HUD.AddModal(waitDialog, false, 0.7f);

			DoUploadInternal(waitDialog).RunAsync();
		}

		private async Task DoUploadInternal(HUDElement spinner)
		{
			try
			{
				byte[] binData;
				using (var ms = new MemoryStream())
				using (var bw = new BinaryWriter(ms))
				{
					Level.BinarySerialize(bw, true, GDConstants.IntVersion, MainGame.Inst.Profile.OnlineUserID, SCCMData.OnlineID);
					binData = ms.ToArray();
				}

				var result = await MainGame.Inst.Backend.UploadUserLevel(MainGame.Inst.Profile, Level, SCCMData, binData);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();

					switch (result)
					{
						case UploadResult.InternalError:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLUPLD_ERR_INTERNAL,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.NoConnection:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_CPP_COMERR,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.FileTooBig:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLUPLD_ERR_FILETOOBIG,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.WrongUserID:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLUPLD_ERR_WRONGUSER,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.LevelIDNotFound:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLUPLD_ERR_LIDNOTFOUND,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.InvalidName:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLUPLD_ERR_INVALIDNAME,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.DuplicateName:
							HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLUPLD_ERR_DUPLNAME,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

								CloseOnClick = true,

							}, true);
							break;
						case UploadResult.AlreadyUploaded:
						case UploadResult.Success:
							SCCMUtils.UpgradeLevel(SCCMData, binData);

							MainGame.Inst.GetCurrentScreen().HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
							{
								L10NText = L10NImpl.STR_LVLED_UPLOAD_FIN,
								TextColor = FlatColors.Clouds,
								Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Nephritis, 16),

								CloseOnClick = true,

							}, true);
							
							MainGame.Inst.SetOverworldScreenWithSCCM(SCCMMainPanel.SCCMTab.MyLevels);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMUSP::DUI", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						L10NText = L10NImpl.STR_CPP_COMERR,
						TextColor = FlatColors.Clouds,
						Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

						CloseOnClick = true,

					}, true);
				});
			}
		}
	}
}
