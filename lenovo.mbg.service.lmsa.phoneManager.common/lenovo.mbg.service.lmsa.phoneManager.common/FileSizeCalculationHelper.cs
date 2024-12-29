using System;
using System.Collections.Generic;
using System.IO;

namespace lenovo.mbg.service.lmsa.phoneManager.common;

public class FileSizeCalculationHelper
{
	public enum SizeUnitMode
	{
		Byte,
		KiloByte,
		MegaByte,
		GigaByte
	}

	public static long GetFileSize(string sFullName)
	{
		long result = 0L;
		if (File.Exists(sFullName))
		{
			result = new FileInfo(sFullName).Length;
		}
		return result;
	}

	public static string FormatSize(object size)
	{
		string result = string.Empty;
		try
		{
			long num = (long)size;
			if ((double)num < 1024.0)
			{
				result = num.ToString("F2") + " Byte";
			}
			else if ((double)num >= 1024.0 && num < 1048576)
			{
				result = ((double)num / 1024.0).ToString("F2") + " K";
			}
			else if (num >= 1048576 && num < 1073741824)
			{
				result = ((double)num / 1024.0 / 1024.0).ToString("F2") + " M";
			}
			else if (num >= 1073741824)
			{
				result = ((double)num / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " G";
			}
		}
		catch
		{
		}
		return result;
	}

	public static double ConvertSizes(string len)
	{
		double num = 0.0;
		return len.Substring(len.Length - 2, 1) switch
		{
			"T" => double.Parse(len.Substring(0, len.Length - 2)) * 1024.0 * 1024.0 * 1024.0 * 1024.0, 
			"G" => double.Parse(len.Substring(0, len.Length - 2)) * 1024.0 * 1024.0 * 1024.0, 
			"M" => double.Parse(len.Substring(0, len.Length - 2)) * 1024.0 * 1024.0, 
			"K" => double.Parse(len.Substring(0, len.Length - 2)) * 1024.0, 
			_ => double.Parse(len.Substring(0, len.Length - 1)), 
		};
	}

	public static string HumanReadableFilesize(long size)
	{
		string[] array = new string[6] { "B", "KB", "MB", "GB", "TB", "PB" };
		long num = 1024L;
		int num2 = 0;
		while (size >= num && num2 + 1 < array.Length)
		{
			size /= num;
			num2++;
		}
		return $"{size:0.##} {array[num2]}";
	}

	public static long GetFilesSize(List<string> filePaths)
	{
		long num = 0L;
		foreach (string filePath in filePaths)
		{
			num += GetFileSize(filePath);
		}
		return num;
	}

	public static long GetFilesSize(string sFilePath, string sMask)
	{
		long num = 0L;
		if (sMask.Trim() == "")
		{
			return num;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(sFilePath);
		if (!directoryInfo.Exists)
		{
			return num;
		}
		FileInfo[] files = directoryInfo.GetFiles(sMask, SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files)
		{
			num += GetFileSize(fileInfo.FullName);
		}
		return num;
	}

	public static long GetFreeSpaceSize(string driveName)
	{
		return Convert.ToInt64(GetFreeSpaceSize(driveName, SizeUnitMode.Byte));
	}

	public static double GetFreeSpaceSize(string driveName, SizeUnitMode sizeUnitMode)
	{
		long num = 0L;
		driveName = driveName + ":" + Path.DirectorySeparatorChar;
		DriveInfo[] drives = DriveInfo.GetDrives();
		foreach (DriveInfo driveInfo in drives)
		{
			if (driveInfo.Name.ToLower() == driveName.ToLower())
			{
				num = driveInfo.TotalFreeSpace;
			}
		}
		switch (sizeUnitMode)
		{
		case SizeUnitMode.MegaByte:
			num /= 1048576;
			break;
		case SizeUnitMode.GigaByte:
			num /= 1073741824;
			break;
		}
		return num;
	}
}
