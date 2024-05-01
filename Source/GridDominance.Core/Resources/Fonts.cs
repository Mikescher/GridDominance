using FontStashSharp;
using GridDominance.Common.TTFFonts;
using Microsoft.Xna.Framework.Content;
using MonoSAMFramework.Portable.Font;

namespace GridDominance.Core.Resources
{
    public static class Fonts
    {
        private static FontSystem _fontAsapBold    = new FontSystem();
        private static FontSystem _fontAsapRegular = new FontSystem();
        private static FontSystem _fontComicJens   = new FontSystem();
#if DEBUG
        private static FontSystem _fontCourierNew  = new FontSystem();
#endif

#if DEBUG
        public static SAMFont DebugFont;
        public static SAMFont DebugFontSmall;
#endif

        public static SAMFont HUDFontBold;
        public static SAMFont HUDFontRegular;
        public static SAMFont LevelBackgroundFont;

        public static void LoadContent(ContentManager content)
        {
            var fontDataAsapBold    = content.Load<TTFFontData>("binfonts/AsapBold");
            var fontDataAsapRegular = content.Load<TTFFontData>("binfonts/AsapRegular");
            var fontDataComicJens   = content.Load<TTFFontData>("binfonts/ComicJens");
#if DEBUG
            var fontDataCourierNew  = content.Load<TTFFontData>("binfonts/CourierNew");
#endif

            _fontAsapBold.AddFont(fontDataAsapBold.BinaryData);
            _fontAsapRegular.AddFont(fontDataAsapRegular.BinaryData);
            _fontComicJens.AddFont(fontDataComicJens.BinaryData);
#if DEBUG
            _fontCourierNew.AddFont(fontDataCourierNew.BinaryData);
#endif

#if DEBUG
            DebugFont = new FontStashFont(_fontCourierNew.GetFont(14));
            DebugFontSmall      = new FontStashFont(_fontCourierNew.GetFont(8));
#endif
            HUDFontBold = new FontStashFont(_fontAsapBold.GetFont(64));
            HUDFontRegular      = new FontStashFont(_fontAsapRegular.GetFont(64));
            LevelBackgroundFont = new FontStashFont(_fontComicJens.GetFont(128));
        }  
    }      
}          
