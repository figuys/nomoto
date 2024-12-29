using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DevicePicManagement : DeviceCommonManagement, IDevicePicManagement
{
	public List<PicServerAlbumInfo> GetAlbums()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return null;
		}
		long sequence = HostProxy.Sequence.New();
		List<PicServerAlbumInfo> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getPICAlbumsInfo", "getPICAlbumsInfoResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<PicServerAlbumInfo>();
	}

	public List<ServerPicGroupInfo> GetPicGroupList(string alblumId)
	{
		List<ServerPicGroupInfo> list = new List<ServerPicGroupInfo>();
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return list;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return list;
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getPICGroupList", "getPICGroupListResponse", new List<string> { alblumId }, sequence, out receiveData) && receiveData != null)
		{
			foreach (PropItem item in receiveData)
			{
				int result = 0;
				int.TryParse(item.Value, out result);
				list.Add(new ServerPicGroupInfo
				{
					GroupKey = item.Key,
					Count = result
				});
			}
			list = list.OrderByDescending((ServerPicGroupInfo m) => m.Date).ToList();
		}
		return list;
	}

	public List<ServerPicInfo> GetPicInfoList(string alblumId, string groupKey)
	{
		List<ServerPicInfo> list = new List<ServerPicInfo>();
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return null;
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getPICIdList", "getPICIdListResponse", new List<string> { alblumId, groupKey }, sequence, out receiveData) && receiveData != null)
		{
			foreach (string item in receiveData)
			{
				list.Add(new ServerPicInfo
				{
					Id = item
				});
			}
		}
		return list;
	}

	public bool FillPicPath(ref List<ServerPicInfo> pics)
	{
		if (pics == null)
		{
			return false;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getPICPathById", "getPICPathByIdResponse", pics.Select((ServerPicInfo m) => m.Id).ToList(), sequence, out receiveData) && receiveData != null && receiveData.Count > 0 && receiveData.Count >= pics.Count)
		{
			for (int i = 0; i < receiveData.Count; i++)
			{
				pics[i].RawFilePath = receiveData[i];
			}
			return true;
		}
		return false;
	}

	public bool ExportThumbnailFromDevice(IAsyncTaskContext context, List<ServerPicInfo> pics, string localStroageDir, Action<ServerPicInfo, bool> callBack, CancellationTokenSource cancelSource)
	{
		if (pics == null || pics.Count == 0)
		{
			return false;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return false;
		}
		try
		{
			List<string> list = pics.Select((ServerPicInfo m) => m.Id).ToList();
			if (messageReaderAndWriter.Send("exportPICThumbnail", list, num))
			{
				bool result = false;
				try
				{
					using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
					if (fileTransferWrapper == null)
					{
						return false;
					}
					int count = list.Count;
					TransferFileInfo recvFileInfo = null;
					bool flag = false;
					for (int i = 0; i < count; i++)
					{
						if (true == cancelSource?.IsCancellationRequested)
						{
							flag = false;
							break;
						}
						flag = fileTransferWrapper.ReceiveFile(localStroageDir, out recvFileInfo);
						fileTransferWrapper.NotifyFileReceiveComplete();
						if (callBack != null)
						{
							ServerPicInfo serverPicInfo = pics.FirstOrDefault((ServerPicInfo p) => p.Id.Equals(recvFileInfo?.FilePath));
							if (serverPicInfo == null)
							{
								callBack(null, arg2: false);
								continue;
							}
							serverPicInfo.LocalFilePath = Path.Combine(localStroageDir, recvFileInfo.VirtualFileName);
							callBack(serverPicInfo, flag);
						}
					}
				}
				finally
				{
					List<PropItem> receiveData = null;
					if (messageReaderAndWriter.Receive("exportPICThumbnailResponse", out receiveData, 15000) && receiveData != null)
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

	public bool DeleteAlbum(string ambumPath)
	{
		return false;
	}

	public bool DeletePics(DateTime? date)
	{
		return false;
	}

	public bool DeletePicFromList(List<ServerPicInfo> pics)
	{
		if (pics == null)
		{
			return false;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		if (tcpAndroidDevice.MessageManager == null)
		{
			LogHelper.LogInstance.Info("MessageManager instance is null");
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		long sequence = HostProxy.Sequence.New();
		bool flag = true;
		int num = 0;
		int count = pics.Count;
		while (num < count)
		{
			List<string> list = new List<string>();
			while (num < count)
			{
				list.Add(pics[num].Id);
				if (++num % 5 == 0)
				{
					break;
				}
			}
			List<PropItem> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("deletePICById", "deletePICByIdResponse", list, sequence, out receiveData) && receiveData != null)
			{
				flag &= receiveData.Exists((PropItem m) => "true".Equals(m.Value));
				if (!flag)
				{
					break;
				}
				continue;
			}
			flag = false;
			break;
		}
		return flag;
	}

	public bool ExportThumbnailFromDevice(List<PicServerAlbumInfo> albums, string localStroageDir, Action<PicServerAlbumInfo, ServerPicInfo, bool> callBack)
	{
		if (albums == null)
		{
			return false;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return false;
		}
		bool flag = true;
		try
		{
			foreach (PicServerAlbumInfo album in albums)
			{
				if (album.FileCount == 0 || !(flag &= messageReaderAndWriter.Send("exportPICThumbnailAlbumFiles", new List<string>
				{
					album.Id,
					album.FileCount.ToString()
				}, num)))
				{
					continue;
				}
				try
				{
					using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
					if (fileTransferWrapper == null)
					{
						return false;
					}
					int fileCount = album.FileCount;
					TransferFileInfo transferFileInfo = null;
					bool flag2 = false;
					for (int i = 0; i < fileCount; i++)
					{
						flag2 = fileTransferWrapper.ReceiveFile(localStroageDir, out transferFileInfo);
						if (callBack != null)
						{
							ServerPicInfo serverPicInfo = null;
							if (transferFileInfo != null)
							{
								serverPicInfo = new ServerPicInfo();
								serverPicInfo.RawFileName = transferFileInfo.LogicFileName;
								serverPicInfo.LocalFilePath = transferFileInfo.localFilePath;
								serverPicInfo.RawFileSize = transferFileInfo.FileSize;
								serverPicInfo.RawFilePath = transferFileInfo.FilePath;
								serverPicInfo.RawModifiedDateTime = transferFileInfo.ModifiedDateTime.ToString("yyyy-MM-dd");
							}
							callBack(album, serverPicInfo, flag2);
							flag = flag && flag2;
						}
						fileTransferWrapper.NotifyFileReceiveComplete();
					}
				}
				finally
				{
					List<PropItem> receiveData = null;
					flag = (flag &= messageReaderAndWriter.Receive("exportPICThumbnailAlbumFilesResponse", out receiveData, 15000) && receiveData != null) && (flag & receiveData.Exists((PropItem m) => "true".Equals(m.Value)));
				}
			}
			return flag;
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	public bool ExportThumbnailFromDevice(PicServerAlbumInfo album, int fileCount, string localStroageDir, Action<PicServerAlbumInfo, ServerPicInfo, bool> callBack)
	{
		if (album == null)
		{
			return false;
		}
		PicServerAlbumInfo picServerAlbumInfo = new PicServerAlbumInfo();
		picServerAlbumInfo.AlbumName = album.AlbumName;
		picServerAlbumInfo.Id = album.Id;
		picServerAlbumInfo.FileCount = fileCount;
		return ExportThumbnailFromDevice(new List<PicServerAlbumInfo> { picServerAlbumInfo }, localStroageDir, delegate(PicServerAlbumInfo albumInfo, ServerPicInfo file, bool result)
		{
			if (callBack != null)
			{
				callBack(album, file, result);
			}
		});
	}

	public bool ExportVirtualPicFromDevice(List<PicServerAlbumInfo> albums, string localStroageDir, Action<PicServerAlbumInfo, string, bool> callBack)
	{
		if (albums == null)
		{
			return false;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return false;
		}
		bool flag = true;
		try
		{
			foreach (PicServerAlbumInfo album in albums)
			{
				if (flag &= messageReaderAndWriter.Send("exportPICAlbumVirtualFiles", new List<string>
				{
					album.Id,
					album.FileCount.ToString()
				}, num))
				{
					try
					{
						using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
						if (fileTransferWrapper == null)
						{
							return false;
						}
						int fileCount = album.FileCount;
						TransferFileInfo transferFileInfo = null;
						bool flag2 = false;
						for (int i = 0; i < fileCount; i++)
						{
							flag2 = fileTransferWrapper.ReceiveFile(localStroageDir, out transferFileInfo);
							callBack?.Invoke(album, transferFileInfo.FilePath, flag2);
							flag = flag && flag2;
							fileTransferWrapper.NotifyFileReceiveComplete();
						}
					}
					finally
					{
						List<PropItem> receiveData = null;
						flag = messageReaderAndWriter.Receive("exportPICAlbumVirtualFilesResponse", out receiveData, 15000) && (flag & (receiveData?.Exists((PropItem p) => "true".Equals(p.Value)) ?? false));
					}
				}
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	public TransferResult ImportPicListToDevice(IAsyncTaskContext context, List<string> localFilePathList, Action<string, bool> callBack)
	{
		if (localFilePathList == null)
		{
			return TransferResult.FAILD_FILE_NOT_EXISTS;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return TransferResult.FAILD;
		}
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
			long num = HostProxy.Sequence.New();
			if (CheckInternalStorageFreeSize(msgRWriter, localFilePathList))
			{
				return TransferResult.NOT_ENOUGH_SPACE;
			}
			if (!msgRWriter.TryEnterLock(10000))
			{
				return TransferResult.FAILD;
			}
			try
			{
				if (msgRWriter.Send("importPICFiles", new List<string> { localFilePathList.Count.ToString() }, num))
				{
					bool flag = false;
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
								return TransferResult.FAILD;
							}
							bool flag2 = false;
							foreach (string localFilePath in localFilePathList)
							{
								if (context.IsCancelCommandRequested)
								{
									return TransferResult.CANCEL;
								}
								flag2 = fileTransfer.SendFile(localFilePath, num);
								callBack?.Invoke(localFilePath, flag2);
								fileTransfer.WaitFileReceiveCompleteNotify(13000);
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
						flag = msgRWriter.Receive("importPICFilesResponse", out receiveData, 15000) && receiveData != null && receiveData.Exists((PropItem m) => "true".Equals(m.Value));
					}
					return (!flag) ? TransferResult.FAILD : TransferResult.SUCCESS;
				}
				return TransferResult.FAILD;
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
}
