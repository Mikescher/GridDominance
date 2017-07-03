using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
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
    class MultiplayerHostPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private readonly GDMultiplayerServer _server;
		private bool _doNotStop = false;

		private HUDLabel _statusLabel;
		
		public MultiplayerHostPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_server = new GDMultiplayerServer();
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

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				Text = "**Create**",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.PeterRiver,
				ColorPressed = FlatColors.BelizeHole,

				Click = OnClickCreateLobby,
			});
		}

		private void OnClickCreateLobby(HUDTextButton sender, HUDButtonEventArgs e)
		{
			_server.CreateSession(2);
		}

		public override void OnRemove()
		{
			if (!_doNotStop)_server.Stop();
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			base.DoDraw(sbatch, bounds);
			
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_server.Update(gameTime, istate);

			_statusLabel.Text = _server.ConnState.ToString();

			if (_server.Mode == SAMNetworkConnection.ServerMode.InLobby)
			{
				_doNotStop = true;
				Remove();
				Owner.HUD.AddModal(new MultiplayerServerLobbyPanel(_server), true, 0.5f);
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				Remove();

				Owner.HUD.ShowToast(_server.ErrorMessage, 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();
		}
	}
}
