using System;

namespace lenovo.mbg.service.framework.services;

public class UserInfoArgs : EventArgs
{
	public UserInfo User { get; }

	public bool online { get; }

	public UserInfoArgs(UserInfo user, bool online)
	{
		User = user;
		this.online = online;
	}
}
