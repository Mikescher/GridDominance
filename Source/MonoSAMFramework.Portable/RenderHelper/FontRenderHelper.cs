using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
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

		public static void DrawTextCentered(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, Vector2 position)
		{
			//TODO How expensive is this calculation each draw call ?
			//     perhaps move everything in some kind of fontrendererCache
			//     which remembers all calculated values until text/size/... changes (like with the HUD)

			var scale = GetFontScale(font, size);

			var bounds = font.MeasureString(text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new Vector2(bounds.X / 2f, bounds.Y / 2f - GetFontVCenterOffset(font)),
				scale,
				SpriteEffects.None,
				0);
		}

		public static void DrawTextVerticallyCentered(IBatchRenderer sbatch, SpriteFont font, float size, string text, Color color, Vector2 position)
		{
			if (text == "") return;

			//TODO How expensive is this calculation each draw call ?
			//     perhaps move everything in some kind of fontrendererCache
			//     which remembers all calculated values until text/size/... changes (like with the HUD)

			var scale = GetFontScale(font, size);

			var bounds = font.MeasureString(text);

			sbatch.DrawString(
				font,
				text,
				position,
				color,
				0,
				new Vector2(0, bounds.Y / 2f - GetFontVCenterOffset(font)),
				scale,
				SpriteEffects.None,
				0);
		}
	}
}
