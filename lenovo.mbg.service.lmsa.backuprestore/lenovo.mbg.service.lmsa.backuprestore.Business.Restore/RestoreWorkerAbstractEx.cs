using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.themes.generic.Exceptions;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

[RestoreStorageInfo("{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
[RestoreStorageInfo("{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
[RestoreStorageInfo("{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
public class RestoreWorkerAbstractEx : IWorker, IDisposable
{
	private int mRequestServiceCode;

	private string mRemoteDir;

	protected IBackupResourceReader BackupResourceReader { get; set; }

	public string ResourceType { get; set; }

	protected TcpAndroidDevice Device { get; set; }

	public virtual string WorkerId { get; set; }

	public int WorkerSequence { get; set; }

	public IAsyncTaskContext TaskContext { get; set; }

	protected BackupResource RootResource { get; set; }

	protected int RequestServiceCode => mRequestServiceCode;

	protected string RemoteDir => mRemoteDir;

	public List<BackupResource> RetryNodes { get; set; }

	public List<BackupResource> ChildResourceNodes { get; set; }

	public Action<string, int, long, long> ItemRestoreProgressCallback { get; set; }

	public Action<string> ItemStartRestoreCallback { get; set; }

	public Action<string, string, AppDataTransferHelper.BackupRestoreResult> ItemFinishRestoreCallback { get; set; }

	protected CancellationTokenSource CancelSource { get; set; }

	protected CancellationTokenSource AbortSource { get; set; }

	public RestoreWorkerAbstractEx(TcpAndroidDevice device, IBackupResourceReader backupResourceReader, string resourceType, int requestServiceCode, string remoteDir)
	{
		RestoreWorkerAbstractEx restoreWorkerAbstractEx = this;
		Device = device;
		mRemoteDir = remoteDir;
		WorkerId = resourceType;
		ResourceType = resourceType;
		mRequestServiceCode = requestServiceCode;
		BackupResourceReader = backupResourceReader;
		ChildResourceNodes = new List<BackupResource>();
		CancelSource = new CancellationTokenSource();
		AbortSource = new CancellationTokenSource();
		(from n in backupResourceReader.GetChildResources(null)
			where n.Tag.Equals(resourceType)
			select n).ToList().ForEach(delegate(BackupResource n)
		{
			List<BackupResource> childResources = restoreWorkerAbstractEx.BackupResourceReader.GetChildResources(n);
			if (childResources != null && childResources.Count > 0)
			{
				restoreWorkerAbstractEx.ChildResourceNodes.AddRange(childResources);
			}
		});
	}

	public void UpdateDevice(TcpAndroidDevice dev)
	{
		Device = dev;
	}

	public int GetRestoreCount()
	{
		List<BackupResource> retryNodes = RetryNodes;
		return ((retryNodes != null && retryNodes.Count > 0) ? RetryNodes : ChildResourceNodes).Count;
	}

	public virtual void DoProcess(object state)
	{
		if (Device == null)
		{
			return;
		}
		List<BackupResource> retryNodes = RetryNodes;
		List<BackupResource> dataPathList = ((retryNodes != null && retryNodes.Count > 0) ? RetryNodes : ChildResourceNodes);
		new AppDataTransferHelper.AppDataInporter<BackupResource>(mRequestServiceCode, dataPathList)
		{
			CloseStreamAfterSend = true,
			TaskContext = TaskContext,
			ItemProgressCallback = delegate(BackupResource rs, int rl, long rt, long tl)
			{
				ItemRestoreProgressCallback?.Invoke(CreateResourceName(rs.Value), rl, rt, tl);
			},
			ResourceItemStartImportCallback = delegate(BackupResource rs)
			{
				ItemStartRestoreCallback?.Invoke(rs.Value);
			},
			ResourceItemFinishImportCallback = delegate(BackupResource rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
			{
				ItemFinishRestoreCallback?.Invoke(rs.Id.ToString(), rs.Value, isSuccess);
				if (isSuccess != 0 && isSuccess != AppDataTransferHelper.BackupRestoreResult.Undo)
				{
					LogHelper.LogInstance.Info($"restore file {rs.Value} {isSuccess}!");
				}
			},
			DataPathConverter = (BackupResource rs) => Path.Combine(RemoteDir, rs.Value),
			CreateDataReadStream = (BackupResource rs, string convertedPath) => (Stream)BackupResourceReader.Seek(rs),
			HeaderOperate = delegate(BackupResource res, Header header)
			{
				if (res.Attributes == null || res.Attributes.Count == 0)
				{
					return;
				}
				foreach (KeyValuePair<string, string> attribute in res.Attributes)
				{
					header.AddOrReplace(attribute.Key, attribute.Value);
				}
			}
		}.Import(isReadStreamString: mRequestServiceCode == 23 || mRequestServiceCode == 21 || mRequestServiceCode == 22, currentDevice: Device, requestHeader: new Header());
	}

	protected virtual string CreateResourceName(string path)
	{
		return Path.GetFileName(path);
	}

	public virtual void Cancel()
	{
		CancelSource?.Cancel();
	}

	public void Abort()
	{
		AbortSource?.Cancel();
	}

	protected virtual void CheckCancel()
	{
		if (TaskContext != null && !TaskContext.IsCancelCommandRequested)
		{
			return;
		}
		throw new CacnelException("operator canceled");
	}

	public void Dispose()
	{
		if (CancelSource != null)
		{
			CancelSource.Dispose();
			CancelSource = null;
		}
		if (AbortSource != null)
		{
			AbortSource.Dispose();
			AbortSource = null;
		}
	}
}
