using System.Linq;
using GridDominance.Core.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Font;
using MonoSAMFramework.Portable.LogProtocol;
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
					case TextureQuality.UNSPECIFIED:
					default:
						SAMLog.Error("TEX::EnumSwitch_DTS", "value = " + TEXTURE_QUALITY);
						return TEXTURE_SCALE_MD;
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
					case TextureQuality.UNSPECIFIED:
					default:
						SAMLog.Error("TEX::EnumSwitch_TA", "value = " + TEXTURE_QUALITY);
						return TEXTURE_ASSETNAME_MD;
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
					case TextureQuality.UNSPECIFIED:
					default:
						SAMLog.Error("TEX::EnumSwitch_TA2", "value = " + TEXTURE_QUALITY);
						return TEXTURE_ASSETNAME_2_MD;
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
		public static TextureRegion2D TexCannonBarrelShadow;
		public static TextureRegion2D TexLaserBarrel;
		public static TextureRegion2D TexLaserBarrelShadow;
		public static TextureRegion2D TexShieldBarrel;
		public static TextureRegion2D TexShieldBarrelShadow;
		public static TextureRegion2D[] TexCannonCore;
		public static TextureRegion2D[] TexCannonCoreShadow;
		public static TextureRegion2D TexCannonCrosshair;
		public static TextureRegion2D[] AnimCannonCog;
		public static TextureRegion2D CannonCog;
		public static TextureRegion2D CannonCogBig;
		public static TextureRegion2D TexLaserBase;
		public static TextureRegion2D TexLaserGlow;
		public static TextureRegion2D TexLaserPointer;
		public static TextureRegion2D TexLaserFlare;
		public static TextureRegion2D TexLaserFlareHalf;
		public static TextureRegion2D TexShieldLaserBase;
		public static TextureRegion2D TexShieldLaserGlow;
		public static TextureRegion2D TexShieldLaserPointer;
		public static TextureRegion2D TexShieldLaserFlare;
		public static TextureRegion2D TexShieldLaserFlareHalf;

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
		public static TextureRegion2D TexHUDButtonIconShare;
		public static TextureRegion2D TexHUDButtonIconReddit;
		public static TextureRegion2D TexHUDButtonIconBFB;
		public static TextureRegion2D TexHUDButtonIconColorblind;
		public static TextureRegion2D TexHUDButtonIconEye;
		public static TextureRegion2D TexHUDButtonIconMagnifier;
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
		public static TextureRegion2D TexHUDIconReload;
		public static TextureRegion2D TexHUDIconGenericUser;
		public static TextureRegion2D TexHUDIconPlay;
		public static TextureRegion2D[] TexHUDFlags;

		public static TextureRegion2D TexPanelBlurEdge;
		public static TextureRegion2D TexPanelBlurCorner;
		public static TextureRegion2D TexScissorBlurEdge;
		public static TextureRegion2D TexScissorBlurCorner;
		public static TextureRegion2D TexPanelCorner;

		public static TextureRegion2D TexGlassEdge;
		public static TextureRegion2D TexGlassCorner;
		public static TextureRegion2D TexGlassFill;

		public static TextureRegion2D TexCannonShieldOuter;
		public static TextureRegion2D TexCannonShieldInner;

		public static TextureRegion2D TexMirrorBlockEdge;
		public static TextureRegion2D TexMirrorBlockCorner;
		public static TextureRegion2D TexMirrorCircleSmall;
		public static TextureRegion2D TexMirrorCircleBig;

		public static TextureRegion2D TexIconBack;
		public static TextureRegion2D TexIconNext;
		public static TextureRegion2D TexIconRedo;
		public static TextureRegion2D TexIconUpload;
		public static TextureRegion2D TexIconScore;
		public static TextureRegion2D TexIconMPScore;
		public static TextureRegion2D TexIconTutorial;
		public static TextureRegion2D TexIconLock;
		public static TextureRegion2D TexIconLockOpen;
		public static TextureRegion2D TexIconInternet;
		public static TextureRegion2D TexIconBluetoothClassic;
		public static TextureRegion2D TexIconDice;
		public static TextureRegion2D TexIconConnection0;
		public static TextureRegion2D TexIconConnection1;
		public static TextureRegion2D TexIconConnection2;
		public static TextureRegion2D TexIconConnection3;
		public static TextureRegion2D TexIconStar;
		public static TextureRegion2D TexIconTetromino;
		public static TextureRegion2D TexIconError;

		public static TextureRegion2D TexIconNetworkBase;
		public static TextureRegion2D TexIconNetworkVertex1;
		public static TextureRegion2D TexIconNetworkVertex2;
		public static TextureRegion2D TexIconNetworkVertex3;
		public static TextureRegion2D TexIconNetworkVertex4;
		public static TextureRegion2D TexIconNetworkVertex5;

		public static TextureRegion2D TexDifficultyLineNone;
		public static TextureRegion2D TexDifficultyLine0;
		public static TextureRegion2D TexDifficultyLine1;
		public static TextureRegion2D TexDifficultyLine2;
		public static TextureRegion2D TexDifficultyLine3;

		public static TextureRegion2D TexLogo;
		public static TextureRegion2D TexGenericTitle;
		public static TextureRegion2D[] TexDescription;
		public static TextureRegion2D[] TexTitleNumber;

		public static TextureRegion2D TexFractionBlob;
		public static TextureRegion2D TexTriCircle;
		public static TextureRegion2D TexMinigunIcon;
		public static TextureRegion2D TexTriCog;
		public static TextureRegion2D TexBlackHoleIcon;
		public static TextureRegion2D TexWhiteHoleIcon;
		public static TextureRegion2D TexMirrorBlockIcon;
		public static TextureRegion2D TexGlassBlockIcon;
		public static TextureRegion2D TexVoidIcon;

		public static TextureRegion2D TexDescription_SCCM;
		public static TextureRegion2D TexTitle_SCCM;

        private static MonoGameSpriteFont _HUDFontRegular;
        private static MonoGameSpriteFont _HUDFontBold;
        private static MonoGameSpriteFont _LevelBackgroundFont;

        public static SAMFont HUDFontRegular      => Fonts.HUDFontRegular;
		public static SAMFont HUDFontBold => Fonts.HUDFontBold;


		public static SAMFont LevelBackgroundFont => Fonts.LevelBackgroundFont;

        #endregion

