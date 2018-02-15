using System.Collections.Generic;
using System.Diagnostics;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.GameMath.Tetromino
{
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
}
