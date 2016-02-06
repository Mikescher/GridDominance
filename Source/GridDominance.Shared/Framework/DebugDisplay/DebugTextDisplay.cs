using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace GridDominance.Shared.Framework.DebugDisplay
{
	class DebugTextDisplay : IDebugTextDisplay
	{
		private const float INERTIA_SPEED = 256f;
		private const float TEXT_OFFSET = 5;
		private const float TEXT_SPACING = 1.15f;

		private List<DebugTextDisplayLine> lines = new List<DebugTextDisplayLine>();

		private SpriteBatch debugBatch;

		private Color background = new Color(Color.White, 0.666f);

		public DebugTextDisplay(GraphicsDevice graphics)
		{
			debugBatch = new SpriteBatch(graphics);
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
			return AddLine(new DebugTextDisplayLine(() => text).SetLifetime(lifetime).SetDecaytime(decaytime).SetSpawntime(spawntime));
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
					var speed = gameTime.GetElapsedSeconds() * INERTIA_SPEED * FloatMath.Max(1, FloatMath.Round((line.inertiaPosition - posY) / Textures.DebugFont.LineSpacing));

					line.inertiaPosition = FloatMath.LimitedDec(line.inertiaPosition, speed, posY);
					posY = line.inertiaPosition;
				}
				else if (posY > line.inertiaPosition)
				{
					// should never happen ^^
					line.inertiaPosition = posY;
				}

				var pos = new Vector2(TEXT_OFFSET, posY);
				var size = Textures.DebugFont.MeasureString(text);

				debugBatch.FillRectangle(new RectangleF(pos, size), BlendColor(background, line.Decay));
				debugBatch.DrawString(Textures.DebugFont, text, new Vector2(5, posY), BlendColor(line.Color, line.Decay));

				posY += size.Y * TEXT_SPACING;
			}

			debugBatch.End();
		}

		private Color BlendColor(Color c, float a)
		{
			if (a < 1)
			{
				return new Color(c.R, c.G, c.B, (c.A / 255f) * a);
			}
			else
			{
				return c;
			}
		}
	}
}
