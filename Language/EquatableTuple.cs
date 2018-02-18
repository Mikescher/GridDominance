using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Language
{
	public static class EquatableTuple
	{
		public static EquatableTuple<T1, T2> Create<T1, T2>(T1 i1, T2 i2) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
		{
			return new EquatableTuple<T1, T2>(i1, i2);
		}

		public static EquatableTuple<T1, T2, T3> Create<T1, T2, T3>(T1 i1, T2 i2, T3 i3) where T1 : IEquatable<T1> where T2 : IEquatable<T2> where T3 : IEquatable<T3>
		{
			return new EquatableTuple<T1, T2, T3>(i1, i2, i3);
		}
	}

	public sealed class EquatableTuple<T1, T2> : IEquatable<EquatableTuple<T1, T2>> where T1 : IEquatable<T1> where T2 : IEquatable<T2>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;

		public EquatableTuple() { }

		public EquatableTuple(T1 i1, T2 i2)
		{
			Item1 = i1;
			Item2 = i2;
		}

		public bool Equals(EquatableTuple<T1, T2> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Item1.Equals(other.Item1) && Item2.Equals(other.Item2);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is EquatableTuple<T1, T2> et) return Equals(et);
			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = EqualityComparer<T1>.Default.GetHashCode(Item1);
				hashCode = (hashCode * 397) ^ EqualityComparer<T2>.Default.GetHashCode(Item2);
				return hashCode;
			}
		}

		public static bool operator ==(EquatableTuple<T1, T2> a, EquatableTuple<T1, T2> b)
		{
			if (a == null) return (b == null);
			return a.Equals(b);
		}

		public static bool operator !=(EquatableTuple<T1, T2> a, EquatableTuple<T1, T2> b)
		{
			if (a == null) return !(b == null);
			return !a.Equals(b);
		}
	}

	public sealed class EquatableTuple<T1, T2, T3> : IEquatable<EquatableTuple<T1, T2, T3>> where T1 : IEquatable<T1> where T2 : IEquatable<T2> where T3 : IEquatable<T3>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;
		public readonly T3 Item3;

		public EquatableTuple() { }

		public EquatableTuple(T1 i1, T2 i2, T3 i3)
		{
			Item1 = i1;
			Item2 = i2;
			Item3 = i3;
		}

		public static EquatableTuple<T1, T2, T3> Create(T1 i1, T2 i2, T3 i3) => new EquatableTuple<T1, T2, T3>(i1, i2, i3);

		public bool Equals(EquatableTuple<T1, T2, T3> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Item1.Equals(other.Item1) && Item2.Equals(other.Item2) && Item3.Equals(other.Item3);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is EquatableTuple<T1, T2, T3> et) return Equals(et);
			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = EqualityComparer<T1>.Default.GetHashCode(Item1);
				hashCode = (hashCode * 397) ^ EqualityComparer<T2>.Default.GetHashCode(Item2);
				hashCode = (hashCode * 397) ^ EqualityComparer<T3>.Default.GetHashCode(Item3);
				return hashCode;
			}
		}

		public static bool operator ==(EquatableTuple<T1, T2, T3> a, EquatableTuple<T1, T2, T3> b)
		{
			if (ReferenceEquals(null, a)) return (b==null);
			return a.Equals(b);
		}

		public static bool operator !=(EquatableTuple<T1, T2, T3> a, EquatableTuple<T1, T2, T3> b)
		{
			if (ReferenceEquals(null, a)) return !(b == null);
			return !a.Equals(b);
		}
	}
	
}
