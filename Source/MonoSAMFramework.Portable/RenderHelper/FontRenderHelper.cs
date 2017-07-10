using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FontRenderHelper
	{
		private const int MEASURE_CACHE_SIZE = 128;

		private static readonly Dictionary<SpriteFont, float> _fontHeight = new Dictionary<SpriteFont, float>();
		private static readonly Dictionary<SpriteFont, float> _fontVCenterOffsetCache = new Dictionary<SpriteFont, float>(); 
		private static readonly Dictionary<SpriteFont, CacheCollection<string, FSize>> _measureCache = new Dictionary<SpriteFont, CacheCollection<string, FSize>>();

		public static float GetFontScale(SpriteFont fnt, float targetSize)
		{
			float y;
			if (!_fontHeight.TryGetValue(fnt, out y)) _fontHeight[fnt] = y = fnt.MeasureString("M").Y;

			return targetSize / y;
		}

		public static float GetFontScale(SpriteFont fnt, string text, FSize targetSize)
		{
			var fsz = fnt.MeasureString(text);

			return FloatMath.Min(targetSize.Width / fsz.X, targetSize.Height / fsz.Y);
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

		public static FSize MeasureStringUncached(SpriteFont font, string text, float size)
		{
			return font.MeasureString(text).ToFSize() * GetFontScale(font, size);
		}

		public static FSize MeasureStringCached(SpriteFont font, string text, float size)
		{
			return MeasureStringCached(font, text) * GetFontScale(font, size);
		}

		public static FSize MeasureStringCached(SpriteFont font, string text)
		{
			CacheCollection<string, FSize> cache;
			if (!_measureCache.TryGetValue(font, out cache))
			{
				cache = new CacheCollection<string, FSize>(MEASURE_CACHE_SIZE);
				var size = font.MeasureString(text).ToFSize();
				cache.Add(text, size);
				return size;
			}
			else
			{
				FSize size;
				if (!cache.TryGetValue(text, out size))
				{
					size = font.MeasureString(text).ToFSize();
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
			var len = MeasureStringCached(font, text, size).Width;
			while (len > maxlen && text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
				len = MeasureStringCached(font, text, size).Width;
			}
			return text;
		}

		public static void DrawTextCentered(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, FPoint position)
		{
			if (text == "") return;
			var bounds = MeasureStringCached(font, text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new FPoint(bounds.Width / 2f, bounds.Height / 2f - GetFontVCenterOffset(font)),
				GetFontScale(font, size),
				SpriteEffects.None,
				0);
		}

		public static void DrawTextCenteredWithScale(IBatchRenderer sbatch, SpriteFont font, float scale, string text, Color color, FPoint position, float rotation = 0f)
		{
			if (text == "") return;
			var bounds = MeasureStringCached(font, text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				rotation,
				new FPoint(bounds.Width / 2f, bounds.Height / 2f - GetFontVCenterOffset(font)),
				scale,
				SpriteEffects.None,
				0);
		}

		public static void DrawTextCenteredWithBackground(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, FPoint position, Color background)
		{
			if (text == "") return;
			var bounds = MeasureStringCached(font, text);
			var scale = GetFontScale(font, size);

			sbatch.FillRectangle(FRectangle.CreateByCenter(position, scale * bounds.Width + size/3f, scale * bounds.Height), background);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new FPoint(bounds.Width / 2f, bounds.Height / 2f - GetFontVCenterOffset(font)), 
				scale,
				SpriteEffects.None,
				0);
		}

		public static void DrawTextVerticallyCentered(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, FPoint position)
		{
			if (text == "") return;

			var bounds = MeasureStringCached(font, text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new FPoint(0, bounds.Height / 2f - GetFontVCenterOffset(font)),
				GetFontScale(font, size),
				SpriteEffects.None,
				0);
		}

		public static void DrawTextVerticallyCenteredWithBackground(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, FPoint position, Color background)
		{
			if (text == "") return;

			var bounds = MeasureStringCached(font, text);
			var scale = GetFontScale(font, size);

			sbatch.FillRectangle(new FRectangle(position.X - size / 6f, position.Y - scale*bounds.Height / 2f, bounds.Width * scale + size / 3f, bounds.Height * scale), background);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new FPoint(0, bounds.Height / 2f - GetFontVCenterOffset(font)),
				scale,
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
				new FPoint(bounds.Width, 0),
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
				DrawTextCentered(sbatch, font, size, text, color, rect.Center);
			else
				DrawTextVerticallyCentered(sbatch, font, size, text, color, new FPoint(rect.X + padding, rect.CenterY));
		}

		public static List<string> WrapLinesIntoWidth(string text, SpriteFont font, float fontSize, float maxWidth, HUDWordWrap wrap)
		{
			if (wrap == HUDWordWrap.NoWrap) return new List<string>{text};

			if (wrap == HUDWordWrap.CropText) return CropTextToLength(text, font, fontSize, maxWidth, "");
			if (wrap == HUDWordWrap.Ellipsis) return CropTextToLength(text, font, fontSize, maxWidth, "...");

			var sz = MeasureStringCached(font, text, fontSize);

			if (sz.Width < maxWidth) return new List<string> { text };


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

					var newlen = MeasureStringUncached(font, line + chr, fontSize).Width;

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

				while (remText.Length > 0 && (remText[0] == ' ' || remText[0] == '\t' || remText[0] == '\r' || remText[0] == '\n'))
				{
					remText = remText.Substring(1);
				}
			}

			return lines;
		}

		private static List<string> CropTextToLength(string text, SpriteFont font, float fontSize, float maxWidth, string ellipsis)
		{
			return Regex.Split(text, @"\r?\n").Select(l => CropLineToLength(l, font, fontSize, maxWidth, ellipsis)).ToList();
		}

		private static string CropLineToLength(string text, SpriteFont font, float fontSize, float maxWidth, string ellipsis)
		{
			var sz = MeasureStringCached(font, text, fontSize);

			if (sz.Width < maxWidth) return text;

			for (int i = 2; i < text.Length; i++)
			{
				var newlen = MeasureStringUncached(font, text.Substring(0, i)+ellipsis, fontSize).Width;

				if (newlen > maxWidth) return text.Substring(0, i - 1).TrimEnd() + ellipsis;
			}

			return text;
		}

		public static string MakeTextSafe(SpriteFont font, string s, char c)
		{
			char[] cc = new char[s.Length];
			for (int i = 0; i < s.Length; i++)
			{
				if (!font.Characters.Contains(s[i]) && s[i] != 0x0A && s[i] != 0x0D) cc[i] = c; else cc[i] = s[i];
			}
			return new string(cc);
		}

		public static string MakeTextSafeWithWarn(SpriteFont font, string s, char c)
		{
			char[] cc = new char[s.Length];
			for (int i = 0; i < s.Length; i++)
			{
				if (!font.Characters.Contains(s[i]) && s[i] != 0x0A && s[i] != 0x0D)
				{
					SAMLog.Warning("FRH::MC", $"Cant render char with font: 0x{((int)s[i]):X2}");
					cc[i] = c;
				}
				else
				{
					cc[i] = s[i];
				}
			}
			return new string(cc);
		}
	}
}
