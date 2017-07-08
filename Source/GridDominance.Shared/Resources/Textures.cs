using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
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

		public const string TEXTURE_ASSETNAME_HD = "textures/spritesheet_default-sheet_hd";
		public const string TEXTURE_ASSETNAME_MD = "textures/spritesheet_default-sheet_md";
		public const string TEXTURE_ASSETNAME_LD = "textures/spritesheet_default-sheet_ld";
		public const string TEXTURE_ASSETNAME_BD = "textures/spritesheet_default-sheet_bd";
		public const string TEXTURE_ASSETNAME_FD = "textures/spritesheet_default-sheet_fd";

		public const string TEXTURE_ASSETNAME_2_HD = "textures/spritesheet_extra-sheet_hd";
		public const string TEXTURE_ASSETNAME_2_MD = "textures/spritesheet_extra-sheet_md";
		public const string TEXTURE_ASSETNAME_2_LD = "textures/spritesheet_extra-sheet_ld";
		public const string TEXTURE_ASSETNAME_2_BD = "textures/spritesheet_extra-sheet_bd";
		public const string TEXTURE_ASSETNAME_2_FD = "textures/spritesheet_extra-sheet_fd";

		public static float DEFAULT_TEXTURE_SCALE_F => DEFAULT_TEXTURE_SCALE.X;

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

		public static string TEXTURE_ASSETNAME_2
		{
			get
			{
				switch (TEXTURE_QUALITY)
				{
					case TextureQuality.HD:
						return TEXTURE_ASSETNAME_2_HD;
					case TextureQuality.MD:
						return TEXTURE_ASSETNAME_2_MD;
					case TextureQuality.LD:
						return TEXTURE_ASSETNAME_2_LD;
					case TextureQuality.BD:
						return TEXTURE_ASSETNAME_2_BD;
					case TextureQuality.FD:
						return TEXTURE_ASSETNAME_2_FD;
					default:
						throw new ArgumentException();
				}
			}
		}

		#endregion

		public static TextureAtlas AtlasTextures;
		public static TextureAtlas AtlasExtraTextures;

		#region Textures

		public static int ANIMATION_CANNONCOG_SIZE = 128;
		public static int ANIMATION_CANNONCOG_SEGMENTS = 4;
		public static int ANIMATION_CANNONCOG_OVERLAP = 3;
		public static int ANIMATION_HUDBUTTONPAUSE_SIZE = 16;
		public static int CANNONCORE_COUNT = 6;

		public static TextureRegion2D TexTileBorder;
		public static TextureRegion2D TexDotLine;

		public static TextureRegion2D TexCircle;
		public static TextureRegion2D TexTriangle;
		public static TextureRegion2D TexCircleEmpty; 
		public static TextureRegion2D TexGradient;

		public static TextureRegion2D TexCannonBody;
		public static TextureRegion2D TexCannonBodyShadow;
		public static TextureRegion2D TexCannonBarrel;
		public static TextureRegion2D[] TexCannonCore;
		public static TextureRegion2D[] TexCannonCoreShadow;
		public static TextureRegion2D TexCannonBarrelShadow;
		public static TextureRegion2D TexCannonCrosshair;
		public static TextureRegion2D[] AnimCannonCog;
		public static TextureRegion2D CannonCog;
		public static TextureRegion2D TexLaserBase;
		public static TextureRegion2D TexLaserGlow;
		public static TextureRegion2D TexLaserPointer;
		public static TextureRegion2D TexLaserFlare;
		public static TextureRegion2D TexLaserFlareHalf;

		public static TextureRegion2D TexVoidCircle_FG;
		public static TextureRegion2D TexVoidWall_FG_L1;
		public static TextureRegion2D TexVoidWall_FG_L2;
		public static TextureRegion2D TexVoidWall_FG_End;
		public static TextureRegion2D TexVoidWall_FG_Middle;
		public static TextureRegion2D TexVoidCircle_BG;
		public static TextureRegion2D TexVoidWall_BG_L1;
		public static TextureRegion2D TexVoidWall_BG_L2;
		public static TextureRegion2D TexVoidWall_BG_End;
		public static TextureRegion2D TexVoidWall_BG_Middle;

		public static TextureRegion2D TexVortex0;
		public static TextureRegion2D TexVortex1;
		public static TextureRegion2D TexVortex2;

		public static TextureRegion2D TexPortalDropEnd1;
		public static TextureRegion2D TexPortalDropEnd2;
		public static TextureRegion2D TexPortalDropMid;

		public static TextureRegion2D TexLevelNodeStructure;
		public static TextureRegion2D TexLevelNodeSegment;

		public static TextureRegion2D TexBullet;
		public static TextureRegion2D TexBulletSplitter;

		public static TextureRegion2D TexPixel;
		public static TextureRegion2D[] TexParticle;

		public static TextureRegion2D TexHUDButtonBase;
		public static TextureRegion2D[] TexHUDButtonPause;
		public static TextureRegion2D TexHUDButtonSpeedHand;
		public static TextureRegion2D TexHUDButtonSpeedSet0;
		public static TextureRegion2D TexHUDButtonSpeedSet1;
		public static TextureRegion2D TexHUDButtonSpeedSet2;
		public static TextureRegion2D TexHUDButtonSpeedSet3;
		public static TextureRegion2D TexHUDButtonSpeedSet4;
		public static TextureRegion2D TexHUDButtonSpeedClock;
		public static TextureRegion2D TexHUDButtonPauseMenuMarker;
		public static TextureRegion2D TexHUDButtonPauseMenuMarkerBackground;
		public static TextureRegion2D TexHUDButtonIconHighscore;
		public static TextureRegion2D TexHUDButtonIconEffectsOn;
		public static TextureRegion2D TexHUDButtonIconEffectsOff;
		public static TextureRegion2D TexHUDButtonIconVolumeOn;
		public static TextureRegion2D TexHUDButtonIconVolumeOff;
		public static TextureRegion2D TexHUDButtonIconMusicOn;
		public static TextureRegion2D TexHUDButtonIconMusicOff;
		public static TextureRegion2D TexHUDButtonIconAbout;
		public static TextureRegion2D TexHUDButtonIconSettings;
		public static TextureRegion2D TexHUDButtonIconAccount;
		public static TextureRegion2D TexHUDIconUser;
		public static TextureRegion2D TexHUDIconPassword;
		public static TextureRegion2D TexHUDIconKeyboardCaps;
		public static TextureRegion2D TexHUDIconKeyboardEnter;
		public static TextureRegion2D TexHUDIconKeyboardBackspace;
		public static TextureRegion2D TexHUDIconTouchUp;
		public static TextureRegion2D TexHUDIconTouchDown;
		public static TextureRegion2D TexHUDIconArrow;
		public static TextureRegion2D TexHUDIconChevronLeft;
		public static TextureRegion2D TexHUDIconChevronRight;
		public static TextureRegion2D[] TexHUDFlags;

		public static TextureRegion2D TexPanelBlurEdge;
		public static TextureRegion2D TexPanelBlurCorner;
		public static TextureRegion2D TexScissorBlurEdge;
		public static TextureRegion2D TexScissorBlurCorner;
		public static TextureRegion2D TexPanelCorner;

		public static TextureRegion2D TexGlassEdge;
		public static TextureRegion2D TexGlassCorner;
		public static TextureRegion2D TexGlassFill;

		public static TextureRegion2D TexMirrorBlockEdge;
		public static TextureRegion2D TexMirrorBlockCorner;
		public static TextureRegion2D TexMirrorCircleSmall;
		public static TextureRegion2D TexMirrorCircleBig;

		public static TextureRegion2D TexIconBack;
		public static TextureRegion2D TexIconNext;
		public static TextureRegion2D TexIconRedo;
		public static TextureRegion2D TexIconScore;
		public static TextureRegion2D TexIconTutorial;
		public static TextureRegion2D TexIconLock;
		public static TextureRegion2D TexIconLockOpen;
		public static TextureRegion2D TexIconInternet;
		public static TextureRegion2D TexIconBluetooth;
		public static TextureRegion2D TexIconConnection0;
		public static TextureRegion2D TexIconConnection1;
		public static TextureRegion2D TexIconConnection2;
		public static TextureRegion2D TexIconConnection3;

		public static TextureRegion2D TexIconNetworkBase;
		public static TextureRegion2D TexIconNetworkVertex1;
		public static TextureRegion2D TexIconNetworkVertex2;
		public static TextureRegion2D TexIconNetworkVertex3;
		public static TextureRegion2D TexIconNetworkVertex4;
		public static TextureRegion2D TexIconNetworkVertex5;

		public static TextureRegion2D TexDifficultyLine0;
		public static TextureRegion2D TexDifficultyLine1;
		public static TextureRegion2D TexDifficultyLine2;
		public static TextureRegion2D TexDifficultyLine3;

		public static TextureRegion2D TexLogo;
		public static TextureRegion2D TexGenericTitle;
		public static TextureRegion2D[] TexDescription;
		public static TextureRegion2D[] TexTitleNumber;

		public static SpriteFont HUDFontRegular;
		public static SpriteFont HUDFontBold;
		public static SpriteFont LevelBackgroundFont;

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
			AtlasTextures      = content.Load<TextureAtlas>(TEXTURE_ASSETNAME);
			AtlasExtraTextures = content.Load<TextureAtlas>(TEXTURE_ASSETNAME_2);

			TexLogo                  = AtlasExtraTextures["logo"];
			TexGenericTitle          = AtlasExtraTextures["title_generic"];
			TexDescription           = new[]
			{
				null,
				null,
				AtlasExtraTextures["description_w2"],
				AtlasExtraTextures["description_w3"],
			};
			TexTitleNumber           = new[]
			{
				null,
				null,
				AtlasExtraTextures["title_w2"],
				AtlasExtraTextures["title_w3"],
				AtlasExtraTextures["title_w4"],
				AtlasExtraTextures["title_w5"],
				AtlasExtraTextures["title_w6"],
				AtlasExtraTextures["title_w7"]
			};

			TexTileBorder            = AtlasTextures["grid"];
			TexDotLine               = AtlasTextures["dotline"];

			TexCannonBody            = AtlasTextures["simple_circle"];
			TexCannonBodyShadow      = AtlasTextures["cannonbody_shadow"];
			TexCannonBarrel          = AtlasTextures["cannonbarrel"];
			TexCannonCore            = Enumerable.Range(1, CANNONCORE_COUNT).Select(p => AtlasTextures[$"cannoncore_{p:00}"]).ToArray();
			TexCannonCoreShadow      = Enumerable.Range(1, CANNONCORE_COUNT).Select(p => AtlasTextures[$"cannoncore_shadow_{p:00}"]).ToArray();
			TexCannonBarrelShadow    = AtlasTextures["cannonbarrel_shadow"];
			TexCannonCrosshair       = AtlasTextures["cannoncrosshair"];
			TexLaserBase             = AtlasTextures["laser_base"];
			TexLaserGlow             = AtlasTextures["laser_glow"];
			TexLaserPointer          = AtlasTextures["laser_pointer"];
			TexLaserFlare            = AtlasTextures["laser_flare"];
			TexLaserFlareHalf        = AtlasTextures["half_laser_flare"];

			TexVoidCircle_FG      = AtlasTextures["voidcircle_fg"];
			TexVoidWall_FG_L1     = AtlasTextures["voidwall_fg_1"];
			TexVoidWall_FG_L2     = AtlasTextures["voidwall_fg_2"];
			TexVoidWall_FG_End    = AtlasTextures["voidwall_fg_outer"];
			TexVoidWall_FG_Middle = AtlasTextures["voidwall_fg_inner"];
			TexVoidCircle_BG      = AtlasTextures["voidcircle_bg"];
			TexVoidWall_BG_L1     = AtlasTextures["voidwall_bg_1"];
			TexVoidWall_BG_L2     = AtlasTextures["voidwall_bg_2"];
			TexVoidWall_BG_End    = AtlasTextures["voidwall_bg_outer"];
			TexVoidWall_BG_Middle = AtlasTextures["voidwall_bg_inner"];

			TexLevelNodeStructure    = AtlasTextures["levelnode_structure"];
			TexLevelNodeSegment      = AtlasTextures["levelnode_segment"];

			AnimCannonCog            = Enumerable.Range(0, 1 +ANIMATION_CANNONCOG_OVERLAP + ANIMATION_CANNONCOG_SIZE / ANIMATION_CANNONCOG_SEGMENTS).Select(p => AtlasTextures[$"cannoncog_{p:000}"]).ToArray();
			CannonCog                = AtlasTextures["cannoncog"];

			TexBullet                = AtlasTextures["cannonball"];
			TexBulletSplitter        = AtlasTextures["cannonball_piece"];

			TexCircle                = AtlasTextures["simple_circle"];
			TexTriangle              = AtlasTextures["simple_triangle"];
			TexCircleEmpty           = AtlasTextures["simple_circle_empty"];
			TexPixel                 = AtlasTextures["simple_pixel"];
			TexPixel                 = new TextureRegion2D(TexPixel.Texture, TexPixel.X + TexPixel.Width / 2, TexPixel.Y + TexPixel.Height / 2, 1, 1); // Anti-Antialising
			TexParticle              = Enumerable.Range(0, 17).Select(p => AtlasTextures[$"particle_{p:00}"]).ToArray();
			TexGradient              = AtlasTextures["alphagradient"];

			TexHUDButtonBase                      = AtlasTextures["hud_button_base"];
			TexHUDButtonPause                     = Enumerable.Range(0, ANIMATION_HUDBUTTONPAUSE_SIZE).Select(p => AtlasTextures[$"hud_pause_{p:00}"]).ToArray();
			TexHUDButtonSpeedHand                 = AtlasTextures["hud_time_hand"];
			TexHUDButtonSpeedSet0                 = AtlasTextures["hud_time_0"];
			TexHUDButtonSpeedSet1                 = AtlasTextures["hud_time_1"];
			TexHUDButtonSpeedSet2                 = AtlasTextures["hud_time_2"];
			TexHUDButtonSpeedSet3                 = AtlasTextures["hud_time_3"];
			TexHUDButtonSpeedSet4                 = AtlasTextures["hud_time_4"];
			TexHUDButtonSpeedClock                = AtlasTextures["hud_time_clock"];
			TexHUDButtonPauseMenuMarker           = AtlasTextures["pausemenu_marker"];
			TexHUDButtonPauseMenuMarkerBackground = AtlasTextures["pausemenu_marker_background"];

			TexHUDButtonIconHighscore     = AtlasTextures["cloud"];
			TexHUDButtonIconEffectsOn     = AtlasTextures["blur_on"];
			TexHUDButtonIconEffectsOff    = AtlasTextures["blur_off"];
			TexHUDButtonIconVolumeOn      = AtlasTextures["volume_up"];
			TexHUDButtonIconVolumeOff     = AtlasTextures["volume_off"];
			TexHUDButtonIconMusicOn       = AtlasTextures["music_on"];
			TexHUDButtonIconMusicOff      = AtlasTextures["music_off"];
			TexHUDButtonIconAbout         = AtlasTextures["info"];
			TexHUDButtonIconSettings      = AtlasTextures["settings"];
			TexHUDButtonIconAccount       = AtlasTextures["fingerprint"];
			TexHUDIconUser                = AtlasTextures["user"];
			TexHUDIconPassword            = AtlasTextures["password"];
			TexHUDIconKeyboardCaps        = AtlasTextures["caps"];
			TexHUDIconKeyboardEnter       = AtlasTextures["enter"];
			TexHUDIconKeyboardBackspace   = AtlasTextures["backspace"];
			TexHUDIconTouchUp             = AtlasTextures["touch_up"];
			TexHUDIconTouchDown           = AtlasTextures["touch_down"];
			TexHUDIconArrow               = AtlasTextures["arrow"];
			TexHUDIconChevronLeft         = AtlasTextures["chevron_left"];
			TexHUDIconChevronRight        = AtlasTextures["chevron_right"];

			TexHUDFlags                   = new[] { AtlasTextures["flag_00"], AtlasTextures["flag_01"] };

			TexPanelBlurEdge     = AtlasTextures["panel_blur_edge"];
			TexPanelBlurCorner   = AtlasTextures["panel_blur_corner"];
			TexScissorBlurEdge   = AtlasTextures["scissor_blur_edge"];
			TexScissorBlurCorner = AtlasTextures["scissor_blur_corner"];
			TexPanelCorner       = AtlasTextures["panel_corner"];

			TexPortalDropEnd1   = AtlasTextures["portalshadow_end1"];
			TexPortalDropEnd2   = AtlasTextures["portalshadow_end2"];
			TexPortalDropMid   = AtlasTextures["portalshadow_mid"];

			TexGlassEdge       = AtlasTextures["glass_side"];
			TexGlassCorner     = AtlasTextures["glass_corner"];
			TexGlassFill       = AtlasTextures["glass_fill"];

			TexMirrorBlockEdge   = AtlasTextures["mirror_edge"];
			TexMirrorBlockCorner = AtlasTextures["mirror_corner"];
			TexMirrorCircleSmall = AtlasTextures["mirrorblock_small"];
			TexMirrorCircleBig   = AtlasTextures["mirrorblock_big"];



			TexVortex0          = AtlasTextures["vortex_circle_0"];
			TexVortex1          = AtlasTextures["vortex_circle_1"];
			TexVortex2          = AtlasTextures["vortex_circle_2"];

			TexIconBack         = AtlasTextures["back"];
			TexIconNext         = AtlasTextures["next"];
			TexIconRedo         = AtlasTextures["redo"];
			TexIconScore        = AtlasTextures["jewels"];
			TexIconTutorial     = AtlasTextures["tutorial"];
			TexIconLock         = AtlasTextures["lock"];
			TexIconLockOpen     = AtlasTextures["lock_open"];
			TexIconInternet     = AtlasTextures["internet"];
			TexIconBluetooth    = AtlasTextures["bluetooth"];
			TexIconConnection0  = AtlasTextures["state_err"];
			TexIconConnection1  = AtlasTextures["state_ok"];
			TexIconConnection2  = AtlasTextures["state_conn_bg"];
			TexIconConnection3  = AtlasTextures["state_conn_fg"];



			TexDifficultyLine0     = AtlasTextures["difficulty_line_01"];
			TexDifficultyLine1     = AtlasTextures["difficulty_line_02"];
			TexDifficultyLine2     = AtlasTextures["difficulty_line_03"];
			TexDifficultyLine3     = AtlasTextures["difficulty_line_04"];

			TexIconNetworkBase    = AtlasTextures["network_base"];
			TexIconNetworkVertex1 = AtlasTextures["network_vertex1"];
			TexIconNetworkVertex2 = AtlasTextures["network_vertex2"];
			TexIconNetworkVertex3 = AtlasTextures["network_vertex3"];
			TexIconNetworkVertex4 = AtlasTextures["network_vertex4"];
			TexIconNetworkVertex5 = AtlasTextures["network_vertex5"];

			HUDFontRegular      = content.Load<SpriteFont>("fonts/hudFontRegular");
			HUDFontBold         = content.Load<SpriteFont>("fonts/hudFontBold");
			LevelBackgroundFont = content.Load<SpriteFont>("fonts/levelBackgroundFont");

