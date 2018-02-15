<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

Color[] COLORS = 
{
	Color.FromArgb(51,  18,  13),
	Color.FromArgb(140, 75,  0),
	Color.FromArgb(222, 230, 115),
	Color.FromArgb(25,  64,  0),
	Color.FromArgb(0,   238, 255),
	Color.FromArgb(38,  42,  51),
	Color.FromArgb(77,  0,   191),
	Color.FromArgb(128, 96,  121),
	Color.FromArgb(255, 208, 191),
	Color.FromArgb(255, 170, 0),
	Color.FromArgb(102, 255, 0),
	Color.FromArgb(57,  230, 149),
	Color.FromArgb(115, 176, 230),
	Color.FromArgb(0,   7,   51),
	Color.FromArgb(204, 102, 197),
	Color.FromArgb(204, 0,   54),
};

Random R = new Random(81580085);

string PATH_OUT = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\other\tetromino_list.txt");

void Main()
{
	HashSet<string> s = new HashSet<string>();
	
	int i = 0;
	for (; ; )
	{
		TetroField f;
		while ((f = CreateNew()) == null);
		var srz = f.Serialize();
		if (!s.Add(srz)) continue;

		i++;
		i.Dump();
		new Hyperlinq(() => { File.AppendAllLines(PATH_OUT, new[] { srz }); }, $"{srz}").Dump();
		Print(f);
		
		if (i>200) return;
	}
}

// Define other methods and classes here

public void Print(TetroField f)
{
	int psz = 16;

	Bitmap b = new Bitmap(5*psz, 5*psz, PixelFormat.Format32bppArgb);
	using (var g = Graphics.FromImage(b))
	{
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 5; y++)
			{
				if ((x % 2 == 0) ^ (y % 2 == 0))
					g.FillRectangle(Brushes.LightGray, x * psz, (4 - y) * psz, psz, psz);
				else
					g.FillRectangle(Brushes.Gray, x * psz, (4 - y) * psz, psz, psz);
			}
		}

		g.FillRectangle(new SolidBrush(Color.Red), 0 * psz, 4 * psz, psz, psz);

		for (int i = 0; i < f.Tetros.Count; i++)
		{
			foreach (var pp in f.Tetros[i].Piece.Points)
			{
				var p = pp + f.Tetros[i].Position;
				g.FillRectangle(new SolidBrush(COLORS[i]), p.X * psz, p.Y * psz, psz, psz);
			}
		}
		
		for (int xx = 0; xx < 5; xx++)
		{
			for (int yy = 0; yy < 5; yy++)
			{
				if (f.Grid[xx, yy]) g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), xx * psz + psz/3, yy * psz + psz/3, psz/3, psz/3);
			}
		}
	}



	b.Dump();
	"".Dump();
}

public TetroField CreateNew()
{
	var f = new TetroField();
	
	for (int i = 0; i < 6; i++)
	{
		var success = false;
		var slist = Enumerable.Range(0, TetroPieces.ALPHABET.Length).ToList();
		Shuffle<int>(slist);
		foreach (var s in slist)
		{
			var piece = TetroPieces.ALPHABET[s];
			if (TryInsert(f, piece)) { success = true; break;}
		}
		if (!success) return null;
		//Print(f);
	}
	return f;
}

bool TryInsert(TetroField f, TetroPiece piece)
{
	for (int i = 0; i < 4; i++)
	{
		if (TryInsert(f, piece, new DPoint(i, 0), new DPoint(0, +1))) return true; // from top
		if (TryInsert(f, piece, new DPoint(3, i), new DPoint(-1, 0))) return true; // from right
	}
	return false;
}

bool TryInsert(TetroField f, TetroPiece p, DPoint start, DPoint delta)
{
	SpecificTetro result = null;
	
	for (int i = 0; i < 12; i++)
	{
		var r = new SpecificTetro { Piece = p, InsertVec=delta, Position=start+delta*i };
		
		if (CheckFit(f, r)) result = r;
	}

	if (result != null) { f.Add(result); return true; }
	
	return false;
}

