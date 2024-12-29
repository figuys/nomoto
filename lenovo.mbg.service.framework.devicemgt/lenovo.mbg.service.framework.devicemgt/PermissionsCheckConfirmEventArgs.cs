using System;

namespace lenovo.mbg.service.framework.devicemgt;

public class PermissionsCheckConfirmEventArgs : EventArgs
{
	public int CheckPermissionsFailedResult { get; private set; }

	public Func<bool, bool> RequestPermissionsAction { get; private set; }

	public PermissionsCheckConfirmEventArgs(Func<bool, bool> rquestPermissionsAction, int _failedType)
	{
		RequestPermissionsAction = rquestPermissionsAction;
		CheckPermissionsFailedResult = _failedType;
	}
}
