using System;
using System.Linq;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Resources
{
	enum TextureQuality
	{
		UNSPECIFIED,

		HD, // x2.000
		MD, // x1.000
		LD, // x0.500
		BD, // x0.250
		FD, // x0.125
	}

	static class Textures
	{
		#region Scaling

		public static TextureQuality TEXTURE_QUALITY = TextureQuality.UNSPECIFIED;

		public static Vector2 TEXTURE_SCALE_HD = new Vector2(0.5f);
		public static Vector2 TEXTURE_SCALE_MD = new Vector2(1.0f);
		public static Vector2 TEXTURE_SCALE_LD = new Vector2(2.0f);
		public static Vector2 TEXTURE_SCALE_BD = new Vector2(4.0f);
		public static Vector2 TEXTURE_SCALE_FD = new Vector2(8.0f);

		public const string TEXTURE_ASSETNAME_HD = "textures/spritesheet-sheet_hd";
		public const string TEXTURE_ASSETNAME_MD = "textures/spritesheet-sheet_md";
		public const string TEXTURE_ASSETNAME_LD = "textures/spritesheet-sheet_ld";
		public const string TEXTURE_ASSETNAME_BD = "textures/spritesheet-sheet_bd";
		public const string TEXTURE_ASSETNAME_FD = "textures/spritesheet-sheet_fd";

		public static Vector2 DEFAULT_TEXTURE_SCALE
		{
			get
			{
				switch (TEXTURE_QUALITY)
				{
					case TextureQuality.HD:
						return TEXTURE_SCALE_HD;
					case TextureQuality.MD:
						return TEXTURE_SCALE_MD;
					case TextureQuality.LD:
						return TEXTURE_SCALE_LD;
					case TextureQuality.BD:
						return TEXTURE_SCALE_BD;
					case TextureQuality.FD:
						return TEXTURE_SCALE_FD;
					default:
						throw new ArgumentException();
				}
			}
		}

		public static string TEXTURE_ASSETNAME
		{
			get
			{
				switch (TEXTURE_QUALITY)
				{
					case TextureQuality.HD:
						return TEXTURE_ASSETNAME_HD;
					case TextureQuality.MD:
						return TEXTURE_ASSETNAME_MD;
					case TextureQuality.LD:
						return TEXTURE_ASSETNAME_LD;
					case TextureQuality.BD:
						return TEXTURE_ASSETNAME_BD;
					case TextureQuality.FD:
						return TEXTURE_ASSETNAME_FD;
					default:
						throw new ArgumentException();
				}
			}
		}

		#endregion

		public static TextureAtlas AtlasTextures;

		#region Textures

		public static int ANIMATION_CANNONCOG_SIZE = 128;
		public static int ANIMATION_HUDBUTTONPAUSE_SIZE = 16;

		public static TextureRegion2D TexTileBorder;

		public static TextureRegion2D TexCannonBody;
		public static TextureRegion2D TexCannonBodyShadow;
		public static TextureRegion2D TexCannonBarrel;
		public static TextureRegion2D TexCannonBarrelShadow;
		public static TextureRegion2D TexCannonCrosshair;
		public static TextureRegion2D[] AnimCannonCog;

		public static TextureRegion2D TexBullet;
		public static TextureRegion2D TexBulletSplitter;

		public static TextureRegion2D TexPixel;

		public static TextureRegion2D TexHUDButtonBase;
		public static TextureRegion2D[] TexHUDButtonPause;
		public static TextureRegion2D TexHUDButtonSpeedHand;
		public static TextureRegion2D TexHUDButtonSpeedSet0;
		public static TextureRegion2D TexHUDButtonSpeedSet1;
		public static TextureRegion2D TexHUDButtonSpeedSet2;
		public static TextureRegion2D TexHUDButtonSpeedSet3;
		public static TextureRegion2D TexHUDButtonSpeedSet4;
		public static TextureRegion2D TexHUDButtonSpeedClock;
		public static TextureRegion2D TexHUDButtonPauseMenuBackground;
		public static TextureRegion2D TexHUDButtonPauseMenuMarker;
		public static TextureRegion2D TexHUDButtonPauseMenuMarkerBackground;

		public static TextureRegion2D TexPanelBlurEdge;
		public static TextureRegion2D TexPanelBlurCorner;
		public static TextureRegion2D TexPanelCorner;


		public static SpriteFont HUDFont;

		#endregion

#if DEBUG
		public static SpriteFont DebugFont;
		public static SpriteFont DebugFontSmall;
#endif

		public static void Initialize(ContentManager content, GraphicsDevice device)
		{

#if __DESKTOP__
			TEXTURE_QUALITY = TextureQuality.HD;
#else
			TEXTURE_QUALITY = GetPreferredQuality(device);
#endif

			LoadContent(content);
		}

		private static void LoadContent(ContentManager content)
		{
			AtlasTextures = content.Load<TextureAtlas>(TEXTURE_ASSETNAME);

			TexTileBorder = AtlasTextures["grid"];

			TexCannonBody         = AtlasTextures["cannonbody"];
			TexCannonBodyShadow   = AtlasTextures["cannonbody_shadow"];
			TexCannonBarrel       = AtlasTextures["cannonbarrel"];
			TexCannonBarrelShadow = AtlasTextures["cannonbarrel_shadow"];
			TexCannonCrosshair    = AtlasTextures["cannoncrosshair"];

			AnimCannonCog = Enumerable.Range(0, ANIMATION_CANNONCOG_SIZE).Select(p => AtlasTextures[$"cannoncog_{p:000}"]).ToArray();

			TexBullet         = AtlasTextures["cannonball"];
			TexBulletSplitter = AtlasTextures["cannonball_piece"];
			
			TexPixel = AtlasTextures["pixel"];
			TexPixel = new TextureRegion2D(TexPixel.Texture, TexPixel.X + TexPixel.Width / 2, TexPixel.Y + TexPixel.Height / 2, 1, 1); // Anti-Antialising

			TexHUDButtonBase = AtlasTextures["hud_button_base"];
			TexHUDButtonPause = Enumerable.Range(0, ANIMATION_HUDBUTTONPAUSE_SIZE).Select(p => AtlasTextures[$"hud_pause_{p:00}"]).ToArray();
			TexHUDButtonSpeedHand  = AtlasTextures["hud_time_hand"];
			TexHUDButtonSpeedSet0  = AtlasTextures["hud_time_0"];
			TexHUDButtonSpeedSet1  = AtlasTextures["hud_time_1"];
			TexHUDButtonSpeedSet2  = AtlasTextures["hud_time_2"];
			TexHUDButtonSpeedSet3  = AtlasTextures["hud_time_3"];
			TexHUDButtonSpeedSet4  = AtlasTextures["hud_time_4"];
			TexHUDButtonSpeedClock = AtlasTextures["hud_time_clock"];
			TexHUDButtonPauseMenuBackground       = AtlasTextures["pausemenu_background"];
			TexHUDButtonPauseMenuMarker           = AtlasTextures["pausemenu_marker"];
			TexHUDButtonPauseMenuMarkerBackground = AtlasTextures["pausemenu_marker_background"];

			TexPanelBlurEdge   = AtlasTextures["panel_blur_edge"];
			TexPanelBlurCorner = AtlasTextures["panel_blur_corner"];
			TexPanelCorner     = AtlasTextures["panel_corner"];

			HUDFont = content.Load<SpriteFont>("fonts/hudFont");

#if DEBUG
			DebugFont      = content.Load<SpriteFont>("fonts/debugFont");
			DebugFontSmall = content.Load<SpriteFont>("fonts/debugFontSmall");
#endif
			
			StaticTextures.DEFAULT_TEXTURE_SCALE = DEFAULT_TEXTURE_SCALE;

			StaticTextures.SinglePixel     = TexPixel;
			StaticTextures.PanelBlurCorner = TexPanelBlurCorner;
			StaticTextures.PanelBlurEdge   = TexPanelBlurEdge;
			StaticTextures.PanelCorner     = TexPanelCorner;
		}

		public static void ChangeQuality(ContentManager content, TextureQuality q)
		{
			TEXTURE_QUALITY = q;

			LoadContent(content);
		}

		public static TextureQuality GetPreferredQuality(GraphicsDevice device)
		{
			float scale = GetDeviceTextureScaling(device);

			if (scale > 1.00f) return TextureQuality.HD;
			if (scale > 0.50f) return TextureQuality.MD;
			if (scale > 0.25f) return TextureQuality.LD;

			return TextureQuality.BD;
		}

		public static float GetDeviceTextureScaling(GraphicsDevice device)
		{
			var screenWidth = device.Viewport.Width;
			var screenHeight = device.Viewport.Height;
			var screenRatio = screenWidth * 1f / screenHeight;

			var worldWidth = GDGameScreen.VIEW_WIDTH;
			var worldHeight = GDGameScreen.VIEW_HEIGHT;
			var worldRatio = worldWidth * 1f / worldHeight;
			
			if (screenRatio < worldRatio)
				return screenWidth * 1f / worldWidth;
			else
				return screenHeight * 1f / worldHeight;
		}
	}
}