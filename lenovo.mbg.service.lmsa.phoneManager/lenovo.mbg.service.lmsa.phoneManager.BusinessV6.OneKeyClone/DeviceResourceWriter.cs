using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class DeviceResourceWriter : IBackupResourceWriter, IDisposable
{
	private Header requestHeader = new Header();

	public TcpAndroidDevice Device { get; private set; }

	public string ResourceType { get; private set; }

	public AppServiceRequest Request { get; private set; }

	public AppServiceResponse ResponseHandler { get; private set; }

	public IAsyncTaskContext TaskContext { get; set; }

	public int ServiceCode { get; set; }

	public string RemoteMethodName { get; set; }

	public bool IsSecurity { get; private set; }

	protected DeviceResourceStreamWriter DeviceResourceStreamWriter { get; set; }

	public BackupResource CurrentResource { get; set; }

	public DeviceResourceWriter(IAsyncTaskContext taskContext, TcpAndroidDevice device, string resourceType, int serviceCode, string remoteMethodName)
	{
		Device = device;
		Request = new AppServiceRequest(device.ExtendDataFileServiceEndPoint, device.RsaSocketEncryptHelper);
		IsSecurity = device.RsaSocketEncryptHelper.IsSecurity;
		ResourceType = resourceType;
		TaskContext = taskContext;
		ServiceCode = serviceCode;
		RemoteMethodName = remoteMethodName;
	}

	public virtual void ReserveDiskSpace(int resourceItemsCount, long reservereSourceStreamGross)
	{
	}

	public virtual void Dispose()
	{
	}

	public virtual void EndWrite()
	{
	}

	public virtual void RemoveEnd()
	{
		try
		{
			ResponseHandler?.Dispose();
		}
		catch (Exception)
		{
		}
		ResponseHandler = null;
	}

	public virtual IBackupResourceStreamWriter Seek(BackupResource resource)
	{
		if (DeviceResourceStreamWriter == null)
		{
			DeviceResourceStreamWriter = new DeviceResourceStreamWriter(this);
		}
		return DeviceResourceStreamWriter;
	}

	public virtual void Write(BackupResource resource)
	{
		CurrentResource = resource;
		requestHeader.Clear();
		if (resource.AssociatedStreamSize == 0L || Device.PhysicalStatus != DevicePhysicalStateEx.Online || Device.SoftStatus != DeviceSoftStateEx.Online || TaskContext.IsCancelCommandRequested)
		{
			return;
		}
		if (ResponseHandler != null)
		{
			ResponseHandler.Dispose();
			ResponseHandler = null;
		}
		if (ResponseHandler == null)
		{
			ResponseHandler = Request.Request(ServiceCode, RemoteMethodName, new Header(), null);
			TaskContext.AddCancelSource(delegate
			{
				ResponseHandler?.Dispose();
			});
		}
		if (ResponseHandler == null)
		{
			return;
		}
		long associatedStreamSize = resource.AssociatedStreamSize;
		requestHeader.AddOrReplace("Status", "-6");
		requestHeader.AddOrReplace("StreamLength", associatedStreamSize.ToString());
		requestHeader.AddOrReplace("FileFullName", resource.Value);
		if (resource.Attributes != null)
		{
			foreach (KeyValuePair<string, string> attribute in resource.Attributes)
			{
				requestHeader.AddOrReplace(attribute.Key, attribute.Value);
			}
		}
		ResponseHandler.WriteHeader(requestHeader);
		if (ResponseHandler.ReadHeader(null).ContainsAndEqual("Status", "-6"))
		{
			return;
		}
		throw new Exception("Create channel failed, import data failed");
	}

	public void BeginWrite()
	{
	}

	public void SetPassword(string password)
	{
		throw new NotSupportedException();
	}

	public bool CanWrite()
	{
		return true;
	}
}
