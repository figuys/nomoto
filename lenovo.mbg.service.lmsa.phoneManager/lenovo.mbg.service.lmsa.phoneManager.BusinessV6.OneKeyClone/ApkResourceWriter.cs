using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class ApkResourceWriter : DeviceResourceWriter
{
	public ApkResourceWriter(IAsyncTaskContext taskContext, TcpAndroidDevice device, string resourceType, int serviceCode, string remoteMethodName)
		: base(taskContext, device, resourceType, serviceCode, remoteMethodName)
	{
	}

	public override IBackupResourceStreamWriter Seek(BackupResource resource)
	{
		if (base.DeviceResourceStreamWriter == null)
		{
			base.DeviceResourceStreamWriter = new ApkResourceStreamWriter(this);
		}
		return base.DeviceResourceStreamWriter;
	}

	public override void Write(BackupResource resource)
	{
		base.CurrentResource = resource;
	}
}
