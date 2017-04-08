using GridDominance.Graphfileformat.Parser;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace GraphEditor
{
	public class PreviewPainter
	{
		public Bitmap Draw(WorldGraphFile wgraph)
		{
			int minX;
			int minY;
			int maxX;
			int maxY;

			if (wgraph == null || wgraph.Nodes.Count == 0)
			{
				Bitmap gb = new Bitmap(1024, 640);
				using (Graphics g = Graphics.FromImage(gb))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.OrangeRed);

					g.DrawLine(new Pen(Color.DarkRed, 32), 0, 0, 1024, 640);
					g.DrawLine(new Pen(Color.DarkRed, 32), 1024, 0, 0, 640);
				}
				return gb;
			}
			else if (wgraph.Nodes.Count == 0)
			{
				Bitmap gb = new Bitmap(1024, 640);
				using (Graphics g = Graphics.FromImage(gb))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.LightSkyBlue);
				}
				return gb;
			}
			else
			{
				minX = (int)wgraph.Nodes.Min(n => n.X) - 250;
				minY = (int)wgraph.Nodes.Min(n => n.Y) - 250;
				maxX = (int)wgraph.Nodes.Max(n => n.X) + 250;
				maxY = (int)wgraph.Nodes.Max(n => n.Y) + 250;
			}
			
			Bitmap buffer = new Bitmap(maxX - minX, maxY - minY);

			using (Graphics g = Graphics.FromImage(buffer))
			{
				g.TranslateTransform(-minX, -minY);

				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.Clear(Color.Black);

				for (int x = minX; x < maxX; x++)
				{
					if ((x + 1000 * 64) % 64 != 0) continue;
					g.DrawLine((x % 2 == 0) ? new Pen(Color.DarkGray, 1) : new Pen(Color.DimGray, 1), x, minY, x, maxY);
				}
				for (int y = minY; y < maxY; y++)
				{
					if ((y + 1000 * 64) % 64 != 0) continue;
					g.DrawLine((y % 2 == 0) ? new Pen(Color.DarkGray, 1) : new Pen(Color.FromArgb(88, 88, 88), 1), minX, y, maxX, y);
				}

				var sbNode = new SolidBrush(Color.FromArgb(127, 140, 141));
				var penExtender = new Pen(Color.FromArgb(231, 76, 60));
				var sbExtender = new SolidBrush(Color.FromArgb(64, 231, 76, 60));

				var diam = 2.75f * 64;
				var exw = 1.7f * 64;

				foreach (var n in wgraph.Nodes)
				{
					foreach (var p in n.OutgoingPipes)
					{
						var o = wgraph.Nodes.Single(nd => nd.LevelID == p.Target);

						ManhattanLine(g, n.X, n.Y, o.X, o.Y, p.PipeOrientation);
					}
				}


				foreach (var n in wgraph.Nodes)
				{
					g.FillRectangle(sbExtender, n.X - diam / 2f - exw, n.Y - diam / 2f, diam + 2 * exw, diam);
					g.FillRectangle(sbExtender, n.X - diam / 2f, n.Y - diam / 2f - exw, diam, diam + 2 * exw);

					g.FillEllipse(sbNode, n.X - diam / 2f, n.Y - diam / 2f, diam, diam);

					g.DrawRectangle(penExtender, n.X - diam / 2f - exw, n.Y - diam / 2f, diam + 2 * exw, diam);
					g.DrawRectangle(penExtender, n.X - diam / 2f, n.Y - diam / 2f - exw, diam, diam + 2 * exw);
					
					g.DrawString(n.LevelID.ToString("D").Replace("-", "\r\n"), new Font("Courier New", 12), new SolidBrush(Color.Chartreuse), new RectangleF(n.X - diam / 2f, n.Y - diam / 2f, diam, diam), new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
				}
			}

			return buffer;
		}

		private void ManhattanLine(Graphics g, float x1, float y1, float x2, float y2, WGPipe.Orientation o)
		{
			var dx = x2 - x1;
			var dy = y2 - y1;

			var p = new Pen(Color.SeaGreen, 24);

			if (dx < 0 && dy < 0)
			{
				if (o == WGPipe.Orientation.Auto || o == WGPipe.Orientation.Clockwise)
				{
					g.DrawLine(p, x1, y1, x2, y1); //H
					g.DrawLine(p, x2, y1, x2, y2); //V
					return;
				}
				else
				{
					g.DrawLine(p, x1, y1, x1, y2); //V
					g.DrawLine(p, x1, y2, x2, y2); //H
					return;
				}
			}
			else if (dx >= 0 && dy < 0)
			{
				if (o == WGPipe.Orientation.Auto || o == WGPipe.Orientation.Counterclockwise)
				{
					g.DrawLine(p, x1, y1, x2, y1); //H
					g.DrawLine(p, x2, y1, x2, y2); //V
					return;
				}
				else
				{
					g.DrawLine(p, x1, y1, x1, y2); //V
					g.DrawLine(p, x1, y2, x2, y2); //H
					return;
				}
			}
			else if (dx < 0 && dy >= 0)
			{
				if (o == WGPipe.Orientation.Auto || o == WGPipe.Orientation.Clockwise)
				{
					g.DrawLine(p, x1, y1, x2, y1); //H
					g.DrawLine(p, x2, y1, x2, y2); //V
					return;
				}
				else
				{
					g.DrawLine(p, x1, y1, x1, y2); //V
					g.DrawLine(p, x1, y2, x2, y2); //H
					return;
				}
			}
			else if (dx >= 0 && dy >= 0)
			{
				if (o == WGPipe.Orientation.Auto || o == WGPipe.Orientation.Counterclockwise)
				{
					g.DrawLine(p, x1, y1, x2, y1); //H
					g.DrawLine(p, x2, y1, x2, y2); //V
					return;
				}
				else
				{
					g.DrawLine(p, x1, y1, x1, y2); //V
					g.DrawLine(p, x1, y2, x2, y2); //H
					return;
				}
			}

			throw new ArgumentException();
		}
	}
}
