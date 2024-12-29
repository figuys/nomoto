using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Business.Apps;

internal class DeviceApp
{
	public bool Export(TcpAndroidDevice currentDevice, string apk, string savePath, Action<string, bool> callback)
	{
		return Export(currentDevice, new List<string> { apk }, savePath, callback, null);
	}

	public bool Export(TcpAndroidDevice currentDevice, List<string> apks, string savePath, Action<string, bool> callback, IAsyncTaskContext context)
	{
		if (currentDevice == null)
		{
			return false;
		}
		MessageReaderAndWriter msgRWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		try
		{
			if (msgRWriter == null)
			{
				return false;
			}
			context.AddCancelSource(delegate
			{
				msgRWriter.Dispose();
			});
			IFileTransferManager fileTransferManager = currentDevice.FileTransferManager;
			long num = HostProxy.Sequence.New();
			try
			{
				if (msgRWriter.Send("exportApks", apks, num))
				{
					bool result = false;
					try
					{
						FileTransferWrapper fileTransfer = fileTransferManager.CreateFileTransfer(num);
						try
						{
							context.AddCancelSource(delegate
							{
								fileTransfer.Dispose();
							});
							if (fileTransfer == null)
							{
								return false;
							}
							int count = apks.Count;
							TransferFileInfo transferFileInfo = null;
							bool flag = false;
							for (int i = 0; i < count; i++)
							{
								if (context.IsCancelCommandRequested)
								{
									break;
								}
								flag = fileTransfer.ReceiveFile(savePath, out transferFileInfo);
								if (callback != null)
								{
									string virtualFileName = transferFileInfo.VirtualFileName;
									HostProxy.ResourcesLoggingService.RegisterFile(transferFileInfo.localFilePath);
									callback(virtualFileName, flag);
								}
								fileTransfer.NotifyFileReceiveComplete();
							}
						}
						finally
						{
							if (fileTransfer != null)
							{
								((IDisposable)fileTransfer).Dispose();
							}
						}
					}
					finally
					{
						List<PropItem> receiveData = null;
						if (msgRWriter.Receive("exportApksResponse", out receiveData, 15000) && receiveData != null)
						{
							result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
						}
					}
					return result;
				}
				return false;
			}
			finally
			{
				msgRWriter.ExitLock();
			}
		}
		finally
		{
			if (msgRWriter != null)
			{
				((IDisposable)msgRWriter).Dispose();
			}
		}
	}

