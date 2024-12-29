using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DeviceVideoManager : DeviceCommonManagement
{
	public bool GetVideoCount(out int videoCnt, out int albumCnt)
	{
		videoCnt = 0;
		albumCnt = 0;
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
		List<PropItem> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getVideoCount", "getVideoCountResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			foreach (PropItem item in receiveData)
			{
				int.TryParse(item.Key, out albumCnt);
				int.TryParse(item.Value, out videoCnt);
			}
		}
		return true;
	}

	public List<ServerAlbumInfo> GetAlbums()
	{
		List<ServerAlbumInfo> list = new List<ServerAlbumInfo>();
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
		if (messageReaderAndWriter.SendAndReceiveSync("getAlbums", "getAlbumsResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			foreach (PropItem item in receiveData)
			{
				ServerAlbumInfo serverAlbumInfo = new ServerAlbumInfo();
				serverAlbumInfo.AlbumPath = item.Key;
				int result = 0;
				int.TryParse(item.Value, out result);
				serverAlbumInfo.FileCount = result;
				list.Add(serverAlbumInfo);
			}
		}
		return list;
	}

	public List<Video> GetVideos(List<ServerAlbumInfo> albums)
	{
		List<Video> list = new List<Video>();
		if (albums == null || albums.Count == 0)
		{
			return list;
		}
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
		List<Video> receiveData = null;
		foreach (ServerAlbumInfo album in albums)
		{
			if (!messageReaderAndWriter.SendAndReceiveSync("getVideos", "getVideosResponse", new List<string> { album.AlbumPath }, sequence, out receiveData) || receiveData == null)
			{
				continue;
			}
			foreach (Video item in receiveData)
			{
				if (string.IsNullOrEmpty(item.Album))
				{
					item.Album = album.AlbumPath;
				}
			}
			if (receiveData != null && receiveData.Count > 0)
			{
				list.AddRange(receiveData);
			}
		}
		return list = list.OrderByDescending((Video m) => m.LongModifyDate).ToList();
	}

	public List<Video> GetVideos(ServerAlbumInfo album)
	{
		List<Video> list = new List<Video>();
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
		List<Video> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getVideos", "getVideosResponse", new List<string> { album.AlbumPath }, sequence, out receiveData) && receiveData != null)
		{
			foreach (Video item in receiveData)
			{
				if (string.IsNullOrEmpty(item.Album))
				{
					item.Album = album.AlbumName;
				}
			}
			if (receiveData != null && receiveData.Count > 0)
			{
				list.AddRange(receiveData);
			}
		}
		return list;
	}

	public bool ExportVideoThumbnails(List<ServerAlbumInfo> albums, string localStroageDir, Action<ServerAlbumInfo, VideoThumbnails, bool> callBack)
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
		ISequence sequence = HostProxy.Sequence;
		long num = -1L;
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return false;
		}
		bool result = false;
		try
		{
			foreach (ServerAlbumInfo album in albums)
			{
				num = sequence.New();
				if (!messageReaderAndWriter.Send("exportVideoThumbnailAlbumFiles", new List<string>
				{
					album.AlbumPath,
					album.FileCount.ToString()
				}, num))
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
					bool flag = false;
					for (int i = 0; i < fileCount; i++)
					{
						flag = fileTransferWrapper.ReceiveFile(localStroageDir, out transferFileInfo);
						if (callBack != null)
						{
							VideoThumbnails videoThumbnails = null;
							if (transferFileInfo != null)
							{
								videoThumbnails = new VideoThumbnails
								{
									LocalFilePath = transferFileInfo.localFilePath,
									FilePath = transferFileInfo.FilePath,
									ModifiedDate = transferFileInfo.ModifiedDateTime,
									Name = transferFileInfo.VirtualFileName
								};
								if (transferFileInfo.FileSize < 1)
								{
									videoThumbnails.Name = "";
								}
							}
							callBack(album, videoThumbnails, flag);
						}
						fileTransferWrapper.NotifyFileReceiveComplete();
					}
				}
				finally
				{
					List<PropItem> receiveData = null;
					if (messageReaderAndWriter.Receive("exportVideoThumbnailAlbumFilesResponse", out receiveData, 15000) && receiveData != null)
					{
						result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
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

	public TransferResult ExportVideoFiles(IAsyncTaskContext context, List<Video> videos, string localStroageDir, Action<Video, bool> callBack)
	{
		if (videos == null)
		{
			return TransferResult.FAILD;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return TransferResult.FAILD;
		}
		MessageReaderAndWriter msgRWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		try
		{
			context.AddCancelSource(delegate
			{
				msgRWriter.Dispose();
			});
			if (msgRWriter == null)
			{
				return TransferResult.FAILD;
			}
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			ISequence sequenceServcie = HostProxy.Sequence;
			long newSequence = -1L;
			if (!msgRWriter.TryEnterLock(10000))
			{
				return TransferResult.FAILD;
			}
			long num = 0L;
			try
			{
				List<string> list = new List<string>();
				foreach (Video video in videos)
				{
					if (context.IsCancelCommandRequested)
					{
						return TransferResult.CANCEL;
					}
					list.Add(video.FullFilePath);
					num += video.Size;
				}
				if (CheckFreeSpaceHelper.CheckPCLocalDirFreeSpace(localStroageDir, num))
				{
					return TransferResult.NOT_ENOUGH_SPACE;
				}
				TransferResult result = TransferResult.SUCCESS;
				DataPartitionWrapper.DoProcess(list.ToList(), 6, delegate(IEnumerable<string> partList)
				{
					if (context.IsCancelCommandRequested)
					{
						result = TransferResult.CANCEL;
						return false;
					}
					newSequence = sequenceServcie.New();
					if (msgRWriter.Send("exportVideoFiles", partList.ToList(), newSequence))
					{
						try
						{
							FileTransferWrapper fileTransfer = fileTransferManager.CreateFileTransfer(newSequence);
							try
							{
								context.AddCancelSource(delegate
								{
									fileTransfer.Dispose();
								});
								if (fileTransfer == null)
								{
									result = TransferResult.FAILD;
									return false;
								}
								int num2 = partList.Count();
								TransferFileInfo receivedFileInfo = null;
								bool flag = false;
								for (int i = 0; i < num2; i++)
								{
									if (context.IsCancelCommandRequested)
									{
										result = TransferResult.CANCEL;
										return false;
									}
									flag = fileTransfer.ReceiveFile(localStroageDir, out receivedFileInfo);
									HostProxy.ResourcesLoggingService.RegisterFile(receivedFileInfo.localFilePath);
									if (callBack != null)
									{
										Video arg = videos.Where((Video m) => m.FullFilePath.Equals(receivedFileInfo.FilePath)).FirstOrDefault();
										callBack(arg, flag);
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
							msgRWriter.Receive("exportVideoFilesResponse", out receiveData, 15000);
						}
						return result == TransferResult.SUCCESS;
					}
					return false;
				});
				return result;
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

	public TransferResult ExportVideoFiles(List<Video> videos, string localStroageDir, Action<Video, bool> callBack, CancellationTokenSource cancelTokenResource)
	{
		if (videos == null)
		{
			return TransferResult.FAILD;
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
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			ISequence sequenceServcie = HostProxy.Sequence;
			long newSequence = -1L;
			if (!msgRWriter.TryEnterLock(10000))
			{
				return TransferResult.FAILD;
			}
			long num = 0L;
			try
			{
				List<string> list = new List<string>();
				foreach (Video video in videos)
				{
					list.Add(video.FullFilePath);
					num += video.Size;
				}
				if (CheckFreeSpaceHelper.CheckPCLocalDirFreeSpace(localStroageDir, num))
				{
					return TransferResult.NOT_ENOUGH_SPACE;
				}
				TransferResult result = TransferResult.SUCCESS;
				DataPartitionWrapper.DoProcess(list.ToList(), 6, delegate(IEnumerable<string> partList)
				{
					newSequence = sequenceServcie.New();
					if (msgRWriter.Send("exportVideoFiles", partList.ToList(), newSequence))
					{
						using (FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(newSequence))
						{
							if (fileTransferWrapper == null)
							{
								result = TransferResult.FAILD;
								return false;
							}
							int num2 = partList.Count();
							TransferFileInfo receivedFileInfo = null;
							bool flag = false;
							for (int i = 0; i < num2; i++)
							{
								if (cancelTokenResource != null && cancelTokenResource.IsCancellationRequested)
								{
									result = TransferResult.CANCEL;
									return false;
								}
								flag = fileTransferWrapper.ReceiveFile(localStroageDir, out receivedFileInfo);
								if (callBack != null)
								{
									Video arg = videos.Where((Video m) => m.FullFilePath.Equals(receivedFileInfo.FilePath)).FirstOrDefault();
									callBack(arg, flag);
								}
								fileTransferWrapper.NotifyFileReceiveComplete();
							}
						}
						return result == TransferResult.SUCCESS;
					}
					return false;
				});
				return result;
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

	public TransferResult ImportVideoListToDevice(List<string> localFilePathList, Action<string, bool> callBack, IAsyncTaskContext context)
	{
		if (localFilePathList == null)
		{
			return TransferResult.FAILD;
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
				if (msgRWriter.Send("importVideoFiles", new List<string> { localFilePathList.Count.ToString() }, num))
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
							return TransferResult.SUCCESS;
						}
						foreach (string localFilePath in localFilePathList)
						{
							if (context.IsCancelCommandRequested)
							{
								return TransferResult.CANCEL;
							}
							fileTransfer.SendFile(localFilePath, num);
							bool arg = fileTransfer.WaitFileReceiveCompleteNotify(13000);
							callBack?.Invoke(localFilePath, arg);
						}
					}
					finally
					{
						if (fileTransfer != null)
						{
							((IDisposable)fileTransfer).Dispose();
						}
					}
					_ = string.Empty;
				}
				return TransferResult.SUCCESS;
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

	public bool DeleteVideoFromList(List<Video> videos)
	{
		if (videos == null)
		{
			return false;
		}
		List<PropItem> receiveData = null;
		List<string> list = new List<string>();
		foreach (Video video in videos)
		{
			list.Add(video.Id.ToString());
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
		if (messageReaderAndWriter.SendAndReceiveSync("deleteVideoFiles", "deleteVideoFilesResponse", list, sequence, out receiveData) && receiveData != null)
		{
			return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
		}
		return false;
	}
}