bool CheckFit(TetroField f, SpecificTetro r)
{
	foreach (var point in r.Piece.Points)
	{
		if (!CheckFit(f, point + r.Position)) return false;
	}
	return true;
}

bool CheckFit(TetroField f, DPoint p)
{
	if (p.X < 0) return false;
	if (p.Y < 0) return false;
	if (p.X > 4) return false;
	if (p.Y > 4) return false;
	
	if (p.X == 0 && p.Y == 4) return false; // base block
	
	if (f.Grid[p.X, p.Y]) return false;
	
	return true;
}

//##############################################

public class TetroField
{
	public bool[,] Grid = new bool[5,5];
	public List<SpecificTetro> Tetros = new List<SpecificTetro>();
	public TetroField() { Grid[0, 4]=true; }
	public void Add(SpecificTetro t)
	{
		foreach (var point in t.Piece.Points)
		{
			var pp = point + t.Position;
			if (Grid[pp.X, pp.Y]) throw new Exception("Overlap");
			Grid[pp.X, pp.Y] = true;
		}
		Tetros.Add(t);
	}

	public string Serialize()
	{
		return Tetros
			.OrderBy(t => t.Position.X)
			.ThenBy(t => -t.Position.Y)
			.Select(t => $"{t.Position.X}{t.Position.Y}{t.Piece.Name}")
			.Aggregate((a, b) => a + b);
	}
}

public class SpecificTetro
{
	public TetroPiece Piece;
	public DPoint Position;
	public DPoint InsertVec;
}

//##############################################

// https://en.wikipedia.org/wiki/Tetromino
public static class TetroPieces
{
	public static TetroPiece LETTER_I_1 = TetroPiece.Parse("I1", "x   ", "x   ", "x   ", "x   ");
	public static TetroPiece LETTER_I_2 = TetroPiece.Parse("I2", "xxxx", "    ", "    ", "    ");
	public static TetroPiece LETTER_O_1 = TetroPiece.Parse("O1", "xx  ", "xx  ", "    ", "    ");
	public static TetroPiece LETTER_Z_1 = TetroPiece.Parse("Z1", "xx  ", " xx ", "    ", "    ");
	public static TetroPiece LETTER_Z_2 = TetroPiece.Parse("Z2", " x  ", "xx  ", "x   ", "    ");
	public static TetroPiece LETTER_S_1 = TetroPiece.Parse("S1", " xx ", "xx  ", "    ", "    ");
	public static TetroPiece LETTER_S_2 = TetroPiece.Parse("S2", "x   ", "xx  ", " x  ", "    ");
	public static TetroPiece LETTER_T_1 = TetroPiece.Parse("T1", " x  ", "xxx ", "    ", "    ");
	public static TetroPiece LETTER_T_2 = TetroPiece.Parse("T2", "x   ", "xx  ", "x   ", "    ");
	public static TetroPiece LETTER_T_3 = TetroPiece.Parse("T3", "xxx ", " x  ", "    ", "    ");
	public static TetroPiece LETTER_T_4 = TetroPiece.Parse("T4", " x  ", "xx  ", " x  ", "    ");
	public static TetroPiece LETTER_L_1 = TetroPiece.Parse("L1", "x   ", "x   ", "xx  ", "    ");
	public static TetroPiece LETTER_L_2 = TetroPiece.Parse("L2", "xxx ", "x   ", "    ", "    ");
	public static TetroPiece LETTER_L_3 = TetroPiece.Parse("L3", "xx  ", " x  ", " x  ", "    ");
	public static TetroPiece LETTER_L_4 = TetroPiece.Parse("L4", "  x ", "xxx ", "    ", "    ");
	public static TetroPiece LETTER_J_1 = TetroPiece.Parse("J1", " x  ", " x  ", "xx  ", "    ");
	public static TetroPiece LETTER_J_2 = TetroPiece.Parse("J2", "x   ", "xxx ", "    ", "    ");
	public static TetroPiece LETTER_J_3 = TetroPiece.Parse("J3", "xx  ", "x   ", "x   ", "    ");
	public static TetroPiece LETTER_J_4 = TetroPiece.Parse("J4", "xxx ", "  x ", "    ", "    ");

