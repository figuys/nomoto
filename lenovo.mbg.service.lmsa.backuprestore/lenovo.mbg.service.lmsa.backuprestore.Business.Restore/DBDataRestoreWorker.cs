using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.common.ImportExport;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

[RestoreStorageInfo("{89D4DB68-4258-4002-8557-E65959C558B3}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
[RestoreStorageInfo("{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
[RestoreStorageInfo("{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
public class DBDataRestoreWorker : RestoreWorkerAbstractEx
{
	public DBDataRestoreWorker(TcpAndroidDevice device, IBackupResourceReader backupResourceReader, string resourceType, int requestServiceCode, string requestMethodName)
		: base(device, backupResourceReader, resourceType, requestServiceCode, requestMethodName)
	{
	}

	public override void DoProcess(object state)
	{
		if (base.Device == null)
		{
			return;
		}
		CheckCancel();
		List<BackupResource> retryNodes = base.RetryNodes;
		List<BackupResource> dataPathList = ((retryNodes != null && retryNodes.Count > 0) ? base.RetryNodes : base.ChildResourceNodes);
		new AppDataTransferHelper.AppDataInporter<BackupResource>(base.RequestServiceCode, dataPathList)
		{
			CloseStreamAfterSend = true,
			TaskContext = base.TaskContext,
			ResourceItemStartImportCallback = delegate(BackupResource rs)
			{
				base.ItemStartRestoreCallback?.Invoke(rs.Value);
			},
			ResourceItemFinishImportCallback = delegate(BackupResource rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
			{
				base.ItemFinishRestoreCallback?.Invoke(rs.Id.ToString(), string.Empty, isSuccess);
			},
			ItemProgressCallback = delegate(BackupResource rs, int rl, long rt, long tl)
			{
				base.ItemRestoreProgressCallback?.Invoke(string.Empty, rl, rt, tl);
			},
			CreateDataReadStream = delegate(BackupResource rs, string convertedPath)
			{
				IBackupResourceStreamReader backupResourceStreamReader = base.BackupResourceReader.Seek(rs);
				int num = 0;
				byte[] buffer = new byte[1024];
				MemoryStream memoryStream = new MemoryStream();
				do
				{
					num = backupResourceStreamReader.Read(buffer, 0, 1024);
					memoryStream.Write(buffer, 0, num);
				}
				while (num > 0);
				backupResourceStreamReader = null;
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}.Import(isReadStreamString: base.RequestServiceCode == 23 || base.RequestServiceCode == 21 || base.RequestServiceCode == 22, currentDevice: base.Device, requestHeader: new Header());
	}
}
