using System.IO;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

public class CheckFreeSpaceHelper
{
	public static bool CheckPCLocalDirFreeSpace(string dir, long size)
	{
		long hardDiskSpace = GetHardDiskSpace(dir.Substring(0, dir.IndexOf(':')));
		return size > hardDiskSpace;
	}

	public static long GetHardDiskSpace(string str_HardDiskName)
	{
		long result = 0L;
		str_HardDiskName += ":\\";
		DriveInfo[] drives = DriveInfo.GetDrives();
		foreach (DriveInfo driveInfo in drives)
		{
			if (driveInfo.Name == str_HardDiskName)
			{
				result = driveInfo.TotalFreeSpace;
			}
		}
		return result;
	}
}
