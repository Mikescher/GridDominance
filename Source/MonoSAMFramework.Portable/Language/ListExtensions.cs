using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Language
{
	public static class ListExtensions
	{
		public static IEnumerable<T> SkipLastN<T>(this IEnumerable<T> source, int n)
		{
			using (var it = source.GetEnumerator())
			{
				bool hasRemainingItems = false;
				var cache = new Queue<T>(n + 1);

				do
				{
					// ReSharper disable once AssignmentInConditionalExpression
					if (hasRemainingItems = it.MoveNext())
					{
						cache.Enqueue(it.Current);
						if (cache.Count > n)
							yield return cache.Dequeue();
					}
				} while (hasRemainingItems);
			}
		}

		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null) throw new ArgumentNullException(nameof(items));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			int retVal = 0;
			foreach (var item in items)
			{
				if (predicate(item)) return retVal;
				retVal++;
			}
			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> items, T item)
		{
			return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
		}

		/// <summary>
		/// Take n items from Enumerable 
		/// not the first n items but n items uniformly distributed over all elements:
		/// 
		///  = TakeUniformlyDistributed(8)
		/// 
		///  0    1    2    3    4    5    6    7
		///  v    v    v    v    v    v    v    v       ( -> output )
		/// |....................................|      ( -> input  )
		/// 
		/// </summary>
		public static IEnumerable<T> TakeUniformlyDistributed<T>(this IEnumerable<T> source, int count)
		{
			var data = source.ToList();

			if (data.Count <= count)
			{
				foreach (var d in data) yield return d;
				yield break;
			}

			if (data.Count == 0) yield break;

			if (count == 1)
			{
				yield return data[data.Count / 2];
				yield break;
			}

			var step = (data.Count * 1d - 1) / (count-1);

			for (int i = 0; i < count; i++)
			{
				var idx = (int)(i * step);
				if (idx < 0) idx = 0;
				if (idx > data.Count-1) idx = data.Count-1;

				yield return data[idx];
			}
		}
	}
}
