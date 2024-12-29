using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class OnkeyCloneWorkerAbstract : IWorker, IDisposable
{
	private static byte[] splitBuffer = Encoding.UTF8.GetBytes("<EOF>");

	protected string currentResourceName;

	protected volatile CloneMode cloneMode;

	public IBackupResourceWriter BackupResourceWriter { get; set; }

	public IAsyncTaskContext TaskContext { get; set; }

	public string ResourceType { get; private set; }

	protected TcpAndroidDevice Device { get; set; }

	public bool GetPathByIdWhenPathIsEmpty { get; protected set; }

	public virtual string WorkerId { get; set; }

	public virtual int WorkerSequence { get; set; }

	public long DataBytesTotalSize { get; set; }

	public int ResourcesCount { get; protected set; }

	public RsaSocketDataSecurityFactory TarEncryptHelper { get; set; }

	public int CurentHandleResourcesCount
	{
		get
		{
			switch (CloneMode)
			{
			case CloneMode.Normal:
				return ResourcesCount;
			case CloneMode.RetryFailedItems:
				if (FailedItems == null)
				{
					return 0;
				}
				return FailedItems.Count;
			default:
				return 0;
			}
		}
	}

	public int RemoteServiceCode { get; protected set; }

	public Dictionary<string, long> IdAndSizeList { get; protected set; }

	public Dictionary<string, string> IdAndPathList { get; protected set; }

	public Dictionary<string, long> FailedItems { get; protected set; }

	public int SuccessedCount { get; protected set; }

	public Dictionary<string, long> CurrentHandleResourcesList => CloneMode switch
	{
		CloneMode.Normal => IdAndSizeList, 
		CloneMode.RetryFailedItems => FailedItems, 
		_ => null, 
	};

	public CloneMode CloneMode
	{
		get
		{
			return cloneMode;
		}
		set
		{
			cloneMode = value;
		}
	}

	public Action<string, string> ItemStartBackupCallback { get; set; }

	public Action<string, string, string, bool> ItemFinishBackupCallback { get; set; }

	public Action<string, string, int, long, long> ItemBackupProgressCallback { get; set; }

	public OnkeyCloneWorkerAbstract(TcpAndroidDevice device, string resourceType, int remoteServiceCode)
	{
		Device = device;
		ResourceType = resourceType;
		RemoteServiceCode = remoteServiceCode;
		FailedItems = new Dictionary<string, long>();
	}

	public virtual bool LoadData()
	{
		IdAndPathList = new Dictionary<string, string>();
		cloneMode = CloneMode.Normal;
		try
		{
			if (Device == null || Device.PhysicalStatus != DevicePhysicalStateEx.Online || Device.ExtendDataFileServiceEndPoint == null)
			{
				return false;
			}
			string value = new AppServiceRequest(Device.ExtendDataFileServiceEndPoint, Device.RsaSocketEncryptHelper).RequestString(RemoteServiceCode, "getIdAndSizeMapping", null);
			if (!string.IsNullOrEmpty(value))
			{
				Dictionary<string, long> dictionary = null;
				try
				{
					dictionary = JsonConvert.DeserializeObject<Dictionary<string, long>>(value);
				}
				catch
				{
					dictionary = new Dictionary<string, long>();
				}
				IdAndSizeList = dictionary;
				ResourcesCount = ((IdAndSizeList != null) ? IdAndSizeList.Count : 0);
			}
			return true;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("One key clone:get resource id list failed, error: " + ex);
			return false;
		}
	}

	public virtual void DoProcess(object state)
	{
		switch (CloneMode)
		{
		case CloneMode.Normal:
			if (IdAndSizeList != null && IdAndSizeList.Count > 0)
			{
				CloneHandler(IdAndSizeList.Keys.ToList());
			}
			break;
		case CloneMode.RetryFailedItems:
		{
			List<string> list = ((FailedItems != null) ? FailedItems.Keys.ToList() : null);
			if (list != null && list.Count > 0)
			{
				CloneHandler(list);
			}
			break;
		}
		}
	}

	protected void FireOnNotifyUnExportRes(string id, string path, bool isOk)
	{
		ItemFinishBackupCallback?.Invoke(ResourceType, id, path, isOk);
	}

	protected virtual void CloneHandler(List<string> idList)
	{
		GetPathByIdWhenPathIsEmpty = true;
		if (Device == null)
		{
			throw new BackupRestoreException("Current device instance is null,bakcup pic failed!");
		}
		if (!IsAvaiable())
		{
			return;
		}
		new Dictionary<string, string>();
		AppDataTransferHelper.AppDataExporter exporter = new AppDataTransferHelper.AppDataExporter(RemoteServiceCode, idList);
		exporter.TaskContext = TaskContext;
		BackupResource rootResource = new BackupResource();
		rootResource.Tag = ResourceType;
		rootResource.Value = ResourcesCount.ToString();
		rootResource.ParentId = 0;
		rootResource.AssociatedStreamSize = 0L;
		BackupResourceWriter.Write(rootResource);
		BackupResource currentResource = null;
		string currentResourcePath = string.Empty;
		IBackupResourceStreamWriter currentWriter = null;
		exporter.OnNotifyUnExportRes = delegate(string id, string path, AppDataTransferHelper.BackupRestoreResult isOk)
		{
			FireOnNotifyUnExportRes(id, path, isOk == AppDataTransferHelper.BackupRestoreResult.Success);
		};
		exporter.ResourceItemStartExportCallback = delegate(string id)
		{
			currentResourcePath = string.Empty;
			ItemStartBackupCallback?.Invoke(ResourceType, id);
		};
		exporter.ResourceItemReceivePrepare = delegate(string id, Header header)
		{
			BackupResource backupResource = (currentResource = CreateResourceHeader(rootResource, id, header));
			currentResourcePath = backupResource.Value;
			IdAndPathList[id] = currentResourcePath;
			BackupResourceWriter.Write(backupResource);
			currentWriter = BackupResourceWriter.Seek(backupResource);
			currentWriter.BeginWrite();
			currentResourceName = CreateResourceName(id, currentResourcePath);
			return true;
		};
		exporter.ReadStreamCallback = delegate(byte[] bytes, int rl, long rt, long tl)
		{
			if (rl > 0)
			{
				byte[] array = new byte[rl];
				Array.Copy(bytes, 0, array, 0, rl);
				Write2TargetDevice(currentWriter, currentResource, array);
			}
			ItemBackupProgressCallback?.Invoke(ResourceType, currentResourceName, rl, rt, tl);
		};
		exporter.ResourceItemFinishExportCallback = delegate(string id, bool isSuccess, AppDataTransferHelper.ErrorCode errorCode)
		{
			if (!isSuccess)
			{
				FailedItems[id] = IdAndSizeList[id];
				if (GetPathByIdWhenPathIsEmpty && string.IsNullOrEmpty(currentResourcePath))
				{
					currentResourcePath = exporter.GetPathById(Device, id);
					IdAndPathList[id] = currentResourcePath;
				}
				BackupResourceWriter.RemoveEnd();
			}
			else
			{
				SuccessedCount += 1;
				currentWriter.EndWrite();
				if (cloneMode == CloneMode.RetryFailedItems)
				{
					FailedItems.Remove(id);
				}
			}
			ItemFinishBackupCallback?.Invoke(ResourceType, id, currentResourcePath, isSuccess);
			currentResourcePath = string.Empty;
		};
		int num = exporter.Export(Device, null, RemoteServiceCode == 23);
		int num2 = CurentHandleResourcesCount - num;
		for (int i = 0; i < num2; i++)
		{
			ItemStartBackupCallback?.Invoke(ResourceType, string.Empty);
			ItemFinishBackupCallback?.Invoke(ResourceType, string.Empty, "Missing", arg4: false);
		}
	}

	protected virtual void Write2TargetDevice(IBackupResourceStreamWriter currentWriter, BackupResource resource, byte[] sourceByte)
	{
		long sourceCount = sourceByte.Length;
		byte[] array = sourceByte;
		if (TarEncryptHelper.IsSecurity)
		{
			byte[] array2 = ((!(ResourceType == "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}") && !(ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}")) ? TarEncryptHelper.Encrypt(sourceByte) : TarEncryptHelper.EncryptBase64(sourceByte));
			array = new byte[array2.Length + splitBuffer.Length];
			Array.Copy(array2, 0, array, 0, array2.Length);
			Array.Copy(splitBuffer, 0, array, array2.Length, splitBuffer.Length);
		}
		currentWriter.Write(array, 0, array.Length, sourceCount);
	}

	protected virtual string CreateResourceName(string id, string path)
	{
		return Path.GetFileName(path);
	}

	protected virtual BackupResource CreateResourceHeader(BackupResource parent, string id, Header header)
	{
		BackupResource backupResource = new BackupResource();
		backupResource.ParentId = parent.Id;
		backupResource.Value = header.GetString("FileFullName");
		backupResource.Tag = "file";
		backupResource.AssociatedStreamSize = header.GetInt64("StreamLength", 0L);
		if (header.ContainsKey("CreateDateTime"))
		{
			backupResource.AddAttribute("CreateDateTime", header.GetString("CreateDateTime"));
		}
		if (header.ContainsKey("LastModifyDateTime"))
		{
			backupResource.AddAttribute("LastModifyDateTime", header.GetString("LastModifyDateTime"));
		}
		return backupResource;
	}

	public void Dispose()
	{
	}

	public bool IsAvaiable()
	{
		if (Device != null && Device.SoftStatus == DeviceSoftStateEx.Online)
		{
			return !TaskContext.IsCancelCommandRequested;
		}
		return false;
	}

	public void Cancel()
	{
	}

	public void Abort()
	{
	}

	public void ResetSuccessCount()
	{
		SuccessedCount = 0;
	}
}
