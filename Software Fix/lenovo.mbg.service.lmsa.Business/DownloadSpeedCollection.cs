using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.framework.services.Download;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Business;

internal class DownloadSpeedCollection
{
	private class CollectionDownloadInfo
	{
		private bool stoped = false;

		public string FileName { get; set; }

		public string OSProductName { get; set; }

		public long Seconds { get; set; }

		public string StartTime { get; set; }

		public void Initialize(DownloadInfo info)
		{
			FileName = info.FileName;
			StartTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
			OSProductName = SetOsProductName();
			if (info.Status == DownloadStatus.DOWNLOADING)
			{
				Start();
			}
		}

		public void Start()
		{
			Task.Factory.StartNew(delegate
			{
				while (true)
				{
					Seconds++;
					if (stoped)
					{
						break;
					}
					Thread.Sleep(1000);
				}
			});
		}

		public void Stop()
		{
			stoped = true;
		}

		private string SetOsProductName()
		{
			GlobalFun.TryGetRegistryKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", out var value);
			string arg = (Environment.Is64BitOperatingSystem ? "x64" : "x86");
			return $"{value} {arg}";
		}
	}

	private static object locker = new object();

	private ConcurrentDictionary<string, CollectionDownloadInfo> _Cache { get; set; }

	public void CollectionAsync(DownloadInfo downloadinfo)
	{
		Task.Factory.StartNew(delegate
		{
			if (downloadinfo.FileType == "ROM")
			{
				if (downloadinfo.Status == DownloadStatus.DOWNLOADING)
				{
					if (!_Cache.ContainsKey(downloadinfo.FileUrl))
					{
						CollectionDownloadInfo collectionDownloadInfo = new CollectionDownloadInfo();
						collectionDownloadInfo.Initialize(downloadinfo);
						_Cache.TryAdd(downloadinfo.FileUrl, collectionDownloadInfo);
					}
				}
				else
				{
					_Cache.TryRemove(downloadinfo.FileUrl, out var value);
					value?.Stop();
					if (downloadinfo.Status == DownloadStatus.SUCCESS || downloadinfo.Status == DownloadStatus.UNZIPSUCCESS)
					{
						Upload(value, downloadinfo.FileSize);
					}
				}
			}
		});
	}

	public DownloadSpeedCollection()
	{
		Load();
		LoopWrite();
	}

	private void LoopWrite()
	{
		Task.Factory.StartNew(delegate
		{
			while (true)
			{
				Thread.Sleep(30000);
				Write();
			}
		});
	}

	private void Load()
	{
		_Cache = JsonHelper.DeserializeJson2Object<ConcurrentDictionary<string, CollectionDownloadInfo>>(FileHelper.ReadWithAesDecrypt(Configurations.DownloadSpeedPath));
		if (_Cache == null)
		{
			_Cache = new ConcurrentDictionary<string, CollectionDownloadInfo>();
		}
	}

	private void Upload(CollectionDownloadInfo info, long size)
	{
		if (info != null)
		{
			JObject jObject = new JObject();
			jObject["romName"] = info.FileName;
			jObject["downloadDuration"] = info.Seconds;
			jObject["downloadStart"] = info.StartTime;
			jObject["romSize"] = size;
			jObject["localPcos"] = info.OSProductName;
			AppContext.WebApi.RequestContent(WebApiUrl.UPLOAD_DOWNLOAD_SPEEDINFO, jObject);
			Write();
		}
	}

	private void Write()
	{
		if (_Cache.Count > 0)
		{
			FileHelper.WriteFileWithAesEncrypt(Configurations.DownloadSpeedPath, JsonHelper.SerializeObject2Json(_Cache));
		}
	}
}
