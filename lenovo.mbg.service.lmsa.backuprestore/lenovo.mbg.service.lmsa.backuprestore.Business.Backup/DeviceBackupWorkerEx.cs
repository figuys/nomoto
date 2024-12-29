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

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Backup;

public class DeviceBackupWorkerEx : IDisposable
{
	private IBackupStorage _storage;

	private IBackupResourceWriter _backupResourceWriter;

	protected IAsyncTaskContext _Context;

	private TcpAndroidDevice device;

	private List<IWorker> _workerList;

	private IWorker _currentWorker;

	private long locker;

	protected volatile bool isReserveDiskSpace;

	private Action<IWorker> _currentWorkerHanler;

	private IBackupResourceWriter backupResourceWriter
	{
		get
		{
			return _backupResourceWriter;
		}
		set
		{
			if (_backupResourceWriter == null)
			{
				_backupResourceWriter = value;
			}
		}
	}

	public Action<string, string> ResourceItemStartBackupCallback { get; set; }

	public Action<string, int> ResourceTypeStartBackupCallback { get; set; }

	public Action<string, int, int> ResourceTypeFinishBackupCallback { get; set; }

	public Action<string, string, string, AppDataTransferHelper.BackupRestoreResult> ResourceItemFinishBackupCallback { get; set; }

	public Action<string, string, int, long, long> ResourceItemBackupProgressCallback { get; set; }

	public Action<Dictionary<string, List<string>>> RetryCallback { get; set; }

	private BackupResource BackupDescription { get; set; }

	private bool writerIsOpen { get; set; }

	public DeviceBackupWorkerEx(TcpAndroidDevice device, IAsyncTaskContext context, FileInfo storageFile, BackupDescription description)
	{
		this.device = device;
		_Context = context;
		_storage = new BackupStorage(storageFile);
		SetDescription(description);
		_workerList = new List<IWorker>();
		RetryCallback = FireRetryCallback;
	}

	public void Cancel()
	{
		_currentWorker?.Cancel();
	}

	public void Abort()
	{
		_currentWorker?.Abort();
	}

	public void Dispose()
	{
		_currentWorker = null;
		if (writerIsOpen)
		{
			writerIsOpen = false;
			backupResourceWriter.EndWrite();
		}
		foreach (IWorker item in _workerList.ToList())
		{
			item.Dispose();
		}
		if (_storage != null)
		{
			_storage.Dispose();
		}
		_backupResourceWriter?.Dispose();
		_backupResourceWriter = null;
	}

	protected void FireRetryCallback(Dictionary<string, List<string>> retryResources)
	{
		try
		{
			if (Interlocked.Read(ref locker) != 0L)
			{
				return;
			}
			Interlocked.Exchange(ref locker, 1L);
			Dictionary<string, Dictionary<string, long>> dictionary = new Dictionary<string, Dictionary<string, long>>();
			if (retryResources == null || _workerList == null)
			{
				return;
			}
			foreach (KeyValuePair<string, List<string>> item in retryResources)
			{
				if (_workerList.FirstOrDefault((IWorker p) => (p as BackupWorkerAbstractEx).ResourceType == item.Key) is BackupWorkerAbstractEx backupWorkerAbstractEx)
				{
					dictionary[item.Key] = backupWorkerAbstractEx.IdAndSizeList.Where((KeyValuePair<string, long> m) => item.Value.Contains(m.Key)).ToDictionary((KeyValuePair<string, long> m) => m.Key, (KeyValuePair<string, long> v) => v.Value);
				}
			}
			PrepareWorker(dictionary, null);
			retryResources.Clear();
			DoProcess(null);
		}
		finally
		{
			Interlocked.Exchange(ref locker, 0L);
		}
	}

