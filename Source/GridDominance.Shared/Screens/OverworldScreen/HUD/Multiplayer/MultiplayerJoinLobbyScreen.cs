using System.Linq;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
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
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
    class MultiplayerJoinLobbyScreen : HUDRoundedPanel
	{
		public const float DISP_WIDTH = 64;
		public const float DISP_PAD   = 10;
		public const float DISP_OFFX  = (WIDTH - (DISP_WIDTH * 8 + DISP_PAD * 7)) / 2;
		
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public const float BTN_WIDTH = 64;
		public const float BTN_PADY = (HEIGHT - 84 - DISP_WIDTH - 4 * BTN_WIDTH) / 5;
		public const float BTN_PADX = BTN_PADY;
		public const float BTN_OFFSET_X = (WIDTH - (4 * BTN_WIDTH + 3 * BTN_PADX)) / 2;
		public const float BTN_OFFSET_Y = BTN_PADY;

		public static readonly char[] BTN_TXT = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

		public override int Depth => 0;

		private readonly GDMultiplayerClient _server;
		private bool _doNotStop = false;

		public int CharIndex = 0;
		public readonly HUDCharacterControl[] CharDisp = new HUDCharacterControl[8];

		private HUDLabel _statusLabel;
		
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

				Text = "**Enter codeZ**",
				TextColor = FlatColors.Clouds,
			});

			AddElement(_statusLabel = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 130),
				Size = new FSize(WIDTH, 48),

				Text = "Offline", //TODO L10N
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTERLEFT,
			});

			for (int i = 0; i < 8; i++)
			{
				CharDisp[i] = new HUDCharacterControl(1)
				{
					Alignment = HUDAlignment.TOPLEFT,
					RelativePosition = new FPoint(DISP_OFFX + i * (DISP_PAD + DISP_WIDTH), 84),
					Size = new FSize(DISP_WIDTH, DISP_WIDTH),

					BackgoundType = HUDBackgroundType.Simple,
					BackgroundColor = FlatColors.Clouds,

					BorderWidth = 4,
					BorderColor = Color.Black,

					TextPadding = 2,
					TextColor = Color.Black
				};
				AddElement(CharDisp[i]);
			}

			for (int x = 0; x < 4; x++)
			{
				for (int y = 0; y < 4; y++)
				{
					int idx = x + y * 4;

					AddElement(new HUDTextButton(1)
					{
						TextAlignment = HUDAlignment.CENTER,
						Alignment = HUDAlignment.BOTTOMLEFT,
						RelativePosition = new FPoint(BTN_OFFSET_X + (BTN_WIDTH + BTN_PADX) * x, BTN_OFFSET_Y + (BTN_WIDTH + BTN_PADY) * (3 - y)),
						Size = new FSize(BTN_WIDTH, BTN_WIDTH),

						Font = Textures.HUDFontBold,
						FontSize = 48,

						Text = BTN_TXT[x + y * 4].ToString(),
						TextColor = FlatColors.Foreground,

						Color = FlatColors.ControlHighlight,
						ColorPressed = FlatColors.Background,
						BackgoundType = HUDBackgroundType.RoundedBlur,
						BackgoundCornerSize = 4f,

						Click = (s, e) => DoClick(BTN_TXT[idx]),
					});
				}
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
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			base.DoDraw(sbatch, bounds);

			//
		}

		private void DoClick(char c)
		{
			if (CharIndex >= 8) return;

			CharDisp[CharIndex].Character = c;

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
			_server.Update(gameTime, istate);

			if (_server.ConnState == SAMNetworkConnection.ConnectionState.Connected)
				_statusLabel.Text = _server.SessionCount + " / " + _server.SessionCapacity; //TODO L10N
			else
				_statusLabel.Text = "Connection lost"; //TODO L10N

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Remove();

				Owner.HUD.ShowToast(L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
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
