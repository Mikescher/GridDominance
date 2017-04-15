using GridDominance.Levelfileformat.Parser;
using MSHC.Math.Geometry;
using System;
using System.Text;

namespace GridDominance.DSLEditor.Helper
{
	public class LevelAsciiDrawer
	{
		private const int MAPWIDTH  = 16 + 17;
		private const int MAPHEIGHT = 10 + 11;

		private LevelFile level;

		private char[,] chrmap;

		public LevelAsciiDrawer(LevelFile f)
		{
			level = f;
		}

		public void Calc()
		{
			chrmap = new char[MAPWIDTH, MAPHEIGHT];
			for (var x = 0; x < MAPWIDTH; x++) for (var y = 0; y < MAPHEIGHT; y++) chrmap[x, y] = ' ';

			foreach (var c in level.BlueprintCannons) DrawCannon(c);
			foreach (var w in level.BlueprintVoidWalls) DrawVoidWall(w);
		}

		private void DrawCannon(LPCannon c)
		{
			var x = c.X / 64;
			var y = c.Y / 64;

			if (c.Player == 0)
			{
				SetMap(x-0.5f, y, '<');
				SetMap(x,      y, 'O');
				SetMap(x+0.5f, y, '>');
			}
			else
			{
				if (c.Scale <= 0.5)
				{
					SetMap(x, y, 'O');
				}
				else
				{
					SetMap(x, y, 'O');

					SetMap(x - 0.5f, y - 0.5f, '/');
					SetMap(x + 0.5f, y - 0.5f, '\\');
					SetMap(x - 0.5f, y + 0.5f, '\\');
					SetMap(x + 0.5f, y + 0.5f, '/');
				}
			}
		}

		private void DrawVoidWall(LPVoidWall w)
		{
			var wx = w.X / 64;
			var wy = w.Y / 64;
			var wl = w.Length / 64;

			var wxs = new Vec2d(wx + wl / 2, wy);
			wxs.RotateAround(new Vec2d(wx, wy), Math.PI * w.Rotation / 180);

			var wxe = new Vec2d(wx - wl / 2, wy);
			wxe.RotateAround(new Vec2d(wx, wy), Math.PI * w.Rotation / 180);

			if ((int)Math.Round(w.Rotation % 180) == 0)
			{
				var sx = wx - wl / 2;
				var ex = wx + wl / 2;
				var y = wy;
				
				var done = false;
				for (var x = sx + 0.5f; x < ex; x += 0.5f)
				{
					SetMap(x, y, '-');
					done = true;
				}
				if (!done) SetMap(sx, y, '-');
			}
			else if ((int)Math.Round(w.Rotation % 180) == 90)
			{
				var x  = wx;
				var sy = wy - wl / 2;
				var ey = wy + wl / 2;

				var done = false;
				for (var y = sy + 0.5f; y < ey; y += 0.5f)
				{
					SetMap(x, y, '|');
					done = true;
				}
				if (!done) SetMap(x, sy, '|');
			}
			else if ((int)Math.Round(w.Rotation % 180) < 90)
			{
				var sx = (float)wxs.X;
				var sy = (float)wxs.Y;

				var ex = (float)(wxe.X);
				var ey = (float)(wxe.Y);

				var dx = (ex - sx) / 32f;
				var dy = (ey - sy) / 32f;

				int ly = -99999;
				int lx = -99999;
				for (int i = 0; i <= 32; i++)
				{
					int ry = (int)Math.Round((sy + i * dy) * 2);
					int rx = (int)Math.Round((sx + i * dx) * 2);

					if (ry != ly && rx != lx)
					{
						SetMap(sx + i * dx, sy + i * dy, '\\');
						ly = ry;
						lx = rx;
					}
				}

				SetMap(sx, sy, '\\');
				SetMap(ex, ey, '\\');
			}
			else
			{
				var sx = (float)wxs.X;
				var sy = (float)wxs.Y;

				var ex = (float)(wxe.X);
				var ey = (float)(wxe.Y);

				var dx = (ex - sx) / 32f;
				var dy = (ey - sy) / 32f;

				int ly = -99999;
				int lx = -99999;
				for (int i = 0; i <= 32; i++)
				{
					int ry = (int)Math.Round((sy + i * dy) * 2);
					int rx = (int)Math.Round((sx + i * dx) * 2);

					if (ry != ly && rx != lx)
					{
						SetMap(sx + i * dx, sy + i * dy, '/');
						ly = ry;
						lx = rx;
					}
				}

				SetMap(sx, sy, '/');
				SetMap(ex, ey, '/');
			}
		}

		private void SetMap(float fx, float fy, char c)
		{
			var x = (int)Math.Round(fx * 2);
			var y = (int)Math.Round(fy * 2);

			if (x < 0) return;
			if (y < 0) return;
			if (x >= MAPWIDTH) return;
			if (y >= MAPHEIGHT) return;

			chrmap[x, y] = c;
		}

		public string Get()
		{
			var builder = new StringBuilder();

			builder.AppendLine("#<map>");
			builder.AppendLine("#");
			builder.AppendLine("#            0 1 2 3 4 5 6 7 8 9 A B C D E F");
			builder.AppendLine("#          # # # # # # # # # # # # # # # # # #");

			for (int y = 0; y < MAPHEIGHT; y++)
			{
				if (y%2==1)
					builder.Append($"#        {y/2} #");
				else
					builder.Append("#           ");

				for (int x = 0; x < MAPWIDTH; x++)
				{
					builder.Append(chrmap[x, y]);
				}

				if (y % 2 == 1) builder.AppendLine("#"); else builder.AppendLine(" ");
			}

			builder.AppendLine("#          # # # # # # # # # # # # # # # # # #");
			builder.AppendLine("#");
			builder.Append("#</map>");

			return builder.ToString();
		}
	}
}
