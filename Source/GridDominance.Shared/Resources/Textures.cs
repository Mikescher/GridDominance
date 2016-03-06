using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Resources
{
	static class Textures
	{
		public static Vector2 DEFAULT_TEXTURE_SCALE = new Vector2(0.5f); // Texturen haben doppelte Auflösung - HD

		public static bool IsLoaded { get; private set; } = false;

		public static TextureAtlas AtlasTextures;

		#region Textures

		public static TextureRegion2D TexTileBorder;

		public static TextureRegion2D TexCannonBody;
		public static TextureRegion2D TexCannonBodyShadow;
		public static TextureRegion2D TexCannonBarrel;
		public static TextureRegion2D TexCannonBarrelShadow;
		public static TextureRegion2D TexCannonCog;

		public static TextureRegion2D TexBullet;

		public static TextureRegion2D TexPixel;

		#endregion

		public static SpriteFont DebugFont;

		public static void LoadContent(ContentManager content)
		{
			AtlasTextures = content.Load<TextureAtlas>("textures/spritesheet-sheet");

			TexTileBorder = AtlasTextures["grid"];

			TexCannonBody = AtlasTextures["cannonbody"];
			TexCannonBodyShadow = AtlasTextures["cannonbody_shadow"];
			TexCannonBarrel = AtlasTextures["cannonbarrel"];
			TexCannonBarrelShadow = AtlasTextures["cannonbarrel_shadow"];
			TexCannonCog = AtlasTextures["cannoncog"];

			TexBullet = AtlasTextures["cannonball"];

			TexPixel = AtlasTextures["pixel"];

			DebugFont = content.Load<SpriteFont>("fonts/debugFont");

			IsLoaded = true;
		}
	}
}
