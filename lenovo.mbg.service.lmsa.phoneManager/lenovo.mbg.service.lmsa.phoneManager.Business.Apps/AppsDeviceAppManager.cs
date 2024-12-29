using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.phoneManager.Business.Apps;

public class AppsDeviceAppManager
{
	private static AppsDeviceAppManager _instance = null;

	private static object locker = new object();

	private SortedList<AppType, List<AppInfo>> _Cache;

	private Dictionary<string, string> _CacheIconPath;

	private Dictionary<string, string> _CacheMapping = new Dictionary<string, string>();

	public static AppsDeviceAppManager Instance
	{
		get
		{
			lock (locker)
			{
				return (_instance != null) ? _instance : (_instance = new AppsDeviceAppManager());
			}
		}
	}

	private DeviceApp _DeviceApp { get; set; }

	private AppDatabase _AppDatabase { get; set; }

	public SortedList<AppType, List<AppInfo>> Cache => _Cache;

	public int Count
	{
		get
		{
			if (_Cache != null)
			{
				return _Cache.SelectMany((KeyValuePair<AppType, List<AppInfo>> n) => n.Value).Count();
			}
			return 0;
		}
	}

	public Dictionary<string, string> CacheIconPath => _CacheIconPath;

	private AppsDeviceAppManager()
	{
		_DeviceApp = new DeviceApp();
		_AppDatabase = new AppDatabase();
		_Cache = new SortedList<AppType, List<AppInfo>>();
		_CacheIconPath = new Dictionary<string, string>();
	}

	public SortedList<AppType, List<AppInfo>> GetAllDeviceApp(bool refresh, int androidApiLevel)
	{
		if (refresh || _Cache == null || _Cache.Count == 0)
		{
			lock (Instance)
			{
				if (refresh || _Cache == null || _Cache.Count == 0)
				{
					_Cache = _DeviceApp.GetApps(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, androidApiLevel);
				}
			}
		}
		return _Cache;
	}

	public bool CheckPermissionToGetAppInfo()
	{
		return _DeviceApp.CheckPermissionToGetAppInfo(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice);
	}

	public bool UpdateIcon(Action<string, string> callback)
	{
		if (_Cache != null)
		{
			List<string> need = VerifyPreTransferIcon(callback);
			if (need == null || need.Count == 0)
			{
				return true;
			}
			Task.Factory.StartNew(delegate
			{
				try
				{
					ExportIcon(need, Configurations.AppIconCacheDir, delegate(string packagename, string iconpath, bool success)
					{
						_Cache.SelectMany((KeyValuePair<AppType, List<AppInfo>> n) => n.Value).ToList().FirstOrDefault((AppInfo n) => n.PackageName == packagename)
							.icon = iconpath;
						_CacheMapping[packagename] = iconpath;
						callback?.Invoke(packagename, iconpath);
					});
				}
				catch (Exception ex)
				{
					LogHelper.LogInstance.Error("Load app icon failed:" + ex);
				}
				finally
				{
					_AppDatabase.Save(AppDatabase.SavePath, _CacheMapping);
				}
			});
		}
		return true;
	}

	public bool ExportIcon(List<string> packagenames, string savePath, Action<string, string, bool> callback)
	{
		return _DeviceApp.ExportIcon(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, packagenames, savePath, callback);
	}

	public bool InstallApp(string appPath, Action<string, bool> callback)
	{
		return InstallApp(new List<string> { appPath }, callback);
	}

	public bool InstallApp(List<string> appPath, Action<string, bool> callback)
	{
		bool flag = true;
		for (int i = 0; i < appPath.Count; i++)
		{
			List<string> list = new List<string> { appPath[i] };
			if (list == null || list.Count == 0)
			{
				break;
			}
			flag &= _DeviceApp.Import(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, list, callback);
		}
		return flag;
	}

	public int Install(string apkPath, TcpAndroidDevice device)
	{
		return _DeviceApp.Install(device, apkPath);
	}

	public void UninstallApp(string packageName, Action<Dictionary<string, bool>> callback)
	{
		_DeviceApp.Uninstall(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, packageName, callback);
	}

