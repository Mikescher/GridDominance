using GridDominance.Levelfileformat.Parser;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GridDominance.DSLEditor.Drawing
{
	public class LevelPreviewPainter
	{
		private static readonly Color[] CANNON_COLORS = new Color[] { Color.LightGray, Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Cyan, Color.Orange, Color.Pink };

		public readonly Bitmap GraphicsBuffer = new Bitmap(1024, 640);

		public Bitmap DrawOverview(LevelFile level)
		{
			Bitmap img;

			try
			{
				img = Draw(level);
			}
			catch (Exception)
			{
				img = Draw(null);
			}

			Bitmap r = new Bitmap(img.Width, img.Height + 48);
			using (Graphics g = Graphics.FromImage(r))
			{
				g.Clear(Color.White);
				g.DrawString(level.Name + ": " + level.FullName, new Font("Calibri", 28, FontStyle.Bold), Brushes.DarkRed, 24, 8);
				g.DrawImageUnscaled(img, 0, 48);
			}
			return r;
		}

		public Bitmap Draw(LevelFile level)
		{
			using (Graphics g = Graphics.FromImage(GraphicsBuffer))
			{
				if (level == null)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.OrangeRed);

					g.DrawLine(new Pen(Color.DarkRed, 32), 0, 0, 1024, 640);
					g.DrawLine(new Pen(Color.DarkRed, 32), 1024, 0, 0, 640);
				}
				else
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.Black);

					for (int x = 0; x < 16; x++)
					{
						g.DrawLine((x % 2 == 0) ? Pens.DarkGray : Pens.DimGray, x * 64, 0, x * 64, 640);
					}
					for (int y = 0; y < 10; y++)
					{
						g.DrawLine((y % 2 == 0) ? Pens.DarkGray : new Pen(Color.FromArgb(88, 88, 88)), 0, y * 64, 1024, y * 64);
					}

					foreach (var c in level.BlueprintCannons)
					{
						var topleftX = (int)(c.X - c.Scale * 64);
						var topleftY = (int)(c.Y - c.Scale * 64);
						var width = (int)(c.Scale * 128);
						var height = (int)(c.Scale * 128);

						var rectReal = new Rectangle(topleftX, topleftY, width, height);
						var rectCircle = new Rectangle(rectReal.Location, rectReal.Size);
						var rectOuter = new Rectangle(rectReal.Location, rectReal.Size);
						rectCircle.Inflate((width * 48 / 64 - width) / 2, (height * 48 / 64 - height) / 2);
						rectOuter.Inflate((width * 80 / 64 - width) / 2, (height * 80 / 64 - height) / 2);

						var save = g.Save();
						{
							g.TranslateTransform(c.X, c.Y);
							g.RotateTransform(c.Rotation);

							var bHeight = height / 4;
							var bWidth = width / 2;
							var bPosX = width / 8;

							var br = new Rectangle(bPosX, -bHeight / 2, bWidth, bHeight);

							g.FillRectangle(new SolidBrush(CANNON_COLORS[c.Player]), br);
						}
						g.Restore(save);

						g.FillRectangle(new SolidBrush(Color.FromArgb(64, CANNON_COLORS[c.Player])), rectReal);
						g.FillEllipse(new SolidBrush(CANNON_COLORS[c.Player]), rectCircle);
						g.DrawEllipse(new Pen(Color.Black), rectCircle);
						g.DrawEllipse(new Pen(CANNON_COLORS[c.Player], 2), rectOuter);
					}

					var voidpen = new Pen(Color.FloralWhite, 8);
					foreach (var vwall in level.BlueprintVoidWalls)
					{
						var save = g.Save();
						g.TranslateTransform(vwall.X, vwall.Y);
						g.RotateTransform(vwall.Rotation);
						g.DrawLine(voidpen, -vwall.Length / 2, 0, +vwall.Length / 2, 0);
						g.Restore(save);
					}
				}
			}

			return GraphicsBuffer;
		}
	}
}
