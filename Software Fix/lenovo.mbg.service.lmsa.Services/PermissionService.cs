using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.Permissions;

namespace lenovo.mbg.service.lmsa.Services;

public class PermissionService : IPermission
{
	private Permission permission = new Permission();

	public bool? AppModuleIsReady(DeviceEx device, List<string> permissionModuleNames)
	{
		return permission.AppModuleIsReady(device, permissionModuleNames);
	}

	public void BeginAutoCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime = 1000)
	{
		permission.BeginAutoCheckAppIsReady(device, permissionModuleNames, cancelToken, finalResultRepoter, periodTime);
	}

	public void BeginCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, Action<bool?> finalResultRepoter)
	{
		permission.BeginCheckAppIsReady(device, permissionModuleNames, finalResultRepoter);
	}

	public void BeginConfirmAppIsReady(DeviceEx device, List<string> permissionModuleNames, FrameworkElement tipView, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter)
	{
		permission.BeginConfirmAppIsReady(device, permissionModuleNames, tipView, cancelToken, finalResultRepoter);
	}

	public void BeginConfirmAppIsReady(DeviceEx device, string permissionModule, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter)
	{
		permission.BeginConfirmAppIsReady(device, permissionModule, cancelToken, finalResultRepoter);
	}

	public void BeginRollPollingAppModuleIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime = 1000)
	{
		permission.BeginRollPollingAppModuleIsReady(device, permissionModuleNames, cancelToken, finalResultRepoter, periodTime);
	}
}
