using System.Linq;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.HUDOperations;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Multiplayer;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Cryptography;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
	class MultiplayerJoinLobbyScreen : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		public readonly GDMultiplayerClient Server;
		private bool _doNotStop = false;

		public int CharIndex = 0;
		public readonly HUDCharacterControl[] CharDisp = new HUDCharacterControl[8];

		private bool _isDying = false;

		public MultiplayerJoinLobbyScreen(MultiplayerConnectionType t, bool btle)
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			Server = new GDMultiplayerClient(t, btle);
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

				L10NText = L10NImpl.STR_MENU_CAP_AUTH,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new MultiplayerConnectionStateControl(Server)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(16, 16)
			});

			var gridDisplay = new HUDFixedUniformGrid
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 84),
				GridWidth = 8,
				GridHeight = 1,
				ColumnWidth = 64,
				RowHeight = 64,
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

			var pad = new HUDKeypad(6, 4, 64, 16)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 164 / 2f),

				ButtonTextAlignment = HUDAlignment.CENTER,

				ButtonFont = Textures.HUDFontBold,
				ButtonFontSize = 48,

				ButtonTextColor = FlatColors.Foreground,

				ButtonBackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ControlHighlight, 4f),
				ButtonBackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Background, 4f),
			};
			AddElement(pad);

			pad.AddKey('1', 0, 0);
			pad.AddKey('2', 1, 0);
			pad.AddKey('3', 2, 0);

			pad.AddKey('4', 0, 1);
			pad.AddKey('5', 1, 1);
			pad.AddKey('6', 2, 1);

			pad.AddKey('7', 0, 2);
			pad.AddKey('8', 1, 2);
			pad.AddKey('9', 2, 2);

			//pad.AddKey('#', 0, 3);
			pad.AddKey('0', 1, 3);
			//pad.AddKey('*', 2, 3);

			pad.AddKey('A', 4, 0);
			pad.AddKey('B', 5, 0);
			pad.AddKey('C', 4, 1);
			pad.AddKey('D', 5, 1);
			pad.AddKey('E', 4, 2);
			pad.AddKey('F', 5, 2);

			pad.PadClick += DoClick;
		}

		public override void OnRemove()
		{
			if (!_doNotStop)
			{
				if (Server.Mode == SAMNetworkConnection.ServerMode.InLobby)
				{
					Server.KillSession();
				}

				Server.Stop();
			}
		}

		private void DoClick(HUDKeypad source, HUDKeypad.HUDKeypadEventArgs args)
		{
			if (CharIndex >= 8) return;

			CharDisp[CharIndex].Character = args.Character;

			CharIndex++;
			if (CharIndex == 8)
			{
				var code = new string(Enumerable.Range(0, 8).Select(i => CharDisp[i].Character).ToArray());

				var d = KiddieCryptography.SpiralHexDecode(code);

				for (int i = 0; i < 8; i++) CharDisp[i].Background = CharDisp[i].Background.WithColor(FlatColors.Concrete);

				Server.JoinSession(d.Item1, d.Item2);
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (_isDying || !Alive) return;

			Server.Update(gameTime, istate);

			if (Server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Owner.HUD.ShowToast(null, L10NImpl.FormatNetworkErrorMessage(Server.Error, Server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);

				switch (Server.Error)
				{
					case SAMNetworkConnection.ErrorType.SessionNotFound:
					case SAMNetworkConnection.ErrorType.AuthentificationFailed:
					case SAMNetworkConnection.ErrorType.LobbyFull:
					case SAMNetworkConnection.ErrorType.GameVersionMismatch:
						_isDying = true;
						AddHUDOperation(new JoinErrorOperation());
						return;

					case SAMNetworkConnection.ErrorType.BluetoothNotEnabled:
					case SAMNetworkConnection.ErrorType.P2PConnectionFailed:
					case SAMNetworkConnection.ErrorType.P2PConnectionLost:
					case SAMNetworkConnection.ErrorType.NetworkMediumInternalError:
					case SAMNetworkConnection.ErrorType.ProxyServerTimeout:
					case SAMNetworkConnection.ErrorType.UserTimeout:
					case SAMNetworkConnection.ErrorType.ServerUserTimeout:
					case SAMNetworkConnection.ErrorType.NotInLobby:
					case SAMNetworkConnection.ErrorType.LevelNotFound:
					case SAMNetworkConnection.ErrorType.LevelVersionMismatch:
					case SAMNetworkConnection.ErrorType.UserDisconnect:
					case SAMNetworkConnection.ErrorType.ServerDisconnect:
					case SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound:
					case SAMNetworkConnection.ErrorType.BluetoothAdapterNoPermission:
					case SAMNetworkConnection.ErrorType.P2PNoServerConnection:
						Remove();
						return;

					case SAMNetworkConnection.ErrorType.None:
					default:
						SAMLog.Error("MJLS::EnumSwitch_DU", "value = " + Server.Error);
						break;
				}

			}
			
			if (Server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();

			if (Server.Mode == SAMNetworkConnection.ServerMode.InLobby)
			{
				_doNotStop = true;
				Remove();
				Owner.HUD.AddModal(new MultiplayerClientLobbyPanel(Server), true, 0.5f);
			}
		}
	}
}
