using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.MathHelper;

namespace MonoSAMFramework.Portable.DebugDisplay
{
#if DEBUG
	public class DebugTextDisplay : IDebugTextDisplay
	{
		private const int OVERFLOW_MAX = 32;

		private const float INERTIA_SPEED = 256f;
		private const float TEXT_OFFSET = 5;
		private const float TEXT_SPACING = 1.15f;

		private List<DebugTextDisplayLine> lines = new List<DebugTextDisplayLine>();

		private SpriteBatch debugBatch;

		private float backgroundAlpha = 0.666f;

		private readonly SpriteFont font;

		public DebugTextDisplay(GraphicsDevice graphics, SpriteFont renderFont)
		{
			debugBatch = new SpriteBatch(graphics);
			font = renderFont;
		}

		public DebugTextDisplayLine AddLine(DebugTextDisplayLine l)
		{
			lines.Add(l);

			return l;
		}

		public DebugTextDisplayLine AddLine(Func<string> text)
		{
			return AddLine(new DebugTextDisplayLine(text));
		}

		public DebugTextDisplayLine AddLine(string text)
		{
			return AddLine(new DebugTextDisplayLine(() => text));
		}

		public DebugTextDisplayLine AddDecayLine(string text, float lifetime = 2f, float decaytime = 0.75f, float spawntime = 0.25f)
		{
			if (lines.Count > OVERFLOW_MAX) decaytime = 0;
			if (lines.Count > OVERFLOW_MAX) spawntime = 0;

			return AddLine(new DebugTextDisplayLine(() => text).SetLifetime(lifetime).SetDecaytime(decaytime).SetSpawntime(spawntime));
		}

		public DebugTextDisplayLine AddErrorDecayLine(string text, float lifetime = 2f, float decaytime = 0.75f, float spawntime = 0.25f)
		{
			if (lines.Count > OVERFLOW_MAX) decaytime = 0;
			if (lines.Count > OVERFLOW_MAX) spawntime = 0;

			return AddLine(new DebugTextDisplayLine(() => text).SetLifetime(lifetime).SetDecaytime(decaytime).SetSpawntime(spawntime).SetBackground(Microsoft.Xna.Framework.Color.Red));
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			for (int i = lines.Count - 1; i >= 0; i--)
			{
				lines[i].Update(gameTime, istate);

				if (!lines[i].IsAlive) lines.RemoveAt(i);
			}
		}

		public void Draw(GameTime gameTime)
		{
			debugBatch.Begin(blendState: BlendState.NonPremultiplied);

			float posY = TEXT_OFFSET;
			foreach (var line in lines.Where(p => p.Active()))
			{
				var text = line.DisplayText();

				if (line.inertiaPosition < 0)
				{
					line.inertiaPosition = posY;
				}
				else if (posY < line.inertiaPosition)
				{
					var speed = gameTime.GetElapsedSeconds() * INERTIA_SPEED * FloatMath.Max(1, FloatMath.Round((line.inertiaPosition - posY) / font.LineSpacing));

					if (lines.Count > OVERFLOW_MAX) speed = 99999;

					line.inertiaPosition = FloatMath.LimitedDec(line.inertiaPosition, speed, posY);
					posY = line.inertiaPosition;
				}
				else if (posY > line.inertiaPosition)
				{
					// should never happen ^^
					line.inertiaPosition = posY;
				}

				var pos = new Vector2(TEXT_OFFSET, posY);
				var size = font.MeasureString(text);

				debugBatch.FillRectangle(new RectangleF(pos, size), BlendColor(line.Background, line.Decay * backgroundAlpha));
				debugBatch.DrawString(font, text, new Vector2(5, posY), BlendColor(line.Color, line.Decay));

				posY += size.Y * TEXT_SPACING;
			}

			debugBatch.End();
		}

		private Microsoft.Xna.Framework.Color BlendColor(Microsoft.Xna.Framework.Color c, float a)
		{
			if (a < 1)
			{
				return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, (c.A / 255f) * a);
			}
			else
			{
				return c;
			}
		}
	}
#endif
}
