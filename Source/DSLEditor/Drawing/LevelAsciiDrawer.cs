using GridDominance.Levelfileformat.Blueprint;
using MSHC.Math.Geometry;
using System;
using System.Linq;
using System.Text;

namespace GridDominance.DSLEditor.Drawing
{
	public class LevelAsciiDrawer
	{
		private readonly int MAPWIDTH_I;
		private readonly int MAPHEIGHT_I;

		private readonly int MAPWIDTH;
		private readonly int MAPHEIGHT;

		private readonly LevelBlueprint level;

		private char[,] chrmap;

		public LevelAsciiDrawer(LevelBlueprint f)
		{
			level = f;

			MAPWIDTH_I  = (int)(f.LevelWidth / 64);
			MAPHEIGHT_I = (int)(f.LevelHeight / 64);

			MAPWIDTH  = MAPWIDTH_I * 2 + 1;
			MAPHEIGHT = MAPHEIGHT_I * 2 + 1;
		}

		public void Calc()
		{
			chrmap = new char[MAPWIDTH, MAPHEIGHT];
			for (var x = 0; x < MAPWIDTH; x++) for (var y = 0; y < MAPHEIGHT; y++) chrmap[x, y] = ' ';

			foreach (var c in level.BlueprintCannons)       DrawCannon(c);
			foreach (var w in level.BlueprintVoidWalls)     DrawVoidWall(w);
			foreach (var c in level.BlueprintVoidCircles)   DrawVoidCircle(c);
			foreach (var c in level.BlueprintGlassBlocks)   DrawGlassBlock(c);
			foreach (var h in level.BlueprintBlackHoles)    DrawBlackHole(h);
			foreach (var p in level.BlueprintPortals)       DrawPortal(p);
			foreach (var b in level.BlueprintMirrorBlocks)  DrawMirrorBlock(b);
			foreach (var c in level.BlueprintMirrorCircles) DrawMirrorCircle(c);
			foreach (var c in level.BlueprintLaserCannons)  DrawLaserCannon(c);
		}

		private void DrawCannon(CannonBlueprint c)
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
				if (c.Diameter <= 48)
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

		private void DrawVoidWall(VoidWallBlueprint w)
		{
			var wx = w.X / 64;
			var wy = w.Y / 64;
			var wl = w.Length / 64;

			DrawLine(wx, wy, wl, w.Rotation, '-', '|', '/', '\\');
		}

		private void DrawVoidCircle(VoidCircleBlueprint c)
		{
			var x = c.X / 64;
			var y = c.Y / 64;

			SetMap(x - 0.5f, y, '(');
			SetMap(x + 0.0f, y, 'X');
			SetMap(x + 0.5f, y, ')');
		}

		private void DrawGlassBlock(GlassBlockBlueprint c)
		{
			var x1 = (c.X - c.Width / 2) / 64;
			var y1 = (c.Y - c.Height / 2) / 64;
			var x2 = (c.X + c.Width / 2) / 64;
			var y2 = (c.Y + c.Height / 2) / 64;

			FillMapRot(x1, y1, x2, y2, c.Rotation, 'H');
		}

		private void DrawBlackHole(BlackHoleBlueprint c)
		{
			var x = c.X / 64;
			var y = c.Y / 64;
			var r = c.Diameter / 2 / 64;

			FillCircle(x, y, r, '.');
			SetMap(x, y, '@');
		}

		private void DrawPortal(PortalBlueprint p)
		{
			var x = p.X / 64;
			var y = p.Y / 64;
			var l = p.Length / 64;
			var r = p.Normal + 90;

			DrawLine(x, y, l, r, '&');
		}

		private void DrawMirrorBlock(MirrorBlockBlueprint c)
		{
			var x1 = (c.X - c.Width / 2) / 64;
			var y1 = (c.Y - c.Height / 2) / 64;
			var x2 = (c.X + c.Width / 2) / 64;
			var y2 = (c.Y + c.Height / 2) / 64;

			FillMapRot(x1, y1, x2, y2, c.Rotation, 'N');
		}

		private void DrawMirrorCircle(MirrorCircleBlueprint c)
		{
			var x = c.X / 64;
			var y = c.Y / 64;

			SetMap(x - 0.5f, y, '(');
			SetMap(x + 0.0f, y, '#');
			SetMap(x + 0.5f, y, ')');
		}

		private void DrawLaserCannon(LaserCannonBlueprint c)
		{
			var x = c.X / 64;
			var y = c.Y / 64;

			if (c.Player == 0)
			{
				SetMap(x - 0.5f, y, '<');
				SetMap(x, y, '+');
				SetMap(x + 0.5f, y, '>');
			}
			else
			{
				if (c.Diameter <= 48)
				{
					SetMap(x, y, '+');
				}
				else
				{
					SetMap(x, y, '+');

					SetMap(x - 0.5f, y - 0.5f, '/');
					SetMap(x + 0.5f, y - 0.5f, '\\');
					SetMap(x - 0.5f, y + 0.5f, '\\');
					SetMap(x + 0.5f, y + 0.5f, '/');
				}
			}
		}

		private void DrawLine(float cx, float cy, float len, double rot, char c)
		{
			DrawLine(cx, cy, len, rot, c, c, c, c);
		}

