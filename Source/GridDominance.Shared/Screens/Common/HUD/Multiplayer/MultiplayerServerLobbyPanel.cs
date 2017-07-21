using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Multiplayer;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Operations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Cryptography;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
    class MultiplayerServerLobbyPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.5f * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2.0f * GDConstants.TILE_WIDTH;

		public const float INFO_C1_LEFT  = 58;
		public const float INFO_C1_WIDTH = 260;
		public const float INFO_C2_LEFT  = 320;
		public const float INFO_C2_WIDTH = 225;

		public override int Depth => 0;

		private readonly GDMultiplayerServer _server;
		private bool _doNotStop = false;

		public readonly HUDCharacterControl[] CharDisp = new HUDCharacterControl[8];
		private HUDTextButton _btnStart;

		public MultiplayerServerLobbyPanel(GDMultiplayerServer server)
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_server = server;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 100),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_MENU_CAP_LOBBY,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new MultiplayerConnectionStateControl(_server)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(16, 16)
			});

			if (_server.ConnType == MultiplayerConnectionType.PROXY)
			{

				var gridDisplay = new HUDFixedUniformGrid
				{
					Alignment = HUDAlignment.TOPCENTER,
					RelativePosition = new FPoint(0, 100),
					GridWidth = 8,
					GridHeight = 1,
					ColumnWidth = 84,
					RowHeight = 84,
					Padding = 16,
				};
				AddElement(gridDisplay);

				for (int i = 0; i < 8; i++)
				{
					CharDisp[i] = new HUDCharacterControl(1)
					{
						Background = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Clouds, Color.Black, 4f),

						TextPadding = 2,
						TextColor = Color.Black
					};

					gridDisplay.AddElement(i, 0, CharDisp[i]);
				}

				var scode = KiddieCryptography.SpiralHexEncode(_server.SessionID, _server.SessionSecret);
				for (int i = 0; i < 8; i++)
				{
					if (i < scode.Length) CharDisp[i].Character = scode[i];
				}

				AddHUDOperation(new CharacterControlWaveOperation(CharDisp));

				AddElement(new HUDLabel
				{
					TextAlignment = HUDAlignment.BOTTOMRIGHT,
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(16, FOOTER_HEIGHT + 16),
					Size = new FSize(320, 128),
					MaxWidth = 320,
					WordWrap = HUDWordWrap.WrapByWordTrusted,

					Font = Textures.HUDFontRegular,
					FontSize = 32,

					L10NText = L10NImpl.STR_MENU_MP_LOBBYINFO,
					TextColor = ColorMath.Blend(FlatColors.Clouds, FlatColors.Background, 0.5f),
				});
			}

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C1_LEFT, 208 + 0 * 32),
				Size = new FSize(INFO_C1_WIDTH, 32),
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = L10NImpl.STR_MENU_MP_LOBBY_PING,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C1_LEFT, 208 + 1 * 32),
				Size = new FSize(INFO_C1_WIDTH, 32),
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = L10NImpl.STR_MENU_MP_LOBBY_USER,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C1_LEFT, 208 + 2 * 32),
				Size = new FSize(INFO_C1_WIDTH, 32),
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = L10NImpl.STR_MENU_MP_LOBBY_LEVEL,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C1_LEFT, 208 + 3 * 32),
				Size = new FSize(INFO_C1_WIDTH, 32),
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = L10NImpl.STR_MENU_MP_LOBBY_MUSIC,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C1_LEFT, 208 + 4 * 32),
				Size = new FSize(INFO_C1_WIDTH, 32),
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = L10NImpl.STR_MENU_MP_LOBBY_SPEED,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C1_LEFT, 208 + 5 * 32),
				Size = new FSize(INFO_C1_WIDTH, 32),
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = L10NImpl.STR_MENU_MP_LOBBY_COLOR,
				TextColor = FlatColors.Clouds,
			});



			AddElement(new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 0 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Lambda = () => $"{(int)(_server.ProxyPing.Value * 1000)}ms",
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 1 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Lambda = () => $"{_server.SessionCount} / {_server.SessionCapacity}",
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 2 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Text = Levels.LEVELS[_server.LevelID].FullName,
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 3 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Text = (_server.MusicIndex+1).ToString(),
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 4 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Text = Fmt(_server.Speed),
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 5 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				L10NText = Fraction.NAME_PLAYER,
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = Fraction.COLOR_PLAYER,
			});




			AddElement(new HUDRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateRounded(FlatColors.BackgroundHUD2, 16, false, false, true, true),
			});

			AddElement(new HUDTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(6.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_MENU_DISCONNECT,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Asbestos, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.MidnightBlue, 16),

				Click = OnClickCancel,
			});

			AddElement(_btnStart = new HUDTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_MENU_MP_START,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Emerald, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Nephritis, 16),

				IsVisible = false,

				Click = OnClickStart,
			});
		}

		private string Fmt(GameSpeedModes s)
		{
			switch (s)
			{
				case GameSpeedModes.SUPERSLOW:
					return "-2";
				case GameSpeedModes.SLOW:
					return "-1";
				case GameSpeedModes.NORMAL:
					return "0";
				case GameSpeedModes.FAST:
					return "+1";
				case GameSpeedModes.SUPERFAST:
					return "+2";
				default:
					SAMLog.Error("MPSLP::EnumSwitch_FMT", "Value = " + s);
					return "?";
			}
		}

		private void OnClickCancel(HUDTextButton sender, HUDButtonEventArgs e)
		{
			_server.KillSession();
			_server.Stop();
			Remove();
		}

		private void OnClickStart(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby && _server.SessionCount == _server.SessionCapacity)
			{
				byte[] binData = _server.GetLobbySyncData();
				_server.StartLobbySync(binData);
			}
		}

		public override void OnRemove()
		{
			if (!_doNotStop)
			{
				if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby)
				{
					_server.KillSession();
				}

				_server.Stop();


				if (!(MainGame.Inst.GetCurrentScreen() is GDOverworldScreen)) MainGame.Inst.SetOverworldScreen();
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_server.Update(gameTime, istate);

			_btnStart.BackgroundNormal = _btnStart.BackgroundNormal.WithColor(ColorMath.Blend(FlatColors.Emerald, FlatColors.GreenSea, FloatMath.PercSin(gameTime.TotalElapsedSeconds * 5)));


			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Remove();

				Owner.HUD.ShowToast(null, L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
			}
			
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();

			if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby && _server.SessionCount == _server.SessionCapacity)
			{
				_btnStart.IsVisible = true;
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.InGame)
			{
				MainGame.Inst.SetMultiplayerServerLevelScreen(Levels.LEVELS[_server.LevelID], _server.Speed, _server.MusicIndex, _server);
			}
		}
	}
}
