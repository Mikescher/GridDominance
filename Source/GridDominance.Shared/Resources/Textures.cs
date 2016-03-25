using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Resources
{
	enum TextureQuality
	{
		HD, // 2.00x
		MD, // 1.00x
		LD, // 0.50x
		BD, // 0.25x
	}

	static class Textures
	{
		#region Scaling

		public static TextureQuality TEXTURE_QUALITY = TextureQuality.BD;

		public static Vector2 TEXTURE_SCALE_HD = new Vector2(0.5f);
		public static Vector2 TEXTURE_SCALE_MD = new Vector2(1.0f);
		public static Vector2 TEXTURE_SCALE_LD = new Vector2(2.0f);
		public static Vector2 TEXTURE_SCALE_BD = new Vector2(4.0f);

		public const string TEXTURE_ASSETNAME_HD = "textures/spritesheet-sheet_hd";
		public const string TEXTURE_ASSETNAME_MD = "textures/spritesheet-sheet_md";
		public const string TEXTURE_ASSETNAME_LD = "textures/spritesheet-sheet_ld";
		public const string TEXTURE_ASSETNAME_BD = "textures/spritesheet-sheet_bd";

		public static Vector2 DEFAULT_TEXTURE_SCALE
		{
			get
			{
				switch (TEXTURE_QUALITY)
				{
					case TextureQuality.HD: return TEXTURE_SCALE_HD;
					case TextureQuality.MD: return TEXTURE_SCALE_MD;
					case TextureQuality.LD: return TEXTURE_SCALE_LD;
					case TextureQuality.BD: return TEXTURE_SCALE_BD;
					default: throw new NotImplementedException();
				}
			}
		}

		public static string TEXTURE_ASSETNAME
		{
			get
			{
				switch (TEXTURE_QUALITY)
				{
					case TextureQuality.HD: return TEXTURE_ASSETNAME_HD;
					case TextureQuality.MD: return TEXTURE_ASSETNAME_MD;
					case TextureQuality.LD: return TEXTURE_ASSETNAME_LD;
					case TextureQuality.BD: return TEXTURE_ASSETNAME_BD;
					default: throw new NotImplementedException();
				}
			}
		}

		#endregion

		public static TextureAtlas AtlasTextures;

		#region Textures

		public static int ANIMATION_CANNONCOG_SIZE = 128;

		public static TextureRegion2D TexTileBorder;

		public static TextureRegion2D TexCannonBody;
		public static TextureRegion2D TexCannonBodyShadow;
		public static TextureRegion2D TexCannonBarrel;
		public static TextureRegion2D TexCannonBarrelShadow;
		public static TextureRegion2D TexCannonCrosshair;
		public static TextureRegion2D[] AnimCannonCog;

		public static TextureRegion2D TexBullet;

		public static TextureRegion2D TexPixel;

		#endregion

#if DEBUG
		public static SpriteFont DebugFont;
		public static SpriteFont DebugFontSmall;
#endif

		public static void LoadContent(ContentManager content)
		{
			AtlasTextures = content.Load<TextureAtlas>(TEXTURE_ASSETNAME);

			TexTileBorder = AtlasTextures["grid"];

			TexCannonBody = AtlasTextures["cannonbody"];
			TexCannonBodyShadow = AtlasTextures["cannonbody_shadow"];
			TexCannonBarrel = AtlasTextures["cannonbarrel"];
			TexCannonBarrelShadow = AtlasTextures["cannonbarrel_shadow"];
			TexCannonCrosshair = AtlasTextures["cannoncrosshair"];

			AnimCannonCog = Enumerable.Range(0, ANIMATION_CANNONCOG_SIZE).Select(p => AtlasTextures[$"cannoncog_{p:000}"]).ToArray();

			TexBullet = AtlasTextures["cannonball"];

			// Anti-Antialising
			TexPixel = AtlasTextures["pixel"];
			TexPixel = new TextureRegion2D(TexPixel.Texture, TexPixel.X + TexPixel.Width / 2, TexPixel.Y + TexPixel.Height / 2, 1, 1);

#if DEBUG
			DebugFont = content.Load<SpriteFont>("fonts/debugFont");
			DebugFontSmall = content.Load<SpriteFont>("fonts/debugFontSmall");
#endif
		}
	}
}
