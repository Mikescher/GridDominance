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
	class MultiplayerFindLobbyScreen : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private readonly GDMultiplayerClient _server;
		private bool _doNotStop = false;

		public MultiplayerFindLobbyScreen(MultiplayerConnectionType t, bool btle)
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_server = new GDMultiplayerClient(t, btle);

			_server.JoinLocalSession();
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

				L10NText = L10NImpl.STR_MENU_CAP_SEARCH,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new MultiplayerConnectionStateControl(_server)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(16, 16)
			});

			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, (-FOOTER_HEIGHT/2) + (96/2f)),
				Image = Textures.CannonCogBig,
				RotationSpeed = 0.35f,
				Color = FlatColors.Clouds,
				Size = new FSize(192, 192)
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
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (!Alive) return;

			_server.Update(gameTime, istate);

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Owner.HUD.ShowToast(null, L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);

				Remove();
				return;
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
