<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Globalization</Namespace>
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

//string PATH_OUT = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\other\tetromino_list.txt");

void Main()
{
	Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
	foreach (var tp in TetroPieces.ALPHABET)
	{
		tp.Name.Dump();
		Print(tp);
	}
}

public void Print(TetroPiece piece)
{
	int psz = 16;

	Bitmap b = new Bitmap(4 * psz, 4 * psz, PixelFormat.Format32bppArgb);
	using (var g = Graphics.FromImage(b))
	{
		for (int x = 0; x < 4; x++)
		{
			for (int y = 0; y < 4; y++)
			{
				if ((x % 2 == 0) ^ (y % 2 == 0))
					g.FillRectangle(Brushes.LightGray, x * psz, (3 - y) * psz, psz, psz);
				else
					g.FillRectangle(Brushes.Gray, x * psz, (3 - y) * psz, psz, psz);
			}
		}


		foreach (var pp in piece.Points)
		{
			var p = pp;
			g.FillRectangle(new SolidBrush(Color.Firebrick), p.X * psz, p.Y * psz, psz, psz);
		}
		
		g.FillRectangle(new SolidBrush(Color.Blue), piece.Center.X * psz - 2, piece.Center.Y * psz - 2, 4, 4);

	}



	b.Dump();
	"".Dump();
}

// Define other methods and classes here


//##############################################

// https://en.wikipedia.org/wiki/Tetromino
public static class TetroPieces
{
	public static readonly TetroPiece LETTER_I1 = TetroPiece.Create("I1", 0, 0, 0, 1, 0, 2, 0, 3, 0.5f, 2.0f);
	public static readonly TetroPiece LETTER_I2 = TetroPiece.Create("I2", 0, 0, 1, 0, 2, 0, 3, 0, 2.0f, 0.5f);
	public static readonly TetroPiece LETTER_O1 = TetroPiece.Create("O1", 0, 0, 0, 1, 1, 0, 1, 1, 1.0f, 1.0f);
	public static readonly TetroPiece LETTER_Z1 = TetroPiece.Create("Z1", 0, 0, 1, 0, 1, 1, 2, 1, 1.5f, 1.0f);
	public static readonly TetroPiece LETTER_Z2 = TetroPiece.Create("Z2", 0, 1, 0, 2, 1, 0, 1, 1, 1.0f, 1.5f);
	public static readonly TetroPiece LETTER_S1 = TetroPiece.Create("S1", 0, 1, 1, 0, 1, 1, 2, 0, 1.5f, 1.0f);
	public static readonly TetroPiece LETTER_S2 = TetroPiece.Create("S2", 0, 0, 0, 1, 1, 1, 1, 2, 1.0f, 1.5f);
	public static readonly TetroPiece LETTER_T1 = TetroPiece.Create("T1", 0, 1, 1, 0, 1, 1, 2, 1, 1.5f, 1.5f);
	public static readonly TetroPiece LETTER_T2 = TetroPiece.Create("T2", 0, 0, 0, 1, 0, 2, 1, 1, 0.5f, 1.5f);
	public static readonly TetroPiece LETTER_T3 = TetroPiece.Create("T3", 0, 0, 1, 0, 1, 1, 2, 0, 1.5f, 0.5f);
	public static readonly TetroPiece LETTER_T4 = TetroPiece.Create("T4", 0, 1, 1, 0, 1, 1, 1, 2, 1.5f, 1.5f);
	public static readonly TetroPiece LETTER_L1 = TetroPiece.Create("L1", 0, 0, 0, 1, 0, 2, 1, 2, 0.5f, 2.5f);
	public static readonly TetroPiece LETTER_L2 = TetroPiece.Create("L2", 0, 0, 0, 1, 1, 0, 2, 0, 0.5f, 0.5f);
	public static readonly TetroPiece LETTER_L3 = TetroPiece.Create("L3", 0, 0, 1, 0, 1, 1, 1, 2, 1.5f, 0.5f);
	public static readonly TetroPiece LETTER_L4 = TetroPiece.Create("L4", 0, 1, 1, 1, 2, 0, 2, 1, 2.5f, 1.5f);
	public static readonly TetroPiece LETTER_J1 = TetroPiece.Create("J1", 0, 2, 1, 0, 1, 1, 1, 2, 1.5f, 2.5f);
	public static readonly TetroPiece LETTER_J2 = TetroPiece.Create("J2", 0, 0, 0, 1, 1, 1, 2, 1, 0.5f, 1.5f);
	public static readonly TetroPiece LETTER_J3 = TetroPiece.Create("J3", 0, 0, 0, 1, 0, 2, 1, 0, 0.5f, 0.5f);
	public static readonly TetroPiece LETTER_J4 = TetroPiece.Create("J4", 0, 0, 1, 0, 2, 0, 2, 1, 2.5f, 0.5f);

	public static readonly TetroPiece[] ALPHABET =
	{
			LETTER_O1,
			LETTER_I1,LETTER_I2,
			LETTER_Z1,LETTER_Z2,
			LETTER_S1,LETTER_S2,
			LETTER_T1,LETTER_T2,LETTER_T3,LETTER_T4,
			LETTER_L1,LETTER_L2,LETTER_L3,LETTER_L4,
			LETTER_J1,LETTER_J2,LETTER_J3,LETTER_J4,
		};

	public static Dictionary<string, TetroPiece> PIECES = ALPHABET.ToDictionary(l => l.Name, l => l);
}

	public class TetroPiece
	{
		// grid:
		//
		//  0 -->     x
		//  |
		//  |    xxxx    r1
		//  v    xxxx    r2
		//       xxxx    r3
		//  y    xxxx    r4
		//

		public readonly string Name;

		public IEnumerable<DPoint> Points => _points;
		public readonly FPoint Center;
		public DPoint P1 => _points[0];
		public DPoint P2 => _points[1];
		public DPoint P3 => _points[2];
		public DPoint P4 => _points[3];

		private readonly bool[,] _shape;
		private readonly DPoint[] _points;

		private TetroPiece(string n, DPoint p1, DPoint p2, DPoint p3, DPoint p4, FPoint center)
		{
			Name = n;
			_points = new[] { p1, p2, p3, p4 };

			_shape = new bool[4, 4];
			_shape[p1.X, p1.Y] = true;
			_shape[p2.X, p2.Y] = true;
			_shape[p3.X, p3.Y] = true;
			_shape[p4.X, p4.Y] = true;

			Center = center;

			Debug.Assert(p1 != p2);
			Debug.Assert(p2 != p3);
			Debug.Assert(p3 != p4);
			Debug.Assert(p4 != p1);
			Debug.Assert(p1 != p3);
			Debug.Assert(p2 != p4);
		}

		private bool Contains(int x, int y)
		{
			return x >= 0 && y >= 0 && x < 4 && y < 4 && _shape[x, y];
	}

	public static TetroPiece Create(string n, int p1x, int p1y, int p2x, int p2y, int p3x, int p3y, int p4x, int p4y, float ccx, float ccy)
	{
		return new TetroPiece(n, new DPoint(p1x, p1y), new DPoint(p2x, p2y), new DPoint(p3x, p3y), new DPoint(p4x, p4y), new FPoint(ccx, ccy));
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

public struct FPoint
{
	public readonly float X;

	public readonly float Y;

	public FPoint(float x, float y) : this()
	{
		X = x;
		Y = y;
	}
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