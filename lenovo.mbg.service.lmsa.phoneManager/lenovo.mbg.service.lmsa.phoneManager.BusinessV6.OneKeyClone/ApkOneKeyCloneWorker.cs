using System;
using System.Collections.Generic;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class ApkOneKeyCloneWorker : OnkeyCloneWorkerAbstract
{
	public ApkOneKeyCloneWorker(TcpAndroidDevice device, string resourceType, int remoteServiceCode)
		: base(device, resourceType, remoteServiceCode)
	{
		base.GetPathByIdWhenPathIsEmpty = false;
	}

	protected override BackupResource CreateResourceHeader(BackupResource parent, string id, Header header)
	{
		return new BackupResource
		{
			ParentId = parent.Id,
			Value = id + ".apk",
			Tag = "file",
			AssociatedStreamSize = header.GetInt64("StreamLength", 0L)
		};
	}

	protected override string CreateResourceName(string id, string path)
	{
		return id + ".apk";
	}

	protected override void CloneHandler(List<string> idList)
	{
		if (!IsAvaiable())
		{
			return;
		}
		base.GetPathByIdWhenPathIsEmpty = true;
		if (base.Device == null)
		{
			throw new BackupRestoreException("Current device instance is null,bakcup pic failed!");
		}
		new Header();
		AppDataTransferHelper.AppDataExporter exporter = new AppDataTransferHelper.AppDataExporter(base.RemoteServiceCode, idList);
		exporter.TaskContext = base.TaskContext;
		BackupResource rootResource = new BackupResource();
		rootResource.Tag = base.ResourceType;
		rootResource.Value = base.ResourcesCount.ToString();
		rootResource.ParentId = 0;
		rootResource.AssociatedStreamSize = 0L;
		base.BackupResourceWriter.Write(rootResource);
		BackupResource currentResource = null;
		string currentResourcePath = string.Empty;
		string currentResourceName = string.Empty;
		IBackupResourceStreamWriter currentWriter = null;
		exporter.OnNotifyUnExportRes = delegate(string id, string path, AppDataTransferHelper.BackupRestoreResult isOk)
		{
			FireOnNotifyUnExportRes(id, path, isOk == AppDataTransferHelper.BackupRestoreResult.Success);
		};
		exporter.ResourceItemStartExportCallback = delegate(string id)
		{
			currentResourcePath = string.Empty;
			base.ItemStartBackupCallback?.Invoke(base.ResourceType, id);
		};
		exporter.ResourceItemReceivePrepare = delegate(string id, Header header)
		{
			BackupResource backupResource = (currentResource = CreateResourceHeader(rootResource, id, header));
			currentResourcePath = backupResource.Value;
			base.IdAndPathList[id] = currentResourcePath;
			base.BackupResourceWriter.Write(backupResource);
			currentWriter = base.BackupResourceWriter.Seek(backupResource);
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
			base.ItemBackupProgressCallback?.Invoke(base.ResourceType, currentResourceName, rl, rt, tl);
		};
		exporter.ResourceItemFinishExportCallback = delegate(string id, bool isSuccess, AppDataTransferHelper.ErrorCode errorCode)
		{
			if (!isSuccess)
			{
				base.FailedItems[id] = base.IdAndSizeList[id];
				if (base.GetPathByIdWhenPathIsEmpty && string.IsNullOrEmpty(currentResourcePath))
				{
					currentResourcePath = exporter.GetPathById(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, id);
					base.IdAndPathList[id] = currentResourcePath;
				}
				base.BackupResourceWriter.RemoveEnd();
			}
			else
			{
				base.SuccessedCount += 1;
				if (cloneMode == CloneMode.RetryFailedItems)
				{
					base.FailedItems.Remove(id);
				}
			}
			base.ItemFinishBackupCallback?.Invoke(base.ResourceType, id, currentResourcePath, isSuccess);
			currentResourcePath = string.Empty;
		};
		Header header2 = new Header();
		header2.AddOrReplace("Timeout", "60000");
		int num = exporter.Export(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, header2, base.RemoteServiceCode == 23);
		int num2 = base.CurentHandleResourcesCount - num;
		for (int i = 0; i < num2; i++)
		{
			base.ItemStartBackupCallback?.Invoke(base.ResourceType, string.Empty);
			base.ItemFinishBackupCallback?.Invoke(base.ResourceType, string.Empty, "Missing", arg4: false);
		}
	}
}
