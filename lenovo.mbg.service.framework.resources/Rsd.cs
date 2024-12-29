using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.framework.resources;

public class Rsd : IRsd
{
	private static Rsd _instance;

	private readonly ISaveDownloadInfo SaveMode;

	private static readonly ReaderWriterLockSlim downloaded_rwl = new ReaderWriterLockSlim();

	private static readonly ReaderWriterLockSlim downloading_rwl = new ReaderWriterLockSlim();

	public static Rsd Instance => _instance ?? (_instance = new Rsd());

	public Rsd()
	{
		SaveMode = new DownloadInfoToJson();
	}

	public List<DownloadInfo> ReadDownloadedResources()
	{
		List<DownloadInfo> list = SafeReadDownloaded(() => SaveMode.Get<DownloadInfo>(SaveMode.DownloadedPath));
		if (list == null)
		{
			list = new List<DownloadInfo>();
		}
		return list;
	}

	public bool WriteDownloadedResources(List<DownloadInfo> resources)
	{
		return SafeWriteDownloaded((List<DownloadInfo> data) => SaveMode.Save(SaveMode.DownloadedPath, data), resources);
	}

	public List<DownloadInfo> ReadDownloadingResources()
	{
		return SafeReadDownloading(() => SaveMode.Get<DownloadInfo>(SaveMode.DownloadingPath));
	}

	public bool WriteDownloadingResources(List<DownloadInfo> resources)
	{
		return SafeWriteDownloading((List<DownloadInfo> data) => SaveMode.Save(SaveMode.DownloadingPath, data), resources);
	}

	public List<DownloadInfo> InitDownloadedResources()
	{
		List<DownloadInfo> list = ReadDownloadedResources();
		if (list == null)
		{
			list = new List<DownloadInfo>();
		}
		string text = Path.Combine(Environment.CurrentDirectory, "res.tmp");
		bool flag = false;
		if (GlobalFun.Exists(text))
		{
			Dictionary<string, List<DownloadInfo>> sources = LoadCopyRomResources(new List<string> { text });
			Dictionary<string, List<DownloadInfo>> dictionary = ValidateCopyRomResources(sources);
			if (dictionary != null && dictionary.Count > 0)
			{
				List<DownloadInfo> second = dictionary.SelectMany((KeyValuePair<string, List<DownloadInfo>> n) => n.Value).ToList();
				list = list.Union(second, EqualityComparerHelper<DownloadInfo>.CreateComparer((DownloadInfo n) => n.FileUrl)).ToList();
				flag = true;
			}
		}
		List<DownloadInfo> list2 = DownloadResourcesCompatible.LoadDownloaded();
		if (list2 != null && list2.Count > 0)
		{
			list = list.Union(list2).ToList();
			flag = true;
		}
		GlobalFun.TryDeleteFile(text);
		if (flag)
		{
			WriteDownloadedResources(list);
		}
		return list;
	}

	public List<DownloadInfo> InitDownloadingResources()
	{
		List<DownloadInfo> list = SaveMode.Get<DownloadInfo>(SaveMode.DownloadingPath);
		if (list == null)
		{
			list = new List<DownloadInfo>();
		}
		List<DownloadInfo> list2 = DownloadResourcesCompatible.LoadDownloading();
		if (list2 != null && list2.Count > 0)
		{
			list = list.Union(list2).ToList();
			WriteDownloadingResources(list);
		}
		return list;
	}

	public DownloadInfo GetDownloadedResource(string downloadurl, out string filePath)
	{
		filePath = null;
		try
		{
			List<DownloadInfo> source = ReadDownloadedResources();
			string url = downloadurl.Split('?')[0];
			DownloadInfo downloadInfo = source.Where((DownloadInfo n) => n.FileUrl == url)?.OrderByDescending((DownloadInfo n) => n.CreateDateTime).FirstOrDefault();
			if (downloadInfo != null)
			{
				if (downloadInfo.UnZip && downloadInfo.FileType != "TOOL")
				{
					filePath = Path.Combine(downloadInfo.LocalPath, Path.GetFileNameWithoutExtension(downloadInfo.FileName));
				}
				else
				{
					filePath = Path.Combine(downloadInfo.LocalPath, Path.GetFileName(downloadInfo.FileName));
				}
				if (GlobalFun.Exists(filePath))
				{
					if (downloadInfo.FileType == "TOOL" && string.IsNullOrEmpty(downloadInfo.ZipPwd))
					{
						GlobalFun.TryDeleteFile(filePath);
						return null;
					}
					return downloadInfo;
				}
			}
			return null;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("", exception);
			return null;
		}
	}

