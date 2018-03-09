using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.OverworldScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Common.HUD.Multiplayer
{
    class MultiplayerClientLobbyPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.5f * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2.0f * GDConstants.TILE_WIDTH;

		public const float INFO_C1_LEFT = 58;
		public const float INFO_C1_WIDTH = 260;
		public const float INFO_C2_LEFT = 320;
		public const float INFO_C2_WIDTH = 225;

		public override int Depth => 0;

		private readonly GDMultiplayerClient _server;
		private bool _doNotStop = false;

		private Guid _lastLevelID;
		private string fracName = null;
		private bool skipUpdate = false;

		private HUDLabel _lblFraction;

		public MultiplayerClientLobbyPanel(GDMultiplayerClient server)
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

			AddElement(new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 2 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Lambda = () => _server.LevelID != null ? Levels.LEVELS[_server.LevelID.Value].FullName : ("?"),
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 3 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Lambda = () => _server.MusicIndex != null ? (_server.MusicIndex.Value + 1).ToString() : "?",
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 4 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Lambda = () => _server.Speed != null ? Fmt(_server.Speed.Value) : "?",
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_lblFraction = new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(INFO_C2_LEFT, 208 + 5 * 32),
				Size = new FSize(INFO_C2_WIDTH, 32),
				MaxWidth = INFO_C2_WIDTH,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				Lambda = () => fracName ?? "?",
				WordWrap = HUDWordWrap.Ellipsis,
				TextColor = FlatColors.Clouds,
			});


			AddElement(new HUDImage
			{
				Image = Textures.CannonCogBig,
				RotationSpeed = 0.3f,
				Color = FlatColors.BackgroundHUD2,

				Size = new FSize(196, 196),

				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(WIDTH/4f, -64)
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
					SAMLog.Error("MPCLP::EnumSwitch_FMT", "Value = " + s);
					return "?";
			}
		}

		private void OnClickCancel(HUDTextButton sender, HUDButtonEventArgs e)
		{
			_server.KillSession();
			_server.Stop();
			Remove();
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

			if (skipUpdate) return;

			if (_server.LevelID != null && _server.LevelID.Value != _lastLevelID)
			{
				_lastLevelID = _server.LevelID.Value;

				if (Levels.LEVELS.TryGetValue(_server.LevelID.Value, out var bp))
				{
					var d = GetFractionColorByID(bp, _server.SessionUserID + 1);
					fracName = d.Item1;
					_lblFraction.TextColor = d.Item2;
				}
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Remove();

				Owner.HUD.ShowToast(null, L10NImplHelper.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
				skipUpdate = true;
			}
			
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();

			if (_server.Mode == SAMNetworkConnection.ServerMode.InGame)
			{
				MainGame.Inst.SetMultiplayerClientLevelScreen(Levels.LEVELS[_server.LevelID.Value], _server.Speed.Value, _server.MusicIndex.Value, _server);
				skipUpdate = true;
			}
		}

		public Tuple<string, Color> GetFractionColorByID(LevelBlueprint lvl, int id)
		{
			var fractionIDList = new List<int>();
			fractionIDList.Add(0);

			foreach (var bPrint in lvl.BlueprintCannons)
			{
				if (!fractionIDList.Contains(bPrint.Player)) fractionIDList.Add(bPrint.Player);
			}
			foreach (var bPrint in lvl.BlueprintLaserCannons)
			{
				if (!fractionIDList.Contains(bPrint.Player)) fractionIDList.Add(bPrint.Player);
			}

			if (!fractionIDList.Contains(0)) fractionIDList.Add(0);
			if (!fractionIDList.Contains(1)) fractionIDList.Add(1);
			if (!fractionIDList.Contains(2)) fractionIDList.Add(2);
			if (!fractionIDList.Contains(3)) fractionIDList.Add(3);
			if (!fractionIDList.Contains(4)) fractionIDList.Add(4);
			if (!fractionIDList.Contains(5)) fractionIDList.Add(5);
			if (!fractionIDList.Contains(6)) fractionIDList.Add(6);

			if (id < 0 || id >= fractionIDList.Count)
			{
				SAMLog.Error("MPCLP::GetFractionByID_1", $"Fraction not found: {id}");
				return Tuple.Create("1", Color.Magenta);
			}

			var fid = fractionIDList[id];

			if (fid == 0) return Tuple.Create(L10N.T(Fraction.NAME_NEUTRAL),     Fraction.COLOR_NEUTRAL);
			if (fid == 1) return Tuple.Create(L10N.T(Fraction.NAME_PLAYER),      Fraction.COLOR_PLAYER);
			if (fid == 2) return Tuple.Create(L10N.T(Fraction.NAME_COMPUTER_01), Fraction.COLOR_COMPUTER_01);
			if (fid == 3) return Tuple.Create(L10N.T(Fraction.NAME_COMPUTER_02), Fraction.COLOR_COMPUTER_02);
			if (fid == 4) return Tuple.Create(L10N.T(Fraction.NAME_COMPUTER_03), Fraction.COLOR_COMPUTER_03);
			if (fid == 5) return Tuple.Create(L10N.T(Fraction.NAME_COMPUTER_04), Fraction.COLOR_COMPUTER_04);
			if (fid == 6) return Tuple.Create(L10N.T(Fraction.NAME_COMPUTER_05), Fraction.COLOR_COMPUTER_05);

			SAMLog.Error("MPCLP::GetFractionByID_2", $"Fraction not found: {id}");
			return Tuple.Create("1", Color.Magenta);
		}
	}
}
