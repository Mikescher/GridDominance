using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	[DebuggerDisplay("{" + nameof(DebugDisplayString) + ",nq}")]
	public struct FMargin : IEquatable<FMargin>
	{
		public static readonly FMargin NONE = new FMargin(0, 0, 0, 0);

		[DataMember]
		public readonly float MarginTop;
		[DataMember]
		public readonly float MarginRight;
		[DataMember]
		public readonly float MarginBottom;
		[DataMember]
		public readonly float MarginLeft;

		public float SumX => MarginLeft + MarginRight;
		public float SumY => MarginTop  + MarginBottom;
		
		public FMargin(float t, float r, float b, float l)
		{
			MarginTop    = t;
			MarginRight  = r;
			MarginBottom = b;
			MarginLeft   = l;
		}
		
		public FMargin(float v)
		{
			MarginTop    = v;
			MarginRight  = v;
			MarginBottom = v;
			MarginLeft   = v;
		}

		[Pure]
		public static bool operator ==(FMargin a, FMargin b)
		{
			return
				FloatMath.EpsilonEquals(a.MarginTop, b.MarginTop) &&
				FloatMath.EpsilonEquals(a.MarginRight, b.MarginRight) &&
				FloatMath.EpsilonEquals(a.MarginBottom, b.MarginBottom) &&
				FloatMath.EpsilonEquals(a.MarginLeft, b.MarginLeft);
		}

		[Pure]
		public static bool operator !=(FMargin a, FMargin b)
		{
			return !(a == b);
		}

		[Pure]
		public override bool Equals(object obj)
		{
			return obj is FMargin m && this == m;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = MarginTop.GetHashCode();
				hashCode = (hashCode * 397) ^ MarginRight.GetHashCode();
				hashCode = (hashCode * 397) ^ MarginBottom.GetHashCode();
				hashCode = (hashCode * 397) ^ MarginLeft.GetHashCode();
				return hashCode;
			}
		}

		[Pure]
		public override string ToString()
		{
			return $"{{N:{MarginTop} E:{MarginRight} S:{MarginBottom} W:{MarginLeft}}}";
		}

		internal string DebugDisplayString => $"{{N:{MarginTop} E:{MarginRight} S:{MarginBottom} W:{MarginLeft}}}";

		[Pure]
		public bool Equals(FMargin other)
		{
			return this == other;
		}
	}
}