	public bool ExportIcon(TcpAndroidDevice currentDevice, List<string> packagenames, string savePath, Action<string, string, bool> callback)
	{
		if (currentDevice == null)
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = currentDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		try
		{
			if (messageReaderAndWriter.Send("exportApkIcon", packagenames, num))
			{
				bool result = false;
				try
				{
					using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
					if (fileTransferWrapper == null)
					{
						return false;
					}
					int count = packagenames.Count;
					TransferFileInfo transferFileInfo = null;
					bool flag = false;
					for (int i = 0; i < count; i++)
					{
						flag = fileTransferWrapper.ReceiveFile(savePath, out transferFileInfo);
						if (callback != null)
						{
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(transferFileInfo.FilePath);
							_ = transferFileInfo.FilePath;
							string arg = Path.Combine(savePath, transferFileInfo.LogicFileName);
							callback(fileNameWithoutExtension, arg, flag);
						}
						fileTransferWrapper.NotifyFileReceiveComplete();
					}
				}
				finally
				{
					List<PropItem> receiveData = null;
					if (messageReaderAndWriter.Receive("exportApkIconResponse", out receiveData, 15000) && receiveData != null)
					{
						result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
					}
				}
				return result;
			}
			return false;
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	public SortedList<AppType, List<AppInfo>> GetApps(TcpAndroidDevice currentDevice, int androidApiLevel)
	{
		if (currentDevice == null)
		{
			return null;
		}
		if (androidApiLevel < 26)
		{
			return GetApps(currentDevice, "getAppInfo", "getAppInfoResponse");
		}
		return GetApps(currentDevice, "getAppList", "getAppListResponse");
	}

	private SortedList<AppType, List<AppInfo>> GetApps(TcpAndroidDevice currentDevice, string requestCmd, string responseCmd)
	{
		if (currentDevice == null)
		{
			return null;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return null;
		}
		long sequence = HostProxy.Sequence.New();
		List<AppInfo> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync(requestCmd, responseCmd, new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			if (receiveData == null)
			{
				return null;
			}
			SortedList<AppType, List<AppInfo>> sortedList = new SortedList<AppType, List<AppInfo>>();
			List<AppInfo> value = (from n in receiveData
				where n.IsSystem
				orderby n.Name
				select n).ToList();
			sortedList.Add(AppType.SystemApp, value);
			List<AppInfo> value2 = (from n in receiveData
				where !n.IsSystem
				orderby n.Name
				select n).ToList();
			sortedList.Add(AppType.MyApp, value2);
			return sortedList;
		}
		return null;
	}

	public bool CheckPermissionToGetAppInfo(TcpAndroidDevice currentDevice)
	{
		if (currentDevice == null)
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("checkPermissionToGetAppInfo", "checkPermissionToGetAppInfoResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
		}
		return false;
	}

	public bool Import(TcpAndroidDevice currentDevice, List<string> apkPath, Action<string, bool> callback)
	{
		if (currentDevice == null)
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = currentDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		if (CheckInternalStorageFreeSize(messageReaderAndWriter, apkPath))
		{
			return false;
		}
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return false;
		}
		try
		{
			bool result = false;
			try
			{
				if (messageReaderAndWriter.Send("importInstallApk", new List<string> { apkPath.Count.ToString() }, num))
				{
					using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
					if (fileTransferWrapper == null)
					{
						return false;
					}
					bool flag = false;
					foreach (string item in apkPath)
					{
						flag = fileTransferWrapper.SendFile(item, num);
						callback?.Invoke(item, flag);
						fileTransferWrapper.WaitFileReceiveCompleteNotify(13000);
					}
				}
			}
			finally
			{
				List<PropItem> receiveData = null;
				if (messageReaderAndWriter.Receive("importInstallApkResponse", out receiveData, 15000))
				{
					result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
				}
			}
			return result;
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	public void Uninstall(TcpAndroidDevice currentDevice, string packageName, Action<Dictionary<string, bool>> callback)
	{
		Uninstall(currentDevice, new List<string> { packageName }, callback);
	}

	public void Uninstall(TcpAndroidDevice currentDevice, List<string> packageNames, Action<Dictionary<string, bool>> callback)
	{
		if (currentDevice == null)
		{
			return;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return;
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		if (!messageReaderAndWriter.SendAndReceiveSync("uninstallAppInfo", "uninstallAppInfoResponse", packageNames, sequence, out receiveData) || receiveData == null)
		{
			return;
		}
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		foreach (PropItem item in receiveData)
		{
			dictionary[item.Value] = bool.Parse(item.Key);
		}
		callback?.BeginInvoke(dictionary, null, null);
	}

	private bool AnalysisResult(string jsonResultString, string valueKey)
	{
		List<PropItem> list = JsonUtils.Parse<List<PropItem>>(jsonResultString);
		if (list != null)
		{
			string text = (from m in list
				where valueKey.Equals(m.Key)
				select m.Value).FirstOrDefault();
			if (!string.IsNullOrEmpty(text) && "true".Equals(text.ToLower()))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckInternalStorageFreeSize(MessageReaderAndWriter msgRWHander, List<string> filePaths)
	{
		double num = 0.0;
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getInternalStorageInfo", "getInternalStorageInfoResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData) && receiveData != null)
		{
			foreach (PropItem item in receiveData)
			{
				if (item.Key.Equals("free"))
				{
					num = FileSizeCalculationHelper.ConvertSizes(item.Value);
				}
			}
		}
		return (double)FileSizeCalculationHelper.GetFilesSize(filePaths) > num;
	}

	private bool InstallWithSocket(TcpAndroidDevice currentDevice, string apkPath)
	{
		if (currentDevice == null)
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = currentDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		if (CheckInternalStorageFreeSize(messageReaderAndWriter, new List<string> { apkPath }))
		{
			return false;
		}
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return false;
		}
		try
		{
			bool result = false;
			if (messageReaderAndWriter.Send("installApp", new List<string> { "1" }, num))
			{
				try
				{
					using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
					if (fileTransferWrapper == null)
					{
						return false;
					}
					fileTransferWrapper.SendFile(apkPath, num);
					fileTransferWrapper.WaitFileReceiveCompleteNotify(13000);
				}
				finally
				{
					List<PropItem> receiveData = null;
					if (!messageReaderAndWriter.Receive("installAppResponse", out receiveData, 15000))
					{
					}
				}
			}
			return result;
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	private int InstallWithAdb(TcpAndroidDevice currentDevice, string apkPath)
	{
		if (currentDevice == null)
		{
			return 0;
		}
		string text = RunCmdIfDeviceExist(currentDevice.Identifer, $"install -r \"{apkPath}\"", -1);
		LogHelper.LogInstance.Debug($"adb Install apk[{apkPath}],the response:{text}");
		string text2 = text.ToUpper();
		if (text2.Contains("SUCCESS"))
		{
			return 1;
		}
		if (text2.Contains("INSTALL_FAILED_INSUFFICIENT_STORAGE") || text2.Contains("INSTALL_FAILED_CONTAINER_ERROR"))
		{
			return -1;
		}
		return 0;
	}

	public static string RunCmdIfDeviceExist(string deviceId, string sourceCommand, int timeout)
	{
		string exe = Path.Combine(Environment.SystemDirectory, "cmd.exe");
		string command = string.Format("/c (\"{0}\" devices | findstr \"{1}\" && \"{0}\" -s \"{1}\" {2})", Path.Combine(Environment.CurrentDirectory, "adb.exe"), deviceId, sourceCommand);
		return ProcessHelper.Instance.Do(exe, command, timeout);
	}

	public int Install(TcpAndroidDevice currentDevice, string apkPath)
	{
		if (currentDevice == null)
		{
			return 0;
		}
		return currentDevice.ConnectType switch
		{
			ConnectType.Adb => InstallWithAdb(currentDevice, apkPath), 
			ConnectType.Wifi => Convert.ToInt16(InstallWithSocket(currentDevice, apkPath)), 
			_ => 0, 
		};
	}

	public bool Uninstall(TcpAndroidDevice currentDevice, IAsyncTaskContext context, string packageName)
	{
		if (currentDevice == null)
		{
			return false;
		}
		return currentDevice.ConnectType switch
		{
			ConnectType.Adb => UninstallWithAdb(currentDevice, context, packageName), 
			ConnectType.Wifi => UninstallWithSocket(currentDevice, context, packageName), 
			_ => false, 
		};
	}

	private bool UninstallWithSocket(TcpAndroidDevice currentDevice, IAsyncTaskContext context, string packageName)
	{
		if (currentDevice == null)
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("uninstallApp", "uninstallAppResponse", new List<string> { packageName }, sequence, out receiveData))
		{
		}
		return false;
	}

	private bool UninstallWithAdb(TcpAndroidDevice currentDevice, IAsyncTaskContext context, string packageName)
	{
		if (currentDevice == null)
		{
			return false;
		}
		string text = currentDevice.DeviceOperator.Command("uninstall " + packageName, -1, currentDevice.Identifer);
		LogHelper.LogInstance.Debug($"Uninstall apk[{packageName}],the response:{text}");
		return text.ToUpper().Contains("SUCCESS");
	}
}
