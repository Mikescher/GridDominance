using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Resources
{
    static class Textures
    {
        public static Vector2 DEFAULT_TEXTURE_SCALE = new Vector2(0.5f);

        public static bool IsLoaded { get; private set; } = false;

        public static TextureAtlas AtlasTextures;

        public static TextureRegion2D TexDebugTile;
        public static TextureRegion2D TexCannonBody;
        public static TextureRegion2D TexCannonBarrel;

	    public static SpriteFont DebugFont;

        public static void LoadContent(ContentManager content)
        {
            AtlasTextures = content.Load<TextureAtlas>("textures/spritesheet-sheet");

            TexDebugTile    = AtlasTextures["tile_debug"];
            TexCannonBody   = AtlasTextures["cannonbody"];
            TexCannonBarrel = AtlasTextures["cannonbarrel"];

			DebugFont = content.Load<SpriteFont>("fonts/debugFont");

			IsLoaded = true;
        }
    }
}
