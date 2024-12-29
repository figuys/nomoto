using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public class CompareObject<T> : IEqualityComparer<T>
{
	public bool Equals(T x, T y)
	{
		return ((Expression<Func<T, T, bool>>)((T x, T y) => (object)x == (object)y)).Compile()(x, y);
	}

	public int GetHashCode(T obj)
	{
		return ((Expression<Func<T, int>>)((T obj) => (int)obj)).Compile()(obj);
	}
}
