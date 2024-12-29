using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Device;

public interface IDeviceOperator
{
	string Command(string command, int timeout = -1, string deviceID = "");

	string Shell(string deviceID, string command);

	void Install(string deviceID, string apkPath);

	void Uninstall(string deviceID, string apkName);

	void ForwardPort(string deviceID, int devicePort, int localPort);

	void RemoveForward(string deviceID, int localPort);

	void RemoveAllForward(string deviceID);

	void PushFile(string deviceID, string localFilePath, string deviceFilePath);

	void Reboot(string deviceID, string mode);

	List<string> FindDevices();
}
