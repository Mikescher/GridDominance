using System;
using System.Linq;

namespace MonoSAMFramework.Portable.GameMath
{
	public static class FloatMath
	{
		public const float HALF_PI       = PI/2f;
		public const float PI            = 3.14159265358979323846f;
		public const float TAU           = 6.28318530717958647693f;
		public const float E             = 2.71828182845904523536f;
		public const float SQRT_TWO      = 1.41421356237309504880f;
		public const float SQRT_ONE_HALF = 0.70710678118654752440f;
		public const float SQRT_THREE    = 1.73205080756887729353f;

		public const float EPSILON       = 1E-10f;
		public const float EPSILON7      = 1E-7f;
		public const float EPSILON6      = 1E-6f;
		public const float EPSILON4      = 1.192092896E-4f;

		public const float RadiansToDegrees = 180f / PI;
		public const float RadDeg           = 180f / PI;
		public const float Rad2Deg          = 180f / PI;
		public const float RadToDeg         = 180f / PI;

		public const float DegreesToRadians = PI / 180f;
		public const float DegRad           = PI / 180f;
		public const float Deg2Rad          = PI / 180f;
		public const float DegToRad         = PI / 180f;

		public const float RAD_NEG_360 = -360 * DegRad;
		public const float RAD_NEG_270 = -270 * DegRad;
		public const float RAD_NEG_240 = -240 * DegRad;
		public const float RAD_NEG_225 = -225 * DegRad;
		public const float RAD_NEG_180 = -180 * DegRad;
		public const float RAD_NEG_120 = -120 * DegRad;
		public const float RAD_NEG_090 = -90  * DegRad;
		public const float RAD_NEG_060 = -60  * DegRad;
		public const float RAD_NEG_045 = -45  * DegRad;
		public const float RAD_NEG_030 = -30  * DegRad;
		public const float RAD_NEG_020 = -20  * DegRad;
		public const float RAD_NEG_010 = -10  * DegRad;
		public const float RAD_NEG_005 = -5   * DegRad;
		public const float RAD_NEG_004 = -4   * DegRad;
		public const float RAD_NEG_003 = -3   * DegRad;
		public const float RAD_NEG_002 = -2   * DegRad;
		public const float RAD_NEG_001 = -1   * DegRad;
		public const float RAD_NEG_000 =  0   * DegRad;
		public const float RAD_000     =  0   * DegRad;
		public const float RAD_POS_000 =  0   * DegRad;
		public const float RAD_POS_001 = +1   * DegRad;
		public const float RAD_POS_002 = +2   * DegRad;
		public const float RAD_POS_003 = +3   * DegRad;
		public const float RAD_POS_004 = +4   * DegRad;
		public const float RAD_POS_005 = +5   * DegRad;
		public const float RAD_POS_010 = +10  * DegRad;
		public const float RAD_POS_015 = +15  * DegRad;
		public const float RAD_POS_020 = +20  * DegRad;
		public const float RAD_POS_030 = +30  * DegRad;
		public const float RAD_POS_045 = +45  * DegRad;
		public const float RAD_POS_060 = +60  * DegRad;
		public const float RAD_POS_090 = +90  * DegRad;
		public const float RAD_POS_120 = +120 * DegRad;
		public const float RAD_POS_135 = +135 * DegRad;
		public const float RAD_POS_180 = +180 * DegRad;
		public const float RAD_POS_225 = +225 * DegRad;
		public const float RAD_POS_240 = +240 * DegRad;
		public const float RAD_POS_270 = +270 * DegRad;
		public const float RAD_POS_288 = +288 * DegRad;
		public const float RAD_POS_300 = +300 * DegRad;
		public const float RAD_POS_306 = +306 * DegRad;
		public const float RAD_POS_315 = +315 * DegRad;
		public const float RAD_POS_324 = +324 * DegRad;
		public const float RAD_POS_342 = +342 * DegRad;
		public const float RAD_POS_360 = +360 * DegRad;

		public static readonly Random Random = new Random();

		public static float Asin(float value)
		{
			return (float)Math.Asin(value);
		}

		public static float Sin(float value)
		{
			return (float)Math.Sin(value);
		}

		public static float AbsSin(float value)
		{
			return (float)Math.Abs(Math.Sin(value));
		}

		public static float PercSin(float value) // sinus from 0->1->0->1...
		{
			return ((float)Math.Sin(value) + 1f) / 2f;
		}

		public static float Sinh(float value)
		{
			return (float)Math.Sinh(value);
		}

		public static float Acos(float value)
		{
			return (float)Math.Acos(value);
		}

		public static float Cos(float value)
		{
			return (float)Math.Cos(value);
		}

		public static float Cosh(float value)
		{
			return (float)Math.Cosh(value);
		}

