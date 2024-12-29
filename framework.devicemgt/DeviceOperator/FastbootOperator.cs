using System;
using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class FastbootOperator : IDeviceOperator
{
	protected static string fastbootExe = Path.Combine(".", "fastbootmonitor.exe");

	public string Command(string command, int timeout = -1, string deviceID = "")
	{
		if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) == 0)
		{
			_ = "-s " + deviceID + " " + command;
		}
		return ProcessRunner.ProcessString(Configurations.FastbootPath, command, timeout);
	}

	public List<string> FindDevices()
	{
		string text = ProcessRunner.ProcessString(fastbootExe, "devices", 6000);
		List<string> list = new List<string>();
		string[] array = text.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (array2.Length >= 2)
			{
				string text2 = array2[0].Trim();
				if (!text2.Contains("??????"))
				{
					list.Add(text2);
				}
			}
		}
		return list;
	}

	public void ForwardPort(string deviceID, int devicePort, int localPort)
	{
		throw new NotImplementedException();
	}

	public void Install(string deviceID, string apkPath)
	{
		throw new NotImplementedException();
	}

	public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
	{
		throw new NotImplementedException();
	}

	public void Reboot(string deviceID, string mode)
	{
		throw new NotImplementedException();
	}

	public void RemoveForward(string deviceID, int localPort)
	{
		throw new NotImplementedException();
	}

	public string Shell(string deviceID, string command)
	{
		throw new NotImplementedException();
	}

	public void Uninstall(string deviceID, string apkName)
	{
		throw new NotImplementedException();
	}

	public void RemoveAllForward(string deviceID)
	{
		throw new NotImplementedException();
	}
}
