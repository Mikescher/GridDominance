using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Language
{
	public class AlwaysSortList<T> : List<T>
	{
		private readonly Comparer<T> comparer;

		public AlwaysSortList(Comparer<T> comp)
		{
			comparer = comp;
		}

		public new void Add(T item)
		{
			if (Count == 0)
			{
				//No list items
				base.Add(item);
				return;
			}
			if (comparer.Compare(item, this[Count - 1]) > 0)
			{
				//Bigger than Max
				base.Add(item);
				return;
			}
			int min = 0;
			int max = Count - 1;
			while ((max - min) > 1)
			{
				//Find half point
				int half = min + ((max - min) / 2);
				//Compare if it's bigger or smaller than the current item.
				int comp = comparer.Compare(item, this[half]);
				if (comp == 0)
				{
					//item is equal to half point
					Insert(half, item);
					return;
				}
				else if (comp < 0) max = half;   //item is smaller
				else min = half;   //item is bigger
			}
			if (comparer.Compare(item, this[min]) <= 0) Insert(min, item);
			else Insert(min + 1, item);
		}
	}
}
