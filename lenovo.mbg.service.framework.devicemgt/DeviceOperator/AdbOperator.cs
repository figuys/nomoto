using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class AdbOperator : IDeviceOperator
{
	private static AdbOperator _instance;

	protected IAdbClient adb = AdbConnectionMonitorEx.m_AdbClient;

	private SortedList<string, DeviceData> devices = new SortedList<string, DeviceData>();

	public static AdbOperator Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			return _instance = new AdbOperator();
		}
	}

	public void ForwardPort(string deviceID, int devicePort, int localPort)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData != null)
		{
			string local = $"tcp:{localPort}";
			string remote = $"tcp:{devicePort}";
			adb.CreateForward(deviceData, local, remote, allowRebind: false);
			adb.ListForward(deviceData).ToList();
		}
	}

	public void Install(string deviceID, string apkPath)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData != null)
		{
			new PackageManager(adb, deviceData).InstallPackage(apkPath, reinstall: true);
		}
	}

	public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData == null)
		{
			return;
		}
		using SyncService syncService = new SyncService(adb, deviceData);
		using Stream stream = File.OpenRead(localFilePath);
		syncService.Push(stream, deviceFilePath, 777, DateTime.Now, null, CancellationToken.None);
	}

	public void Reboot(string deviceID, string mode)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData != null)
		{
			adb.Reboot(mode, deviceData);
		}
	}

	public void RemoveForward(string deviceID, int localPort)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData != null)
		{
			adb.RemoveForward(deviceData, localPort);
		}
	}

	public void RemoveAllForward(string deviceID)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData != null)
		{
			adb.RemoveAllForwards(deviceData);
		}
	}

	public string Shell(string deviceID, string command)
	{
		ConsoleOutputReceiver consoleOutputReceiver = new ConsoleOutputReceiver();
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData == null)
		{
			return "failed";
		}
		try
		{
			adb.ExecuteRemoteCommandAsync(command, deviceData, consoleOutputReceiver, default(CancellationToken)).Wait();
			string text = consoleOutputReceiver.ToString();
			if (text.EndsWith("\r\n"))
			{
				text = text.Remove(text.LastIndexOf("\r\n"));
			}
			return text;
		}
		catch
		{
			return "failed";
		}
	}

	public void Uninstall(string deviceID, string apkName)
	{
		DeviceData deviceData = FindDeviceData(deviceID);
		if (deviceData != null)
		{
			new PackageManager(adb, deviceData).UninstallPackage(apkName);
		}
	}

	public string Command(string command, int timeout = -1, string deviceID = "")
	{
		string adbPath = Configurations.AdbPath;
		string command2 = command;
		if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
		{
			command2 = "-s " + deviceID + " " + command;
		}
		return ProcessRunner.ProcessString(adbPath, command2, timeout);
	}

	public DeviceData FindDeviceData(string deviceID)
	{
		return FindAdbDevices().FirstOrDefault((DeviceData n) => n.Serial == deviceID);
	}

	public List<string> FindDevices()
	{
		return (from n in FindAdbDevices()
			select n.Serial).ToList();
	}

	public List<DeviceData> FindAdbDevices()
	{
		try
		{
			if (!AdbServer.Instance.GetStatus().IsRunning)
			{
				AdbServer.Instance.StartServer("adb.exe", restartServerIfNewer: true);
			}
			List<DeviceData> list = adb.GetDevices();
			if (list == null)
			{
				list = new List<DeviceData>();
			}
			return list.Where((DeviceData n) => !string.IsNullOrEmpty(n.Serial)).ToList();
		}
		catch
		{
			return new List<DeviceData>();
		}
	}
}
