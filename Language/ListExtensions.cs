using System;
using System.Collections.Generic;

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
	}
}
