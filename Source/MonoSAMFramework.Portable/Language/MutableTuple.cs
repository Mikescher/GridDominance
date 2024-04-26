namespace MonoSAMFramework.Portable.Language
{
	public sealed class MutableTuple<T1>
	{
		public T1 Item1;

		public MutableTuple() {}

		public MutableTuple(T1 i1)
		{
			Item1 = i1;
		}
	}

	public sealed class MutableTuple<T1, T2>
	{
		public T1 Item1;
		public T2 Item2;

		public MutableTuple() { }

		public MutableTuple(T1 i1, T2 i2)
		{
			Item1 = i1;
			Item2 = i2;
		}
	}

	public sealed class MutableTuple<T1, T2, T3>
	{
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;

		public MutableTuple() { }

		public MutableTuple(T1 i1, T2 i2, T3 i3)
		{
			Item1 = i1;
			Item2 = i2;
			Item3 = i3;
		}
	}

	public sealed class MutableTuple<T1, T2, T3, T4>
	{
		public T1 Item1;
		public T2 Item2;
		public T3 Item3;
		public T4 Item4;

		public MutableTuple() { }

		public MutableTuple(T1 i1, T2 i2, T3 i3, T4 i4)
		{
			Item1 = i1;
			Item2 = i2;
			Item3 = i3;
			Item4 = i4;
		}
	}
}
