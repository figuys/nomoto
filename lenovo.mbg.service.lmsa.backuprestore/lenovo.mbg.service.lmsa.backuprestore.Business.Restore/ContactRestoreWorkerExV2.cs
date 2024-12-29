using System.Collections.Generic;
using System.IO;
using System.Text;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.common.ImportExport;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

[RestoreStorageInfo("{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", "{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}")]
public class ContactRestoreWorkerExV2 : RestoreWorkerAbstractEx
{
	public ContactRestoreWorkerExV2(TcpAndroidDevice device, IBackupResourceReader backupResourceReader, string resourceType, int requestServiceCode, string requestMethodName)
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
		List<string> list = new List<string>();
		TcpAndroidDevice currentDevice = base.Device;
		int num = 10240;
		byte[] array = new byte[num];
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (BackupResource childResourceNode in base.ChildResourceNodes)
		{
			if (childResourceNode.AssociatedStreamSize == 0L)
			{
				continue;
			}
			if (num < childResourceNode.AssociatedStreamSize)
			{
				array = new byte[childResourceNode.AssociatedStreamSize];
			}
			IBackupResourceStreamReader backupResourceStreamReader = base.BackupResourceReader.Seek(childResourceNode);
			int num3 = 0;
			do
			{
				num2 = backupResourceStreamReader.Read(array, num3, num);
				num3 += num2;
				if (currentDevice.PhysicalStatus != DevicePhysicalStateEx.Online && base.TaskContext.IsCancelCommandRequested)
				{
					return;
				}
			}
			while (num2 != 0);
			string @string = Encoding.UTF8.GetString(array, 0, num3);
			string empty = string.Empty;
			using StringReader stringReader = new StringReader(@string);
			while ((empty = stringReader.ReadLine()) != null)
			{
				if ("END:VCARD".Equals(empty))
				{
					stringBuilder.AppendLine(empty);
					list.Add(stringBuilder.ToString());
					stringBuilder.Clear();
				}
				stringBuilder.AppendLine(empty);
			}
		}
		AppDataTransferHelper.AppDataInporter<string> appDataInporter = new AppDataTransferHelper.AppDataInporter<string>(base.RequestServiceCode, list);
		appDataInporter.CloseStreamAfterSend = true;
		appDataInporter.TaskContext = base.TaskContext;
		appDataInporter.ResourceItemStartImportCallback = delegate(string rs)
		{
			base.ItemStartRestoreCallback?.Invoke(rs);
		};
		appDataInporter.ResourceItemFinishImportCallback = delegate(string rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
		{
			base.ItemFinishRestoreCallback?.Invoke(rs, string.Empty, isSuccess);
		};
		appDataInporter.ItemProgressCallback = delegate(string rs, int rl, long rt, long tl)
		{
			base.ItemRestoreProgressCallback?.Invoke(string.Empty, rl, rt, tl);
		};
		appDataInporter.CreateDataReadStream = delegate(string rs, string convertedPath)
		{
			string s = JsonConvert.SerializeObject(rs);
			return new MemoryStream(currentDevice.RsaSocketEncryptHelper.EncryptBase64(Encoding.UTF8.GetBytes(s)));
		};
		appDataInporter.Import(currentDevice, new Header(), base.RequestServiceCode == 23);
	}
}
