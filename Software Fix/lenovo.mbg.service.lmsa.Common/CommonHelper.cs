using System.Reflection;

namespace lenovo.mbg.service.lmsa.Common;

public class CommonHelper
{
	public static string GetCurrentVersion()
	{
		return Assembly.GetEntryAssembly().GetName().Name + " V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}

	public static string VersionDownloadUrlAnalysis(string path)
	{
		return ApplcationClass.VersionDownloadUrlHeader + path;
	}
}
