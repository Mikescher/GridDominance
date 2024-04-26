using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.GameMath.Tetromino
{
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
		
		public static readonly TetroPiece[] DISTINCT_ALPHABET =
		{
			LETTER_O1,
			LETTER_I1,
			LETTER_Z1,
			LETTER_S1,
			LETTER_T1,
			LETTER_L1,
			LETTER_J1,
		};

		public static readonly Dictionary<string, TetroPiece> PIECES = ALPHABET.ToDictionary(l => l.Name, l => l);
	}
}
