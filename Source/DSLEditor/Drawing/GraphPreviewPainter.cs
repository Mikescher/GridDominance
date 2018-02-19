using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using GridDominance.DSLEditor.Helper;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace GridDominance.DSLEditor.Drawing
{
	public class GraphPreviewPainter
	{
		private readonly ConcurrentDictionary<string, Tuple<byte[], LevelBlueprint>> _levelCache = new ConcurrentDictionary<string, Tuple<byte[], LevelBlueprint>>();

		public Bitmap Draw(GraphBlueprint wgraph, string path, Action<string> logwrite, Image last)
		{
			var idmap = MapLevels(path, logwrite);

			int minX;
			int minY;
			int maxX;
			int maxY;

			if (wgraph == null || wgraph.LevelNodes.Count == 0)
			{
				Bitmap gb = new Bitmap(1024, 640);
				if (last != null) gb = new Bitmap(last.Width, last.Height);
				
				using (Graphics g = Graphics.FromImage(gb))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;

					if (last == null)
					{
						g.Clear(Color.OrangeRed);
						g.DrawLine(new Pen(Color.DarkRed, 32), 0, 0, gb.Width, gb.Height);
						g.DrawLine(new Pen(Color.DarkRed, 32), gb.Width, 0, 0, gb.Height);
					}
					else
					{
						g.DrawImageUnscaled(last, 0, 0);
						g.FillRectangle(new SolidBrush(Color.FromArgb(32, Color.OrangeRed)), 0, 0, gb.Width, gb.Height);

						g.DrawLine(new Pen(Color.FromArgb(64, Color.DarkRed), 32), 0, 0, gb.Width, gb.Height);
						g.DrawLine(new Pen(Color.FromArgb(64, Color.DarkRed), 32), gb.Width, 0, 0, gb.Height);
					}

				}
				return gb;
			}
			else if (wgraph.LevelNodes.Count == 0)
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
				minX = (int)wgraph.AllNodes.Min(n => n.X) - 250;
				minY = (int)wgraph.AllNodes.Min(n => n.Y) - 250;
				maxX = (int)wgraph.AllNodes.Max(n => n.X) + 250;
				maxY = (int)wgraph.AllNodes.Max(n => n.Y) + 250;
			}
			
			Bitmap buffer = new Bitmap(maxX - minX, maxY - minY);

			using (Graphics g = Graphics.FromImage(buffer))
			{
				g.TranslateTransform(-minX, -minY);

				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.Clear(Color.Black);

				DrawGrid(minX, maxX, g, minY, maxY);

				DrawPipes(wgraph, g);
				
				DrawNodes(wgraph, g, idmap);
				DrawRootNode(wgraph, g);
				DrawWarpNodes(wgraph, g);

				DrawPriorityMarker(wgraph, g);
			}

			return buffer;
		}

		private static void DrawRootNode(GraphBlueprint wgraph, Graphics g)
		{
			var sbNode = new SolidBrush(Color.FromArgb(127, 140, 141));
			var diamRoot = 3f * 64;

			g.FillRectangle(sbNode, wgraph.RootNode.X - diamRoot / 2f, wgraph.RootNode.Y - diamRoot / 2f, diamRoot, diamRoot);
		}

		private void DrawPriorityMarker(GraphBlueprint wgraph, Graphics g)
		{
			var sbMarker = new SolidBrush(Color.FromArgb(255, 255, 255));

			var diam = 2.75f * 64;
			var diamRoot = 3f * 64;

			foreach (var n in wgraph.AllNodes)
			{
				if (n.Pipes.Count > 1 && n.Pipes.Select(p => p.Priority).Distinct().Count() > 1)
					foreach (var p in n.Pipes)
					{
						var o = wgraph.AllNodes.Single(nd => nd.ConnectionID == p.Target);

						var start = new Vector2(n.X, n.Y);
						var end = new Vector2(o.X, o.Y);
						var delta = end - start;
						delta.Normalize();

						var thisdia = n is RootNodeBlueprint ? (float) Math.Sqrt(2 * diamRoot * diamRoot) : diam;

						var marker = start + delta * (thisdia / 2f + 8);

						g.FillEllipse(sbMarker, marker.X - 16, marker.Y - 16, 32, 32);
						DrawFit(g, p.Priority.ToString(), Color.Black, new Font("Arial", 24), new RectangleF(marker.X - 16, marker.Y - 16, 32, 32));
					}
			}
		}

		private void DrawNodes(GraphBlueprint wgraph, Graphics g, Dictionary<Guid, LevelBlueprint> idmap)
		{
			var sbNode = new SolidBrush(Color.FromArgb(127, 140, 141));
			var penExtender = new Pen(Color.FromArgb(231, 76, 60));
			var sbExtender = new SolidBrush(Color.FromArgb(64, 231, 76, 60));

			var diam = 2.75f * 64;
			var exw = 1.7f * 64;

			foreach (var n in wgraph.LevelNodes)
			{
				g.FillRectangle(sbExtender, n.X - diam / 2f - exw, n.Y - diam / 2f, diam + 2 * exw, diam);
				g.FillRectangle(sbExtender, n.X - diam / 2f, n.Y - diam / 2f - exw, diam, diam + 2 * exw);

				g.FillEllipse(sbNode, n.X - diam / 2f, n.Y - diam / 2f, diam, diam);

				g.DrawRectangle(penExtender, n.X - diam / 2f - exw, n.Y - diam / 2f, diam + 2 * exw, diam);
				g.DrawRectangle(penExtender, n.X - diam / 2f, n.Y - diam / 2f - exw, diam, diam + 2 * exw);

				var pl = BlueprintAnalyzer.ListPlayer(idmap[n.LevelID]).ToList();
				var al = (2 * Math.PI) / pl.Count;
				var ap = Math.PI-al/2;

				var R2D = (float) (180 / Math.PI);

				foreach (var p in pl)
				{
					g.DrawArc(new Pen(LevelPreviewPainter.CANNON_COLORS[p], 12), n.X - diam / 2f, n.Y - diam / 2f, diam, diam, R2D * (float)ap, R2D * (float)al);
					ap += al;
				}

				if (idmap.ContainsKey(n.LevelID))
				{
					DrawFit(g, idmap[n.LevelID].Name, Color.Black, new Font("Courier New", 24, FontStyle.Bold), new RectangleF(n.X - diam / 2f, n.Y - diam / 2f, diam, diam));
				}
				else
				{
					DrawFit(g, n.LevelID.ToString("D").Replace("-", "\r\n"), Color.Black, new Font("Courier New", 24, FontStyle.Bold), new RectangleF(n.X - diam / 2f, n.Y - diam / 2f, diam, diam));
				}
			}
		}

		private static void DrawWarpNodes(GraphBlueprint wgraph, Graphics g)
		{
			var sbNode = new SolidBrush(Color.FromArgb(127, 140, 141));
			var pen = new Pen(Color.Black, 4);
			var diamRoot = 3f * 64;

			foreach (var node in wgraph.WarpNodes)
			{
				g.FillRectangle(sbNode, node.X - diamRoot / 2f, node.Y - diamRoot / 2f, diamRoot, diamRoot);
				g.DrawEllipse(pen, node.X - diamRoot / 2f, node.Y - diamRoot / 2f, diamRoot, diamRoot);
			}
		}

		private void DrawPipes(GraphBlueprint wgraph, Graphics g)
		{
			foreach (var n in wgraph.AllNodes)
			{
				foreach (var p in n.Pipes)
				{
					var o = wgraph.AllNodes.Single(nd => nd.ConnectionID == p.Target);

					ManhattanLine(g, n.X, n.Y, o.X, o.Y, p.PipeOrientation);
				}
			}
		}

		private static void DrawGrid(int minX, int maxX, Graphics g, int minY, int maxY)
		{
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
		}

		private Dictionary<Guid, LevelBlueprint> MapLevels(string path, Action<string> logwrite)
		{
			path = Path.GetDirectoryName(path);

			if (!Directory.Exists(path)) return new Dictionary<Guid, LevelBlueprint>();

			var d = new Dictionary<Guid, LevelBlueprint>();

			foreach (var f in Directory.EnumerateFiles(path).Where(p => p.ToLower().EndsWith(".gslevel")))
			{
				try
				{
					var dat = File.ReadAllBytes(f);

					Tuple<byte[], LevelBlueprint> cache;
					if (_levelCache.TryGetValue(f, out cache))
					{
						if (cache.Item1.SequenceEqual(dat))
						{
							d[cache.Item2.UniqueID] = cache.Item2;
							continue;
						}
					}

					logwrite("Scan level: " + Path.GetFileName(f));
					var lf = DSLUtil.ParseLevelFromFile(f, false);

					d[lf.UniqueID] = lf;

					_levelCache.AddOrUpdate(f, Tuple.Create(dat, lf), (a, b) => Tuple.Create(dat, lf));
				}
				catch (Exception)
				{
					//
				}
			}
			return d;
		}

		private void ManhattanLine(Graphics g, float x1, float y1, float x2, float y2, PipeBlueprint.Orientation o)
		{
			var dx = x2 - x1;
			var dy = y2 - y1;

			var p = new Pen(Color.SeaGreen, 24);

			if (o == PipeBlueprint.Orientation.Direct)
			{
				g.DrawLine(p, x1, y1, x2, y2); //D
				return;
			}

			if (dx < 0 && dy < 0)
			{
				if (o == PipeBlueprint.Orientation.Auto || o == PipeBlueprint.Orientation.Clockwise)
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

			if (dx >= 0 && dy < 0)
			{
				if (o == PipeBlueprint.Orientation.Auto || o == PipeBlueprint.Orientation.Counterclockwise)
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

			if (dx < 0 && dy >= 0)
			{
				if (o == PipeBlueprint.Orientation.Auto || o == PipeBlueprint.Orientation.Counterclockwise)
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

			if (dx >= 0 && dy >= 0)
			{
				if (o == PipeBlueprint.Orientation.Auto || o == PipeBlueprint.Orientation.Clockwise)
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