	public static TetroPiece[] ALPHABET = new[] 
	{
		LETTER_I_1,LETTER_I_2,
		LETTER_O_1,LETTER_Z_1,LETTER_Z_2,
		LETTER_S_1,LETTER_S_2,
		LETTER_T_1,LETTER_T_2,LETTER_T_3,LETTER_T_4,
		LETTER_L_1,LETTER_L_2,LETTER_L_3,LETTER_L_4,
		LETTER_J_1,LETTER_J_2,LETTER_J_3,LETTER_J_4
	};
}

public class TetroPiece
{
	//  0 -->     x
	//  |
	//  |    xxxx    r1
	//  v    xxxx    r2
	//       xxxx    r3
	//  y    xxxx    r4
	//
	public readonly string Name;
	
	private readonly bool[,] _shape;
	private readonly List<DPoint> _points;
	
	public IEnumerable<DPoint> Points => _points;

	private TetroPiece(string name, bool[,] data)
	{
		Name = name;
		_shape = data;
		_points = new List<DPoint>();

		for (var xx = 0; xx < 4; xx++) for (var yy = 0; yy < 4; yy++) if (IsSolid(xx, yy)) _points.Add(new DPoint(xx, yy));

		Debug.Assert(data.GetLength(0) == 4);
		Debug.Assert(data.GetLength(1) == 4);
		Debug.Assert(_points.Count == 4);
	}

	private bool IsSolid(int x, int y)
	{
		return x >= 0 && y >= 0 && x < 4 && y < 4 && _shape[x, y];
	}

	internal static TetroPiece Parse(string n, string r1, string r2, string r3, string r4)
	{
		var d = new bool[4, 4];
		for (int i = 0; i < 4; i++) d[i, 0] = r1[i] != ' ';
		for (int i = 0; i < 4; i++) d[i, 1] = r2[i] != ' ';
		for (int i = 0; i < 4; i++) d[i, 2] = r3[i] != ' ';
		for (int i = 0; i < 4; i++) d[i, 3] = r4[i] != ' ';

		return new TetroPiece(n, d);
	}
}

public struct DPoint : IEquatable<DPoint>
{
	public readonly int X;

	public readonly int Y;

	public static readonly DPoint Zero = new DPoint(0, 0);

	public static readonly DPoint MaxValue = new DPoint(int.MaxValue, int.MaxValue);

	public DPoint(int x, int y) : this()
	{
		X = x;
		Y = y;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			return X.GetHashCode() + Y.GetHashCode();
		}
	}

	public static bool operator ==(DPoint a, DPoint b)
	{
		return (a.X == b.X) && (a.Y == b.Y);
	}

	public static bool operator !=(DPoint a, DPoint b)
	{
		return !(a == b);
	}

	public static DPoint operator +(DPoint value1, DPoint value2)
	{
		return new DPoint(value1.X + value2.X, value1.Y + value2.Y);
	}

	public static DPoint operator -(DPoint value1, DPoint value2)
	{
		return new DPoint(value1.X - value2.X, value1.Y - value2.Y);
	}

	public static DPoint operator *(DPoint value1, int value2)
	{
		return new DPoint(value1.X * value2, value1.Y * value2);
	}

	public bool Equals(DPoint other)
	{
		return (X == other.X) && (Y == other.Y);
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		return obj is DPoint && Equals((DPoint)obj);
	}

	public override string ToString() => $"[{X},{Y}]";
}

public void Shuffle<T>(IList<T> list)
{
	int n = list.Count;
	while (n > 1)
	{
		n--;
		int k = R.Next(n + 1);
		T value = list[k];
		list[k] = list[n];
		list[n] = value;
	}
}