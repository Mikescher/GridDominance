using Microsoft.Xna.Framework;
using System;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	// COpied and slimmed down from MonoGame (https://github.com/ManojLakshan/monogame/blob/d5f9892d20bdca1168c5e5718c410d38bc4deeb9/MonoGame.Framework/Matrix.cs)

	// ReSharper disable once InconsistentNaming
	public struct Matrix3x3 : IEquatable<Matrix3x3>
	{
		public Matrix3x3(
			float m11, float m12, float m13, 
			float m21, float m22, float m23, 
			float m31, float m32, float m33)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M31 = m31;
			M32 = m32;
			M33 = m33;
		}
		
		public float M11;
		public float M12;
		public float M13;
		public float M21;
		public float M22;
		public float M23;
		public float M31;
		public float M32;
		public float M33;

		public static Matrix3x3 Identity { get; } = new Matrix3x3(1f, 0f, 0f,
			                                                      0f, 1f, 0f,
			                                                      0f, 0f, 1f);


		// required for OpenGL 2.0 projection matrix stuff
		public static float[] ToFloatArray(Matrix3x3 mat)
		{
			float[] matarray = {
									mat.M11, mat.M12, mat.M13,
									mat.M21, mat.M22, mat.M23,
									mat.M31, mat.M32, mat.M33
								};
			return matarray;
		}
		
		public Vector2 Translation
		{
			get
			{
				return new Vector2(M31, M32);
			}
			set
			{
				M31 = value.X;
				M32 = value.Y;
			}
		}
		
		public static Matrix3x3 Add(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			matrix1.M11 += matrix2.M11;
			matrix1.M12 += matrix2.M12;
			matrix1.M13 += matrix2.M13;
			matrix1.M21 += matrix2.M21;
			matrix1.M22 += matrix2.M22;
			matrix1.M23 += matrix2.M23;
			matrix1.M31 += matrix2.M31;
			matrix1.M32 += matrix2.M32;
			matrix1.M33 += matrix2.M33;

			return matrix1;
		}
		
		public static void Add(ref Matrix3x3 matrix1, ref Matrix3x3 matrix2, out Matrix3x3 result)
		{
			result.M11 = matrix1.M11 + matrix2.M11;
			result.M12 = matrix1.M12 + matrix2.M12;
			result.M13 = matrix1.M13 + matrix2.M13;
			result.M21 = matrix1.M21 + matrix2.M21;
			result.M22 = matrix1.M22 + matrix2.M22;
			result.M23 = matrix1.M23 + matrix2.M23;
			result.M31 = matrix1.M31 + matrix2.M31;
			result.M32 = matrix1.M32 + matrix2.M32;
			result.M33 = matrix1.M33 + matrix2.M33;
		}
		
		public static Matrix3x3 CreateRotationZ(float radians)
		{
			Matrix3x3 returnMatrix = Identity;

			returnMatrix.M11 = (float)Math.Cos(radians);
			returnMatrix.M12 = (float)Math.Sin(radians);
			returnMatrix.M21 = -returnMatrix.M12;
			returnMatrix.M22 = returnMatrix.M11;

			return returnMatrix;
		}
		
		public static void CreateRotationZ(float radians, out Matrix3x3 result)
		{
			result = Identity;

			result.M11 = (float)Math.Cos(radians);
			result.M12 = (float)Math.Sin(radians);
			result.M21 = -result.M12;
			result.M22 = result.M11;
		}

		public static Matrix3x3 CreateScaleTranslation(float transX, float transY, float scaleX, float scaleY)
		{
			/*
			 * result matrix := 
			 *
			 * | scaleX  0      transx  |
			 * | 0       scaleY transy  |
			 * | 0       0      1       |
			 *
			 *  = Scale(scaleX, scaleY) * translate(RealOffsetX, offsetY)
			*/

			return new Matrix3x3
			(
				scaleX, 0,      0,
				0,      scaleY, 0,
				transX, transY, 1
			);
		}
		
		public static Matrix3x3 CreateScale(float scale)
		{
			Matrix3x3 m = Identity;
			m.M11 = m.M22 = scale;
			return m;
		}
		
		public static void CreateScale(float scale, out Matrix3x3 result)
		{
			result = CreateScale(scale);
		}
		
		public static Matrix3x3 CreateScale(float xScale, float yScale)
		{
			Matrix3x3 returnMatrix;
			returnMatrix.M11 = xScale;
			returnMatrix.M12 = 0;
			returnMatrix.M13 = 0;
			returnMatrix.M21 = 0;
			returnMatrix.M22 = yScale;
			returnMatrix.M23 = 0;
			returnMatrix.M31 = 0;
			returnMatrix.M32 = 0;
			returnMatrix.M33 = 1;
			return returnMatrix;
		}
		
		public static void CreateScale(float xScale, float yScale, out Matrix3x3 result)
		{
			result = CreateScale(xScale, yScale);
		}
		
		public static Matrix3x3 CreateScale(Vector2 scales)
		{
			return CreateScale(scales.X, scales.Y);
		}
		
		public static void CreateScale(ref Vector2 scales, out Matrix3x3 result)
		{
			result = CreateScale(scales.X, scales.Y);
		}

		public static Matrix3x3 CreateTranslation(float xPosition, float yPosition)
		{
			Matrix3x3 m = Identity;
			m.M31 = xPosition;
			m.M32 = yPosition;
			return m;
		}
		
		public static void CreateTranslation(ref Vector2 position, out Matrix3x3 result)
		{
			result = CreateTranslation(position.X, position.Y);
		}
		
		public static Matrix3x3 CreateTranslation(Vector2 position)
		{
			return CreateTranslation(position.X, position.Y);
		}
		
		public static void CreateTranslation(float xPosition, float yPosition, out Matrix3x3 result)
		{
			result = CreateTranslation(xPosition, yPosition);
		}
		
		public bool Equals(Matrix3x3 other)
		{
			return
				FloatMath.EpsilonEquals(M11, other.M11) &&
				FloatMath.EpsilonEquals(M12, other.M12) &&
				FloatMath.EpsilonEquals(M13, other.M13) &&
				FloatMath.EpsilonEquals(M21, other.M21) &&
				FloatMath.EpsilonEquals(M22, other.M22) &&
				FloatMath.EpsilonEquals(M23, other.M23) &&
				FloatMath.EpsilonEquals(M31, other.M31) &&
				FloatMath.EpsilonEquals(M32, other.M32) &&
				FloatMath.EpsilonEquals(M33, other.M33);
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Matrix3x3) return Equals((Matrix3x3)obj);

			return false;
		}
		
		public override int GetHashCode()
		{
			throw new NotSupportedException();
		}
		
		public static Matrix3x3 Multiply(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			Matrix3x3 ret;
			Multiply(ref matrix1, ref matrix2, out ret);
			return ret;
		}
		
		public static void Multiply(ref Matrix3x3 matrix1, ref Matrix3x3 matrix2, out Matrix3x3 result)
		{
			result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31;
			result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32;
			result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33;

			result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31;
			result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32;
			result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33;

			result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31;
			result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32;
			result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33;
		}
		
		public static Matrix3x3 Multiply(Matrix3x3 matrix1, float factor)
		{
			matrix1.M11 *= factor;
			matrix1.M12 *= factor;
			matrix1.M13 *= factor;
			matrix1.M21 *= factor;
			matrix1.M22 *= factor;
			matrix1.M23 *= factor;
			matrix1.M31 *= factor;
			matrix1.M32 *= factor;
			matrix1.M33 *= factor;
			return matrix1;
		}
		
		public static void Multiply(ref Matrix3x3 matrix1, float factor, out Matrix3x3 result)
		{
			result.M11 = matrix1.M11 * factor;
			result.M12 = matrix1.M12 * factor;
			result.M13 = matrix1.M13 * factor;
			result.M21 = matrix1.M21 * factor;
			result.M22 = matrix1.M22 * factor;
			result.M23 = matrix1.M23 * factor;
			result.M31 = matrix1.M31 * factor;
			result.M32 = matrix1.M32 * factor;
			result.M33 = matrix1.M33 * factor;
		}
		
		public static Matrix3x3 operator +(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			Add(ref matrix1, ref matrix2, out matrix1);
			return matrix1;
		}
		
		public static bool operator ==(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			return matrix1.Equals(matrix2);
		}
		
		public static bool operator !=(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			return ! matrix1.Equals(matrix2);
		}
		
		public static Matrix3x3 operator *(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			return Multiply(matrix1, matrix2);
		}


		public static Matrix3x3 operator *(Matrix3x3 matrix, float scaleFactor)
		{
			return Multiply(matrix, scaleFactor);
		}

		public Matrix ToMatrix()
		{
			return new Matrix
			(
				M11, M12, M13, 0,
				M21, M22, M23, 0,
				M31, M32, M33, 0,
				0,   0,   0,   1
			);
		}
	}
}
