using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

public class FileTransferManager : IFileTransferManager, IDisposable
{
	private class TransferMappingModel : IDisposable
	{
		public string InstanceID { get; private set; }

		public FileTransferWrapper Transfer { get; set; }

		public AutoResetEvent CreatedResetEvent { get; set; }

		public TransferMappingModel()
		{
			InstanceID = Guid.NewGuid().ToString();
		}

		public void Dispose()
		{
		}
	}

	private IPEndPointInfo _remoteEndPoint;

	protected RsaSocketDataSecurityFactory _RsaSocketEncryptHelper;

	private volatile bool mIsDispose;

	private readonly object _mappingLock;

	private readonly Dictionary<long, TransferMappingModel> _mapping;

	public string InstanceID { get; private set; }

	public bool IsDispose
	{
		get
		{
			return mIsDispose;
		}
		set
		{
			mIsDispose = value;
		}
	}

	public FileTransferManager(IPEndPointInfo remoteEndPoint, RsaSocketDataSecurityFactory encryptHelper)
	{
		InstanceID = Guid.NewGuid().ToString();
		_mappingLock = new object();
		_mapping = new Dictionary<long, TransferMappingModel>();
		_remoteEndPoint = remoteEndPoint;
		_RsaSocketEncryptHelper = encryptHelper;
	}

	public IPEndPointInfo GetIPEndPointInfo()
	{
		return _remoteEndPoint;
	}

	public FileTransferWrapper CreateFileTransfer(long messageSequence)
	{
		LogHelper.LogInstance.Debug(string.Format(InstanceID + ":public FileTransferWrapper CreateFileTransfer(long messageSequence) entered,[sequence:{0}],[instancedId:{1},disposed:{2}]", messageSequence, InstanceID, IsDispose));
		if (IsDispose)
		{
			return null;
		}
		using AutoResetEvent autoResetEvent = new AutoResetEvent(initialState: false);
		TransferMappingModel transferMappingModel = new TransferMappingModel();
		transferMappingModel.CreatedResetEvent = autoResetEvent;
		lock (_mappingLock)
		{
			_mapping[messageSequence] = transferMappingModel;
		}
		BeginCreateFreeTransferAndMapping();
		try
		{
			if (autoResetEvent.WaitOne(120000))
			{
				if (IsDispose)
				{
					return null;
				}
				TransferMappingModel transferMappingModel2 = null;
				lock (_mappingLock)
				{
					transferMappingModel2 = _mapping[messageSequence];
				}
				FileTransferWrapper transfer = transferMappingModel2.Transfer;
				transferMappingModel2.Dispose();
				return transfer;
			}
			LogHelper.LogInstance.Debug(string.Format("Create file transfer time out, mapping{2},Instance:{0},sequence:{1}", InstanceID, messageSequence, transferMappingModel.InstanceID));
			return null;
		}
		catch
		{
			return null;
		}
	}

	private void BeginCreateFreeTransferAndMapping()
	{
		Task.Factory.StartNew(delegate
		{
			long secquence = -1L;
			try
			{
				FileTransferWrapper fileTransferWrapper = new FileTransferWrapper(_remoteEndPoint, _RsaSocketEncryptHelper);
				SocketWrapper channel = fileTransferWrapper.Channel;
				MessageReaderAndWriter messageReaderAndWriter = new MessageReaderAndWriter(new MessageWriter(channel, appendSpliterString: false, _RsaSocketEncryptHelper), new MessageReader(channel, new PackageSpliter(Constants.Encoding.GetBytes("\\r\\n"), removePackageSpliter: true), _RsaSocketEncryptHelper));
				LogHelper.LogInstance.Debug($"Connect to {_remoteEndPoint.IPAddress}:{_remoteEndPoint.Point}, and send transfer channel ready!");
				List<PropItem> receiveData = null;
				if (messageReaderAndWriter.Receive("fileTransferChannelIsReady", out secquence, out receiveData, 15000))
				{
					LogHelper.LogInstance.Debug($"File transfer channel is ready  received,Instance:{InstanceID},sequence:{secquence}");
					TransferMappingModel transferMappingModel = null;
					try
					{
						lock (_mappingLock)
						{
							transferMappingModel = _mapping[secquence];
						}
						transferMappingModel.Transfer = fileTransferWrapper;
					}
					finally
					{
						LogHelper.LogInstance.Debug($"Begin send file transfer channel is ready response,Instance:{InstanceID},sequence:{secquence}");
						messageReaderAndWriter.Send("fileTransferChannelIsReadyResponse", new List<string>(), secquence);
					}
					try
					{
						LogHelper.LogInstance.Debug(string.Format("Begin release mapping{2},Instance:{0},sequence:{1}", InstanceID, secquence, transferMappingModel.InstanceID));
						transferMappingModel.CreatedResetEvent.Set();
						return;
					}
					catch (Exception ex)
					{
						LogHelper.LogInstance.Error(string.Format(InstanceID + ":Set mappint thread notify [sequence:{0},Disposed:{1}] throw exception:{2}", secquence, IsDispose, ex.ToString()));
						return;
					}
				}
				LogHelper.LogInstance.Debug("send transfer channel ready failed!");
			}
			catch (Exception ex2)
			{
				LogHelper.LogInstance.Error(string.Format(InstanceID + ":Create new file transfer[sequence:{0},Disposed:{1}] throw exception:{2}", secquence, IsDispose, ex2.ToString()));
			}
		});
	}

	public void Dispose()
	{
		if (IsDispose)
		{
			return;
		}
		List<TransferMappingModel> list = null;
		lock (_mappingLock)
		{
			list = _mapping.Values.ToList();
		}
		foreach (TransferMappingModel item in list)
		{
			item.Transfer?.Dispose();
			try
			{
				item.CreatedResetEvent.Set();
			}
			catch (Exception)
			{
			}
		}
	}
}
