using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
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

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDMultiplayerScorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;
		public const float FOOTER_COLBAR_HEIGHT = GDConstants.TILE_WIDTH / 4f;
		public const float ICON_MARGIN = GDConstants.TILE_WIDTH * (3/8f);
		public const float ICON_SIZE = GDConstants.TILE_WIDTH * 2;

		private GDGameScreen GDScreen => (GDGameScreen)HUD.Screen;

		public override int Depth => 0;

		private readonly bool successScreen;
		private readonly PlayerProfile profile;
		private readonly LevelBlueprint Level;

		private readonly GDMultiplayerCommon _server;
		private readonly Action _preventStopOnRem;

		private HUDIconTextButton _btnNext;
		private HUDImage _loadingCog;

		public HUDMultiplayerScorePanel(LevelBlueprint lvl, PlayerProfile playerprofile, bool playerHasWon, GDMultiplayerCommon srv, Action preventStopOnRem)
		{
			successScreen = playerHasWon;
			profile = playerprofile;
			Level = lvl;
			_server = srv;
			_preventStopOnRem = preventStopOnRem;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize() //TODO a little bit ... empty
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

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_LEVEL,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = Level.Name,
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(_loadingCog = new HUDImage(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 16),
				Size = new FSize(WIDTH / 3f, 80),

				Image = Textures.CannonCog,
				RotationSpeed = 0.3f,
				Color = FlatColors.TextHUD,
				ImageAlignment = HUDImageAlignment.UNDERSCALE,

				IsVisible = false,
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

				Click = (s, a) => GDScreen.ExitToMap(),
			});

			if (_server.SessionUserID== 0 && _server.Mode != SAMNetworkConnection.ServerMode.Stopped && _server.Mode != SAMNetworkConnection.ServerMode.Error)
			{
				AddElement(_btnNext = new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
					Size = new FSize(6f * GDConstants.TILE_WIDTH, 60),

					L10NText = L10NImpl.STR_HSP_NEWGAME,
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconRedo,

					BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.SunFlower, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Orange, 16),

					Click = (s, a) => OnNextLevel(),
				});
			}

			#endregion

			#region Icons

			var finDiff0 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_EASY);
			var finDiff1 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_NORMAL);
			var finDiff2 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_HARD);
			var finDiff3 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_IMPOSSIBLE);

			var modeDiff0 =
				finDiff0 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff1 =
				finDiff1 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff2 =
				finDiff2 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff3 =
				finDiff3 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_EASY, modeDiff0)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_NORMAL, modeDiff1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_HARD, modeDiff2)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_IMPOSSIBLE, modeDiff3)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(7 * ICON_MARGIN + 3 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			#endregion
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped || _server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				if (_btnNext != null)
				{
					_btnNext.BackgroundNormal  = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16);
					_btnNext.BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16);
					_btnNext.IsEnabled = false;
				}
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.BroadcastNewGame ||_server.Mode == SAMNetworkConnection.ServerMode.BeforeNewGame)
			{
				_loadingCog.IsVisible = true;
			}

			if (_server.SessionUserID == 0)
			{
				if (_server.Mode == SAMNetworkConnection.ServerMode.CreatingNewGame)
				{
					_preventStopOnRem();

					Remove();
					HUD.AddModal(new MultiplayerRehostPanel(_server as GDMultiplayerServer), false, 0.5f, 0.5f);
				}
			}
			else
			{
				if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby)
				{
					_preventStopOnRem();

					Remove();
					HUD.AddModal(new MultiplayerClientLobbyPanel(_server as GDMultiplayerClient), false, 0.5f, 0.5f);
				}
			}
		}

		private void OnNextLevel()
		{
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped || _server.Mode == SAMNetworkConnection.ServerMode.Error) return;

			if (_server.Mode != SAMNetworkConnection.ServerMode.BroadcastNewGame)
			{
				_server.StartBroadcastNewGame();
			}
		}
	}
}
