using System;
using System.Collections.Generic;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.Model;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenCapture.BLL;

public class VideoBLL
{
	public List<string> GetIdList(string albumName, string sortPropertyName, bool isSortDesc)
	{
		List<string> list = null;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { MessageManager: not null } tcpAndroidDevice))
		{
			return list;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return list;
			}
			_ = tcpAndroidDevice.FileTransferManager;
			long sequence = HostProxy.Sequence.New();
			List<string> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("getScreenCAPIdList", "getScreenCAPIdListResponse", new List<string>
			{
				albumName,
				sortPropertyName,
				isSortDesc.ToString()
			}, sequence, out receiveData) && receiveData != null)
			{
				list = receiveData;
			}
		}
		if (list == null)
		{
			list = new List<string>();
		}
		return list;
	}

	public List<VideoDetailModel> GetVideoInfoList(List<string> idList)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { MessageManager: not null } tcpAndroidDevice))
		{
			return null;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return null;
		}
		_ = tcpAndroidDevice.FileTransferManager;
		long sequence = HostProxy.Sequence.New();
		List<VideoDetailModel> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getScreenCAPInfoList", "getScreenCAPInfoListResponse", idList, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return null;
	}

	public void ExportVideoThumbnailList(List<string> idList, string exportFolder, Action<string, bool, string> callback)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return;
		}
		IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
		ISequence sequence = HostProxy.Sequence;
		long num = sequence.New();
		List<PropItem> list = null;
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return;
		}
		try
		{
			num = sequence.New();
			if (!messageReaderAndWriter.Send("exportScreenCAPThumbnail", idList, num))
			{
				return;
			}
			try
			{
				using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
				if (fileTransferWrapper == null)
				{
					return;
				}
				TransferFileInfo transferFileInfo = null;
				for (int i = 0; i < idList.Count; i++)
				{
					if (fileTransferWrapper.ReceiveFile(exportFolder, out transferFileInfo) && transferFileInfo != null)
					{
						string virtualFileName = transferFileInfo.VirtualFileName;
						string arg = virtualFileName.Substring(0, virtualFileName.IndexOf('_'));
						callback(arg, arg2: true, transferFileInfo.localFilePath);
					}
					else
					{
						callback(string.Empty, arg2: false, string.Empty);
					}
					fileTransferWrapper.NotifyFileReceiveComplete();
				}
			}
			finally
			{
				list = null;
				if (!messageReaderAndWriter.Receive("exportScreenCAPThumbnailResponse", out list, 13000))
				{
				}
			}
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	public TransferResult ExportVideo(List<string> idList, string exportFolder, IAsyncTaskContext context, Action<string, TransferResult> callback)
	{
		if (idList == null)
		{
			return TransferResult.FAILD;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return TransferResult.FAILD;
		}
		TransferResult result = TransferResult.SUCCESS;
		MessageReaderAndWriter msgRWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		try
		{
			if (msgRWriter == null)
			{
				return TransferResult.FAILD;
			}
			context.AddCancelSource(delegate
			{
				msgRWriter.Dispose();
			});
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			ISequence sequence = HostProxy.Sequence;
			long num = sequence.New();
			List<PropItem> list = null;
			if (!msgRWriter.TryEnterLock(10000))
			{
				return TransferResult.FAILD;
			}
			try
			{
				if (context.IsCancelCommandRequested)
				{
					return TransferResult.CANCEL;
				}
				result = TransferResult.FAILD;
				num = sequence.New();
				if (msgRWriter.Send("exportScreenCAPFile", idList, num))
				{
					try
					{
						FileTransferWrapper fileTransfer = fileTransferManager.CreateFileTransfer(num);
						try
						{
							if (fileTransfer != null)
							{
								context.AddCancelSource(delegate
								{
									fileTransfer.Dispose();
								});
								TransferFileInfo transferFileInfo = null;
								for (int i = 0; i < idList.Count; i++)
								{
									if (context.IsCancelCommandRequested)
									{
										return TransferResult.CANCEL;
									}
									if (!fileTransfer.ReceiveFile(exportFolder, out transferFileInfo))
									{
										return result = TransferResult.FAILD;
									}
									callback?.Invoke(string.Empty, TransferResult.SUCCESS);
									HostProxy.ResourcesLoggingService.RegisterFile(transferFileInfo.localFilePath);
									fileTransfer.NotifyFileReceiveComplete();
								}
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
						list = null;
						if (msgRWriter.Receive("exportScreenCAPFileResponse", out list, 13000) && list != null)
						{
							result = ((!list.Exists((PropItem m) => "SUCCESS".Equals(m.Value))) ? ((!list.Exists((PropItem m) => "NONEXISTENCE".Equals(m.Value))) ? TransferResult.FAILD : TransferResult.FAILD_FILE_NOT_EXISTS) : TransferResult.SUCCESS);
						}
					}
				}
			}
			finally
			{
				msgRWriter.ExitLock();
			}
			return result;
		}
		finally
		{
			if (msgRWriter != null)
			{
				((IDisposable)msgRWriter).Dispose();
			}
		}
	}

	public bool DeleteVideo(List<string> idList)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		_ = tcpAndroidDevice.FileTransferManager;
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("deleteScreenCAP", "deleteScreenCAPResponse", idList, sequence, out receiveData) && receiveData != null)
		{
			return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
		}
		return false;
	}

	public void StartScreenCapture(Action<List<PropItem>> callback)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			callback?.Invoke(null);
			return;
		}
		List<PropItem> receiveData = null;
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				callback?.Invoke(null);
				return;
			}
			_ = tcpAndroidDevice.FileTransferManager;
			long sequence = HostProxy.Sequence.New();
			if (tcpAndroidDevice is AdbDeviceEx adbDeviceEx)
			{
				adbDeviceEx.FocuseApp();
			}
			else
			{
				Thread.Sleep(1000);
			}
			if (messageReaderAndWriter.SendAndReceiveSync("readyScreenRecord", "readyScreenRecordResponse", new List<PropItem>(), sequence, out receiveData) && receiveData != null)
			{
				callback?.Invoke(receiveData);
				return;
			}
		}
		callback?.Invoke(receiveData);
	}
}
