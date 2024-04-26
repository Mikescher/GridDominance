using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonoSAMFramework.Portable.Input
{
	public class KeyCombinationList : IEnumerable<KeyCombination>
	{
		public readonly IReadOnlyCollection<KeyCombination> KeyCombinations;

		public KeyCombinationList(KeyCombination a, params KeyCombination[] args)
		{
			KeyCombinations = new ReadOnlyCollection<KeyCombination>(new[] { a }.Concat(args).ToList());
		}

		public IEnumerator<KeyCombination> GetEnumerator()
		{
			return KeyCombinations.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return KeyCombinations.GetEnumerator();
		}
	}
}
