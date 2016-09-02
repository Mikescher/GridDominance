using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class LinqExtensions
	{
		/// http://stackoverflow.com/a/648240/1761622
		public static T Random<T>(this IEnumerable<T> source, Random rng)
		{
			T current = default(T);
			int count = 0;
			foreach (T element in source)
			{
				count++;
				if (rng.Next(count) == 0)
				{
					current = element;
				}
			}
			if (count == 0)
			{
				throw new InvalidOperationException("Sequence was empty");
			}
			return current;
		}

		/// http://stackoverflow.com/a/648240/1761622
		public static T RandomOrDefault<T>(this IEnumerable<T> source, Random rng)
		{
			T current = default(T);
			int count = 0;

			foreach (T element in source)
				if (rng.Next(++count) == 0) current = element;

			return current;
		}
		
		public static T RandomOrDefault<T>(this IEnumerable<T> source, ConstantRandom crng)
		{
			T current = default(T);
			int count = 0;

			crng.Reseed();
			foreach (T element in source)
				if (crng.Next(++count) == 0) current = element;

			return current;
		}

		/// <summary>
		/// All elements that have the smallest value of 'compareFunc'
		/// with a tolerance of epsilon
		/// </summary>
		public static IEnumerable<T> WhereSmallestBy<T>(this IEnumerable<T> source, Func<T, float> compareFunc, float epsilon)
		{
			var s = source.ToList();

			if (!s.Any()) return Enumerable.Empty<T>();

			var min = s.Min(compareFunc);

			return s.Where(p => compareFunc(p) - epsilon <= min);
		}
	}
}