#if DEBUG
        public static MonoGameSpriteFont DebugFont;
		public static MonoGameSpriteFont DebugFontSmall;
#endif

		public static void Initialize(ContentManager content, GraphicsDevice device)
		{
			if (MonoSAMGame.IsDesktop())
				TEXTURE_QUALITY = TextureQuality.HD;
			else
				TEXTURE_QUALITY = GetPreferredQuality(device);

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
				AtlasExtraTextures["description_w4"],
				null,
				null,
				null,
			};
			TexTitleNumber           = new[]
			{
				null,
				AtlasExtraTextures["title_w1"],
				AtlasExtraTextures["title_w2"],
				AtlasExtraTextures["title_w3"],
				AtlasExtraTextures["title_w4"],
				AtlasExtraTextures["title_w5"],
				AtlasExtraTextures["title_w6"],
				AtlasExtraTextures["title_w7"]
			};

			TexDescription_SCCM = AtlasExtraTextures["description_sccm"];
			TexTitle_SCCM = AtlasExtraTextures["title_sccm"];

			TexTileBorder            = AtlasTextures["grid"];
			TexDotLine               = AtlasTextures["dotline"];

			TexCannonBody            = AtlasTextures["simple_circle"];
			TexCannonBodyShadow      = AtlasTextures["cannonbody_shadow"];
			TexCannonBarrel          = AtlasTextures["cannonbarrel"];
			TexCannonBarrelShadow    = AtlasTextures["cannonbarrel_shadow"];
			TexLaserBarrel           = AtlasTextures["laserbarrel"];
			TexLaserBarrelShadow     = AtlasTextures["laserbarrel_shadow"];
			TexShieldBarrel          = AtlasTextures["shieldlaserbarrel"];
			TexShieldBarrelShadow    = AtlasTextures["laserbarrel_shadow"];
			TexCannonCore            = Enumerable.Range(1, CANNONCORE_COUNT).Select(p => AtlasTextures[$"cannoncore_{p:00}"]).ToArray();
			TexCannonCoreShadow      = Enumerable.Range(1, CANNONCORE_COUNT).Select(p => AtlasTextures[$"cannoncore_shadow_{p:00}"]).ToArray();
			TexCannonCrosshair       = AtlasTextures["cannoncrosshair"];
			TexLaserBase             = AtlasTextures["laser_base"];
			TexLaserGlow             = AtlasTextures["laser_glow"];
			TexLaserPointer          = AtlasTextures["laser_pointer"];
			TexLaserFlare            = AtlasTextures["laser_flare"];
			TexLaserFlareHalf        = AtlasTextures["half_laser_flare"];
			TexShieldLaserBase       = AtlasTextures["shieldlaser_base"];
			TexShieldLaserGlow       = AtlasTextures["laser_glow"];
			TexShieldLaserPointer    = AtlasTextures["shieldlaser_pointer"];
			TexShieldLaserFlare      = AtlasTextures["shieldlaser_flare"];
			TexShieldLaserFlareHalf  = AtlasTextures["half_shieldlaser_flare"];

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

			TexCannonShieldOuter  = AtlasTextures["cannonshield_border"];
			TexCannonShieldInner  = AtlasTextures["cannonshield_overlay"];

			TexLevelNodeStructure    = AtlasTextures["levelnode_structure"];
			TexLevelNodeSegment      = AtlasTextures["levelnode_segment"];

			AnimCannonCog            = Enumerable.Range(0, 1 +ANIMATION_CANNONCOG_OVERLAP + ANIMATION_CANNONCOG_SIZE / ANIMATION_CANNONCOG_SEGMENTS).Select(p => AtlasTextures[$"cannoncog_{p:000}"]).ToArray();
			CannonCog                = AtlasTextures["cannoncog"];
			CannonCogBig             = AtlasTextures["cannoncog_big"];

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
			TexHUDButtonIconShare         = AtlasTextures["share"];
			TexHUDButtonIconReddit        = AtlasTextures["reddit"];
			TexHUDButtonIconBFB           = AtlasTextures["bfb"];
			TexHUDButtonIconColorblind    = AtlasTextures["colorblind"];
			TexHUDButtonIconEye           = AtlasTextures["eye"];
			TexHUDIconGenericUser         = AtlasTextures["genericuser"];
			TexHUDButtonIconMagnifier     = AtlasTextures["magnifier"];
			TexHUDIconPlay                = AtlasTextures["play"];

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
			TexHUDIconReload              = AtlasTextures["reload"];




			TexHUDFlags                   = new[] { AtlasTextures["flag_00"], AtlasTextures["flag_01"], AtlasTextures["flag_02"], AtlasTextures["flag_03"], AtlasTextures["flag_04"] };

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



			TexVortex0              = AtlasTextures["vortex_circle_0"];
			TexVortex1              = AtlasTextures["vortex_circle_1"];
			TexVortex2              = AtlasTextures["vortex_circle_2"];

			TexIconBack             = AtlasTextures["back"];
			TexIconNext             = AtlasTextures["next"];
			TexIconRedo             = AtlasTextures["redo"];
			TexIconUpload           = AtlasTextures["upload"];
			TexIconScore            = AtlasTextures["jewels"];
			TexIconMPScore          = AtlasTextures["pokerchip"];
			TexIconTutorial         = AtlasTextures["tutorial"];
			TexIconLock             = AtlasTextures["lock"];
			TexIconLockOpen         = AtlasTextures["lock_open"];
			TexIconInternet         = AtlasTextures["internet"];
			TexIconBluetoothClassic = AtlasTextures["bluetooth"];
			TexIconConnection0      = AtlasTextures["state_err"];
			TexIconConnection1      = AtlasTextures["state_ok"];
			TexIconConnection2      = AtlasTextures["state_conn_bg"];
			TexIconConnection3      = AtlasTextures["state_conn_fg"];
			TexIconDice             = AtlasTextures["dice"];



			TexDifficultyLineNone  = AtlasTextures["difficulty_line_00"];
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

			TexFractionBlob = AtlasTextures["fraction_blob"];
			TexTriCircle    = AtlasTextures["tricircle"];
			TexMinigunIcon  = AtlasTextures["minigunicon"];
			TexTriCog       = AtlasTextures["tricog"];

			TexBlackHoleIcon   = AtlasTextures["backhole_icon"];
			TexWhiteHoleIcon   = AtlasTextures["whitehole_icon"];
			TexMirrorBlockIcon = AtlasTextures["mirrorblock_icon"];
			TexGlassBlockIcon  = AtlasTextures["glassblock_icon"];
			TexVoidIcon        = AtlasTextures["void_icon"];

			TexIconStar        = AtlasTextures["star"];
			TexIconTetromino   = AtlasTextures["tetro0"];
			TexIconError       = AtlasTextures["error"];

			_HUDFontRegular      = new MonoGameSpriteFont(content.Load<SpriteFont>("fonts/hudFontRegular"));
			_HUDFontBold         = new MonoGameSpriteFont(content.Load<SpriteFont>("fonts/hudFontBold"));
			_LevelBackgroundFont = new MonoGameSpriteFont(content.Load<SpriteFont>("fonts/levelBackgroundFont"));

#if DEBUG
			DebugFont          = new MonoGameSpriteFont(content.Load<SpriteFont>("fonts/debugFont"));
			DebugFontSmall     = new MonoGameSpriteFont(content.Load<SpriteFont>("fonts/debugFontSmall"));
#endif
			
			StaticTextures.SinglePixel           = TexPixel;
			StaticTextures.MonoCircle            = TexCircle;
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

		public static TextureQuality GetPreferredQuality(GraphicsDevice device)
		{
			float scale = GetDeviceTextureScaling(device);

			if (scale > 1.00f) return TextureQuality.HD; // 2.0x
			if (scale > 0.50f) return TextureQuality.MD; // 1.0x
			if (scale > 0.25f) return TextureQuality.LD; // 0.5x

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