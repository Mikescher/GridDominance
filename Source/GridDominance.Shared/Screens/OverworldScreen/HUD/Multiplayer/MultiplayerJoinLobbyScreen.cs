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
using MonoSAMFramework.Portable.Network.Multiplayer;
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

		private readonly GDMultiplayerClient _server;
		private bool _doNotStop = false;

		public int CharIndex = 0;
		public readonly HUDCharacterControl[] CharDisp = new HUDCharacterControl[8];

		private bool _isDying = false;

		public MultiplayerJoinLobbyScreen()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_server = new GDMultiplayerClient();
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

			AddElement(new MultiplayerConnectionStateControl(_server)
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
					BackgroundType = HUDBackgroundType.Simple,
					BackgroundColor = FlatColors.Clouds,

					BorderWidth = 4,
					BorderColor = Color.Black,

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

				ButtonColor = FlatColors.ControlHighlight,
				ButtonColorPressed = FlatColors.Background,
				ButtonBackgroundType = HUDBackgroundType.RoundedBlur,
				ButtonBackgoundCornerSize = 4f,
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

		private void DoClick(HUDKeypad source, HUDKeypad.HUDKeypadEventArgs args)
		{
			if (CharIndex >= 8) return;

			CharDisp[CharIndex].Character = args.Character;

			CharIndex++;
			if (CharIndex == 8)
			{
				var code = new string(Enumerable.Range(0, 8).Select(i => CharDisp[i].Character).ToArray());

				var d = KiddieCryptography.SpiralHexDecode(code);

				for (int i = 0; i < 8; i++) CharDisp[i].BackgroundColor = FlatColors.Concrete;

				_server.JoinSession(d.Item1, d.Item2);
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (_isDying || !Alive) return;

			_server.Update(gameTime, istate);

			//todo status

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Owner.HUD.ShowToast(L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);

				switch (_server.Error)
				{
					case SAMNetworkConnection.ErrorType.SessionNotFound:
					case SAMNetworkConnection.ErrorType.AuthentificationFailed:
					case SAMNetworkConnection.ErrorType.LobbyFull:
					case SAMNetworkConnection.ErrorType.GameVersionMismatch:
						_isDying = true;
						AddHUDOperation(new JoinErrorOperation());
						return;
					default:
						Remove();
						return;
				}

			}
			
			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();

			if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby)
			{
				_doNotStop = true;
				Remove();
				Owner.HUD.AddModal(new MultiplayerClientLobbyPanel(_server), true, 0.5f);
			}
		}
	}
}
