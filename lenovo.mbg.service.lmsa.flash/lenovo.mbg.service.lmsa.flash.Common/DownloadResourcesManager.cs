using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class DownloadResourcesManager
{
	private Dictionary<PackageDownloadModel, Action<string, DownloadStatus>> downlaodTasks;

	private readonly object downlaodTasksLock;

	private static DownloadResourcesManager _resourceManager;

	public Dictionary<string, object> Cache { get; private set; }

	public static DownloadResourcesManager SingleInstance
	{
		get
		{
			if (_resourceManager == null)
			{
				_resourceManager = new DownloadResourcesManager();
			}
			return _resourceManager;
		}
	}

	private DownloadResourcesManager()
	{
		Cache = new Dictionary<string, object>();
		downlaodTasksLock = new object();
		downlaodTasks = new Dictionary<PackageDownloadModel, Action<string, DownloadStatus>>();
		if (FlashContext.SingleInstance.DownloadManager != null)
		{
			FlashContext.SingleInstance.DownloadManager.PackageDownloadSuccessfully += DownloadManager_PackageDownloadSuccessfully;
			FlashContext.SingleInstance.DownloadManager.PackageDownloadPause += DownloadManager_PackageDownloadPause;
			FlashContext.SingleInstance.DownloadManager.PackageDownloading += DownloadManager_PackageDownloading;
			FlashContext.SingleInstance.DownloadManager.PackageUnzipping += DownloadManager_PackageUnzipping;
			FlashContext.SingleInstance.DownloadManager.PackageDownloadFailed += DownloadManager_PackageDownloadFailed;
		}
	}

	private void DownloadManager_PackageUnzipping(object sender, PackageDownloadEventArgs e)
	{
		lock (downlaodTasksLock)
		{
			if (downlaodTasks.ContainsKey(e.Package))
			{
				downlaodTasks[e.Package](e.Package.ModelName, DownloadStatus.UNZIPPING);
			}
		}
	}

	private void DownloadManager_PackageDownloadFailed(object sender, PackageDownloadEventArgs e)
	{
		lock (downlaodTasksLock)
		{
			if (downlaodTasks.ContainsKey(e.Package))
			{
				downlaodTasks[e.Package](e.Package.ModelName, DownloadStatus.FAILED);
			}
		}
	}

	private void DownloadManager_PackageDownloading(object sender, PackageDownloadEventArgs e)
	{
		lock (downlaodTasksLock)
		{
			if (downlaodTasks.ContainsKey(e.Package))
			{
				downlaodTasks[e.Package](e.Package.ModelName, DownloadStatus.DOWNLOADING);
			}
		}
	}

	private void DownloadManager_PackageDownloadPause(object sender, PackageDownloadEventArgs e)
	{
		lock (downlaodTasksLock)
		{
			if (downlaodTasks.ContainsKey(e.Package))
			{
				downlaodTasks[e.Package](e.Package.ModelName, DownloadStatus.MANUAL_PAUSE);
			}
		}
	}

	private void DownloadManager_PackageDownloadSuccessfully(object sender, PackageDownloadEventArgs e)
	{
		lock (downlaodTasksLock)
		{
			if (downlaodTasks.ContainsKey(e.Package))
			{
				downlaodTasks[e.Package](e.Package.ModelName, DownloadStatus.SUCCESS);
			}
		}
	}

	public void DownloadIcon(DownloadInfo icondownloadinfo, Action<string, DownloadStatus> callBack)
	{
		PackageDownloadModel packageDownloadModel = new PackageDownloadModel();
		if (callBack != null)
		{
			lock (downlaodTasksLock)
			{
				downlaodTasks[packageDownloadModel] = callBack;
			}
		}
		packageDownloadModel.Add(icondownloadinfo);
		FlashContext.SingleInstance.DownloadManager.StartDownloading(packageDownloadModel);
	}

	public bool ResourceReadly(List<DownloadInfo> resources)
	{
		try
		{
			foreach (DownloadInfo resource in resources)
			{
				DownloadInfo downloadedResource = FlashContext.SingleInstance.DownloadManager.DownloadService.GetDownloadedResource(resource.FileUrl);
				if (downloadedResource == null)
				{
					return false;
				}
				if (downloadedResource.FileType == "TOOL")
				{
					string text = Path.Combine(downloadedResource.LocalPath, Path.GetFileName(downloadedResource.FileName));
					if (!SevenZipHelper.Instance.CheckExtractorWithPwd(text, downloadedResource.ZipPwd))
					{
						LogHelper.LogInstance.Debug("check zip file failed. Will delete [" + text + "].");
						GlobalFun.TryDeleteFile(text);
						return false;
					}
				}
			}
			return true;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"resource readly exception:[{arg}].");
			return false;
		}
	}

	public void PrepareFlashingResources(string modelname, List<DownloadInfo> resourceinfos, Action<string, DownloadStatus> callBack)
	{
		if (resourceinfos == null || resourceinfos.Count == 0)
		{
			callBack(null, DownloadStatus.UNDEFINEERROR);
			return;
		}
		List<DownloadInfo> list = new List<DownloadInfo>();
		foreach (DownloadInfo resourceinfo in resourceinfos)
		{
			if (FlashContext.SingleInstance.DownloadManager.DownloadService.GetDownloadedResource(resourceinfo.FileUrl) == null)
			{
				list.Add(resourceinfo);
			}
		}
		if (list.Count == 0)
		{
			callBack(modelname, DownloadStatus.SUCCESS);
			return;
		}
		RemoveAfterDownload(modelname);
		PackageDownloadModel packageDownloadModel = new PackageDownloadModel(modelname);
		if (callBack != null)
		{
			lock (downlaodTasksLock)
			{
				downlaodTasks[packageDownloadModel] = callBack;
			}
		}
		foreach (DownloadInfo item in list)
		{
			packageDownloadModel.Add(item);
		}
		FlashContext.SingleInstance.DownloadManager.StartDownloading(packageDownloadModel);
	}

	public void RemoveAfterDownload(string modelName)
	{
		lock (downlaodTasksLock)
		{
			KeyValuePair<PackageDownloadModel, Action<string, DownloadStatus>> keyValuePair = downlaodTasks.FirstOrDefault((KeyValuePair<PackageDownloadModel, Action<string, DownloadStatus>> p) => p.Key.ModelName == modelName);
			if (keyValuePair.Key != null)
			{
				downlaodTasks.Remove(keyValuePair.Key);
			}
		}
	}
}