		public static float Atan(float value)
		{
			return (float)Math.Atan(value);
		}

		public static float Atan2(float y, float x)
		{
			// Fun Fact: Atan(0,0)=0, because the C standard says so
			return (float)Math.Atan2(y, x);
		}

		public static float PositiveAtan2(float y, float x)
		{
			var r = (float)Math.Atan2(y, x) + TAU;

			if (r >= TAU) r -= TAU;
			if (r < 0) r = 0;

			return r;
		}

		public static float Tan(float value)
		{
			return (float)Math.Tan(value);
		}

		public static float Tanh(float value)
		{
			return (float)Math.Tanh(value);
		}

		public static float Abs(float value)
		{
			return Math.Abs(value);
		}

		public static int Ceiling(float value)
		{
			return (int)Math.Ceiling(value);
		}

		public static int Floor(float value)
		{
			return (int)Math.Floor(value);
		}

		public static int Round(float value)
		{
			return (int)Math.Round(value);
		}

		public static float Round(float value, int digits)
		{
			return (float)Math.Round(value, digits);
		}

		public static float Sqrt(float value)
		{
			return (float)Math.Sqrt(value);
		}

		public static float Log(float value, float newBase)
		{
			return (float)Math.Log(value, newBase);
		}

		public static float Log(float value)
		{
			return (float)Math.Log(value);
		}

		public static float Log10(float value)
		{
			return (float)Math.Log10(value);
		}

		public static float Log2(float value)
		{
			return Log(value, 2);
		}

		public static float Exp(float value)
		{
			return (float)Math.Exp(value);
		}

		public static float Pow(float x, float y)
		{
			return (float)Math.Pow(x, y);
		}

		public static float Pow2(float x)
		{
			return x * x;
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

		public static int Min(int val1, int val2)
		{
			if (val1 < val2) return val1; else return val2;
		}

		public static int Min(int val1, int val2, int val3)
		{
			return Min(val1, Min(val2, val3));
		}

		public static int Min(int val1, params int[] valTail)
		{
			return valTail.Aggregate(val1, Min);
		}

		public static int Max(int val1, int val2)
		{
			if (val1 > val2) return val1; else return val2;
		}

		public static int Max(int val1, int val2, int val3)
		{
			return Max(val1, Max(val2, val3));
		}

		public static int Max(int val1, params int[] valTail)
		{
			return valTail.Aggregate(val1, Max);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min) return min;
			if (value > max) return max;
			return value;
		}

