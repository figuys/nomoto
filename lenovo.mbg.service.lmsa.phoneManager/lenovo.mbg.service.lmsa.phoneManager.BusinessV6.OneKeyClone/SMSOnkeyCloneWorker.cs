using System.Collections.Generic;
using lenovo.mbg.service.framework.devicemgt;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class SMSOnkeyCloneWorker : DBDataOneKeyCloneWorker
{
	public SMSOnkeyCloneWorker(TcpAndroidDevice device, string resourceType, int remoteServiceCode)
		: base(device, resourceType, remoteServiceCode)
	{
	}

	protected override void CloneHandler(List<string> idList)
	{
		if (IsAvaiable())
		{
			new DeviceSmsManagement().DoProcessWithChangeSMSDefault(((DeviceResourceWriter)base.BackupResourceWriter).Device, delegate
			{
				base.CloneHandler(idList);
			});
		}
	}
}
