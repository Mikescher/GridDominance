using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.RenderHelper;
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

		public const float INERTIA_SPEED = 2048f;
		public const float TEXT_OFFSET = 5;
		public const float TEXT_SPACING = 2.15f;

		private readonly ConcurrentQueue<DebugTextDisplayLine> asyncBacklog = new ConcurrentQueue<DebugTextDisplayLine>();
		private List<IDebugTextDisplayLineProvider> lines = new List<IDebugTextDisplayLineProvider>();

		private readonly IBatchRenderer debugBatch;

#if DEBUG
		public int LastRenderSpriteCount => debugBatch.LastDebugRenderSpriteCount + debugBatch.LastReleaseRenderSpriteCount;
		public int LastRenderTextCount => debugBatch.LastDebugRenderTextCount + debugBatch.LastReleaseRenderTextCount;
#endif

		private float backgroundAlpha = 0.666f;

		private readonly SpriteFont font;

		public DebugTextDisplay(GraphicsDevice graphics, SpriteFont renderFont)
		{
			debugBatch = new SpriteBatchWrapper(new SpriteBatch(graphics));
			font = renderFont;
		}

		public bool IsEnabled { get; set; }
		public float Scale { get; set; } = 1f;

		public DebugTextDisplayLine AddLine(DebugTextDisplayLine l)
		{
			lines.Add(l);

			lines = lines.OrderBy(n => n.Order).ToList();
			
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

		public DebugTextDisplayLine AddLine(string debugSettingsKey, Func<string> text, ILifetimeObject owner)
		{
			return AddLine(new DebugTextDisplayLine(text, () => DebugSettings.Get(debugSettingsKey)) {Owner = owner});
		}

		public DebugTextDisplayLine AddTabularLine(string debugSettingsKey, Func<List<string>> texts)
		{
			return AddLine(new DebugTextDisplayLine(texts, () => DebugSettings.Get(debugSettingsKey)));
		}

		public DebugTextDisplayLine AddLine(string text)
		{
			return AddLine(new DebugTextDisplayLine(() => text));
		}

		public DebugTextDisplayLine AddDecayLine(string text, float lifetime = 8f, float decaytime = 0.75f, float spawntime = 0.25f)
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
				var columns = line.DisplayText();
				if (columns.Count == 0 || columns.All(string.IsNullOrWhiteSpace)) continue;

				line.UpdatePosition(gameTime, font, lines.Count, ref posY);
			}
		}

		public void Draw()
		{
			if (!IsEnabled) return;

			debugBatch.Begin(1f, blendState: BlendState.NonPremultiplied); //scale=1f is ok because we use no textures
			
			foreach (var line in lines.SelectMany(p => p.GetLines()).Where(p => p.Active()))
			{
				var columns = line.DisplayText();

				var pos = new FPoint(TEXT_OFFSET * Scale, line.PositionY * Scale);

				if (columns.Count == 0 || columns.All(string.IsNullOrWhiteSpace)) continue;

				foreach (var _text in columns)
				{
					var text = FontRenderHelper.MakeTextSafeWithWarn(font, _text, '_');

					var size = font.MeasureString(text) * Scale;

					var bg = line.Background;
					if (bg.A == 255) bg = ColorMath.Fade(line.Background, line.Decay * backgroundAlpha);

					debugBatch.FillRectangle(
						new FRectangle(pos.X - TEXT_OFFSET * Scale, pos.Y, size.X + 2 * TEXT_OFFSET * Scale, size.Y),
						bg);

					debugBatch.DrawString(
						font,
						text,
						pos,
						ColorMath.Fade(line.Color, line.Decay),
						0,
						FPoint.Zero,
						Scale,
						SpriteEffects.None,
						0);

					pos = new FPoint(pos.X + 3 * TEXT_OFFSET * Scale + size.X, pos.Y);
				}
				
			}

			debugBatch.End();
		}

		public void Clear()
		{
			foreach (var line in lines.SelectMany(p => p.GetLines()))
			{
				if (line.IsDecaying) line.IsAlive = false;
			}
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
