using System;
using System.Configuration;

namespace lenovo.mbg.service.common.log;

public class ReadConfigInfoHelper
{
	public static Configuration GetExecuteConfig()
	{
		if (AppDomain.CurrentDomain.IsDefaultAppDomain())
		{
			return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		}
		return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
		{
			ExeConfigFilename = "./Software Fix.exe.config"
		}, ConfigurationUserLevel.None);
	}
}
