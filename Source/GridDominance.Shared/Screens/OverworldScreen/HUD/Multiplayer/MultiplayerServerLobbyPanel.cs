using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Cryptography;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
    class MultiplayerServerLobbyPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private readonly GDMultiplayerServer _server;
		private bool _doNotStop = false;

		private HUDLabel _statusLabel;
		private HUDLabel _infoLabel;


		private readonly LevelBlueprint _level;
		private readonly GameSpeedModes _speed;
		private readonly int _musicIndex;

		public MultiplayerServerLobbyPanel(GDMultiplayerServer server)
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_level = Levels.LEVELS[Levels.WORLD_001.Nodes[3].LevelID];
			_speed = GameSpeedModes.NORMAL;
			_musicIndex = 2;

			_server = server;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(_statusLabel = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32),
				Size = new FSize(WIDTH, 48),

				Text = "Offline", //TODO L10N
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTERLEFT,
			});

			AddElement(_infoLabel = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 128),
				Size = new FSize(WIDTH, 48),

				Text = "?", //TODO L10N
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTERLEFT,
			});
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
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			base.DoDraw(sbatch, bounds);
			
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_server.Update(gameTime, istate);

			if (_server.ConnState == SAMNetworkConnection.ConnectionState.Connected)
				_statusLabel.Text = _server.SessionCount + " / " + _server.SessionCapacity; //TODO L10N
			else
				_statusLabel.Text = "Connection lost"; //TODO L10N

			_infoLabel.Text = KiddieCryptography.SpiralHexEncode(_server.SessionID, _server.SessionSecret);

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Remove();

				Owner.HUD.ShowToast(_server.ErrorMessage, 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
			}
			
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();

			if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby && _server.SessionCount == _server.SessionCapacity)
			{
				byte[] binData;
				using (var ms = new MemoryStream())
				using (var bw = new BinaryWriter(ms))
				{
					bw.Write(_level.UniqueID.ToByteArray());
					bw.Write((byte)_speed);
					bw.Write((byte)_musicIndex);

					binData = ms.ToArray();
				}

				_server.StartLobbySync(binData);
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.InGame)
			{
				MainGame.Inst.SetMultiplayerServerLevelScreen(_level, _speed, _musicIndex, _server);
			}
		}
	}
}
