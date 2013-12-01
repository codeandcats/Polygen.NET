using System;
using System.Collections.Generic;

namespace Polygen
{
	public class StringEqualityComparer : IEqualityComparer<string>
	{
		public StringEqualityComparer(bool ignoreCase)
		{
			this.ignoreCase = ignoreCase;
		}

		private bool ignoreCase = false;

		public bool Equals(string x, string y)
		{
			if (ignoreCase)
				return x.Equals(y, StringComparison.OrdinalIgnoreCase);
			else
				return x.Equals(y);
		}

		public int GetHashCode(string obj)
		{
			if (ignoreCase)
				return obj.ToLower().GetHashCode();
			else
				return obj.GetHashCode();
		}
	}
}