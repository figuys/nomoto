using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

public class SmsRestoreWorkerEx : RestoreWorkerAbstractEx
{
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
