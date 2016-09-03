using System.Collections.Generic;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	/// <summary>
	//  All Letters are scaled to a height of 1f
	/// </summary>
	public static class PathPresets
	{
		private const float LETTER_SCALE = 1 / 40f;

		public static readonly VectorPath PATH_LETTER_A = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 40)
																	.LineTo(15, 0)
																	.LineTo(30, 40)
																	.MoveTo(5.625f, 25)
																	.LineTo(24.375f, 25)
																	.Build();

		public static readonly VectorPath PATH_LETTER_B = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 0)
																	.LineTo(15, 0)
																	.HalfCircleDown(9)
																	.LineTo(0, 18)
																	.MoveTo(16, 18)
																	.HalfCircleDown(11)
																	.LineTo(0, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_C = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.Ellipse(14, 20, 14, 20, 45, 315)
																	.Build();

		public static readonly VectorPath PATH_LETTER_D = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 0)
																	.LineTo(15, 0)
																	.HalfEllipseDown(14, 20)
																	.LineTo(0, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_E = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 0)
																	.LineTo(22, 0)
																	.MoveTo(0, 20)
																	.LineTo(18, 20)
																	.MoveTo(0, 40)
																	.LineTo(22, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_F = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 0)
																	.LineTo(22, 0)
																	.MoveTo(0, 20)
																	.LineTo(18, 20)
																	.Build();

		public static readonly VectorPath PATH_LETTER_G = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.Ellipse(14, 20, 14, 20, 0, 300)
																	.MoveTo(28, 20)
																	.LineTo(18, 20)
																	.Build();

		public static readonly VectorPath PATH_LETTER_H = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(24, 0)
																	.LineTo(24, 40)
																	.MoveTo(0, 20)
																	.LineTo(24, 20)
																	.Build();

		public static readonly VectorPath PATH_LETTER_I = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_J = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(22, 0)
																	.LineTo(22, 29)
																	.HalfCircleLeft(11)
																	.Build();

		public static readonly VectorPath PATH_LETTER_K = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 17)
																	.LineTo(20, 0)
																	.MoveTo(0, 17)
																	.LineTo(22, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_L = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.LineTo(22, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_M = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 40)
																	.LineTo(0, 0)
																	.LineTo(16, 40)
																	.LineTo(32, 0)
																	.LineTo(32, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_N = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 40)
																	.LineTo(0, 0)
																	.LineTo(26, 40)
																	.LineTo(26, 0)
																	.Build();

		public static readonly VectorPath PATH_LETTER_O = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.FullEllipse(14, 20, 14, 20)
																	.Build();

		public static readonly VectorPath PATH_LETTER_P = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 0)
																	.LineTo(15, 0)
																	.HalfCircleDown(9)
																	.LineTo(0, 18)
																	.Build();

		public static readonly VectorPath PATH_LETTER_Q = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.FullEllipse(14, 20, 14, 20)
																	.MoveTo(20, 30)
																	.LineTo(30, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_R = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 40)
																	.MoveTo(0, 0)
																	.LineTo(15, 0)
																	.HalfCircleDown(9)
																	.LineTo(0, 18)
																	.MoveTo(15, 18)
																	.LineTo(24, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_S = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.Ellipse(12, 10, 12, 10, 90, 360)
																	.Ellipse(12, 10, 12, 30, -90, 180)
																	.Build();

		public static readonly VectorPath PATH_LETTER_T = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.LineTo(0, 0)
																	.LineTo(24, 0)
																	.LineTo(12, 0)
																	.LineTo(12, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_U = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(0, 25)
																	.HalfCircleRight(15)
																	.LineTo(30, 0)
																	.Build();

		public static readonly VectorPath PATH_LETTER_V = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(12, 40)
																	.LineTo(24, 0)
																	.Build();

		public static readonly VectorPath PATH_LETTER_W = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(12, 40)
																	.LineTo(24, 0)
																	.LineTo(36, 40)
																	.LineTo(48, 0)
																	.Build();

		public static readonly VectorPath PATH_LETTER_X = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(31, 40)
																	.MoveTo(0, 40)
																	.LineTo(31, 0)
																	.Build();

		public static readonly VectorPath PATH_LETTER_Y = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(13, 14)
																	.LineTo(26, 0)
																	.MoveTo(13, 14)
																	.LineTo(13, 40)
																	.Build();

		public static readonly VectorPath PATH_LETTER_Z = VectorPathBuilder
																	.Start(LETTER_SCALE)
																	.MoveTo(0, 0)
																	.LineTo(31, 0)
																	.LineTo(0, 40)
																	.LineTo(31, 40)
																	.Build();


		public static readonly Dictionary<char, VectorPath> LETTERS = new Dictionary<char, VectorPath>
		{
			{ 'A', PATH_LETTER_A},
			{ 'B', PATH_LETTER_B},
			{ 'C', PATH_LETTER_C},
			{ 'D', PATH_LETTER_D},
			{ 'E', PATH_LETTER_E},
			{ 'F', PATH_LETTER_F},
			{ 'G', PATH_LETTER_G},
			{ 'H', PATH_LETTER_H},
			{ 'I', PATH_LETTER_I},
			{ 'J', PATH_LETTER_J},
			{ 'K', PATH_LETTER_K},
			{ 'L', PATH_LETTER_L},
			{ 'M', PATH_LETTER_M},
			{ 'N', PATH_LETTER_N},
			{ 'O', PATH_LETTER_O},
			{ 'P', PATH_LETTER_P},
			{ 'Q', PATH_LETTER_Q},
			{ 'R', PATH_LETTER_R},
			{ 'S', PATH_LETTER_S},
			{ 'T', PATH_LETTER_T},
			{ 'U', PATH_LETTER_U},
			{ 'V', PATH_LETTER_V},
			{ 'W', PATH_LETTER_W},
			{ 'X', PATH_LETTER_X},
			{ 'Y', PATH_LETTER_Y},
			{ 'Z', PATH_LETTER_Z},
		};

	}
}
