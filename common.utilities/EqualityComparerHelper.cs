using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.common.utilities;

public static class EqualityComparerHelper<T>
{
	private class CommonEqualityComparer<V> : IEqualityComparer<T>
	{
		private Func<T, V> keySelector;

		private IEqualityComparer<V> comparer;

		public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
		{
			this.keySelector = keySelector;
			this.comparer = comparer;
		}

		public CommonEqualityComparer(Func<T, V> keySelector)
			: this(keySelector, (IEqualityComparer<V>)EqualityComparer<V>.Default)
		{
		}

		public bool Equals(T x, T y)
		{
			return comparer.Equals(keySelector(x), keySelector(y));
		}

		public int GetHashCode(T obj)
		{
			return comparer.GetHashCode(keySelector(obj));
		}
	}

	public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector)
	{
		return new CommonEqualityComparer<V>(keySelector);
	}

	public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector, IEqualityComparer<V> comparer)
	{
		return new CommonEqualityComparer<V>(keySelector, comparer);
	}
}