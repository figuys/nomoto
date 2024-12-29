using System;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DataPartitionWrapper
{
	public static void DoProcess<T>(IEnumerable<T> datas, int partitionCount, Func<IEnumerable<T>, bool> callBack)
	{
		int num = 0;
		while (true)
		{
			IEnumerable<T> enumerable = datas.Skip(num * partitionCount).Take(partitionCount);
			if (enumerable.Count() != 0 && callBack(enumerable))
			{
				num++;
				continue;
			}
			break;
		}
	}
}
