using System;
using System.Linq;

namespace MonoSAMFramework.Portable.GameMath
{
	public static class FloatMath
	{
		public const float PI = 3.1415927f;
		public const float TAU = PI * 2;
		public const float E = 2.7182818f;

		public const float EPSILON = 1E-10f;

		public const float RadiansToDegrees = 180f / PI;
		public const float RadDeg = RadiansToDegrees;

		public const float DegreesToRadians = PI / 180;
		public const float DegRad = DegreesToRadians;
		
		public const float RAD_NEG_360 = -360 * DegRad;
		public const float RAD_NEG_270 = -270 * DegRad;
		public const float RAD_NEG_240 = -240 * DegRad;
		public const float RAD_NEG_225 = -225 * DegRad;
		public const float RAD_NEG_180 = -180 * DegRad;
		public const float RAD_NEG_120 = -120 * DegRad;
		public const float RAD_NEG_090 = -90  * DegRad;
		public const float RAD_NEG_060 = -60  * DegRad;
		public const float RAD_NEG_045 = -45  * DegRad;
		public const float RAD_000     =  0   * DegRad;
		public const float RAD_POS_045 = +45  * DegRad;
		public const float RAD_POS_060 = +60  * DegRad;
		public const float RAD_POS_090 = +90  * DegRad;
		public const float RAD_POS_120 = +120 * DegRad;
		public const float RAD_POS_180 = +180 * DegRad;
		public const float RAD_POS_225 = +225 * DegRad;
		public const float RAD_POS_240 = +240 * DegRad;
		public const float RAD_POS_270 = +270 * DegRad;
		public const float RAD_POS_360 = +360 * DegRad;

		public static readonly Random Random = new Random();

		public static float Asin(float value)
		{
			return (float)System.Math.Asin(value);
		}

		public static float Sin(float value)
		{
			return (float)System.Math.Sin(value);
		}

		public static float Sinh(float value)
		{
			return (float)System.Math.Sinh(value);
		}

		public static float Acos(float value)
		{
			return (float)System.Math.Acos(value);
		}

		public static float Cos(float value)
		{
			return (float)System.Math.Cos(value);
		}

		public static float Cosh(float value)
		{
			return (float)System.Math.Cosh(value);
		}

		public static float Atan(float value)
		{
			return (float)System.Math.Atan(value);
		}

		public static float Atan2(float y, float x)
		{
			return (float)System.Math.Atan2(y, x);
		}

		public static float PositiveAtan2(float y, float x)
		{
			var r = (float)System.Math.Atan2(y, x) + TAU;

			if (r >= TAU) r -= TAU;
			if (r < 0) r = 0;

			return r;
		}

		public static float Tan(float value)
		{
			return (float)System.Math.Tan(value);
		}

		public static float Tanh(float value)
		{
			return (float)System.Math.Tanh(value);
		}

		public static float Abs(float value)
		{
			return System.Math.Abs(value);
		}

		public static int Ceiling(float value)
		{
			return (int)System.Math.Ceiling(value);
		}

		public static int Floor(float value)
		{
			return (int)System.Math.Floor(value);
		}

		public static int Round(float value)
		{
			return (int)System.Math.Round(value);
		}

		public static float Round(float value, int digits)
		{
			return (float)System.Math.Round(value, digits);
		}

		public static float Sqrt(float value)
		{
			return (float)System.Math.Sqrt(value);
		}

		public static float Log(float value, float newBase)
		{
			return (float)System.Math.Log(value, newBase);
		}

		public static float Log(float value)
		{
			return (float)System.Math.Log(value);
		}

		public static float Log10(float value)
		{
			return (float)System.Math.Log10(value);
		}

		public static float Log2(float value)
		{
			return Log(value, 2);
		}

		public static float Exp(float value)
		{
			return (float)System.Math.Exp(value);
		}

		public static float Pow(float x, float y)
		{
			return (float)System.Math.Pow(x, y);
		}

		public static float Min(float val1, float val2)
		{
			if (val1 < val2)
				return val1;

			return float.IsNaN(val1) ? val1 : val2;
		}

		public static float Min(float val1, float val2, float val3)
		{
			return Min(val1, Min(val2, val3));
		}

		public static float Min(float val1, params float[] valTail)
		{
			return valTail.Aggregate(val1, Min);
		}

		public static float Max(float val1, float val2)
		{
			if (val1 > val2)
				return val1;

			return float.IsNaN(val1) ? val1 : val2;
		}

		public static float Max(float val1, float val2, float val3)
		{
			return Max(val1, Max(val2, val3));
		}

		public static float Max(float val1, params float[] valTail)
		{
			return valTail.Aggregate(val1, Max);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min) return min;
			if (value > max) return max;
			return value;
		}

		public static float Lerp(float start, float end, float percentage)
		{
			return start + (end - start) * percentage;
		}

		public static int Sign(float value)
		{
			if (value < 0)
				return -1;

			if (value > 0)
				return 1;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (value == 0)
				return 0;

			// value is NaN
			throw new ArithmeticException();
		}

