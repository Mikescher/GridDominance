using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Multiplayer;
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

		private GDGameScreen_Display _displayScreen;

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

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 96),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_MENU_CAP_CGAME,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new MultiplayerConnectionStateControl(_server)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(16, 16)
			});

			_displayScreen = new GDGameScreen_Display(MainGame.Inst, MainGame.Inst.Graphics, Levels.LEVEL_1_1);
			AddElement(new HUDSubScreenProxyRenderer(_displayScreen)
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

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 4f, true, false, false, true),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 4f, true, false, false, true),
			});

			AddElement(_lblLevelID1 = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((8 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),

				Size = new FSize(96, 48),
				FontSize = 48,
				Font = Textures.HUDFontRegular,

				Text = "1",
				TextAlignment = HUDAlignment.CENTER,
				TextColor = FlatColors.Clouds,

				Background = HUDBackgroundDefinition.CreateSimple(FlatColors.BackgroundHUD2),
			});

			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((17 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image = Textures.TexHUDIconChevronRight,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 4f, false, true, true, false),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 4f, false, true, true, false),
			});



			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((24 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image = Textures.TexHUDIconChevronLeft,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 4f, true, false, false, true),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 4f, true, false, false, true),
			});

			AddElement(_lblLevelID2 = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((27 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),

				Size = new FSize(96, 48),
				FontSize = 48,
				Font = Textures.HUDFontRegular,

				Text = "1",
				TextAlignment = HUDAlignment.CENTER,
				TextColor = FlatColors.Clouds,

				Background = HUDBackgroundDefinition.CreateSimple(FlatColors.BackgroundHUD2),
			});

			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint((36 / 6f) * GDConstants.TILE_WIDTH, 2.25f * GDConstants.TILE_WIDTH),
				Size = new FSize(32, 48),

				Image = Textures.TexHUDIconChevronRight,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 4f, false, true, true, false),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 4f, false, true, true, false),
			});



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

		}

		private void OnClickCreateLobby(HUDIconTextButton sender, HUDButtonEventArgs e)
		{
			_server.CreateSession(2);

			_btnCreate.Icon = Textures.CannonCog;
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

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			base.DoDraw(sbatch, bounds);
			
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
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

				Owner.HUD.ShowToast(L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);
			}

			if (_server.Mode == SAMNetworkConnection.ServerMode.Stopped) Remove();
		}
	}
}
