using FontStashSharp;
using GridDominance.Common.TTFFonts;
using Microsoft.Xna.Framework.Content;

namespace GridDominance.Core.Resources
{
    public static class Fonts
    {
        private static FontSystem _fontAsapBold    = new FontSystem();
        private static FontSystem _fontAsapRegular = new FontSystem();
        private static FontSystem _fontComicJens   = new FontSystem();
        private static FontSystem _fontCourierNew  = new FontSystem();

        public static DynamicSpriteFont DebugFont;
        public static DynamicSpriteFont DebugFontSmall;
        public static DynamicSpriteFont HUDFontBold;
        public static DynamicSpriteFont HUDFontRegular;
        public static DynamicSpriteFont LevelBackgroundFont;

        public static void LoadContent(ContentManager content)
        {
            var fontDataAsapBold    = content.Load<TTFFontData>("binfonts/AsapBold");
            var fontDataAsapRegular = content.Load<TTFFontData>("binfonts/AsapRegular");
            var fontDataComicJens   = content.Load<TTFFontData>("binfonts/ComicJens");
            var fontDataCourierNew  = content.Load<TTFFontData>("binfonts/CourierNew");

            _fontAsapBold.AddFont(fontDataAsapBold.BinaryData);
            _fontAsapRegular.AddFont(fontDataAsapRegular.BinaryData);
            _fontComicJens.AddFont(fontDataComicJens.BinaryData);
            _fontCourierNew.AddFont(fontDataCourierNew.BinaryData);

            DebugFont           = _fontCourierNew.GetFont(14);
            DebugFontSmall      = _fontCourierNew.GetFont(8);
            HUDFontBold         = _fontAsapBold.GetFont(64);
            HUDFontRegular      = _fontAsapRegular.GetFont(64);
            LevelBackgroundFont = _fontComicJens.GetFont(128);
        }  
    }      
}          
