<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
</Query>

Random R = new Random(2123);

const int WIDTH = 180;
const int HEIGHT = 90;

const int BLOP_D = 24;
const int BLOP_C = 6;

const int CELLW = 10;

const bool DUMP_STEPS = false;
const bool DUMP       = true;

void Main()
{
	var g = GetGrid();
	
	var p = Partition(g).ToList();

	if (DUMP) DumpGrid(g, p, new List<(int, int)>(), new List<(int, int)>());

	if (DUMP) $"{p.Count} / ({WIDTH * HEIGHT})  = {(p.Count*100d) / (WIDTH * HEIGHT):0.000}%".Dump();
	
	if (DUMP) p.Dump();
}

IEnumerable<(int, int, int, int)> Partition(int[,] grid)
{
	bool[,] finished = new bool[WIDTH, HEIGHT];
	
	var pointQueue = new Queue<(int, int)>();
	pointQueue.Enqueue((0,0));
	
	var intermed = new List<(int, int, int, int)>();
	
	int cnt=0;
	while (pointQueue.Any())
	{
		cnt++;
		
		var (tl_x, tl_y) = pointQueue.Dequeue();
		if (finished[tl_x, tl_y]) continue;

		if (tl_x > 0 && !finished[tl_x - 1, tl_y]) { pointQueue.Enqueue((tl_x, tl_y)); continue; }
		if (tl_y > 0 && !finished[tl_x, tl_y - 1]) { pointQueue.Enqueue((tl_x, tl_y)); continue; }
		
		int type = grid[tl_x, tl_y];
		
		int s = 1;
		int w = 1;
		int h = 1;
		
		for (int i = 1; tl_x+i <= WIDTH; i++)
		{
			if (grid[tl_x + i - 1, tl_y] == type && !finished[tl_x + i - 1, tl_y]) s = w = i;
			else break;
		}

		int idx = w;
		for (int idy = h; tl_y+idy <= HEIGHT; idy++)
		{
			int nidx = 0;
			while(nidx<idx && grid[tl_x+nidx, tl_y+idy-1] == type && !finished[tl_x+nidx, tl_y+idy-1]) nidx++;
			idx=nidx;
			if (idx <= 0) break;
			if (idx * idy > s) s = (w = idx) * (h = idy);
		}

		for (int iw = 0; iw < w; iw++)
		{
			for (int ih = 0; ih < h; ih++)
			{
				if (finished[tl_x + iw, tl_y + ih])
					throw new Exception();
			}
		}


		for (int iw = 0; iw < w; iw++)
		{
			for (int ih = 0; ih < h; ih++)
			{
				finished[tl_x + iw, tl_y + ih] = true;
			}
		}

		yield return (tl_x, tl_y, w, h);
		if (DUMP_STEPS) intermed.Add((tl_x, tl_y, w, h));

		if (tl_x + w < WIDTH) pointQueue.Enqueue((tl_x + w, tl_y));
		if (tl_y + h < HEIGHT) pointQueue.Enqueue((tl_x, tl_y + h));
		
		if (DUMP_STEPS)
			DumpGrid(
				grid, 
				intermed, 
				pointQueue.Where(p => !finished[p.Item1, p.Item2]).ToList(), 
				Enumerable
					.Range(0,WIDTH)
					.SelectMany(x => 
						Enumerable
							.Range(0,HEIGHT)
							.Where(y => finished[x,y])
							.Select(y => (x,y) )
					).ToList());
	}
	
	cnt.Dump();
}

int[,] GetGrid()
{
	int[,] grid = new int[WIDTH, HEIGHT];
	
	var areas = new Stack<(int, int, int)>();
	
	for (int i = 0; i < BLOP_C; i++)	areas.Push((R.Next(BLOP_D/2, WIDTH-BLOP_D), R.Next(BLOP_D/2, HEIGHT-BLOP_D), R.Next(BLOP_D/32, (int)(BLOP_D*2))));
	
	while (areas.Any())
	{
		var a = areas.Pop();
		
		var mx = a.Item3 * a.Item3;
		
		for (int x = -mx; x <= mx; x++)
		{
			for (int y = -mx; y <= mx; y++)
			{
				var rx = a.Item1 + x;
				var ry = a.Item2 + y;

				if (rx < 0) continue;
				if (ry < 0) continue;
				if (rx >= WIDTH) continue;
				if (ry >= HEIGHT) continue;

				var p = (int)(((a.Item3 - Math.Sqrt(x*x + y*y)) / a.Item3) * 10);

				p = Math.Min(p, 9);
				p = Math.Max(p, 0);
				
				grid[rx, ry] = Math.Min(9, grid[rx, ry] + p);
			}
		}
		
	}
	
	return grid;
}

Color GridCol(int v)
{
	var cs = Color.Gray;
	var ce = Color.DarkGreen;

	var r = cs.R + (v / 10f) * (ce.R - cs.R);
	var g = cs.G + (v / 10f) * (ce.G - cs.G);
	var b = cs.B + (v / 10f) * (ce.B - cs.B);
	
	return Color.FromArgb(255, (int)r, (int)g, (int)b);
}

void DumpGrid(int[,] data, List<(int, int, int, int)> partitions, List<(int, int)> marks, List<(int, int)> fins)
{
	var fnt = new Font("Courier New", CELLW-2, FontStyle.Regular, GraphicsUnit.Pixel);
	
	var bmp = new Bitmap(WIDTH * CELLW, HEIGHT * CELLW);
	using (var g = Graphics.FromImage(bmp))
	{
		for (int xx = 0; xx < WIDTH; xx++)
		{
			for (int yy = 0; yy < HEIGHT; yy++)
			{
				g.FillRectangle(new SolidBrush(GridCol(data[xx,yy])), xx*CELLW, yy*CELLW, CELLW, CELLW);
			}
		}

		for (int xx = 0; xx <= WIDTH; xx++) g.DrawLine(Pens.Black, xx * CELLW, 0, xx * CELLW, bmp.Height);
		for (int yy = 0; yy <= HEIGHT; yy++) g.DrawLine(Pens.Black, 0, yy * CELLW, bmp.Width, yy * CELLW);
		
		foreach (var (x,y,w,h) in partitions)
		{
			g.DrawRectangle(Pens.Red, x * CELLW + CELLW / 4f, y * CELLW + CELLW / 4f, w * CELLW - CELLW / 2f, h * CELLW - CELLW / 2f);
		}

		foreach (var (x, y) in fins)
		{
			g.DrawLine(Pens.Red, x * CELLW, y * CELLW, x * CELLW + CELLW, y * CELLW + CELLW);
			g.DrawLine(Pens.Red, x * CELLW, y * CELLW + CELLW, x * CELLW + CELLW, y * CELLW);
		}

		foreach (var (x, y) in marks)
		{
			g.FillEllipse(Brushes.Blue, x * CELLW + CELLW*0.125f, y * CELLW + CELLW*0.125f, CELLW * 0.75f, CELLW * 0.75f);
		}

		for (int xx = 0; xx < WIDTH; xx++)
		{
			for (int yy = 0; yy < HEIGHT; yy++)
			{
				g.DrawString(data[xx, yy].ToString(), fnt, Brushes.Black, xx * CELLW + 1, yy * CELLW + 1);
			}
		}
	}

	bmp.Dump();
}