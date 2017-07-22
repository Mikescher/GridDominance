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
using GridDominance.Shared.Screens.WorldMapScreen;
using MonoSAMFramework.Portable.GameMath;

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
		private readonly int _deltaPoints;
		private readonly PlayerProfile profile;
		private readonly LevelBlueprint Level;

		private readonly GDMultiplayerCommon _server;
		private readonly GDMultiplayerServer _serverHost;
		private readonly Action _preventStopOnRem;
		private bool _nextLevelRandom = false;

		private HUDIconTextButton _btnNext;
		private HUDIconTextButton _btnRand;
		private HUDImage _loadingCog;

		public HUDMultiplayerScorePanel(LevelBlueprint lvl, PlayerProfile playerprofile, bool playerHasWon, int addPoints, GDMultiplayerCommon srv, Action preventStopOnRem)
		{
			successScreen = playerHasWon;
			profile = playerprofile;
			Level = lvl;
			_server = srv;
			_serverHost = srv as GDMultiplayerServer;
			_preventStopOnRem = preventStopOnRem;
			_deltaPoints = addPoints;

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

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_MPPOINTS,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(new HUDIncrementIndicatorLabel(profile.MultiplayerPoints.ToString(), (successScreen ? "+" : "-") + Math.Abs(_deltaPoints), 2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

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

				Image = Textures.CannonCogBig,
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

				Click = (s, a) => { _server.KillSession(); _server.Stop(); GDScreen.ExitToMap(); },
			});

			if (_server.SessionUserID== 0 && _server.Mode != SAMNetworkConnection.ServerMode.Stopped && _server.Mode != SAMNetworkConnection.ServerMode.Error)
			{
				AddElement(_btnRand = new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 90),
					Size = new FSize(6f * GDConstants.TILE_WIDTH, 60),

					L10NText = L10NImpl.STR_HSP_RANDOMGAME,
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconDice,

					BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Wisteria, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Amethyst, 16),

					Click = (s, a) => OnRandomLevel(),
				});

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

			AddElement(new HUDMultiplayerDifficultyButton(2, successScreen, 0)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDMultiplayerDifficultyButton(2, successScreen, 1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDMultiplayerDifficultyButton(2, successScreen, 2)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN),
				IsEnabled = false,
			});

			AddElement(new HUDMultiplayerDifficultyButton(2, successScreen, 3)
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
					_btnNext.BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16);
					_btnNext.BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16);
					_btnNext.IsEnabled = false;
					
				}
				if (_btnRand != null)
				{
					_btnRand.BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16);
					_btnRand.BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16);
					_btnRand.IsEnabled = false;
				}
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.BroadcastNewGame ||
				_server.Mode == SAMNetworkConnection.ServerMode.BeforeNewGame ||
				_server.Mode == SAMNetworkConnection.ServerMode.CreatingNewGame)
			{
				_loadingCog.IsVisible = true;
			}

			if (_server.SessionUserID == 0 && _nextLevelRandom)
			{
				if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby)
				{
					byte[] binData = _serverHost.GetLobbySyncData();
					_server.StartLobbySync(binData);
				}

				if (_server.Mode == SAMNetworkConnection.ServerMode.InGame)
				{
					_preventStopOnRem();
					MainGame.Inst.SetMultiplayerServerLevelScreen(Levels.LEVELS[_serverHost.LevelID], _serverHost.Speed, _serverHost.MusicIndex, _serverHost);
				}

			}

			if (_server.SessionUserID == 0)
			{
				if (_server.Mode == SAMNetworkConnection.ServerMode.CreatingNewGame)
				{
					if (_nextLevelRandom)
					{
						SetLevelDataRandom();

						_server.Mode = SAMNetworkConnection.ServerMode.InLobby;

						byte[] binData = _serverHost.GetLobbySyncData();
						_serverHost.StartLobbySync(binData);
					}
					else
					{
						_preventStopOnRem();
						Remove();

						HUD.AddModal(new MultiplayerRehostPanel(_server as GDMultiplayerServer), false, 0.5f, 0.5f);
					}
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

		private void SetLevelDataRandom()
		{
			List<LevelBlueprint> pool = new List<LevelBlueprint>();

			foreach (var world in Levels.WORLDS_MULTIPLAYER)
			{
				if (world.ID != Levels.WORLD_001.ID && !world.Nodes.Any(n => MainGame.Inst.Profile.GetLevelData(n).HasAnyCompleted())) continue;

				foreach (var item in world.Nodes.Select(p => Levels.LEVELS[p.LevelID]).Where(l => BlueprintAnalyzer.PlayerCount(l) == _server.SessionCapacity))
				{
					if (item.UniqueID != _serverHost.LevelID) pool.Add(item);
				}
			}

			if (!pool.Any()) return;

			_serverHost.LevelID = pool[FloatMath.GetRangedIntRandom(pool.Count)].UniqueID;
		}

		private void OnNextLevel()
		{
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped || _server.Mode == SAMNetworkConnection.ServerMode.Error) return;

			if (_server.Mode == SAMNetworkConnection.ServerMode.BroadcastAfterGame)
			{
				_server.StartBroadcastNewGame();
				_nextLevelRandom = false;

				_btnNext.IsVisible = false;
				_btnRand.IsVisible = false;
			}
		}

		private void OnRandomLevel()
		{
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped || _server.Mode == SAMNetworkConnection.ServerMode.Error) return;
			if (_serverHost == null) return;

			if (_server.Mode == SAMNetworkConnection.ServerMode.BroadcastAfterGame)
			{
				_server.StartBroadcastNewGame();
				_nextLevelRandom = true;

				_btnNext.IsVisible = false;
				_btnRand.IsVisible = false;
			}
		}
	}
}
