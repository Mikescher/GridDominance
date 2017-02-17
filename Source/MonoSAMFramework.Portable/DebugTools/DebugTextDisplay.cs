using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class DebugTextDisplay : IDebugTextDisplay, ISAMUpdateable
	{
		public const int OVERFLOW_MAX = 32;

		public const float INERTIA_SPEED = 256f;
		public const float TEXT_OFFSET = 5;
		public const float TEXT_SPACING = 1.15f;

		private readonly ConcurrentQueue<DebugTextDisplayLine> asyncBacklog = new ConcurrentQueue<DebugTextDisplayLine>();
		private readonly List<IDebugTextDisplayLineProvider> lines = new List<IDebugTextDisplayLineProvider>();

		private readonly IBatchRenderer debugBatch;

#if DEBUG
		public int LastRenderSpriteCount => debugBatch.LastDebugRenderSpriteCount + debugBatch.LastReleaseRenderSpriteCount;
		public int LastRenderTextCount => debugBatch.LastDebugRenderTextCount + debugBatch.LastReleaseRenderTextCount;
#endif

		private float backgroundAlpha = 0.666f;

		private readonly SpriteFont font;

		public DebugTextDisplay(GraphicsDevice graphics, SpriteFont renderFont)
		{
			debugBatch = new StandardSpriteBatchWrapper(new SpriteBatch(graphics));
			font = renderFont;
		}

		public bool IsEnabled { get; set; }
		public float Scale { get; set; } = 1f;

		public DebugTextDisplayLine AddLine(DebugTextDisplayLine l)
		{
			lines.Add(l);

			return l;
		}

		public DebugTextDisplayLine AddLine(Func<string> text)
		{
			return AddLine(new DebugTextDisplayLine(text));
		}

		public DebugTextDisplayLine AddLine(Func<string> text, Color background, Color foreground)
		{
			var l = new DebugTextDisplayLine(text);
			l.SetColor(foreground);
			l.SetBackground(background);
			return AddLine(l);
		}

		public DebugTextDisplayLine AddLine(string text, Color background, Color foreground)
		{
			var l = new DebugTextDisplayLine(() => text);
			l.SetColor(foreground);
			l.SetBackground(background);
			return AddLine(l);
		}

		public DebugTextDisplayLine AddLine(string debugSettingsKey, Func<string> text)
		{
			return AddLine(new DebugTextDisplayLine(text, () => DebugSettings.Get(debugSettingsKey)));
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

		public void AddLogLines()
		{
			lines.Add(new DebugTextLogLine());
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			DebugTextDisplayLine newLine;
			while (asyncBacklog.TryDequeue(out newLine)) lines.Add(newLine);

			foreach (var line in lines)
			{
				line.Update();
			}

			bool hasFirst = false;
			foreach (var line in lines.SelectMany(p => p.GetLines()))
			{
				if (hasFirst)
				{
					line.UpdateDecay(gameTime, false);
				}
				else
				{
					if (line.IsDecaying)
					{
						line.UpdateDecay(gameTime, true);
						hasFirst = true;
					}
					else
					{
						line.UpdateDecay(gameTime, false);
					}
				}
			}
			
			for (int i = lines.Count - 1; i >= 0; i--)
			{
				if (!lines[i].RemoveZombies()) lines.RemoveAt(i);
			}

			float posY = TEXT_OFFSET;
			foreach (var line in lines.SelectMany(p => p.GetLines()).Where(p => p.Active()))
			{
				line.UpdatePosition(gameTime, font, lines.Count, ref posY);
			}
		}

		public void Draw()
		{
			if (!IsEnabled) return;

			debugBatch.Begin(1f, blendState: BlendState.NonPremultiplied); //scale=1f is ok because we use no textures
			
			foreach (var line in lines.SelectMany(p => p.GetLines()).Where(p => p.Active()))
			{
				var text = line.DisplayText();

				var pos = new Vector2(TEXT_OFFSET * Scale, line.PositionY * Scale);
				var size = font.MeasureString(text) * Scale;

				var bg = line.Background;
				if (bg.A == 255) bg = ColorMath.Fade(line.Background, line.Decay * backgroundAlpha);

				debugBatch.FillRectangle(
					new FRectangle(pos.X - TEXT_OFFSET * Scale, pos.Y, size.X + 2 * TEXT_OFFSET * Scale, size.Y), 
					bg);

				debugBatch.DrawString(
					font, 
					text, 
					new Vector2(5, pos.Y), 
					ColorMath.Fade(line.Color, line.Decay), 
					0, 
					Vector2.Zero, 
					Scale, 
					SpriteEffects.None, 
					0);
			}

			debugBatch.End();
		}

		public DebugTextDisplayLine AddLineFromAsync(Func<string> text)
		{
			return AddLineFromAsync(new DebugTextDisplayLine(text));
		}

		public DebugTextDisplayLine AddLineFromAsync(Func<string> text, Color background, Color foreground)
		{
			var l = new DebugTextDisplayLine(text);
			l.SetColor(foreground);
			l.SetBackground(background);
			return AddLineFromAsync(l);
		}

		public DebugTextDisplayLine AddLineFromAsync(string text, Color background, Color foreground)
		{
			var l = new DebugTextDisplayLine(() => text);
			l.SetColor(foreground);
			l.SetBackground(background);
			return AddLineFromAsync(l);
		}

		public DebugTextDisplayLine AddLineFromAsync(DebugTextDisplayLine l)
		{
			asyncBacklog.Enqueue(l);
			return l;
		}
	}
}
