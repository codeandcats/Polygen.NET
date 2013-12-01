using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Polygen
{
	public class RecentItemList<T> : IEnumerable<T>
	{
		public RecentItemList()
		{
		}

		public RecentItemList(IEqualityComparer<T> comparer)
		{
			this.comparer = comparer;
		}

		private List<T> items = new List<T>();
		private IEqualityComparer<T> comparer = null;

		public IEnumerator<T> GetEnumerator()
		{
			for (var index = 0; index < items.Count; index++)
				yield return items[index];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (var index = 0; index < items.Count; index++)
				yield return items[index];
		}

		private int IndexOf(T item)
		{
			for (var index = 0; index < items.Count; index++)
			{
				var loopItem = items[index];

				if (comparer != null)
				{
					if (comparer.Equals(item, loopItem))
						return index;
				}
				else
					if (item.Equals(loopItem))
						return index;
			}

			return -1;
		}

		public T MostRecent
		{
			get
			{
				if (items.Count > 0)
					return items[0];
				else
					return default(T);
			}
			set
			{
				int index = IndexOf(value);

				if (index > -1)
					items.RemoveAt(index);

				items.Insert(0, value);
			}
		}

		public T this[int index]
		{
			get { return items[index]; }
		}

		public int Count { get { return items.Count; } }

		public void Remove(T item)
		{
			int index = IndexOf(item);
			if (index > -1)
				items.RemoveAt(index);
		}

		public void Clear()
		{
			items.Clear();
		}
	}
}