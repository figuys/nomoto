using System;

namespace lenovo.mbg.service.framework.devicemgt;

public class AppAssistTipsEventArgs : EventArgs
{
	public AppAssistTips AssistTipsType { get; private set; }

	public object Data { get; private set; }

	public AppAssistTipsEventArgs(AppAssistTips assistTipsType, object data)
	{
		AssistTipsType = assistTipsType;
		Data = data;
	}
}
