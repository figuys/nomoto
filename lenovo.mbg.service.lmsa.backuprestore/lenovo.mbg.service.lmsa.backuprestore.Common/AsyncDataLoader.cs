using System;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.backuprestore.Common;

public class AsyncDataLoader
{
	public static void BeginLoading(Action action, ILoading view)
	{
		object handler = new object();
		if (action == null)
		{
			return;
		}
		action.BeginInvoke(delegate(IAsyncResult ar)
		{
			try
			{
				(ar.AsyncState as Action).EndInvoke(ar);
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					view?.Hiden(handler);
				});
			}
			catch (Exception)
			{
			}
		}, action);
		view?.Show(handler);
	}
}
