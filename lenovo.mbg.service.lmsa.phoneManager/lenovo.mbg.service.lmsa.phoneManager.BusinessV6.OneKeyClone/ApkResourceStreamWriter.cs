using System;
using System.IO;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.phonemanager.apps.Common;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class ApkResourceStreamWriter : DeviceResourceStreamWriter
{
	public FileStream FileWriterStream { get; set; }

	public ApkResourceStreamWriter(DeviceResourceWriter deviceResourceWriter)
		: base(deviceResourceWriter)
	{
	}

	public override void BeginWrite()
	{
		string currentApkFileFullName = GetCurrentApkFileFullName();
		try
		{
			if (File.Exists(currentApkFileFullName))
			{
				File.Delete(currentApkFileFullName);
			}
			FileWriterStream = new FileStream(currentApkFileFullName, FileMode.CreateNew, FileAccess.Write);
		}
		catch (Exception)
		{
			FileWriterStream = null;
		}
	}

	private string GetCurrentApkFileFullName()
	{
		return Path.Combine(Configurations.TempDir, base.DeviceResourceWriter.CurrentResource.Value);
	}

	public override void EndWrite()
	{
		if (FileWriterStream != null)
		{
			try
			{
				FileWriterStream.Close();
			}
			catch (Exception)
			{
			}
			string currentApkFileFullName = GetCurrentApkFileFullName();
			DeviceAppManager.Instance.Install(currentApkFileFullName, base.DeviceResourceWriter.Device);
		}
	}

	public override void Write(byte[] buffer, int offset, int count, long sourceCount)
	{
		if (FileWriterStream != null)
		{
			FileWriterStream.Write(buffer, offset, count);
		}
	}
}
