using System;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.Services;

public class User : IUser
{
	public UserInfo user { get; private set; }

	public bool onLine { get; private set; }

	public event EventHandler<UserInfoArgs> OnUserChanged;

	public void FireUserChanged(UserInfoArgs user)
	{
		this.user = user.User;
		onLine = user.online;
		EventHandler<UserInfoArgs> eventHandler = this.OnUserChanged;
		if (eventHandler != null)
		{
			Delegate[] invocationList = eventHandler.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				EventHandler<UserInfoArgs> eventHandler2 = (EventHandler<UserInfoArgs>)invocationList[i];
				eventHandler2(this, user);
			}
		}
	}
}
