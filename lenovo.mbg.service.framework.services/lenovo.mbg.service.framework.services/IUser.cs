using System;

namespace lenovo.mbg.service.framework.services;

public interface IUser
{
	UserInfo user { get; }

	bool onLine { get; }

	event EventHandler<UserInfoArgs> OnUserChanged;

	void FireUserChanged(UserInfoArgs user);
}
