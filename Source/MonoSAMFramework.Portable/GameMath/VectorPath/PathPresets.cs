namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	/// <summary>
	//  All Letters are scaled to a height of 1f
	/// </summary>
	public static class PathPresets
	{
		public static readonly VectorPath PATH_LETTER_A = VectorPathBuilder
																	.Start(1f / 40)
																	.MoveTo(0, 40)
																	.LineTo(15, 0)
																	.LineTo(30, 40)
																	.MoveTo(5.625f, 25)
																	.LineTo(24.375f, 25)
																	.Build();

		public static readonly VectorPath PATH_LETTER_B = VectorPathBuilder
																	.Start(1f / 40)
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



	}
}
