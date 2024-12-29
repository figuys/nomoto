using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class OnekeyCloneManager : IDisposable
{
	private TcpAndroidDevice masterDevice;

	private TcpAndroidDevice slaveDevice;

	protected IAsyncTaskContext context;

	private Action<string, string> resourceItemStartBackupCallback;

	private Dictionary<string, int> resourceTypeAndServiceCodeMaping = new Dictionary<string, int>
	{
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", 17 },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", 19 },
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", 20 },
		{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", 23 }
	};

	private Action<string, string, string, bool> resourceItemFinishBackupCallback;

	private Action<string, string, int, long, long> resourceItemBackupProgressCallback;

	private Action finishCloneCallback;

	private Action cancelCallback;

	private Action<CloneMode, int, long> startCloneCallback;

	private Action<string, int> resourceTypeStartBackupCallback;

	private Action<string, int, int> resourceTypeFinishBackupCallback;

	public List<IWorker> Workers { get; set; }

	public List<IWorker> CurrentActivedWorkers { get; set; }

	public Action<string, string> ResourceItemStartBackupCallback
	{
		get
		{
			return resourceItemStartBackupCallback;
		}
		set
		{
			resourceItemStartBackupCallback = value;
			if (Workers == null)
			{
				return;
			}
			foreach (OnkeyCloneWorkerAbstract worker in Workers)
			{
				worker.ItemStartBackupCallback = resourceItemStartBackupCallback;
			}
		}
	}

	public Action<string, string, string, bool> ResourceItemFinishBackupCallback
	{
		get
		{
			return resourceItemFinishBackupCallback;
		}
		set
		{
			resourceItemFinishBackupCallback = value;
			if (Workers == null)
			{
				return;
			}
			foreach (OnkeyCloneWorkerAbstract worker in Workers)
			{
				worker.ItemFinishBackupCallback = resourceItemFinishBackupCallback;
			}
		}
	}

	public Action<string, string, int, long, long> ResourceItemBackupProgressCallback
	{
		get
		{
			return resourceItemBackupProgressCallback;
		}
		set
		{
			resourceItemBackupProgressCallback = value;
			if (Workers == null)
			{
				return;
			}
			foreach (OnkeyCloneWorkerAbstract worker in Workers)
			{
				worker.ItemBackupProgressCallback = resourceItemBackupProgressCallback;
			}
		}
	}

	public Action FinishCloneCallback
	{
		get
		{
			return finishCloneCallback;
		}
		set
		{
			finishCloneCallback = value;
		}
	}

	public Action CancelCallback
	{
		get
		{
			return cancelCallback;
		}
		set
		{
			cancelCallback = value;
		}
	}

	public Action<CloneMode, int, long> StartCloneCallback
	{
		get
		{
			return startCloneCallback;
		}
		set
		{
			startCloneCallback = value;
		}
	}

	public Action<string, int> ResourceTypeStartBackupCallback
	{
		get
		{
			return resourceTypeStartBackupCallback;
		}
		set
		{
			resourceTypeStartBackupCallback = value;
		}
	}

	public Action<string, int, int> ResourceTypeFinishBackupCallback
	{
		get
		{
			return resourceTypeFinishBackupCallback;
		}
		set
		{
			resourceTypeFinishBackupCallback = value;
		}
	}

	public OnekeyCloneManager()
	{
		Workers = new List<IWorker>();
		CurrentActivedWorkers = new List<IWorker>();
		context = new AsyncTaskContext(null);
	}

	public void InitWorker(TcpAndroidDevice dev)
	{
		int num = 0;
		masterDevice = dev;
		foreach (OnkeyCloneWorkerAbstract worker in Workers)
		{
			worker.ItemStartBackupCallback = null;
			worker.ItemFinishBackupCallback = null;
			worker.ItemBackupProgressCallback = null;
			worker.TaskContext = null;
		}
		Workers.Clear();
		CurrentActivedWorkers.Clear();
		foreach (KeyValuePair<string, int> item in resourceTypeAndServiceCodeMaping)
		{
			OnkeyCloneWorkerAbstract onkeyCloneWorkerAbstract = null;
			switch (item.Key)
			{
			case "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}":
			case "{242C8F16-6AC7-431B-BBF1-AE24373860F1}":
			case "{8BEBE14B-4E45-4D36-8726-8442E6242C01}":
				onkeyCloneWorkerAbstract = new OnkeyCloneWorkerAbstract(dev, item.Key, item.Value);
				break;
			case "{958781C8-0788-4F87-A4C3-CBD793AAB1A0}":
				onkeyCloneWorkerAbstract = new ApkOneKeyCloneWorker(dev, item.Key, item.Value);
				break;
			case "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}":
				onkeyCloneWorkerAbstract = new SMSOnkeyCloneWorker(dev, item.Key, item.Value);
				break;
			case "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}":
			case "{89D4DB68-4258-4002-8557-E65959C558B3}":
				onkeyCloneWorkerAbstract = new DBDataOneKeyCloneWorker(dev, item.Key, item.Value);
				break;
			}
			if (onkeyCloneWorkerAbstract != null)
			{
				onkeyCloneWorkerAbstract.ItemStartBackupCallback = ResourceItemStartBackupCallback;
				onkeyCloneWorkerAbstract.ItemFinishBackupCallback = ResourceItemFinishBackupCallback;
				onkeyCloneWorkerAbstract.ItemBackupProgressCallback = ResourceItemBackupProgressCallback;
				onkeyCloneWorkerAbstract.WorkerSequence = num++;
				onkeyCloneWorkerAbstract.TaskContext = context;
				Workers.Add(onkeyCloneWorkerAbstract);
			}
		}
	}

	public void LoadData(Action<bool, string, Dictionary<string, long>> callback)
	{
		Task[] array = new Task[Workers.Count];
		int num = 0;
		foreach (OnkeyCloneWorkerAbstract item in Workers)
		{
			array[num] = Task.Run(delegate
			{
				item.LoadData();
				if (item.IdAndPathList != null)
				{
					callback?.Invoke(arg1: false, item.ResourceType, item.IdAndSizeList);
				}
			});
			num++;
		}
		Task.WhenAll(array).ContinueWith(delegate
		{
			callback?.Invoke(arg1: true, null, null);
		});
	}

	public long GetResourcesTotalSize(List<string> resourcesType)
	{
		long num = 0L;
		foreach (OnkeyCloneWorkerAbstract worker in Workers)
		{
			if (!resourcesType.Contains(worker.ResourceType) || worker.IdAndSizeList == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, long> idAndSize in worker.IdAndSizeList)
			{
				num += idAndSize.Value;
			}
		}
		return num;
	}

	public int GetResourcesTotalCount(List<string> resourcesType)
	{
		int num = 0;
		foreach (OnkeyCloneWorkerAbstract worker in Workers)
		{
			if (resourcesType.Contains(worker.ResourceType) && worker.IdAndSizeList != null)
			{
				num += worker.IdAndSizeList.Count;
			}
		}
		return num;
	}

	public void Clone(TcpAndroidDevice slaveDevice, List<string> selectedResources)
	{
		Clone(CloneMode.Normal, slaveDevice, selectedResources);
	}

	public void CloneRetry()
	{
		Clone(CloneMode.RetryFailedItems, slaveDevice, CurrentActivedWorkers.Select((IWorker m) => ((OnkeyCloneWorkerAbstract)m).ResourceType).ToList());
	}

	public List<IWorker> GetWorkers(List<string> resourcesType)
	{
		return Workers.Where((IWorker m) => resourcesType.Contains(((OnkeyCloneWorkerAbstract)m).ResourceType)).ToList();
	}

	public void Cancel()
	{
		context?.Cancel();
	}

	public void Dispose()
	{
		foreach (IWorker item in Workers.ToList())
		{
			item.Dispose();
		}
	}

	private void Clone(CloneMode cloneMode, TcpAndroidDevice slaveDevice, List<string> selectedResources)
	{
		context.ResetCancelStatus();
		CurrentActivedWorkers.Clear();
		this.slaveDevice = slaveDevice;
		int num = 0;
		long num2 = 0L;
		foreach (OnkeyCloneWorkerAbstract worker in Workers)
		{
			worker.CloneMode = cloneMode;
			if (cloneMode == CloneMode.Normal)
			{
				worker.ResetSuccessCount();
			}
			if (selectedResources.Contains(worker.ResourceType))
			{
				if ("{958781C8-0788-4F87-A4C3-CBD793AAB1A0}".Equals(worker.ResourceType))
				{
					worker.BackupResourceWriter = new ApkResourceWriter(context, slaveDevice, worker.ResourceType, resourceTypeAndServiceCodeMaping[worker.ResourceType], "importData");
				}
				else
				{
					worker.BackupResourceWriter = new DeviceResourceWriter(context, slaveDevice, worker.ResourceType, resourceTypeAndServiceCodeMaping[worker.ResourceType], "importData");
				}
				num += worker.CurentHandleResourcesCount;
				num2 += ((worker.CurrentHandleResourcesList != null) ? worker.CurrentHandleResourcesList.Sum((KeyValuePair<string, long> m) => m.Value) : 0);
				worker.TarEncryptHelper = slaveDevice.RsaSocketEncryptHelper;
				CurrentActivedWorkers.Add(worker);
			}
		}
		StartCloneCallback?.Invoke(cloneMode, num, num2);
		try
		{
			foreach (OnkeyCloneWorkerAbstract currentActivedWorker in CurrentActivedWorkers)
			{
				ResourceTypeStartBackupCallback?.Invoke(currentActivedWorker.ResourceType, currentActivedWorker.CurentHandleResourcesCount);
				if (!context.IsCancelCommandRequested)
				{
					currentActivedWorker.DoProcess(null);
					continue;
				}
				break;
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			if (context.IsCancelCommandRequested)
			{
				CancelCallback?.Invoke();
			}
			else
			{
				FinishCloneCallback?.Invoke();
			}
		}
	}
}
