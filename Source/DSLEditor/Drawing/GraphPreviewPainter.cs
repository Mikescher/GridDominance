using GridDominance.Graphfileformat.Parser;
using GridDominance.Levelfileformat.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace GridDominance.DSLEditor.Drawing
{
	public class GraphPreviewPainter
	{
		public Bitmap Draw(WorldGraphFile wgraph, string path)
		{
			var idmap = MapLevels(path);

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
						if (wgraph.Nodes.Count(nd => nd.LevelID == p.Target) != 1)
							throw new Exception($"Pipe Target {p.Target:B} is not unambiguous");

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

					if (idmap.ContainsKey(n.LevelID))
					{
						DrawFit(g, idmap[n.LevelID], Color.Black, new Font("Courier New", 24, FontStyle.Bold), new RectangleF(n.X - diam / 2f, n.Y - diam / 2f, diam, diam));
					}
					else
					{
						DrawFit(g, n.LevelID.ToString("D").Replace("-", "\r\n"), Color.Black, new Font("Courier New", 24, FontStyle.Bold), new RectangleF(n.X - diam / 2f, n.Y - diam / 2f, diam, diam));
					}
				}
			}

			return buffer;
		}

		private Dictionary<Guid, string> MapLevels(string path)
		{
			path = Path.GetDirectoryName(path);

			if (!Directory.Exists(path)) return new Dictionary<Guid, string>();

			var d = new Dictionary<Guid, string>();

			var includes = Directory.EnumerateFiles(path, "*.gsheader").ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));
			Func<string, string> includesFunc = x => includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value;

			foreach (var f in Directory.EnumerateFiles(path).Where(p => p.ToLower().EndsWith(".gslevel")))
			{
				try
				{
					var fp = new LevelFileParser(File.ReadAllText(f), includesFunc);
					var lf = fp.Parse(Path.GetFileName(f));

					d[lf.UniqueID] = lf.Name;
				}
				catch (Exception)
				{
					//
				}
			}
			return d;
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

		private void DrawFit(Graphics g, string str, Color c, Font f, RectangleF r)
		{
			var font = FindFont(g, str, r.Size.ToSize(), f);

			StringFormat stringFormat = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			g.DrawString(str, font, new SolidBrush(c), r, stringFormat);
		}

		private Font FindFont(Graphics g, string longString, Size room, Font preferedFont)
		{
			SizeF realSize = g.MeasureString(longString, preferedFont);
			float heightScaleRatio = room.Height / realSize.Height;
			float widthScaleRatio = room.Width / realSize.Width;
			float scaleRatio = (heightScaleRatio < widthScaleRatio) ? heightScaleRatio : widthScaleRatio;
			float scaleFontSize = preferedFont.Size * scaleRatio;
			return new Font(preferedFont.FontFamily, scaleFontSize);
		}
	}
}
