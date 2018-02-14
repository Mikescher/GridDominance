using System.Collections.Generic;
using System.Diagnostics;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.GameMath.Tetromino
{
	public class TetroPiece
	{
		//  0 -->     x
		//  |
		//  |    xxxx    r1
		//  v    xxxx    r2
		//       xxxx    r3
		//  y    xxxx    r4
		//
		private readonly bool[,] _shape;

		private readonly List<DPoint> _points;
		public IEnumerable<DPoint> Points => _points;

		private TetroPiece(bool[,] data)
		{
			_shape = data;
			_points = new List<DPoint>();

			for (var xx = 0; xx < 4; xx++) for (var yy = 0; yy < 4; yy++) if (IsSolid(xx, yy)) _points.Add(new DPoint(xx, yy));

			Debug.Assert(data.GetLength(0)==4);
			Debug.Assert(data.GetLength(1)==4);
			Debug.Assert(_points.Count==4);
		}

		private bool IsSolid(int x, int y)
		{
			return x >= 0 && y >= 0 && x < 4 && y < 4 && _shape[x, y];
		}
		
		internal static TetroPiece Parse(string r1, string r2, string r3, string r4)
		{
			var d = new bool[4, 4];
			for (int i = 0; i < 4; i++) d[i, 0] = r1[i]!=' ';
			for (int i = 0; i < 4; i++) d[i, 1] = r2[i]!=' ';
			for (int i = 0; i < 4; i++) d[i, 2] = r3[i]!=' ';
			for (int i = 0; i < 4; i++) d[i, 3] = r4[i]!=' ';

			return new TetroPiece(d);
		}
	}
}
