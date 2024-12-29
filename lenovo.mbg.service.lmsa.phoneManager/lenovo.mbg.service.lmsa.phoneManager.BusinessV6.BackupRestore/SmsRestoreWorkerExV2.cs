using System.Collections.Generic;
using System.IO;
using System.Text;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

[RestoreStorageInfo("{89D4DB68-4258-4002-8557-E65959C558B3}", "{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}")]
public class SmsRestoreWorkerExV2 : RestoreWorkerAbstractEx
{
	private DeviceSmsManagement _smsMgt = new DeviceSmsManagement();

	private DeviceSmsManagement _mgt = new DeviceSmsManagement();

	public SmsRestoreWorkerExV2(TcpAndroidDevice device, IBackupResourceReader backupResourceReader, string resourceType, int requestServiceCode, string requestMethodName)
		: base(device, backupResourceReader, resourceType, requestServiceCode, requestMethodName)
	{
	}

	public override void DoProcess(object state)
	{
		if (base.Device != null)
		{
			CheckCancel();
			_mgt.DoProcessWithChangeSMSDefault(base.Device, delegate
			{
				PrivateDoProcess(state);
			});
		}
	}

	private void PrivateDoProcess(object state)
	{
		List<SMS> list = new List<SMS>();
		TcpAndroidDevice device = base.Device;
		int num = 10240;
		byte[] array = new byte[num];
		int num2 = 0;
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
				if (device.PhysicalStatus != DevicePhysicalStateEx.Online && base.TaskContext.IsCancelCommandRequested)
				{
					return;
				}
			}
			while (num2 != 0);
			string @string = Encoding.UTF8.GetString(array, 0, num3);
			list.AddRange(JsonConvert.DeserializeObject<List<SMS>>(@string));
		}
		AppDataTransferHelper.AppDataInporter<SMS> obj = new AppDataTransferHelper.AppDataInporter<SMS>(base.RequestServiceCode, list)
		{
			CloseStreamAfterSend = false,
			TaskContext = base.TaskContext,
			ResourceItemStartImportCallback = delegate(SMS rs)
			{
				base.ItemStartRestoreCallback?.Invoke(rs._id);
			},
			ResourceItemFinishImportCallback = delegate(SMS rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
			{
				base.ItemFinishRestoreCallback?.Invoke(string.Empty, string.Empty, isSuccess == AppDataTransferHelper.BackupRestoreResult.Success);
			},
			ItemProgressCallback = delegate(SMS rs, int rl, long rt, long tl)
			{
				base.ItemRestoreProgressCallback?.Invoke(string.Empty, rl, rt, tl);
			}
		};
		MemoryStream memoryStream = new MemoryStream();
		obj.CreateDataReadStream = delegate(SMS rs, string convertedPath)
		{
			string s = JsonConvert.SerializeObject(rs);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			int num4 = bytes.Length;
			memoryStream.Seek(0L, SeekOrigin.Begin);
			memoryStream.Write(bytes, 0, num4);
			memoryStream.SetLength(num4);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		};
		obj.Import(base.Device, new Header());
	}

	protected override string CreateResourceName(string path)
	{
		return string.Empty;
	}
}