	public void UninstallApp(List<string> packageNames, Action<Dictionary<string, bool>> callback)
	{
		_DeviceApp.Uninstall(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, packageNames, callback);
	}

	public bool Uninstall(IAsyncTaskContext context, string packageName)
	{
		return _DeviceApp.Uninstall(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, context, packageName);
	}

	public List<AppInfo> Sort(AppType apptype, params string[] orders)
	{
		if (_Cache == null || _Cache[apptype] == null)
		{
			return null;
		}
		if (orders == null || orders.Length == 0)
		{
			return _Cache[apptype];
		}
		int num = 1;
		IOrderedEnumerable<AppInfo> source = null;
		foreach (string name in orders)
		{
			PropertyInfo propertyinfo = typeof(AppInfo).GetProperty(name);
			if (propertyinfo == null)
			{
				continue;
			}
			if (num == 1)
			{
				source = _Cache[apptype].OrderBy((AppInfo n) => propertyinfo.GetValue(n));
				num++;
			}
			else
			{
				source = source.ThenBy((AppInfo n) => propertyinfo.GetValue(n));
			}
		}
		return _Cache[apptype] = source.ToList();
	}

	public List<AppInfo> SortDesc(AppType apptype, params string[] orders)
	{
		if (_Cache == null || _Cache[apptype] == null)
		{
			return null;
		}
		if (orders == null || orders.Length == 0)
		{
			return _Cache[apptype];
		}
		int num = 1;
		IOrderedEnumerable<AppInfo> source = null;
		foreach (string name in orders)
		{
			PropertyInfo propertyinfo = typeof(AppInfo).GetProperty(name);
			if (propertyinfo == null)
			{
				continue;
			}
			if (num == 1)
			{
				source = _Cache[apptype].OrderByDescending((AppInfo n) => propertyinfo.GetValue(n));
				num++;
			}
			else
			{
				source = source.ThenByDescending((AppInfo n) => propertyinfo.GetValue(n));
			}
		}
		return _Cache[apptype] = source.ToList();
	}

	public List<AppInfo> Select(string name, AppType apptype)
	{
		if (_Cache == null || _Cache[apptype] == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			return _Cache[apptype];
		}
		string lowerName = name.ToLowerInvariant();
		return (from n in _Cache[apptype]
			where n.Name.ToLowerInvariant().Contains(lowerName)
			orderby n.Name
			select n).ToList();
	}

	public bool Export(List<string> apks, string savePath, Action<string, bool> callback, IAsyncTaskContext context)
	{
		return _DeviceApp.Export(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, apks, savePath, callback, context);
	}

	private List<string> VerifyPreTransferIcon(Action<string, string> callback)
	{
		List<string> list = (from n in _Cache.SelectMany((KeyValuePair<AppType, List<AppInfo>> n) => n.Value)
			select n.PackageName).ToList();
		JObject jObject = null;
		_CacheMapping = new Dictionary<string, string>();
		if (File.Exists(AppDatabase.SavePath))
		{
			jObject = _AppDatabase.Get<JObject>(AppDatabase.SavePath);
		}
		if (jObject != null)
		{
			foreach (JProperty item in jObject.Properties())
			{
				string text = item.Value.ToString();
				if (!_CacheMapping.ContainsKey(item.Name) && File.Exists(text))
				{
					_CacheMapping.Add(item.Name, text);
				}
			}
			List<string> exists = new List<string>();
			_Cache.SelectMany((KeyValuePair<AppType, List<AppInfo>> n) => n.Value).ToList().ForEach(delegate(AppInfo n)
			{
				if (_CacheMapping.ContainsKey(n.PackageName))
				{
					string arg = (n.icon = _CacheMapping[n.PackageName]);
					callback?.Invoke(n.PackageName, arg);
					exists.Add(n.PackageName);
				}
			});
			list = list.Except(exists).ToList();
		}
		return list;
	}

	public void Clear()
	{
		_Cache?.Clear();
		_CacheMapping?.Clear();
	}
}