		private void DrawLine(float cx, float cy, float len, double rot, char cH, char cV, char cTN, char cTP)
		{
			var wxs = new Vec2d(cx + len / 2, cy);
			wxs.RotateAround(new Vec2d(cx, cy), Math.PI * rot / 180);

			var wxe = new Vec2d(cx - len / 2, cy);
			wxe.RotateAround(new Vec2d(cx, cy), Math.PI * rot / 180);

			if ((int)Math.Round(rot % 180) == 0)
			{
				var sx = cx - len / 2;
				var ex = cx + len / 2;
				var y = cy;

				var done = false;
				for (var x = sx + 0.5f; x < ex; x += 0.5f)
				{
					SetMap(x, y, cH);
					done = true;
				}
				if (!done) SetMap(sx, y, cH);
			}
			else if ((int)Math.Round(rot % 180) == 90)
			{
				var x = cx;
				var sy = cy - len / 2;
				var ey = cy + len / 2;

				var done = false;
				for (var y = sy + 0.5f; y < ey; y += 0.5f)
				{
					SetMap(x, y, cV);
					done = true;
				}
				if (!done) SetMap(x, sy, cV);
			}
			else if ((int)Math.Round(rot % 180) < 90)
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
						SetMap(sx + i * dx, sy + i * dy, cTN);
						ly = ry;
						lx = rx;
					}
				}

				SetMap(sx, sy, cTN);
				SetMap(ex, ey, cTN);
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
						SetMap(sx + i * dx, sy + i * dy, cTP);
						ly = ry;
						lx = rx;
					}
				}

				SetMap(sx, sy, cTP);
				SetMap(ex, ey, cTP);
			}
		}

		private void FillMap(float x1, float y1, float x2, float y2, char c)
		{
			int ix1 = (int)Math.Floor(2 * x1 - 1);
			int iy1 = (int)Math.Floor(2 * y1 - 1);
			int ix2 = (int)Math.Ceiling(2 * x2 + 1);
			int iy2 = (int)Math.Ceiling(2 * y2 + 1);

			for (int ix = ix1; ix <= ix2; ix++)
			{
				for (int iy = iy1; iy <= iy2; iy++)
				{
					float rx = ix / 2f;
					float ry = iy / 2f;

					if (rx > x1 + 0.001f && rx < x2 - 0.001f && ry > y1 + 0.001f && ry < y2 - 0.001f) SetMap(rx, ry, c);
				}
			}
		}

		private void FillMapRot(float x1, float y1, float x2, float y2, float deg, char c)
		{
			if (deg == 0)   { FillMap(x1, y1, x2, y2, c); return; }
			if (deg == 180) { FillMap(x1, y1, x2, y2, c); return; }

			int ix1 = (int)Math.Floor(2 * x1 - 1);
			int iy1 = (int)Math.Floor(2 * y1 - 1);
			int ix2 = (int)Math.Ceiling(2 * x2 + 1);
			int iy2 = (int)Math.Ceiling(2 * y2 + 1);

			var cc = new Vec2d((x1 + x2) / 2, (y1 + y2) / 2);

			for (int ix = ix1; ix <= ix2; ix++)
			{
				for (int iy = iy1; iy <= iy2; iy++)
				{
					float rx = ix / 2f;
					float ry = iy / 2f;

					Vec2d crd = new Vec2d(rx, ry);
					crd.RotateAround(cc, (deg / 360f) * (2 * Math.PI));

					if (rx > x1 + 0.001f && rx < x2 - 0.001f && ry > y1 + 0.001f && ry < y2 - 0.001f) SetMap((float)crd.X, (float)crd.Y, c);
				}
			}
		}

		private void FillCircle(float x, float y, float rad, char c)
		{
			float x1 = x - rad;
			float y1 = y - rad;
			float x2 = x + rad;
			float y2 = y + rad;

			int ix1 = (int)Math.Floor(2 * x1 - 1);
			int iy1 = (int)Math.Floor(2 * y1 - 1);
			int ix2 = (int)Math.Ceiling(2 * x2 + 1);
			int iy2 = (int)Math.Ceiling(2 * y2 + 1);

			for (int ix = ix1; ix <= ix2; ix++)
			{
				for (int iy = iy1; iy <= iy2; iy++)
				{
					var d = Math.Sqrt(Math.Pow((ix / 2f - x), 2) + Math.Pow((iy / 2f - y), 2));
					if (d > rad) continue;

					float rx = ix / 2f;
					float ry = iy / 2f;

					if (rx > x1 + 0.001f && rx < x2 - 0.001f && ry > y1 + 0.001f && ry < y2 - 0.001f) SetMap(rx, ry, c);
				}
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
			builder.AppendLine("#            " + string.Join(" ", Enumerable.Range(0, MAPWIDTH_I).Select(ToDig)) );
			builder.AppendLine("#          # " + string.Join(" ", Enumerable.Range(0, MAPWIDTH_I+1).Select(i=>"#")));

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

			builder.AppendLine("#          # " + string.Join(" ", Enumerable.Range(0, MAPWIDTH_I + 1).Select(i => "#")));
			builder.AppendLine("#");
			builder.Append("#</map>");

			return builder.ToString();
		}

		private string ToDig(int arg)
		{
			if (arg < 10)
				return arg.ToString();
			else
				return ((char)('A' + (arg - 10))).ToString();
		}
	}
}
