using System;
using lenovo.mbg.service.lmsa.Login.Model;
using lenovo.mbg.service.lmsa.Login.Protocol;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class OnlineUserChangedEventArgs : EventArgs
{
	public DateTime Timestamp { get; private set; }

	public bool IsOnline { get; private set; }

	public OnlineUserInfo UserInfo { get; private set; }

	public UserLoginFormData UserLoginFormData { get; private set; }

	public OnlineUserChangedEventArgs(OnlineUserInfo userInfo, UserLoginFormData formData, bool isOnline, DateTime timestamp)
	{
		Timestamp = timestamp;
		IsOnline = isOnline;
		UserInfo = userInfo;
		UserLoginFormData = formData;
	}
}
