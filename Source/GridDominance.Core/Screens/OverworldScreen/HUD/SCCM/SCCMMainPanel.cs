using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
    class SCCMMainPanel : HUDRoundedPanel
	{
		public enum SCCMTab { MyLevels, Hot, Top, New, Search }

		public const float WIDTH = 15.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private HUDWrapperContainer _container;
		private HUDTextButton _btnHeaderMyLevel;
		private HUDTextButton _btnHeaderHot;
		private HUDTextButton _btnHeaderTop;
		private HUDTextButton _btnHeaderNew;
		private HUDTextButton _btnHeaderSearch;

		public SCCMMainPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDSeperator(HUDOrientation.Horizontal)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, GDConstants.TILE_WIDTH),
				Size = new FSize(WIDTH, 1),
				SeperatorWidth = 2,
				Color = FlatColors.BackgroundHUD2,
			});

			AddElement(_btnHeaderMyLevel = new HUDTextButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0.25f * GDConstants.TILE_WIDTH, 8),
				Size = new FSize(3.00f * GDConstants.TILE_WIDTH, 48),

				L10NText = L10NImpl.STR_LVLED_TAB_MYLEVELS,
				TextColor = FlatColors.Foreground,
				Font = Textures.HUDFontBold,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = (o, e) => SelectTab(SCCMTab.MyLevels),
			});

			AddElement(_btnHeaderHot = new HUDTextButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(3.75f * GDConstants.TILE_WIDTH + HUD.PixelWidth, 8),
				Size = new FSize(2.50f * GDConstants.TILE_WIDTH, 48),

				L10NText = L10NImpl.STR_LVLED_TAB_HOT,
				TextColor = FlatColors.Foreground,
				Font = Textures.HUDFontBold,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = (o, e) => SelectTab(SCCMTab.Hot),
			});

			AddElement(_btnHeaderTop = new HUDTextButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(6.25f * GDConstants.TILE_WIDTH, 8),
				Size = new FSize(2.50f * GDConstants.TILE_WIDTH, 48),

				L10NText = L10NImpl.STR_LVLED_TAB_TOP,
				TextColor = FlatColors.Foreground,
				Font = Textures.HUDFontBold,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = (o, e) => SelectTab(SCCMTab.Top),
			});

			AddElement(_btnHeaderNew = new HUDTextButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(8.75f * GDConstants.TILE_WIDTH - HUD.PixelWidth, 8),
				Size = new FSize(2.50f * GDConstants.TILE_WIDTH, 48),

				L10NText = L10NImpl.STR_LVLED_TAB_NEW,
				TextColor = FlatColors.Foreground,
				Font = Textures.HUDFontBold,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = (o, e) => SelectTab(SCCMTab.New),
			});

			AddElement(_btnHeaderSearch = new HUDTextButton
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(11.75f * GDConstants.TILE_WIDTH, 8),
				Size = new FSize(3.00f * GDConstants.TILE_WIDTH, 48),

				L10NText = L10NImpl.STR_LVLED_TAB_SEARCH,
				TextColor = FlatColors.Foreground,
				Font = Textures.HUDFontBold,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = (o, e) => SelectTab(SCCMTab.Search),
			});

			AddElement(_container = new HUDWrapperContainer(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, HEIGHT - 1 * GDConstants.TILE_WIDTH),
			});

			SelectTab(SCCMTab.Hot);
		}

		private void SelectTab(SCCMTab tab)
		{
			var bg1Normal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Clouds, Color.Black, HUD.PixelWidth);
			var bg1Pressd = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Silver, Color.Black, HUD.PixelWidth);

			var bg2Normal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.BelizeHole, Color.Black, HUD.PixelWidth);
			var bg2Pressd = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.PeterRiver, Color.Black, HUD.PixelWidth);
			
			_btnHeaderMyLevel.BackgroundNormal  = (tab == SCCMTab.MyLevels) ? bg2Normal : bg1Normal;
			_btnHeaderMyLevel.BackgroundPressed = (tab == SCCMTab.MyLevels) ? bg2Pressd : bg1Pressd;
			
			_btnHeaderNew.BackgroundNormal      = (tab == SCCMTab.New)      ? bg2Normal : bg1Normal;
			_btnHeaderNew.BackgroundPressed     = (tab == SCCMTab.New)      ? bg2Pressd : bg1Pressd;

			_btnHeaderHot.BackgroundNormal      = (tab == SCCMTab.Hot)      ? bg2Normal : bg1Normal;
			_btnHeaderHot.BackgroundPressed     = (tab == SCCMTab.Hot)      ? bg2Pressd : bg1Pressd;

			_btnHeaderTop.BackgroundNormal      = (tab == SCCMTab.Top)      ? bg2Normal : bg1Normal;
			_btnHeaderTop.BackgroundPressed     = (tab == SCCMTab.Top)      ? bg2Pressd : bg1Pressd;

			_btnHeaderSearch.BackgroundNormal   = (tab == SCCMTab.Search)   ? bg2Normal : bg1Normal;
			_btnHeaderSearch.BackgroundPressed  = (tab == SCCMTab.Search)   ? bg2Pressd : bg1Pressd;

			switch (tab)
			{
				case SCCMTab.MyLevels:
					_container.SetElement(new SCCMTabMyLevels());
					break;
				case SCCMTab.Hot:
					_container.SetElement(new SCCMTabHot());
					break;
				case SCCMTab.Top:
					_container.SetElement(new SCCMTabTop());
					break;
				case SCCMTab.New:
					_container.SetElement(new SCCMTabNew());
					break;
				case SCCMTab.Search:
					//_container.SetElement(new SCCMTabSearch()); //TODO tab search
					break;
				default: 
					SAMLog.Error("SCCMP::EnumSwitch_ST", "tab: " + tab);
					break;
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			if (!Alive) return;

			//
		}
	}
}
