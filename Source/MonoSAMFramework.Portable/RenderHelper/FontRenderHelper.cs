using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using System.Collections.Generic;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

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

		private static string LimitStringLength(SpriteFont font, float size, string text, float maxlen)
		{
			var len = MeasureStringCached(font, text, size).X;
			while (len > maxlen && text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
				len = MeasureStringCached(font, text, size).X;
			}
			return text;
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

		public static void DrawSingleLineInBox(IBatchRenderer sbatch, SpriteFont font, string text, FRectangle rect, float padding, bool horzCenter, Color color)
		{
			if (text == "") return;

			var size = rect.Height - 2 * padding;
			var maxwidth = rect.Width - 2 * padding;

			text = LimitStringLength(font, size, text, maxwidth);

			if (horzCenter)
				DrawTextCentered(sbatch, font, size, text, color, rect.VecCenter);
			else
				DrawTextVerticallyCentered(sbatch, font, size, text, color, new Vector2(rect.X + padding, rect.CenterY));
		}

		public static List<string> WrapLinesIntoWidth(string text, SpriteFont font, float fontSize, float maxWidth, HUDWordWrap wrap)
		{
			var sz = MeasureStringCached(font, text, fontSize);

			if (sz.X < maxWidth) return new List<string> { text };


			List<string> lines = new List<string>();

			var remText = text;
			while (remText.Length > 0)
			{
				var line = "";
				while (remText.Length > 0 && remText[0] != '\n')
				{
					var chr = remText[0];

					if (chr == '\r')
					{
						remText = remText.Substring(1);
						continue;
					}

					var newlen = MeasureStringUncached(font, line + chr, fontSize).X;

					if (line.Length > 1 && newlen > maxWidth)
					{
						if (wrap == HUDWordWrap.WrapByCharacter)
						{
							break; // break exactly here
						}
						else if (wrap == HUDWordWrap.WrapByWordTrusted)
						{
							for (int i = line.Length - 1; i > 0; i--) // find last breakable
							{
								if (line[i] == ' ' || line[i] == '\t')
								{
									remText = line.Substring(i + 1) + remText;
									line = line.Substring(0, i);
									break;
								}
							}

							break; // break at character
						}
						else if (wrap == HUDWordWrap.WrapByWordWithOverflow)
						{
							for (int i = line.Length-1; i > 0; i--) // find last breakable
							{
								if (line[i] == ' ' || line[i] == '\t')
								{
									remText = line.Substring(i + 1) + remText;
									line = line.Substring(0, i - 1);
									break;
								}
							}

							while (remText.Length > 0 && (remText[0] != ' ' && remText[0] != '\t')) // break at next breakable
							{
								line += remText[0];
								remText = remText.Substring(1);
							}
							break;
						}
						else throw new ArgumentException("wrap");
					}
					else
					{
						line += chr;
						remText = remText.Substring(1);
					}
				}
				lines.Add(line.Trim());

				while (remText.Length > 0 && (remText[0] == ' ' || remText[0] == '\t'))
				{
					remText = remText.Substring(1);
				}
			}

			return lines;
		}
	}
}
