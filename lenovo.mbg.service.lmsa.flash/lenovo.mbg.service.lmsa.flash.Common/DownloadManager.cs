using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class DownloadManager
{
	private class PackageDownloadTask
	{
		private DownloadManager _outer;

		public PackageDownloadModel Package { get; private set; }

		public PackageDownloadTask(DownloadManager outer, PackageDownloadModel package)
		{
			_outer = outer;
			Package = package;
		}

		public void StartDownloading()
		{
			foreach (DownloadInfo resource in Package.Resources)
			{
				_outer.DownloadService.Add(resource);
			}
		}

		public void HandleDownloadResult(RemoteDownloadStatusEventArgs resource)
		{
			string url = resource.FileUrl;
			if (Package.Resources.FirstOrDefault((DownloadInfo n) => n.FileUrl.Equals(url)) == null)
			{
				return;
			}
			LogHelper.LogInstance.Debug($"rescue download {url}: {resource.Status}");
			int downloadingCount = 0;
			int unzippingCount = 0;
			int successCount = 0;
			Package.Resources.ForEach(delegate(DownloadInfo n)
			{
				DownloadInfo downloadInfo = _outer.DownloadService.GetDownloadingResource(n.FileUrl);
				if (downloadInfo == null)
				{
					downloadInfo = _outer.DownloadService.GetDownloadedResource(n.FileUrl);
					if (downloadInfo != null)
					{
						successCount++;
					}
				}
				if (downloadInfo != null && downloadInfo.Status == DownloadStatus.DOWNLOADING)
				{
					downloadingCount++;
				}
				else if (downloadInfo != null && downloadInfo.Status == DownloadStatus.UNZIPPING)
				{
					unzippingCount++;
				}
			});
			if (downloadingCount > 0)
			{
				_outer.OnPackageDownloading(_outer, new PackageDownloadEventArgs(Package));
			}
			else if (unzippingCount > 0)
			{
				_outer.OnPackageUnzipping(_outer, new PackageDownloadEventArgs(Package));
			}
			else if (successCount == Package.Resources.Count)
			{
				_outer.OnPackageDownloadSuccessfully(_outer, new PackageDownloadEventArgs(Package));
			}
			else
			{
				_outer.OnPackageDownloadFailed(_outer, new PackageDownloadEventArgs(Package));
			}
		}
	}

	private List<PackageDownloadTask> DownloadingTasks;

	private object locktask = new object();

	public IFileDownload DownloadService { get; private set; }

	public List<PackageDownloadModel> Packages { get; private set; }

	public event EventHandler<PackageDownloadEventArgs> PackageDownloadSuccessfully;

	public event EventHandler<PackageDownloadEventArgs> PackageDownloadFailed;

	public event EventHandler<PackageDownloadEventArgs> PackageDownloading;

	public event EventHandler<PackageDownloadEventArgs> PackageUnzipping;

	public event EventHandler<PackageDownloadEventArgs> PackageDownloadPause;

	public void Initialize()
	{
		DownloadService = HostProxy.DownloadServerV6;
		DownloadService.OnRemoteDownloadStatusChanged += Warpper_OnLocalDownloadStatusChanged;
	}

	private void Warpper_OnLocalDownloadStatusChanged(object sender, RemoteDownloadStatusEventArgs e)
	{
		HandleDownloadResult(e);
	}

	public DownloadManager()
	{
		DownloadingTasks = new List<PackageDownloadTask>();
		Packages = new List<PackageDownloadModel>();
	}

	protected void OnPackageDownloadSuccessfully(object sender, PackageDownloadEventArgs e)
	{
		if (this.PackageDownloadSuccessfully != null)
		{
			this.PackageDownloadSuccessfully(sender, e);
		}
	}

	protected void OnPackageDownloadFailed(object sender, PackageDownloadEventArgs e)
	{
		if (this.PackageDownloadFailed != null)
		{
			this.PackageDownloadFailed(sender, e);
		}
	}

	protected void OnPackageDownloading(object sender, PackageDownloadEventArgs e)
	{
		if (this.PackageDownloading != null)
		{
			this.PackageDownloading(sender, e);
		}
	}

	protected void OnPackageUnzipping(object sender, PackageDownloadEventArgs e)
	{
		this.PackageUnzipping?.Invoke(sender, e);
	}

	protected void OnPackageDownlaodPause(object sender, PackageDownloadEventArgs e)
	{
		if (this.PackageDownloadPause != null)
		{
			this.PackageDownloadPause(sender, e);
		}
	}

	public void StartDownloading(PackageDownloadModel package)
	{
		lock (locktask)
		{
			string text = "resuce download package resource: " + package.ModelName;
			foreach (DownloadInfo resource in package.Resources)
			{
				text = text + Environment.NewLine + resource.FileUrl;
			}
			LogHelper.LogInstance.Debug(text);
			PackageDownloadTask packageDownloadTask = new PackageDownloadTask(this, package);
			if (!string.IsNullOrEmpty(package.ModelName))
			{
				int num = DownloadingTasks.FindIndex((PackageDownloadTask n) => n.Package.ModelName == package.ModelName);
				if (num >= 0)
				{
					DownloadingTasks.RemoveAt(num);
				}
			}
			DownloadingTasks.Add(packageDownloadTask);
			packageDownloadTask.StartDownloading();
		}
	}

	private void HandleDownloadResult(RemoteDownloadStatusEventArgs resource)
	{
		lock (locktask)
		{
			for (int i = 0; i < DownloadingTasks.Count; i++)
			{
				DownloadingTasks[i].HandleDownloadResult(resource);
			}
		}
	}
}
