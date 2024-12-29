using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.services;

public interface IPermission
{
	bool? AppModuleIsReady(DeviceEx device, List<string> permissionModuleNames);

	void BeginCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, Action<bool?> finalResultRepoter);

	void BeginRollPollingAppModuleIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime = 1000);

	void BeginAutoCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime = 1000);

	void BeginConfirmAppIsReady(DeviceEx device, List<string> permissionModuleNames, FrameworkElement tipView, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter);

	void BeginConfirmAppIsReady(DeviceEx device, string permissionModule, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter);
}
