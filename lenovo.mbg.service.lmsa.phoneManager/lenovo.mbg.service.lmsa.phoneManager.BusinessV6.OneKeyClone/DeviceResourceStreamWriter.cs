using System;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class DeviceResourceStreamWriter : IBackupResourceStreamWriter
{
	protected RsaSocketDataSecurityFactory rsaSocketDataSecurityFactory;

	public DeviceResourceWriter DeviceResourceWriter { get; private set; }

	public DeviceResourceStreamWriter(DeviceResourceWriter deviceResourceWriter)
	{
		DeviceResourceWriter = deviceResourceWriter;
		rsaSocketDataSecurityFactory = deviceResourceWriter.Device.RsaSocketEncryptHelper;
	}

	public virtual void BeginWrite()
	{
	}

	public virtual void EndWrite()
	{
	}

	public virtual void Seek(long offset)
	{
	}

	public virtual void Write(byte[] buffer, int offset, int count, long sourceCount)
	{
		DeviceResourceWriter.ResponseHandler.Write(buffer, offset, count);
		int num = 0;
		int num2 = (DeviceResourceWriter.IsSecurity ? 28 : 4);
		byte[] array = new byte[num2];
		do
		{
			if (DeviceResourceWriter.ResponseHandler.Read(array, 0, num2) >= num2)
			{
				num += BitConverter.ToInt32(rsaSocketDataSecurityFactory.DecryptBase64(array), 0);
			}
		}
		while (num < sourceCount);
	}
}
