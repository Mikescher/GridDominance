using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FontRenderHelper
	{
		private const int MEASURE_CACHE_SIZE = 128;

		private static readonly Dictionary<SpriteFont, float> _fontHeight = new Dictionary<SpriteFont, float>();
		private static readonly Dictionary<SpriteFont, float> _fontVCenterOffsetCache = new Dictionary<SpriteFont, float>(); 
		private static readonly Dictionary<SpriteFont, CacheCollection<string, Vector2>> _measureCache = new Dictionary<SpriteFont, CacheCollection<string, Vector2>>();

		public static float GetFontScale(SpriteFont fnt, float targetSize)
		{
			float y;
			if (!_fontHeight.TryGetValue(fnt, out y)) _fontHeight[fnt] = y = fnt.MeasureString("M").Y;

			return targetSize / y;
		}

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

		public static Vector2 MeasureStringUncached(SpriteFont font, string text, float size)
		{
			return font.MeasureString(text) * GetFontScale(font, size);
		}

		public static Vector2 MeasureStringCached(SpriteFont font, string text, float size)
		{
			return MeasureStringCached(font, text) * GetFontScale(font, size);
		}

		public static Vector2 MeasureStringCached(SpriteFont font, string text)
		{
			CacheCollection<string, Vector2> cache;
			if (!_measureCache.TryGetValue(font, out cache))
			{
				cache = new CacheCollection<string, Vector2>(MEASURE_CACHE_SIZE);
				var size = font.MeasureString(text);
				cache.Add(text, size);
				return size;
			}
			else
			{
				Vector2 size;
				if (!cache.TryGetValue(text, out size))
				{
					size = font.MeasureString(text);
					cache.Add(text, size);
					return size;
				}
				else
				{
					return size;
				}
			}
		}

		public static void DrawTextCentered(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, Vector2 position)
		{
			if (text == "") return;
			var bounds = MeasureStringCached(font, text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new Vector2(bounds.X / 2f, bounds.Y / 2f - GetFontVCenterOffset(font)),
				GetFontScale(font, size),
				SpriteEffects.None,
				0);
		}

		public static void DrawTextVerticallyCentered(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, Vector2 position)
		{
			if (text == "") return;
			
			var bounds = MeasureStringCached(font, text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new Vector2(0, bounds.Y / 2f - GetFontVCenterOffset(font)),
				GetFontScale(font, size),
				SpriteEffects.None,
				0);
		}

		public static void DrawTextTopRight(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, FPoint position)
		{
			if (text == "") return;

			var bounds = MeasureStringCached(font, text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new Vector2(bounds.X, 0),
				GetFontScale(font, size),
				SpriteEffects.None,
				0);
		}
	}
}
