using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.DebugDisplay
{
	public class DebugTextDisplay : IDebugTextDisplay, ISAMUpdateable
	{
		public const int OVERFLOW_MAX = 32;

		public const float INERTIA_SPEED = 256f;
		public const float TEXT_OFFSET = 5;
		public const float TEXT_SPACING = 1.15f;

		private readonly List<DebugTextDisplayLine> lines = new List<DebugTextDisplayLine>();

		private readonly SpriteBatch debugBatch;

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

			return AddLine(new DebugTextDisplayLine(() => text).SetLifetime(lifetime).SetDecaytime(decaytime).SetSpawntime(spawntime).SetBackground(Color.Red));
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			bool hasFirst = false;
			for (int i = 0; i < lines.Count; i++)
			{
				if (hasFirst)
				{
					lines[i].UpdateDecay(gameTime, false);
				}
				else
				{
					if (lines[i].IsDecaying)
					{
						lines[i].UpdateDecay(gameTime, true);
						hasFirst = true;
					}
					else
					{
						lines[i].UpdateDecay(gameTime, false);
					}
				}

				if (!lines[i].IsAlive) lines.RemoveAt(i);
			}

			for (int i = lines.Count - 1; i >= 0; i--)
			{
				if (!lines[i].IsAlive) lines.RemoveAt(i);
			}

			float posY = TEXT_OFFSET;
			foreach (var line in lines.Where(p => p.Active()))
			{
				line.UpdatePosition(gameTime, font, lines.Count, ref posY);
			}
		}

		public void Draw()
		{
			debugBatch.Begin(blendState: BlendState.NonPremultiplied);
			
			foreach (var line in lines.Where(p => p.Active()))
			{
				var text = line.DisplayText();

				var pos = new Vector2(TEXT_OFFSET, line.PositionY);
				var size = font.MeasureString(text);

				debugBatch.FillRectangle(new RectangleF(pos, size), BlendColor(line.Background, line.Decay * backgroundAlpha));
				debugBatch.DrawString(font, text, new Vector2(5, pos.Y), BlendColor(line.Color, line.Decay));
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
