using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Backup;

public class BackupWorkerAbstractEx : IWorker, IDisposable
{
	protected IBackupResourceWriter BackupResourceWriter { get; set; }

	public IAsyncTaskContext TaskContext { get; set; }

	public string ResourceType { get; private set; }

	protected TcpAndroidDevice Device { get; set; }

	public bool GetPathByIdWhenPathIsEmpty { get; protected set; }

	public virtual string WorkerId { get; set; }

	public virtual int WorkerSequence { get; set; }

	public int ResourcesCount { get; protected set; }

	public int RemoteServiceCode { get; protected set; }

	public List<int> FailedIdList { get; private set; }

	public Dictionary<string, long> IdAndSizeList { get; protected set; }

	public Action<string> ItemStartBackupCallback { get; set; }

	public Action<string, string, AppDataTransferHelper.BackupRestoreResult> ItemFinishBackupCallback { get; set; }

	public Action<string, int, long, long> ItemBackupProgressCallback { get; set; }

	public BackupWorkerAbstractEx(TcpAndroidDevice device, IBackupResourceWriter backupResourceWriter, string resourceType, Dictionary<string, long> idAndSizeMapping, int remoteServiceCode)
	{
		Device = device;
		BackupResourceWriter = backupResourceWriter;
		ResourceType = resourceType;
		IdAndSizeList = idAndSizeMapping;
		ResourcesCount = idAndSizeMapping.Count;
		RemoteServiceCode = remoteServiceCode;
		FailedIdList = new List<int>();
	}

	public virtual void DoProcess(object state)
	{
		GetPathByIdWhenPathIsEmpty = true;
		if (Device == null)
		{
			LogHelper.LogInstance.Info("The device has been disconnected during backup");
			return;
		}
		BackupResource rootResource = new BackupResource();
		rootResource.Tag = ResourceType;
		rootResource.Value = ResourcesCount.ToString();
		rootResource.ParentId = 0;
		rootResource.AssociatedStreamSize = 0L;
		try
		{
			BackupResourceWriter.Write(rootResource);
		}
		catch
		{
			foreach (KeyValuePair<string, long> idAndSize in IdAndSizeList)
			{
				ItemFinishBackupCallback?.Invoke(idAndSize.Key, $"{idAndSize.Value}", AppDataTransferHelper.BackupRestoreResult.Undo);
			}
			return;
		}
		List<string> value = IdAndSizeList.Keys.Select((string n) => n).ToList();
		if (state != null && state is Dictionary<string, List<string>>)
		{
			(state as Dictionary<string, List<string>>).TryGetValue(ResourceType, out value);
		}
		AppDataTransferHelper.AppDataExporter exporter = new AppDataTransferHelper.AppDataExporter(RemoteServiceCode, value);
		exporter.TaskContext = TaskContext;
		IBackupResourceStreamWriter currentWriter = null;
		HugeMemoryStream encryptMs = new HugeMemoryStream();
		BackupResource currentResource = null;
		string currentResourcePath = string.Empty;
		string currentResourceName = string.Empty;
		bool canRemoveEndResources = false;
		exporter.OnNotifyUnExportRes = ItemFinishBackupCallback;
		exporter.ResourceItemStartExportCallback = delegate(string id)
		{
			canRemoveEndResources = false;
			currentResourcePath = string.Empty;
			ItemStartBackupCallback?.Invoke(id);
		};
		exporter.ResourceItemReceivePrepare = delegate(string id, Header header)
		{
			BackupResource backupResource = (currentResource = CreateResourceHeader(rootResource, id, header));
			currentResourcePath = backupResource.Value;
			BackupResourceWriter.Write(backupResource);
			canRemoveEndResources = true;
			currentWriter = BackupResourceWriter.Seek(backupResource);
			currentWriter.BeginWrite();
			currentResourceName = CreateResourceName(id, currentResourcePath);
			return true;
		};
		exporter.ReadStreamCallback = delegate(byte[] bytes, int rl, long rt, long tl)
		{
			if (rl > 0)
			{
				encryptMs.Write(bytes, 0, rl);
			}
			ItemBackupProgressCallback?.Invoke(currentResourceName, rl, rt, tl);
		};
		exporter.ResourceItemFinishExportCallback = delegate(string id, bool isSuccess, AppDataTransferHelper.ErrorCode errorCode)
		{
			if (!isSuccess)
			{
				if (GetPathByIdWhenPathIsEmpty && string.IsNullOrEmpty(currentResourcePath))
				{
					currentResourcePath = exporter.GetPathById(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, id);
				}
				if (canRemoveEndResources)
				{
					BackupResourceWriter.RemoveEnd();
				}
				if (int.TryParse(id, out var result))
				{
					FailedIdList.Add(result);
				}
				LogHelper.LogInstance.Info("backup id was not in a correct format: " + id + "\t" + currentResourcePath);
			}
			else
			{
				encryptMs.Position = 0L;
				int num = 0;
				byte[] buffer = new byte[1024];
				do
				{
					num = encryptMs.Read(buffer, 0, 1024);
					currentWriter.Write(buffer, 0, num, encryptMs.Length);
				}
				while (num > 0);
				currentWriter.EndWrite();
			}
			encryptMs = new HugeMemoryStream();
			if (isSuccess)
			{
				ItemFinishBackupCallback?.Invoke(id, currentResourcePath, AppDataTransferHelper.BackupRestoreResult.Success);
			}
			else
			{
				ItemFinishBackupCallback?.Invoke(id, currentResourcePath, AppDataTransferHelper.BackupRestoreResult.Fail);
			}
			currentResourcePath = string.Empty;
		};
		if (HostProxy.deviceManager.MasterDevice == null)
		{
			foreach (KeyValuePair<string, long> idAndSize2 in IdAndSizeList)
			{
				ItemFinishBackupCallback?.Invoke(idAndSize2.Key, $"{idAndSize2.Value}", AppDataTransferHelper.BackupRestoreResult.Undo);
			}
			return;
		}
		bool isReadStreamString = RemoteServiceCode == 23 || RemoteServiceCode == 21 || RemoteServiceCode == 22;
		int num2 = exporter.Export(Device, null, isReadStreamString);
		int num3 = ResourcesCount - num2;
		for (int i = 0; i < num3; i++)
		{
			ItemStartBackupCallback?.Invoke(string.Empty);
			ItemFinishBackupCallback?.Invoke(string.Empty, "Missing", AppDataTransferHelper.BackupRestoreResult.Undo);
		}
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

	public void Cancel()
	{
	}

	public void Abort()
	{
	}
}
