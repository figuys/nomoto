using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class HardDisk
{
	public static long GetHardDiskFreeSpace(string path)
	{
		return new DriveInfo(Path.GetPathRoot(path)).AvailableFreeSpace;
	}
}
