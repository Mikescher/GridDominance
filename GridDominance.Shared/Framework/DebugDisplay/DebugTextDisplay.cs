using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace GridDominance.Shared.Framework.DebugDisplay
{
	class DebugTextDisplay : IDebugTextDisplay
	{
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

		public void Update(GameTime gameTime, InputState istate)
		{
			for (int i = lines.Count - 1; i >= 0; i--)
			{
				lines[i].Update(gameTime, istate);

				if (! lines[i].IsAlive) lines.RemoveAt(i);
			}
		}

		public void Draw()
		{
			debugBatch.Begin(blendState: BlendState.NonPremultiplied);

			float posY = 5;
			foreach (var line in lines.Where(p => p.Active()))
			{
				var text = line.DisplayText();

				var pos = new Vector2(5, posY);
				var size = Textures.DebugFont.MeasureString(text);

				debugBatch.FillRectangle(new RectangleF(pos, size), background);
				debugBatch.DrawString(Textures.DebugFont, text, new Vector2(5, posY), line.Color);

				posY += size.Y * 1.2f;
			}

			debugBatch.End();
		}
	}
}