		public static bool IsZero(float value, float tolerance)
		{
			return Abs(value) <= tolerance;
		}

		public static bool IsZero(float value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return value == 0;
		}

		public static bool IsNotZero(float value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return value != 0;
		}

		public static bool IsOne(float value, float tolerance)
		{
			return Abs(value - 1) <= tolerance;
		}

		public static bool IsOne(float value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return value == 1;
		}

		public static bool IsNotOne(float value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return value != 1;
		}

		public static float ToDegree(float radians)
		{
			return radians * RadiansToDegrees;
		}

		public static float ToRadians(float degree)
		{
			return degree * DegreesToRadians;
		}

		public static float DiffRadians(float a1, float a2) // [a1 - a2]
		{
			return DiffModulo(a1, a2, TAU);
		}

		public static float DiffModulo(float a1, float a2, float mod) // [a1 - a2]
		{
			if (a1 > a2)
			{
				float diff = a1 - a2;
				if (Abs(diff) < mod / 2) return diff;
				if (diff <= -mod / 2) return diff + mod;
				if (diff >= +mod / 2) return diff - mod;

				throw new ArgumentException();
			}
			else if (a2 > a1)
			{
				float diff = a1 - a2;
				if (Abs(diff) < mod / 2) return diff;
				if (diff <= -mod / 2) return diff + mod;
				if (diff >= +mod / 2) return diff - mod;

				throw new ArgumentException();
			}
			else
			{
				return 0f;
			}
		}

		public static float AddRads(float a, float b)
		{
			float r = a + b;

			while (r < 0) r += TAU;
			while (r >= TAU) r -= TAU;

			return r;
		}

		public static float LimitedDec(float value, float dec, float limit)
		{
			if (dec < 0) return LimitedInc(value, -dec, limit);

			value -= dec;
			if (value < limit) value = limit;
			return value;
		}

		public static float LimitedDec(float value, float dec, float limit, out bool limitHit)
		{
			if (dec < 0) return LimitedInc(value, -dec, limit, out limitHit);

			value -= dec;
			if (value <= limit)
			{
				limitHit = true;
				return limit;
			}

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			limitHit = (value == limit);
			return value;
		}

		public static float LimitedInc(float value, float inc, float limit)
		{
			if (inc < 0) return LimitedDec(value, -inc, limit);

			value += inc;
			if (value > limit) value = limit;
			return value;
		}

		public static float LimitedInc(float value, float inc, float limit, out bool limitHit)
		{
			if (inc < 0) return LimitedDec(value, -inc, limit, out limitHit);

			value += inc;
			if (value >= limit)
			{
				limitHit = true;
				return limit;
			}

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			limitHit = (value == limit);
			return value;
		}

		public static float GetRangedRandom(float min, float max)
		{
			return (float)(Random.NextDouble() * (max - min) + min);
		}

		public static float GetRangedRandom(float max)
		{
			return (float)(Random.NextDouble() * max);
		}

		public static int GetRandomSign()
		{
			return (Random.Next()%2)*2 - 1; // returns -1 or +1
		}

		public static bool EpsilonEquals(float a, float b)
		{
			return Math.Abs(a - b) < EPSILON;
		}

		public static bool FloatEquals(float a, float b)
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			return (a == b);
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		public static bool FloatEquals(float a, float b, float c)
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			return (a == b) && (b == c);
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		public static bool FloatEquals(float a, float b, float c, float d)
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			return (a == b) && (b == c) && (c == d);
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		public static bool FloatInequals(float a, float b)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return (a != b);
		}

		public static float IncModulo(float value, float add, float max)
		{
			value += add;

			if (value < 0) return max - (value % max);

			return value%max;
		}

		public static float FunctionEaseOutElastic(float t, float power = 0.3f, float bounces = 2)
		{
			t *= 0.175f * bounces + 0.0875f;
			return Pow(2, -10 * t) * Sin((t - power / 4) * (2 * PI) / power) + 1;
		}

		/// <summary>
		/// 
		///         ^
		///         |
		///         |
		///         |
		///    /|  /|  /|  /|  /|  /|  /
		///   / | / | / | / | / | / | /
		///  /  |/  |/  |/  |/  |/  |/
		/// --------+----------------------->
		/// 
		/// </summary>
		public static float PositiveModulo(float value, float mod)
		{
			value %= mod;
			if (value < 0)
				return mod + value;
			else
				return value;
		}

		public static float NormalizeAngle(float radians)
		{
			radians %= TAU;
			if (radians < 0)
				return TAU + radians;
			else
				return radians;
		}
		
		public static void Swap(ref float a, ref float b)
		{
			float tmp = a;
			a = b;
			b = tmp;
		}

		public static bool ArcContainsAngle(float aStart, float aEnd, float angle) // [aStart -> AEnd] is Clockwise
		{
			angle -= aStart;
			aEnd -= aStart;

			angle = NormalizeAngle(angle);

			if (aEnd >= TAU) return true; // Full Circle
			
			return angle >= 0 && angle <= aEnd;
		}
	}
}