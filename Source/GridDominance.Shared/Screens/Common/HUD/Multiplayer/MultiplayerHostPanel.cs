using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Multiplayer;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
    class MultiplayerHostPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9.5f * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private readonly GDMultiplayerServer _server;
		private bool _doNotStop = false;

		private HUDIconTextButton _btnCreate;
		private HUDLabel _lblLevelID1;
		private HUDLabel _lblLevelID2;

		private GraphBlueprint _currentWorld = Levels.WORLD_001;
		private LevelBlueprint _currentLevel = Levels.LEVELS[Levels.WORLD_001.LevelNodes.First().LevelID];
		private int _levelUserCount = 2;

		private HUDSubScreenProxyRenderer _displayScreen;

		private HUDRadioSpeedButton _speed1;
		private HUDRadioSpeedButton _speed2;
		private HUDRadioSpeedButton _speed3;
		private HUDRadioSpeedButton _speed4;
		private HUDRadioSpeedButton _speed5;

		private HUDRadioMusicButton _music1;
		private HUDRadioMusicButton _music2;
		private HUDRadioMusicButton _music3;
		private HUDRadioMusicButton _music4;
		private HUDRadioMusicButton _music5;
		private HUDRadioMusicButton _music6;

		public MultiplayerHostPanel(MultiplayerConnectionType t, bool btle)
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_server = new GDMultiplayerServer(t, btle);
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 96),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText  = _server.ConnType == MultiplayerConnectionType.PROXY ? L10NImpl.STR_MENU_CAP_CGAME_PROX : L10NImpl.STR_MENU_CAP_CGAME_BT,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new MultiplayerConnectionStateControl(_server)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(16, 16)
			});

			var screen = new GDGameScreen_Display(MainGame.Inst, MainGame.Inst.Graphics, _currentLevel);
			AddElement(_displayScreen = new HUDSubScreenProxyRenderer(screen)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((2/3f) * GDConstants.TILE_WIDTH, 3.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(6 * GDConstants.TILE_WIDTH, 3.75f * GDConstants.TILE_WIDTH),
			});


			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((5 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image=Textures.TexHUDIconChevronLeft,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 8f, true, false, true, false),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 8f, true, false, true, false),

				Click = (s, a) => ChangeID1(-1),
			});

			AddElement(_lblLevelID1 = new HUDClickableLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((8 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),

				Size = new FSize(96, 48),
				FontSize = 48,
				Font = Textures.HUDFontRegular,

				Text = "?",
				TextAlignment = HUDAlignment.CENTER,
				TextColor = FlatColors.Clouds,

				Background = HUDBackgroundDefinition.CreateSimple(FlatColors.BackgroundHUD2),

				Click = (s, a) => ChangeID1(+1),
				ClickSound = true,
			});

			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((17 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image = Textures.TexHUDIconChevronRight,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 8f, false, true, false, true),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 8f, false, true, false, true),

				Click = (s, a) => ChangeID1(+1),
			});
			

			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((24 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image = Textures.TexHUDIconChevronLeft,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 8f, true, false, true, false),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 8f, true, false, true, false),

				Click = (s,a) => ChangeID2(-1),
			});

			AddElement(_lblLevelID2 = new HUDClickableLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((27 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),

				Size = new FSize(96, 48),
				FontSize = 48,
				Font = Textures.HUDFontRegular,

				Text = "?",
				TextAlignment = HUDAlignment.CENTER,
				TextColor = FlatColors.Clouds,

				Background = HUDBackgroundDefinition.CreateSimple(FlatColors.BackgroundHUD2),

				Click = (s, a) => ChangeID2(+1),
				ClickSound = true,
			});

			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((36 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image = Textures.TexHUDIconChevronRight,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 8f, false, true, false, true),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 8f, false, true, false, true),

				Click = (s, a) => ChangeID2(+1),
			});


			AddElement(new HUDLambdaLabel
			{
				TextAlignment = HUDAlignment.BOTTOMLEFT,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(100 + 8, 375),
				Size = new FSize(200, 32),

				Font = Textures.HUDFontRegular,
				FontSize = 32,

				Lambda = () => L10N.TF(L10NImpl.STR_MENU_MP_LOBBY_USER_FMT, _levelUserCount),
				TextColor = Color.White,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.BOTTOMLEFT,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(100 + 8, 331),
				Size = new FSize(200, 32),

				Font = Textures.HUDFontRegular,
				FontSize = 32,

				L10NText = L10NImpl.STR_MENU_MP_MUSIC,
				TextColor = Color.White,
			});

			int initialMusic = FloatMath.GetRangedIntRandom(5);

			AddElement(_music1 = new HUDRadioMusicButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(37, 261),
				Size = new FSize(62, 62),
				MusicIndex = 0,
				Selected = initialMusic == 0,
			});

			AddElement(_music2 = new HUDRadioMusicButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(109, 261),
				Size = new FSize(62, 62),
				MusicIndex = 1,
				Selected = initialMusic == 1,
			});

			AddElement(_music3 = new HUDRadioMusicButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(179, 261),
				Size = new FSize(62, 62),
				MusicIndex = 2,
				Selected = initialMusic == 2,
			});

			AddElement(_music4 = new HUDRadioMusicButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(249, 261),
				Size = new FSize(62, 62),
				MusicIndex = 3,
				Selected = initialMusic == 3,
			});

			AddElement(_music5 = new HUDRadioMusicButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(319, 261),
				Size = new FSize(62, 62),
				MusicIndex = 4,
				Selected = initialMusic == 4,
			});

			AddElement(_music6 = new HUDRadioMusicButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(389, 261),
				Size = new FSize(62, 62),
				MusicIndex = 5,
				Selected = initialMusic == 5,
			});

			_music1.RadioGroup = new List<HUDRadioMusicButton> { _music1, _music2, _music3, _music4, _music5, _music6 };
			_music2.RadioGroup = new List<HUDRadioMusicButton> { _music1, _music2, _music3, _music4, _music5, _music6 };
			_music3.RadioGroup = new List<HUDRadioMusicButton> { _music1, _music2, _music3, _music4, _music5, _music6 };
			_music4.RadioGroup = new List<HUDRadioMusicButton> { _music1, _music2, _music3, _music4, _music5, _music6 };
			_music5.RadioGroup = new List<HUDRadioMusicButton> { _music1, _music2, _music3, _music4, _music5, _music6 };
			_music6.RadioGroup = new List<HUDRadioMusicButton> { _music1, _music2, _music3, _music4, _music5, _music6 };

			var initialSpeed = MainGame.Inst.Profile.LastMultiplayerHostedSpeed;

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.BOTTOMLEFT,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(100 + 8, 221),
				Size = new FSize(200, 32),

				Font = Textures.HUDFontRegular,
				FontSize = 32,

				L10NText = L10NImpl.STR_MENU_MP_GAMESPEED,
				TextColor = Color.White,
			});

			AddElement(_speed1 = new HUDRadioSpeedButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(37, 150),
				Size = new FSize(62, 62),
				Speed = GameSpeedModes.SUPERSLOW,
				Selected = initialSpeed == GameSpeedModes.SUPERSLOW,
			});

			AddElement(_speed2 = new HUDRadioSpeedButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(109, 150),
				Size = new FSize(62, 62),
				Speed = GameSpeedModes.SLOW,
				Selected = initialSpeed == GameSpeedModes.SLOW,
			});

			AddElement(_speed3 = new HUDRadioSpeedButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(179, 150),
				Size = new FSize(62, 62),
				Speed = GameSpeedModes.NORMAL,
				Selected = initialSpeed == GameSpeedModes.NORMAL,
			});

			AddElement(_speed4 = new HUDRadioSpeedButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(249, 150),
				Size = new FSize(62, 62),
				Speed = GameSpeedModes.FAST,
				Selected = initialSpeed == GameSpeedModes.FAST,
			});

			AddElement(_speed5 = new HUDRadioSpeedButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(319, 150),
				Size = new FSize(62, 62),
				Speed = GameSpeedModes.SUPERFAST,
				Selected = initialSpeed == GameSpeedModes.SUPERFAST,
			});

			_speed1.RadioGroup = new List<HUDRadioSpeedButton> { _speed1, _speed2, _speed3, _speed4, _speed5 };
			_speed2.RadioGroup = new List<HUDRadioSpeedButton> { _speed1, _speed2, _speed3, _speed4, _speed5 };
			_speed3.RadioGroup = new List<HUDRadioSpeedButton> { _speed1, _speed2, _speed3, _speed4, _speed5 };
			_speed4.RadioGroup = new List<HUDRadioSpeedButton> { _speed1, _speed2, _speed3, _speed4, _speed5 };
			_speed5.RadioGroup = new List<HUDRadioSpeedButton> { _speed1, _speed2, _speed3, _speed4, _speed5 };

			AddElement(new HUDRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateRounded(FlatColors.BackgroundHUD2, 16, false, false, true, true),
			});

			AddElement(_btnCreate = new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				Icon = null,
				IconRotationSpeed = 0.25f, 

				L10NText = L10NImpl.STR_MENU_MP_CREATE,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.PeterRiver, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

				Click = OnClickCreateLobby,
			});

			AddElement(new HUDTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_MENU_CANCEL,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Asbestos, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.MidnightBlue, 16),

				Click = OnClickCancel,
			});

			//---------------------

			if (!Levels.LEVELS.TryGetValue(MainGame.Inst.Profile.LastMultiplayerHostedLevel, out _currentLevel))
				_currentLevel = Levels.LEVELS[Levels.LEVELID_1_3];

			_currentWorld = Levels.WORLDS_MULTIPLAYER.FirstOrDefault(w => w.AllNodes.Any(n => n.ConnectionID == _currentLevel.UniqueID));
			if (_currentWorld == null)
			{
				_currentWorld = Levels.WORLD_001;
				_currentLevel = Levels.LEVELS[Levels.LEVELID_1_3];
			}

			UpdateLabels();
		}

		private void OnClickCreateLobby(HUDIconTextButton sender, HUDButtonEventArgs e)
		{
			_server.LevelID = _currentLevel.UniqueID;

			if (_music1.Selected) _server.MusicIndex = _music1.MusicIndex;
			if (_music2.Selected) _server.MusicIndex = _music2.MusicIndex;
			if (_music3.Selected) _server.MusicIndex = _music3.MusicIndex;
			if (_music4.Selected) _server.MusicIndex = _music4.MusicIndex;
			if (_music5.Selected) _server.MusicIndex = _music5.MusicIndex;
			if (_music6.Selected) _server.MusicIndex = _music6.MusicIndex;

			if (_speed1.Selected) _server.Speed = _speed1.Speed;
			if (_speed2.Selected) _server.Speed = _speed2.Speed;
			if (_speed3.Selected) _server.Speed = _speed3.Speed;
			if (_speed4.Selected) _server.Speed = _speed4.Speed;
			if (_speed5.Selected) _server.Speed = _speed5.Speed;

			_server.CreateSession(BlueprintAnalyzer.PlayerCount(_currentLevel));

			MainGame.Inst.Profile.LastMultiplayerHostedLevel = _server.LevelID;
			MainGame.Inst.Profile.LastMultiplayerHostedSpeed = _server.Speed;
			MainGame.Inst.SaveProfile();

			_btnCreate.Icon = Textures.CannonCogBig;
		}

		private void OnClickCancel(HUDTextButton sender, HUDButtonEventArgs e)
		{
			_server.Stop();
			Remove();
		}

		public override void OnRemove()
		{
			if (!_doNotStop)_server.Stop();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (!Alive) return;

			_server.Update(gameTime, istate);

			if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby)
			{
				_doNotStop = true;
				Remove();
				Owner.HUD.AddModal(new MultiplayerServerLobbyPanel(_server), true, 0.5f);
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Remove();

				Owner.HUD.ShowToast(null, L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();
		}

		private void ChangeID1(int delta)
		{
			int i1 = Levels.WORLDS_MULTIPLAYER.ToList().IndexOf(_currentWorld);

			i1 = (i1 + delta + Levels.WORLDS_MULTIPLAYER.Length) % Levels.WORLDS_MULTIPLAYER.Length;
			_currentWorld = Levels.WORLDS_MULTIPLAYER[i1];

			_currentLevel = Levels.LEVELS[_currentWorld.LevelNodes.First().LevelID];
			UpdateLabels();
		}

		private void ChangeID2(int delta)
		{
			var data = _currentWorld.LevelNodes.Select(n => Levels.LEVELS[n.LevelID]).OrderBy(n => n.Name.Split('-').Last().PadLeft(3, '0')).ToList();
			var idx = data.IndexOf(_currentLevel);

			idx = (idx + data.Count + delta) % data.Count;

			if (_server.ConnType == MultiplayerConnectionType.P2P)
			{
				for (int i = 0; i < 128; i++)
				{
					if (BlueprintAnalyzer.PlayerCount(data[idx]) != 2) { idx++; continue; }
					break;
				}
			}

			_currentLevel = data[idx];
			UpdateLabels();
		}

		private void UpdateLabels()
		{
			_lblLevelID1.Text = _currentLevel.Name.Split('-').First();
			_lblLevelID2.Text = _currentLevel.Name.Split('-').Last();
			_levelUserCount = BlueprintAnalyzer.PlayerCount(_currentLevel);

			var screen = new GDGameScreen_Display(MainGame.Inst, MainGame.Inst.Graphics, _currentLevel);
			_displayScreen.ChangeScreen(screen);
		}
	}
}