		public static int IClamp(int value, int min, int max)
		{
			if (value < min) return min;
			if (value >= max) return max-1;
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

		public static int SignNonZero(float value, int zeroV)
		{
			if (value < 0)
				return -1;

			if (value > 0)
				return 1;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (value == 0) return zeroV;

			// value is NaN
			throw new ArithmeticException();
		}

		public static bool IsZero(float value, float tolerance)
		{
			return Abs(value) <= tolerance;
		}

		public static bool IsEpsilonZero(float value)
		{
			return Abs(value) <= EPSILON;
		}

		public static bool IsEpsilonOne(float value)
		{
			return Abs(value-1) <= EPSILON;
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

		public static float DiffRadiansAbs(float a1, float a2) // [a1 <-> a2]
		{
			return Abs(DiffModulo(a1, a2, TAU));
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

		public static bool ProgressDec(ref float value, float dec)
		{
			if (dec < 0) { ProgressInc(ref value, -dec); return false; }

			value -= dec;
			if (value < 0) { value = 0; return true; }
			return false;
		}

		public static bool ProgressInc(ref float value, float dec)
		{
			if (dec < 0) { ProgressDec(ref value, -dec); return false; }

			value += dec;
			if (value > 1) { value = 1; return true; }
			return false;
		}

		public static float GetRangedRandom(float min, float max)
		{
			return (float)(Random.NextDouble() * (max - min) + min);
		}

		public static float GetRangedRandom(float max)
		{
			return (float)(Random.NextDouble() * max);
		}

		public static int GetRangedIntRandom(int min, int max)
		{
			return min + Random.Next(max - min);
		}

		public static int GetRangedIntRandom(int max)
		{
			return Random.Next(max);
		}

		public static int GetRandomSign()
		{
			return (Random.Next() % 2) * 2 - 1; // returns -1 or +1
		}

		public static float GetRandom()
		{
			return (float) Random.NextDouble();
		}

		public static bool EpsilonEquals(float a, float b)
		{
			return Math.Abs(a - b) < EPSILON;
		}

		public static bool EpsilonEquals(float a, float b, float eps)
		{
			return Math.Abs(a - b) < eps;
		}

		public static bool EpsilonInequals(float a, float b)
		{
			return Math.Abs(a - b) > EPSILON;
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

			return value % max;
		}

		public static float IncRadians(float value, float add)
		{
			value += add;

			if (value < 0) return TAU - (value % TAU);

			return value % TAU;
		}

		/// no easing, no acceleration
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseLinear(float t) => t;

		/// accelerating from zero velocity
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInQuad(float t) => t * t;

		/// decelerating to zero velocity
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseOutQuad(float t) => t * (2 - t);

		/// acceleration until halfway, then deceleration
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInOutQuad(float t) => t < .5 ? 2 * t * t : -1 + (4 - 2 * t) * t;

		/// accelerating from zero velocity 
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInCubic(float t) => t * t * t;

		/// decelerating to zero velocity 
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseOutCubic(float t) => (t - 1) * (t - 1) * (t - 1) + 1;

		/// acceleration until halfway, then deceleration 
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInOutCubic(float t) => t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

		/// accelerating from zero velocity 
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInQuart(float t) => t * t * t * t;

		/// decelerating to zero velocity 
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseOutQuart(float t) => 1 - (t - 1) * (t - 1) * (t - 1) * (t - 1);

		/// acceleration until halfway, then deceleration
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInOutQuart(float t) => t < .5 ? 8 * t * t * t * t : 1 - 8 * (t - 1) * (t - 1) * (t - 1) * (t - 1);

		/// accelerating from zero velocity
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInQuint(float t) => t * t * t * t * t;

		/// decelerating to zero velocity
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseOutQuint(float t) => 1 + (t - 1) * (t - 1) * (t - 1) * (t - 1) * (t - 1);

		/// acceleration until halfway, then deceleration 
		/// https://gist.github.com/gre/1650294
		public static float FunctionEaseInOutQuint(float t) => t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (t - 1) * (t - 1) * (t - 1) * (t - 1) * (t - 1);

		/// accelerating from zero velocity 
		/// http://robertpenner.com/easing/
		public static float FunctionEaseInSine(float t) => FloatMath.Sin(t * FloatMath.PI / 2);

		/// decelerating to zero velocity 
		/// http://robertpenner.com/easing/
		public static float FunctionEaseOutSine(float t) => 1 + FloatMath.Sin((t - 1) * FloatMath.PI / 2);

		/// acceleration until halfway, then deceleration 
		/// http://robertpenner.com/easing/
		public static float FunctionEaseInOutSine(float t) => (1 - FloatMath.Cos(FloatMath.PI * t)) / 2;

		public static float FunctionEaseOutElastic(float t, float power = 0.3f, float bounces = 2)
		{
			t *= 0.175f * bounces + 0.0875f;
			return Pow(2, -10 * t) * Sin((t - power / 4) * (2 * PI) / power) + 1;
		}

		/// accelerating from zero velocity 
		/// http://robertpenner.com/easing/
		public static float FunctionEaseInExpo(float t, float p = 10f)
		{
			var s = FloatMath.Pow(2, -p);
			return (FloatMath.Pow(2, p * (t - 1)) - s) / (1 - s);
		}

		/// decelerating to zero velocity 
		/// http://robertpenner.com/easing/
		public static float FunctionEaseOutExpo(float t, float p = 10)
		{
			return (1 - FloatMath.Pow(2, -p * t)) / (1 - FloatMath.Pow(2, -p));
		}

		/// acceleration until halfway, then deceleration 
		/// http://robertpenner.com/easing/
		public static float FunctionEaseInOutExpo(float t, float p = 10)
		{
			var s = FloatMath.Pow(2, -2 * p) / 2;
			var e = 1 - FloatMath.Pow(2, -p) / 2;
			if (t < 0.5) return (FloatMath.Pow(2, p * (2 * t - 1)) / 2 - s) / (e - s);
			return (1 - FloatMath.Pow(2, -p * (2 * t - 1)) / 2 - s) / (e - s);
		}

		public static float FunctionCustomBounce(float t)
		{
			var t2 = t  * t;
			var t4 = t2 * t2;
			var t8 = t4 * t4;
			return (20.5f * t8 * t2 + -69 * t8 + 87 * t4 * t2 + -50 * t4 + 12.5f * t2);
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

		public static void Swap(ref int a, ref int b)
		{
			int tmp = a;
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

		public static float PythSquared(float a, float b) => a*a + b*b;

		public static float GetNearestAngleRepresentation(float reference, float angle)
		{
			reference = NormalizeAngle(reference);
			angle = NormalizeAngle(angle);

			var dn0 = Abs(reference - angle);
			var dp1 = Abs(reference - (angle + TAU));
			var dn1 = Abs(reference - (angle - TAU));

			if (dp1 < dn0 && dp1 < dn1) return angle + TAU;
			if (dn1 < dn0 && dn1 < dp1) return angle - TAU;
			return angle;
		}
	}
}