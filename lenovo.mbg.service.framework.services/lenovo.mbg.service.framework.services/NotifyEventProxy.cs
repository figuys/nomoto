using System;

namespace lenovo.mbg.service.framework.services;

public class NotifyEventProxy
{
	public Action<string> OnNotifyLanguageChangeAction;

	public void CallNotifyLanguageChangeAction(string param)
	{
		if (OnNotifyLanguageChangeAction != null)
		{
			OnNotifyLanguageChangeAction(param);
		}
	}
}
