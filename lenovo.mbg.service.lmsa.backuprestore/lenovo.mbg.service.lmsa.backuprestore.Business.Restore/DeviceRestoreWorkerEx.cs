using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Exceptions;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

public class DeviceRestoreWorkerEx : IDisposable
{
	private IBackupStorage _storage;

	private IBackupResourceReader _backupResourceReader;

	private string storageVersion = string.Empty;

	protected IAsyncTaskContext _Context;

	private TcpAndroidDevice device;

	private volatile bool _cancel;

	public List<IWorker> _workerList;

	private IWorker _currentWorker;

	private List<string> BigFileList = new List<string> { "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "{958781C8-0788-4F87-A4C3-CBD793AAB1A0}" };

	private Dictionary<string, int> resourcesCountMapping;

	private Action<string, string, object> _notifyHandler;

	private Action<IWorker> _currentWorkerHanler;

	public string StorageFileVersion => storageVersion;

	public IAsyncTaskContext TaskContext
	{
		get
		{
			return _Context;
		}
		set
		{
			_Context = value;
		}
	}

	public Dictionary<string, List<BackupResource>> TypeResourceMapping { get; private set; }

	public Action<string, string> ResourceItemStartRestoreCallback { get; set; }

	public Action<string, int> ResourceTypeStartRestoreCallback { get; set; }

	public Action<string, int, int> ResourceTypeFinishRestoreCallback { get; set; }

	public Action<string, string, string, AppDataTransferHelper.BackupRestoreResult> ResourceItemFinishRestoreCallback { get; set; }

	public Action<string, string, int, long, long> ResourceItemRestoreProgressCallback { get; set; }

	public Action<List<string>, Dictionary<string, List<string>>> RetryCallback { get; set; }

	public Dictionary<string, int> ResourcesCountMapping => resourcesCountMapping;

	public static bool CloseModeWin { get; set; }

	public DeviceRestoreWorkerEx(IAsyncTaskContext context, TcpAndroidDevice device, IBackupStorage storage)
	{
		this.device = device;
		_Context = context;
		_storage = storage;
		_backupResourceReader = _storage.OpenRead(out storageVersion);
		_workerList = new List<IWorker>();
		RetryCallback = FireRetryCallback;
	}

	public void Cancel()
	{
		_cancel = true;
		_currentWorker?.Cancel();
	}

	public void Abort()
	{
		_cancel = true;
		_currentWorker?.Abort();
	}

	public void Dispose()
	{
		_currentWorker = null;
		foreach (IWorker item in _workerList.ToList())
		{
			item.Dispose();
		}
		CloseModeWin = true;
		_storage?.Dispose();
		_backupResourceReader?.Dispose();
		_backupResourceReader = null;
	}

	protected void FireRetryCallback(List<string> selectedType, Dictionary<string, List<string>> retryIds)
	{
		PrepareWorker(selectedType, retryIds);
		DoProcess(null);
	}

	public void UpdateDevice(TcpAndroidDevice dev)
	{
		device = dev;
		foreach (IWorker worker in _workerList)
		{
			RestoreWorkerAbstractEx obj = worker as RestoreWorkerAbstractEx;
			obj.UpdateDevice(dev);
			obj.TaskContext = TaskContext;
		}
	}

	public void PrepareWorker(List<string> resourcesType, Dictionary<string, List<string>> retryIds = null)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: not null } tcpAndroidDevice))
		{
			return;
		}
		TypeResourceMapping = new Dictionary<string, List<BackupResource>>();
		string internalStoragePath = tcpAndroidDevice.Property.InternalStoragePath;
		int num = 0;
		resourcesCountMapping = new Dictionary<string, int>();
		_workerList.Clear();
		foreach (string item in resourcesType)
		{
			RestoreWorkerAbstractEx restoreWorkerAbstractEx = null;
			switch (item)
			{
			case "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}":
				restoreWorkerAbstractEx = new RestoreWorkerAbstractEx(device, _backupResourceReader, item, 17, internalStoragePath);
				break;
			case "{242C8F16-6AC7-431B-BBF1-AE24373860F1}":
				restoreWorkerAbstractEx = new RestoreWorkerAbstractEx(device, _backupResourceReader, item, 19, internalStoragePath);
				break;
			case "{8BEBE14B-4E45-4D36-8726-8442E6242C01}":
				restoreWorkerAbstractEx = new RestoreWorkerAbstractEx(device, _backupResourceReader, item, 20, internalStoragePath);
				break;
			case "{580C48C8-6CEF-4BBB-AF37-D880B349D142}":
				restoreWorkerAbstractEx = new RestoreWorkerAbstractEx(device, _backupResourceReader, item, 25, internalStoragePath);
				break;
			case "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}":
				switch (storageVersion)
				{
				case "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}":
					restoreWorkerAbstractEx = new ContactRestoreWorkerExV2(device, _backupResourceReader, item, 22, string.Empty);
					break;
				case "{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}":
				case "{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}":
					restoreWorkerAbstractEx = new DBDataRestoreWorker(device, _backupResourceReader, item, 23, string.Empty);
					break;
				}
				break;
			case "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}":
				switch (storageVersion)
				{
				case "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}":
					restoreWorkerAbstractEx = new SmsRestoreWorkerExV2(device, _backupResourceReader, item, 21, string.Empty);
					break;
				case "{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}":
				case "{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}":
					restoreWorkerAbstractEx = new SmsRestoreWorkerEx(device, _backupResourceReader, item, 21, string.Empty);
					break;
				}
				break;
			case "{89D4DB68-4258-4002-8557-E65959C558B3}":
				switch (storageVersion)
				{
				case "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}":
					restoreWorkerAbstractEx = new CallLogRestoreWorkerExV2(device, _backupResourceReader, item, 22, string.Empty);
					break;
				case "{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}":
				case "{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}":
					restoreWorkerAbstractEx = new DBDataRestoreWorker(device, _backupResourceReader, item, 22, string.Empty);
					break;
				}
				break;
			}
			if (restoreWorkerAbstractEx == null)
			{
				continue;
			}
			TypeResourceMapping.Add(item, restoreWorkerAbstractEx.ChildResourceNodes);
			resourcesCountMapping[restoreWorkerAbstractEx.ResourceType] = restoreWorkerAbstractEx.ChildResourceNodes.Count;
			restoreWorkerAbstractEx.WorkerSequence = num++;
			restoreWorkerAbstractEx.TaskContext = _Context;
			if (retryIds != null && retryIds.Count > 0)
			{
				LogHelper.LogInstance.Debug("Restore all ids: " + JsonHelper.SerializeObject2Json(retryIds));
				List<BackupResource> all = GetAll();
				if (all != null && all.Count > 0)
				{
					restoreWorkerAbstractEx.RetryNodes = all.Where((BackupResource n) => retryIds[item].Exists((string m) => m == n.Id.ToString())).ToList();
				}
			}
			_workerList.Add(restoreWorkerAbstractEx);
		}
		int num2 = _workerList.FindIndex((IWorker n) => n.WorkerId == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}");
		if (num2 != -1 && num2 != _workerList.Count - 1)
		{
			IWorker item2 = _workerList[num2];
			_workerList.RemoveAt(num2);
			_workerList.Add(item2);
		}
	}

	public List<BackupResource> GetAll()
	{
		List<BackupResource> result = new List<BackupResource>();
		_backupResourceReader.Foreach(null, delegate(BackupResource n)
		{
			result.Add(n);
		});
		return result;
	}

	public long CalculateSize(List<string> resourcesType)
	{
		long size = 0L;
		List<BackupResource> childResources = _backupResourceReader.GetChildResources(null);
		List<string> type = resourcesType.Intersect(BigFileList).ToList();
		List<BackupResource> list = childResources.Where((BackupResource n) => type.Contains(n.Tag)).ToList();
		if (list != null && list.Count > 0)
		{
			list.ForEach(delegate(BackupResource n)
			{
				_backupResourceReader.Foreach(n, delegate(BackupResource o)
				{
					size += o.AssociatedStreamSize;
				});
			});
		}
		return size;
	}

	public int GetFileCount()
	{
		int num = 0;
		foreach (IWorker worker in _workerList)
		{
			if (worker is RestoreWorkerAbstractEx restoreWorkerAbstractEx)
			{
				num += restoreWorkerAbstractEx.ChildResourceNodes.Count;
			}
		}
		return num;
	}

	public void DoProcess(object state)
	{
		foreach (IWorker worker in _workerList)
		{
			_currentWorker = worker;
			RestoreWorkerAbstractEx w = (RestoreWorkerAbstractEx)worker;
			int restoreCount = w.GetRestoreCount();
			if (restoreCount == 0)
			{
				continue;
			}
			ResourceTypeStartRestoreCallback?.Invoke(w.ResourceType, restoreCount);
			if (ResourceItemStartRestoreCallback != null)
			{
				w.ItemStartRestoreCallback = delegate(string path)
				{
					ResourceItemStartRestoreCallback(w.ResourceType, path);
				};
			}
			if (ResourceItemFinishRestoreCallback != null)
			{
				w.ItemFinishRestoreCallback = delegate(string id, string path, AppDataTransferHelper.BackupRestoreResult isSuccess)
				{
					ResourceItemFinishRestoreCallback(w.ResourceType, id, path, isSuccess);
				};
			}
			if (ResourceItemRestoreProgressCallback != null)
			{
				w.ItemRestoreProgressCallback = delegate(string path, int rl, long rt, long len)
				{
					ResourceItemRestoreProgressCallback(w.ResourceType, path, rl, rt, len);
				};
			}
			_currentWorkerHanler?.Invoke(_currentWorker);
			if (_cancel)
			{
				LogHelper.LogInstance.Info("restore resource canceled");
				break;
			}
			try
			{
				if (resourcesCountMapping.ContainsKey(w.ResourceType))
				{
					worker.DoProcess(resourcesCountMapping[w.ResourceType]);
				}
				else
				{
					worker.DoProcess(null);
				}
			}
			catch (CacnelException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				LogHelper.LogInstance.Info(ex2.ToString());
			}
		}
	}

	public void SetNotifyHandler(Action<string, string, object> notifyHandler)
	{
		_notifyHandler = notifyHandler;
	}

	public void SetCurrentWorkerHandler(Action<IWorker> handler)
	{
		_currentWorkerHanler = handler;
	}
}