#if DEBUG
			DebugFont          = content.Load<SpriteFont>("fonts/debugFont");
			DebugFontSmall     = content.Load<SpriteFont>("fonts/debugFontSmall");
#endif
			
			StaticTextures.SinglePixel           = TexPixel;
			StaticTextures.PanelBlurCorner       = TexPanelBlurCorner;
			StaticTextures.PanelBlurEdge         = TexPanelBlurEdge;
			StaticTextures.PanelCorner           = TexPanelCorner;
			StaticTextures.PanelBlurCornerPrecut = TexScissorBlurCorner;
			StaticTextures.PanelBlurEdgePrecut   = TexScissorBlurEdge;

			StaticTextures.KeyboardBackspace = TexHUDIconKeyboardBackspace;
			StaticTextures.KeyboardEnter     = TexHUDIconKeyboardEnter;
			StaticTextures.KeyboardCaps      = TexHUDIconKeyboardCaps;
			StaticTextures.KeyboardCircle    = TexCircle;

		}

		public static void ChangeQuality(ContentManager content, TextureQuality q)
		{
			TEXTURE_QUALITY = q;

			LoadContent(content);
		}

		public static TextureQuality GetPreferredQuality(GraphicsDevice device) //TODO Does this actually work, bzw should we be more generous 
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

			var worldWidth = GDConstants.VIEW_WIDTH;
			var worldHeight = GDConstants.VIEW_HEIGHT;
			var worldRatio = worldWidth * 1f / worldHeight;
			
			if (screenRatio < worldRatio)
				return screenWidth * 1f / worldWidth;
			else
				return screenHeight * 1f / worldHeight;
		}
	}
}