	public void PrepareWorker(Dictionary<string, Dictionary<string, long>> resourcesResource, string password)
	{
		_workerList.Clear();
		if (backupResourceWriter == null)
		{
			backupResourceWriter = _storage.OpenWrite("{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}");
		}
		if (!isReserveDiskSpace)
		{
			try
			{
				ReserveDiskSpace(resourcesResource);
				isReserveDiskSpace = true;
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Reserve disk space failed:" + ex);
				try
				{
					_storage?.Dispose();
					_storage?.Delete();
				}
				catch (Exception)
				{
				}
				throw;
			}
		}
		if (!string.IsNullOrWhiteSpace(password))
		{
			backupResourceWriter.SetPassword(password);
		}
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<string, long>> item in resourcesResource)
		{
			BackupWorkerAbstractEx backupWorkerAbstractEx = null;
			switch (item.Key)
			{
			case "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}":
				backupWorkerAbstractEx = new BackupWorkerAbstractEx(device, backupResourceWriter, item.Key, item.Value, 17);
				break;
			case "{242C8F16-6AC7-431B-BBF1-AE24373860F1}":
				backupWorkerAbstractEx = new BackupWorkerAbstractEx(device, backupResourceWriter, item.Key, item.Value, 19);
				break;
			case "{8BEBE14B-4E45-4D36-8726-8442E6242C01}":
				backupWorkerAbstractEx = new BackupWorkerAbstractEx(device, backupResourceWriter, item.Key, item.Value, 20);
				break;
			case "{580C48C8-6CEF-4BBB-AF37-D880B349D142}":
				backupWorkerAbstractEx = new BackupWorkerAbstractEx(device, backupResourceWriter, item.Key, item.Value, 25);
				break;
			case "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}":
				backupWorkerAbstractEx = new DBDataBackupWorker(device, backupResourceWriter, item.Key, item.Value, 23);
				break;
			case "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}":
				backupWorkerAbstractEx = new DBDataBackupWorker(device, backupResourceWriter, item.Key, item.Value, 21);
				break;
			case "{89D4DB68-4258-4002-8557-E65959C558B3}":
				backupWorkerAbstractEx = new DBDataBackupWorker(device, backupResourceWriter, item.Key, item.Value, 22);
				break;
			}
			backupWorkerAbstractEx.WorkerSequence = num++;
			backupWorkerAbstractEx.TaskContext = _Context;
			_workerList.Add(backupWorkerAbstractEx);
		}
	}

	private void ReserveDiskSpace(Dictionary<string, Dictionary<string, long>> resourcesResource)
	{
		long plaintextByteLength = resourcesResource.Sum((KeyValuePair<string, Dictionary<string, long>> m) => m.Value.Sum((KeyValuePair<string, long> n) => n.Value));
		int num = resourcesResource.Sum((KeyValuePair<string, Dictionary<string, long>> m) => m.Value.Count);
		_ = resourcesResource.FirstOrDefault((KeyValuePair<string, Dictionary<string, long>> m) => "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}".Equals(m.Key)).Value;
		long encryptByteLength = AES.GetEncryptByteLength(plaintextByteLength, partBlockForPlaintext: true);
		long encryptByteLength2 = AES.GetEncryptByteLength(6036 * num, partBlockForPlaintext: true);
		int num2 = 104857600;
		long num3 = encryptByteLength + encryptByteLength2 + 97 + num2;
		LogHelper.LogInstance.Info("Reserve disk space:" + GlobalFun.ConvertLong2String(num3));
		backupResourceWriter.ReserveDiskSpace(num, num3);
	}

	public void DoProcess(object state)
	{
		if (!writerIsOpen)
		{
			_workerList.Sum((IWorker m) => ((BackupWorkerAbstractEx)m).IdAndSizeList.Count);
			backupResourceWriter.BeginWrite();
			writerIsOpen = true;
		}
		backupResourceWriter.Write(BackupDescription);
		foreach (IWorker worker in _workerList)
		{
			BackupWorkerAbstractEx w = (BackupWorkerAbstractEx)worker;
			ResourceTypeStartBackupCallback?.Invoke(w.ResourceType, w.ResourcesCount);
			_currentWorker = worker;
			if (ResourceItemStartBackupCallback != null)
			{
				w.ItemStartBackupCallback = delegate(string path)
				{
					ResourceItemStartBackupCallback(w.ResourceType, path);
				};
			}
			if (ResourceItemFinishBackupCallback != null)
			{
				w.ItemFinishBackupCallback = delegate(string id, string path, AppDataTransferHelper.BackupRestoreResult isSuccess)
				{
					ResourceItemFinishBackupCallback(w.ResourceType, id, path, isSuccess);
				};
			}
			if (ResourceItemBackupProgressCallback != null)
			{
				w.ItemBackupProgressCallback = delegate(string path, int rl, long rt, long len)
				{
					ResourceItemBackupProgressCallback(w.ResourceType, path, rl, rt, len);
				};
			}
			_currentWorkerHanler?.Invoke(_currentWorker);
			if (_Context.IsCancelCommandRequested)
			{
				LogHelper.LogInstance.Info("backup resource canceled");
				break;
			}
			worker.DoProcess(state);
		}
		_currentWorker = null;
	}

	private void SetDescription(BackupDescription description)
	{
		string value = JsonUtils.Stringify(description);
		BackupResource backupResource = new BackupResource();
		backupResource.ParentId = 0;
		backupResource.Value = value;
		backupResource.Tag = "{AF7750C4-A38C-400F-9A9C-5C3DAC0CA829}";
		backupResource.AssociatedStreamSize = 0L;
		BackupDescription = backupResource;
	}

	public void SetCurrentWorkerHandler(Action<IWorker> handler)
	{
		_currentWorkerHanler = handler;
	}
}