	public Dictionary<string, List<DownloadInfo>> LoadCopyRomResources(List<string> desriptionFilePaths)
	{
		Dictionary<string, List<DownloadInfo>> dictionary = new Dictionary<string, List<DownloadInfo>>();
		foreach (string desriptionFilePath in desriptionFilePaths)
		{
			List<DownloadInfo> list = JsonHelper.DeserializeJson2ListFromFile<DownloadInfo>(desriptionFilePath);
			if (list != null && list.Count > 0)
			{
				dictionary.Add(desriptionFilePath, list);
			}
		}
		return dictionary;
	}

	public Dictionary<string, List<DownloadInfo>> ValidateCopyRomResources(Dictionary<string, List<DownloadInfo>> sources)
	{
		Dictionary<string, List<DownloadInfo>> dictionary = new Dictionary<string, List<DownloadInfo>>();
		foreach (KeyValuePair<string, List<DownloadInfo>> source in sources)
		{
			List<DownloadInfo> removes = new List<DownloadInfo>();
			source.Value.ForEach(delegate(DownloadInfo n)
			{
				if (!GlobalFun.Exists(Path.Combine(n.LocalPath, n.FileName)))
				{
					removes.Add(n);
				}
			});
			List<DownloadInfo> list = source.Value.Except(removes).ToList();
			if (list.Count > 0)
			{
				dictionary.Add(source.Key, list);
			}
		}
		return dictionary;
	}

	public int Unzip(DownloadInfo downloadInfo)
	{
		return Unzip(downloadInfo, _downloadUnzip: false);
	}

	public bool UnzipTool(string _toolZipPath, string _dir, out string response)
	{
		try
		{
			response = null;
			DownloadInfo downloadInfo = (from n in ReadDownloadedResources()
				orderby n.CreateDateTime descending
				select n).FirstOrDefault((DownloadInfo n) => n.FileName == Path.GetFileName(_toolZipPath));
			if (!SevenZipHelper.Instance.ExtractorWithPwd(_toolZipPath, _dir, downloadInfo.ZipPwd))
			{
				GlobalFun.TryDeleteFile(_toolZipPath);
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			response = ex.ToString();
			LogHelper.LogInstance.Error($"unzip flash_tool exception:[{ex}].");
			return false;
		}
	}

	public int Unzip(DownloadInfo downloadInfo, bool _downloadUnzip)
	{
		LogHelper.LogInstance.Debug($"File {downloadInfo.FileUrl} download success, filename: {downloadInfo.FileName} unzip start");
		string text = Path.Combine(downloadInfo.LocalPath, downloadInfo.FileName);
		string directory = Path.Combine(downloadInfo.LocalPath, Path.GetFileNameWithoutExtension(downloadInfo.FileName));
		if (downloadInfo.FileType == "TOOL")
		{
			if (!_downloadUnzip)
			{
				return 4;
			}
			int num = SevenZipHelper.Instance.Extractor(text, directory);
			if (num == 0)
			{
				SevenZipHelper.Instance.CompressWithPwd(directory, text, downloadInfo.ZipPwd);
				downloadInfo.FileSize = GlobalFun.GetFileSize(text);
			}
			return num;
		}
		int num2 = SevenZipHelper.Instance.Extractor(text, directory);
		if (num2 == 0)
		{
			GlobalFun.TryDeleteFile(text);
		}
		return num2;
	}

	private T SafeReadDownloaded<T>(Func<T> func)
	{
		downloaded_rwl.EnterReadLock();
		try
		{
			return func();
		}
		finally
		{
			downloaded_rwl.ExitReadLock();
		}
	}

	private bool SafeWriteDownloaded<T>(Func<T, bool> ac, T data)
	{
		downloaded_rwl.EnterWriteLock();
		try
		{
			return ac(data);
		}
		finally
		{
			downloaded_rwl.ExitWriteLock();
		}
	}

	private T SafeReadDownloading<T>(Func<T> func)
	{
		downloading_rwl.EnterReadLock();
		try
		{
			return func();
		}
		finally
		{
			downloading_rwl.ExitReadLock();
		}
	}

	private bool SafeWriteDownloading<T>(Func<T, bool> ac, T data)
	{
		downloading_rwl.EnterWriteLock();
		try
		{
			return ac(data);
		}
		finally
		{
			downloading_rwl.ExitWriteLock();
		}
	}
}
