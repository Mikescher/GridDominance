<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

string PATH_IN   = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\other\tetromino_all.txt");
string PATH_OUT  = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\other\tetromino_all.txt");
string PATH_OUT2 = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\graphics\tetromino_data.png");

void Main()
{
	var combinations = File.ReadAllLines(PATH_IN).Where(l => !string.IsNullOrWhiteSpace(l)).Select(p => p.Split(' ', '\t')[0]).ToList();
	
	StringBuilder bout = new StringBuilder();


	Bitmap b = new Bitmap(44*2, 28*2, PixelFormat.Format24bppRgb);

	int i = 0;
	ulong cumsum = 0;
	foreach (var c1 in combinations)
	{
		var c2 = TetroField.Parse(c1);
		var c3 = c2.NumSerialize();
		var c4 = TetroField.Parse(c3);
		var c5 = c4.StrSerialize();
		var c6 = TetroField.Parse(c5);
		var c7 = c6.NumSerialize();

		if (c3 != c7) throw new Exception();
		if (c1 != c5) throw new Exception();

		bout.Append($"{c1}\t0x{c3:X16}\n");
		$"{c1}  0x{c3:X16}".Dump();

		var v1 = (int)((c7 >> 0*8) & 0xFF);
		var v2 = (int)((c7 >> 1*8) & 0xFF);
		var v3 = (int)((c7 >> 2*8) & 0xFF);
		var v4 = (int)((c7 >> 3*8) & 0xFF);
		var v5 = (int)((c7 >> 4*8) & 0xFF);
		var v6 = (int)((c7 >> 5*8) & 0xFF);
		var v7 = (int)((c7 >> 6*8) & 0xFF);
		var v8 = (int)((c7 >> 7*8) & 0xFF);

		var v9  = (int)((cumsum >> 0 * 8) & 0xFF);
		var v10 = (int)((cumsum >> 1 * 8) & 0xFF);
		var v11 = (int)((cumsum >> 2 * 8) & 0xFF);
		var v12 = (int)((cumsum >> 3 * 8) & 0xFF);

		cumsum += c7;

		b.SetPixel((i % 44) * 2 + 0, (i / 44) * 2 + 0, Color.FromArgb(255, v1, v2, v3));
		b.SetPixel((i % 44) * 2 + 1, (i / 44) * 2 + 0, Color.FromArgb(255, v4, v5, v6));
		b.SetPixel((i % 44) * 2 + 0, (i / 44) * 2 + 1, Color.FromArgb(255, v7, v8, v9));
		b.SetPixel((i % 44) * 2 + 1, (i / 44) * 2 + 1, Color.FromArgb(255, v10, v11, v12));
		i++;
	}

	b.Dump();

	File.WriteAllText(PATH_OUT, bout.ToString());
	b.Save(PATH_OUT2);
}

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

	public TetroField Clone()
	{
		return new TetroField() { Grid = (bool[,])Grid.Clone(), Tetros = Tetros.ToList() };
	}

	public string StrSerialize()
	{
		return Tetros
			.OrderBy(t => t.Position.X)
			.ThenBy(t => -t.Position.Y)
			.Select(t => $"{t.Position.X}{t.Position.Y}{t.Piece.Name}")
			.Aggregate((a, b) => a + b);
	}

	public static TetroField Parse(string config)
	{
		var r = new TetroField();
		foreach (var i in Enumerable.Range(0, 6))
		{
			var x = config[4 * i + 0] - '0';
			var y = config[4 * i + 1] - '0';
			var id = config[4 * i + 2] + "" + config[4 * i + 3];

			r.Add(new SpecificTetro { Position = new DPoint(x, y), Piece = TetroPieces.PIECES[id] });
		}
		return r;
	}
	
	public ulong NumSerialize() // [5 bit: position] [5 bit: tetropiece] ... (x6)
	{
		ulong r = 0;
		for (int i = 0; i < 6; i++)
		{
			r = r << 5;
			r |= (uint)(TetroPieces.ALPHABET.ToList().IndexOf(Tetros[i].Piece));
			r = r << 5;
			r |= (uint)(Tetros[i].Position.X + Tetros[i].Position.Y * 5);
		}
		return r;
	}

	public static TetroField Parse(ulong ident)
	{
		var r = new TetroField();
		for (int i = 0; i < 6; i++)
		{
			var x  = (int) (((ident >> (i*10+0)) & 0x1F) % 5);
			var y  = (int) (((ident >> (i*10+0)) & 0x1F) / 5);
			var id = (int) (((ident >> (i*10+5)) & 0x1F));

			r.Add(new SpecificTetro { Position = new DPoint(x, y), Piece = TetroPieces.ALPHABET[id] });
		}
		return r;
	}
}

public class SpecificTetro
{
	public TetroPiece Piece;
	public DPoint Position;
}

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

	public static TetroPiece[] ALPHABET = new[]
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

	private bool IsSolid(int x, int y)
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

public struct FPoint : IEquatable<FPoint>
{
	public readonly float X;

	public readonly float Y;

	public static readonly DPoint Zero = new DPoint(0, 0);

	public static readonly DPoint MaxValue = new DPoint(int.MaxValue, int.MaxValue);

	public FPoint(float x, float y) : this()
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

	public static bool operator ==(FPoint a, FPoint b)
	{
		return (a.X == b.X) && (a.Y == b.Y);
	}

	public static bool operator !=(FPoint a, FPoint b)
	{
		return !(a == b);
	}

	public static FPoint operator +(FPoint value1, FPoint value2)
	{
		return new FPoint(value1.X + value2.X, value1.Y + value2.Y);
	}

	public static FPoint operator -(FPoint value1, FPoint value2)
	{
		return new FPoint(value1.X - value2.X, value1.Y - value2.Y);
	}

	public static FPoint operator *(FPoint value1, int value2)
	{
		return new FPoint(value1.X * value2, value1.Y * value2);
	}

	public bool Equals(FPoint other)
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