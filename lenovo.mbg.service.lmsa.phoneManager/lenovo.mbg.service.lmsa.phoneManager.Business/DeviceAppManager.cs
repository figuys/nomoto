using System.Collections.Generic;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business.Apps;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DeviceAppManager
{
	public int GetThirdAppCount()
	{
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		return new AppServiceRequest(tcpAndroidDevice.ExtendDataFileServiceEndPoint, tcpAndroidDevice.RsaSocketEncryptHelper).Request(18, "getDataCount", null, null).ReadHeader(null).GetInt32("DataCount");
	}

	private List<AppInfoModel> ConvertAppInfo(List<AppInfo> list)
	{
		List<AppInfoModel> appinfos = new List<AppInfoModel>();
		if (list == null || list.Count == 0)
		{
			return appinfos;
		}
		list.ForEach(delegate(AppInfo n)
		{
			AppInfoModel item = new AppInfoModel
			{
				AppName = n.Name,
				Size = n.Size,
				DataSize = n.DataUse,
				Version = n.Version,
				IsSystemApp = n.IsSystem,
				PackageName = n.PackageName,
				AppImage = ImageHandleHelper.LoadBitmap(n.icon)
			};
			appinfos.Add(item);
		});
		return appinfos;
	}

	public SortedList<AppType, List<AppInfoModel>> GetAppInfo(bool refresh)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: not null } tcpAndroidDevice))
		{
			return null;
		}
		SortedList<AppType, List<AppInfoModel>> sortedList = new SortedList<AppType, List<AppInfoModel>>();
		SortedList<AppType, List<AppInfo>> sortedList2 = null;
		int apiLevel = tcpAndroidDevice.Property.ApiLevel;
		sortedList2 = AppsDeviceAppManager.Instance.GetAllDeviceApp(refresh, apiLevel);
		if (sortedList2 != null && sortedList2[AppType.MyApp] != null)
		{
			sortedList.Add(AppType.MyApp, ConvertAppInfo(sortedList2[AppType.MyApp]));
		}
		if (sortedList2 != null && sortedList2[AppType.SystemApp] != null)
		{
			sortedList.Add(AppType.SystemApp, ConvertAppInfo(sortedList2[AppType.SystemApp]));
		}
		return sortedList;
	}

	public List<AppInfoModel> Select(string name, AppType apptype)
	{
		List<AppInfoModel> result = new List<AppInfoModel>();
		AppType apptype2 = ((apptype != 0) ? AppType.SystemApp : AppType.MyApp);
		List<AppInfo> list = AppsDeviceAppManager.Instance.Select(name, apptype2);
		if (list == null || list.Count == 0)
		{
			return result;
		}
		return ConvertAppInfo(list);
	}
}
