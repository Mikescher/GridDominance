using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Resources
{
	static class Textures
	{
		public static Vector2 DEFAULT_TEXTURE_SCALE = new Vector2(0.5f); // Texturen haben doppelte Auflösung - HD
		
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
			AtlasTextures = content.Load<TextureAtlas>("textures/spritesheet-sheet");

			TexTileBorder = AtlasTextures["grid"];

			TexCannonBody = AtlasTextures["cannonbody"];
			TexCannonBodyShadow = AtlasTextures["cannonbody_shadow"];
			TexCannonBarrel = AtlasTextures["cannonbarrel"];
			TexCannonBarrelShadow = AtlasTextures["cannonbarrel_shadow"];
			TexCannonCrosshair = AtlasTextures["cannoncrosshair"];

			AnimCannonCog = Enumerable.Range(0, ANIMATION_CANNONCOG_SIZE).Select(p => AtlasTextures[$"cannoncog_{p:000}"]).ToArray();

			TexBullet = AtlasTextures["cannonball"];

			TexPixel = AtlasTextures["pixel"];

#if DEBUG
			DebugFont = content.Load<SpriteFont>("fonts/debugFont");
			DebugFontSmall = content.Load<SpriteFont>("fonts/debugFontSmall");
#endif
		}
	}
}
