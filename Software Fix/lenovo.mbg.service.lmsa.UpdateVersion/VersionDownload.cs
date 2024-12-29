using System;
using System.IO;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadControllerImpl;
using lenovo.mbg.service.framework.download.DownloadUnity;
using lenovo.mbg.service.framework.updateversion;

namespace lenovo.mbg.service.lmsa.UpdateVersion;

public class VersionDownload : IVersionDownload
{
	private GeneralDownloadController controller;

	private long cancelInterlocked = 0L;

	private AbstractDownloadInfo DownloadInfo;

	private bool Canceling => Interlocked.Read(ref cancelInterlocked) != 0;

	public event EventHandler<DownloadStatusChangedArgs> OnDownloadStatusChanged;

	public VersionDownload()
	{
		controller = new GeneralDownloadController();
		controller.DownloadStatusChanged = FireDownloadStatusChanged;
	}

	public void Cancel()
	{
		if (Canceling)
		{
			return;
		}
		try
		{
			Interlocked.Exchange(ref cancelInterlocked, 1L);
			controller.Stop(DownloadInfo);
		}
		catch (Exception)
		{
		}
		finally
		{
			Interlocked.Exchange(ref cancelInterlocked, 0L);
		}
	}

	public void DownloadVersion(object data)
	{
		if (data == null)
		{
			return;
		}
		DownloadInfo = data as AbstractDownloadInfo;
		long num = 0L;
		string text = Path.Combine(DownloadInfo.saveLocalPath, DownloadInfo.downloadFileName);
		if (File.Exists(text))
		{
			LogHelper.LogInstance.Info($"lenovo.mbg.service.lmsa.UpdateVersion.VersionDownload: '{text}' Already Exists");
			FileInfo fileInfo = new FileInfo(text);
			num = fileInfo.Length;
			if (num == DownloadInfo.downloadFileSize)
			{
				LogHelper.LogInstance.Info($"lenovo.mbg.service.lmsa.UpdateVersion.VersionDownload: '{text}' Size Equals DownloadFileSize");
				LogHelper.LogInstance.Info("lenovo.mbg.service.lmsa.UpdateVersion.VersionDownload: MD5 Check");
				if (DownloadInfo.downloadMD5.Equals(GlobalFun.GetMd5Hash(text), StringComparison.CurrentCultureIgnoreCase))
				{
					DownloadInfo.downloadStatus = DownloadStatus.SUCCESS;
					FireDownloadStatusChanged(DownloadInfo);
					return;
				}
			}
			try
			{
				File.Delete(text);
			}
			catch (Exception exception)
			{
				ApplcationClass.IsUpdatingPlug = false;
				LogHelper.LogInstance.Error($"lenovo.mbg.service.lmsa.UpdateVersion.VersionDownload: '{text}' Delete Failed", exception);
				return;
			}
		}
		string text2 = Path.Combine(DownloadInfo.saveLocalPath, DownloadInfo.tempFileName);
		if (File.Exists(text2))
		{
			LogHelper.LogInstance.Info($"'{DownloadInfo.tempFileName}' Already Exists");
			FileInfo fileInfo2 = new FileInfo(text2);
			if (fileInfo2.Length == DownloadInfo.downloadFileSize && DownloadInfo.downloadMD5.Equals(GlobalFun.GetMd5Hash(text2), StringComparison.CurrentCultureIgnoreCase))
			{
				try
				{
					fileInfo2.MoveTo(text);
					DownloadInfo.downloadStatus = DownloadStatus.SUCCESS;
					FireDownloadStatusChanged(DownloadInfo);
					return;
				}
				catch (Exception exception2)
				{
					ApplcationClass.IsUpdatingPlug = false;
					LogHelper.LogInstance.Error($"'{DownloadInfo.tempFileName}' Rename Failed", exception2);
					return;
				}
			}
		}
		try
		{
			controller.Start(DownloadInfo);
		}
		catch (Exception exception3)
		{
			ApplcationClass.IsUpdatingPlug = false;
			LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.UpdateVersion.VersionDownload: Download Exception", exception3);
		}
	}

	private void FireDownloadStatusChanged(AbstractDownloadInfo downloadInfo)
	{
		if (this.OnDownloadStatusChanged != null)
		{
			this.OnDownloadStatusChanged(this, new DownloadStatusChangedArgs(downloadInfo));
		}
	}
}
