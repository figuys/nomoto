using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.phonemanager.apps.Model;

namespace lenovo.mbg.service.lmsa.phonemanager.apps.Contract;

internal interface IDeviceApp
{
	SortedList<AppType, List<AppInfo>> GetApps(TcpAndroidDevice currentDevice, int androidApiLevel);

	bool ExportIcon(TcpAndroidDevice currentDevice, List<string> packagenames, string savePath, Action<string, string, bool> callback);

	bool Import(TcpAndroidDevice currentDevice, List<string> apkPath, Action<string, bool> callback);

	int Install(TcpAndroidDevice currentDevice, string apkPath);

	bool Export(TcpAndroidDevice currentDevice, string apk, string savePath, Action<string, bool> callback);

	bool Export(TcpAndroidDevice currentDevice, List<string> apks, string savePath, Action<string, bool> callback, IAsyncTaskContext context);

	void Uninstall(TcpAndroidDevice currentDevice, string packageName, Action<Dictionary<string, bool>> callback);

	void Uninstall(TcpAndroidDevice currentDevice, List<string> packageNames, Action<Dictionary<string, bool>> callback);

	bool Uninstall(TcpAndroidDevice currentDevice, IAsyncTaskContext context, string packageName);

	bool CheckPermissionToGetAppInfo(TcpAndroidDevice currentDevice);
}
