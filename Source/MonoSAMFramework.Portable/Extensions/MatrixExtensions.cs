using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class MatrixExtensions
	{
		public static Matrix CreateScaleTranslation(float transX, float transY, float transZ, float scaleX, float scaleY, float scaleZ)
		{
			/*
			 * result matrix := 
			 *
			 * | scaleX  0      0       transx  |
			 * | 0       scaleY 0       transy  |
			 * | 0       0      scaleZ  transZ  |
			 * | 0       0      0       1       |
			 *
			 *  = Scale(scaleX, scaleY) * translate(RealOffsetX, offsetY)
			*/

			return new Matrix
			(
				scaleX, 0f,     0f,     0, 
				0,      scaleY, 0,      0, 
				0,      0,      scaleZ, 0,
				transX, transY, transZ, 1
			);
		}

		public static Matrix CreateScaleTranslation(float transX, float transY, float scaleX, float scaleY)
		{
			/*
			 * result matrix := 
			 *
			 * | scaleX  0      0       transx  |
			 * | 0       scaleY 0       transy  |
			 * | 0       0      1       0       |
			 * | 0       0      0       1       |
			 *
			 *  = Scale(scaleX, scaleY) * translate(RealOffsetX, offsetY)
			*/
			
			return new Matrix
			(
				scaleX, 0f,     0f,     0, 
				0,      scaleY, 0,      0, 
				0,      0,      1,      0,
				transX, transY, 0,      1
			);
		}
	}
}
