using System;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class PluginHelper
{
	private static string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

	public static string GetPluginAssemblyPath(string plugin)
	{
		return Path.Combine(pluginsPath, plugin);
	}
}
