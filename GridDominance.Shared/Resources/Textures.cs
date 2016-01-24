using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Resources
{
    static class Textures
    {
        public static bool IsLoaded { get; private set; } = false;

        public static TextureAtlas AtlasTextures;

        public static TextureRegion2D TexDebugTile;
        public static TextureRegion2D TexDebugCannonBody;

        public static void LoadContent(ContentManager content)
        {
            AtlasTextures = content.Load<TextureAtlas>("textures/spritesheet-sheet");

            TexDebugTile        = AtlasTextures["tile_debug"];
            TexDebugCannonBody  = AtlasTextures["cannonbody_debug"];

            IsLoaded = true;
        }
    }
}
