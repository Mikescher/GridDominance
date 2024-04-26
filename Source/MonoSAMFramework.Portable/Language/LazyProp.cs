using System;

namespace MonoSAMFramework.Portable.Language
{
	public class LazyProp<T>
	{
		private bool initialized;
		private T value;
		private readonly Func<T> calc;

		public LazyProp(Func<T> getter)
		{
			calc = getter;
		}

		public T Get()
		{
			if (!initialized)
			{
				value = calc();
				initialized = true;
			}

			return value;
		}
	}
}
