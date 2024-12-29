using System;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class LogStatic
{
	private static string logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

	private static string mainlogsPath = "logs";

	public static string GetMainLogPath()
	{
		return mainlogsPath;
	}

	public static string GetMainLogAbsolutePathPath()
	{
		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mainlogsPath);
	}

	public static string GetPluginLogPath(string logPath)
	{
		return Path.Combine(logsPath, logPath);
	}
}
