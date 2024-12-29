using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa.Business;

public class LoadDownloadResources
{
	private DownloadSpeedCollection _DownloadSpeedCollection;

	public void Load()
	{
		Dictionary<DownloadInfoType, List<DownloadInfo>> dictionary = global::Smart.FileDownloadV6.Load();
		_DownloadSpeedCollection = new DownloadSpeedCollection();
		global::Smart.FileDownloadV6.OnRemoteDownloadStatusChanged += FileDownload_OnDownloadStatusChanged;
		if (dictionary == null)
		{
			return;
		}
		if (dictionary[DownloadInfoType.DownloadingInfo] != null)
		{
			List<DownloadInfo> list = dictionary[DownloadInfoType.DownloadingInfo].OrderBy((DownloadInfo n) => n.CreateDateTime).ToList();
			foreach (DownloadInfo item in list)
			{
				if (item.FileType == "ROM" || item.FileType == "COUNTRYCODE" || item.ShowInUI)
				{
					item.LocalFileSize = GlobalFun.GetFileSize(Path.Combine(item.LocalPath, item.FileName + ".tmp"));
					item.Status = DownloadStatus.MANUAL_PAUSE;
				}
				else
				{
					item.Status = DownloadStatus.WAITTING;
				}
				global::Smart.FileDownloadV6.Add(item, autoStart: false);
				DownloadControlViewModel.SingleInstance.AddDownloadingTask(item);
			}
		}
		if (dictionary[DownloadInfoType.DownloadedInfo] == null)
		{
			return;
		}
		(from n in dictionary[DownloadInfoType.DownloadedInfo]
			where n.FileType == "ROM" || n.FileType == "COUNTRYCODE" || n.ShowInUI
			orderby n.CreateDateTime descending
			select n).ToList().ForEach(delegate(DownloadInfo n)
		{
			if (n.UnZip)
			{
				string text = Path.Combine(n.LocalPath, Path.GetFileNameWithoutExtension(n.FileName));
				if (GlobalFun.Exists(text))
				{
					n.FileSize = GlobalFun.GetDirectorySize(text);
				}
				else
				{
					n.FileSize = GlobalFun.GetFileSize(Path.Combine(n.LocalPath, n.FileName));
				}
			}
			else
			{
				n.FileSize = GlobalFun.GetFileSize(Path.Combine(n.LocalPath, n.FileName));
			}
			DownloadControlViewModel.SingleInstance.AddDownloadedTask(n, insertFirst: false);
		});
	}

	private void FileDownload_OnDownloadStatusChanged(object sender, RemoteDownloadStatusEventArgs e)
	{
		DownloadInfo info = e.Info;
		_DownloadSpeedCollection.CollectionAsync(info);
		switch (e.Status)
		{
		case DownloadStatus.WAITTING:
		case DownloadStatus.DOWNLOADING:
		case DownloadStatus.MANUAL_PAUSE:
		case DownloadStatus.SUCCESS:
		case DownloadStatus.ALREADYEXISTS:
		case DownloadStatus.UNZIPSUCCESS:
			if (info.FileType == "ROM" || info.FileType == "COUNTRYCODE" || info.ShowInUI)
			{
				DownloadControlViewModel.SingleInstance.Load(info);
			}
			break;
		case DownloadStatus.AUTO_PAUSE:
		case DownloadStatus.FAILED:
		case DownloadStatus.DELETED:
		case DownloadStatus.UNZIPPING:
		case DownloadStatus.UNZIPFAILED:
		case DownloadStatus.UNZIPNOSPACE:
		case DownloadStatus.MD5CHECKFAILED:
		case DownloadStatus.GETFILESIZEFAILED:
		case DownloadStatus.UNENOUGHDISKSPACE:
		case DownloadStatus.CREATEDIRECTORYFAILED:
		case DownloadStatus.DOWNLOADFILENOTFOUND:
		case DownloadStatus.UNDEFINEERROR:
		case DownloadStatus.NETWORKCONNECTIONERROR:
		case DownloadStatus.FILERENAMEFAILED:
			break;
		}
	}
}
