using lenovo.mbg.service.framework.devicemgt;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class SmsRestoreWorkerEx : RestoreWorkerAbstractEx
{
	private DeviceSmsManagement _smsMgt = new DeviceSmsManagement();

	private DeviceSmsManagement _mgt = new DeviceSmsManagement();

	public SmsRestoreWorkerEx(TcpAndroidDevice device, IBackupResourceReader backupResourceReader, string resourceType, int requestServiceCode, string requestMethodName)
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
				base.DoProcess(state);
			});
		}
	}

	protected override string CreateResourceName(string path)
	{
		return string.Empty;
	}
}
