using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Language
{
	public class CacheCollection<TKey, TValue>
	{
		private readonly Dictionary<TKey, Node> entries;
		private readonly int capacity;
		private Node head;
		private Node tail;

		private class Node
		{
			public Node Next { get; set; }
			public Node Previous { get; set; }
			public TKey Key { get; set; }
			public TValue Value { get; set; }
		}

		public CacheCollection(int capacity = 16)
		{
			if (capacity <= 0)
				throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity should be greater than zero");
			this.capacity = capacity;
			entries = new Dictionary<TKey, Node>();
			head = null;
		}

		public void Add(TKey key, TValue value)
		{
			Node entry;
			if (!entries.TryGetValue(key, out entry))
			{
				entry = new Node { Key = key, Value = value };
				if (entries.Count == capacity)
				{
					entries.Remove(tail.Key);
					tail = tail.Previous;
					if (tail != null) tail.Next = null;
				}
				entries.Add(key, entry);
			}

			entry.Value = value;
			MoveToHead(entry);
			if (tail == null) tail = head;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			value = default(TValue);
			Node entry;
			if (!entries.TryGetValue(key, out entry)) return false;
			MoveToHead(entry);
			value = entry.Value;
			return true;
		}

		private void MoveToHead(Node entry)
		{
			if (entry == head || entry == null) return;

			var next = entry.Next;
			var previous = entry.Previous;

			if (next != null) next.Previous = entry.Previous;
			if (previous != null) previous.Next = entry.Next;

			entry.Previous = null;
			entry.Next = head;

			if (head != null) head.Previous = entry;
			head = entry;

			if (tail == entry) tail = previous;
		}
	}
}
