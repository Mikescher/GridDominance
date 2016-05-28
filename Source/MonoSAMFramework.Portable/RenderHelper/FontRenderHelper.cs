using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FontRenderHelper
	{
		public static float GetFontScale(SpriteFont fnt, float targetSize)
		{
			return targetSize / fnt.MeasureString("M").Y;
		}

		private static readonly Dictionary<SpriteFont, float> _fontVCenterOffsetCache = new Dictionary<SpriteFont, float>(); 

		public static float GetFontVCenterOffset(SpriteFont fnt)
		{
			if (!_fontVCenterOffsetCache.ContainsKey(fnt))
			{
				var glyph = fnt.GetGlyphs()['M'];
				var top = glyph.Cropping.Y;
				var bot = fnt.LineSpacing - glyph.BoundsInTexture.Height - top;

				var offset = (bot - top) / 2f;

				_fontVCenterOffsetCache[fnt] = offset;
			}

			return _fontVCenterOffsetCache[fnt];
		}
	}
}
