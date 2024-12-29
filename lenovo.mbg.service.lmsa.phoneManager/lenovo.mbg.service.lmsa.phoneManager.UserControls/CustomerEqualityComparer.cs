using System;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

internal class CustomerEqualityComparer<T> : IEqualityComparer<T>
{
	private IEnumerable<Func<T, object>> Selectors { get; }

	public CustomerEqualityComparer(params Func<T, object>[] selectors)
	{
		Selectors = selectors;
	}

	public bool Equals(T left, T right)
	{
		if (left != null && right != null)
		{
			return Selectors.All((Func<T, object> selector) => selector(left).Equals(selector(right)));
		}
		return false;
	}

	public int GetHashCode(T obj)
	{
		return Selectors.Select((Func<T, object> selector) => selector(obj).GetHashCode()).Aggregate(17, (int hashCode, int subHashCode) => hashCode * 31 + subHashCode);
	}
}
