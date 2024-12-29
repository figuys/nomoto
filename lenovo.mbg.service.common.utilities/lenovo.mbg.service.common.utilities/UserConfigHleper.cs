using System;
using System.IO;
using System.Reflection;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.common.utilities;

public class UserConfigHleper
{
	private string fileName;

	private string langRoot;

	private static UserConfigHleper instance;

	public SysConfig Config { get; private set; }

	public static UserConfigHleper Instance => instance ?? (instance = new UserConfigHleper());

	public string LangRoot => langRoot;

	public string ConfigReadyLang => Config.ReadyLang;

	public string ConfigLanguage => Config.Language;

	public string ConfigBackupPath
	{
		get
		{
			try
			{
				if (!Directory.Exists(Config.BackupPath))
				{
					Directory.CreateDirectory(Config.BackupPath);
				}
			}
			catch
			{
			}
			return Config.BackupPath;
		}
	}

	public string ConfigGifSavePath => Config.GifSavePath;

	private UserConfigHleper()
	{
		InitializeContext();
		InitializeUserConfig();
	}

	private void InitializeContext()
	{
		string currentDirectory = Environment.CurrentDirectory;
		fileName = Path.Combine(currentDirectory, LMSAContext.USER_CONFIG_FILE);
		langRoot = Path.Combine(currentDirectory, LMSAContext.LANGUAGE);
	}

	private void InitializeUserConfig()
	{
		if (File.Exists(fileName))
		{
			Config = XmlSerializeHelper.DeserializeFromFile<SysConfig>(fileName) as SysConfig;
		}
	}

	public void SetCurrentLanguage(string selectLanguage)
	{
		Config.Language = selectLanguage;
		Config.ReadyLang = selectLanguage;
		GlobalFun.WriteRegistryKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Rescue and Smart Assistant", "NSIS:language", LMSAContext.GetLanguageId(selectLanguage));
		XmlSerializeHelper.Serializer<SysConfig>(fileName, Config, FileShare.Write);
	}

	public void SetReadyLanguage(string selectLanguage)
	{
		Config = XmlSerializeHelper.DeserializeFromFile<SysConfig>(fileName) as SysConfig;
		Config.ReadyLang = selectLanguage;
		XmlSerializeHelper.Serializer<SysConfig>(fileName, Config, FileShare.Write);
	}

	public void SetBackUpPath(string selectpath)
	{
		Config = XmlSerializeHelper.DeserializeFromFile<SysConfig>(fileName) as SysConfig;
		Config.BackupPath = selectpath;
		XmlSerializeHelper.Serializer<SysConfig>(fileName, Config, FileShare.Write);
	}

	public void SetGifSavePath(string selectpath)
	{
		Config = XmlSerializeHelper.DeserializeFromFile<SysConfig>(fileName) as SysConfig;
		Config.GifSavePath = selectpath;
		XmlSerializeHelper.Serializer<SysConfig>(fileName, Config, FileShare.Write);
	}

	public void UpdateBackUpPathConfig(string key, string value)
	{
		PropertyInfo[] properties = Config.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.PropertyType == typeof(string))
			{
				propertyInfo.GetValue(Config, null);
				_ = propertyInfo.Name;
			}
		}
		XmlSerializeHelper.Serializer<SysConfig>(fileName, Config, FileShare.Write);
	}

	public void MigrateDataToOptions()
	{
		try
		{
			if (File.Exists(fileName))
			{
				Configurations.BackupPath = Config.BackupPath;
				Configurations.GifSavePath = Config.GifSavePath;
				GlobalFun.TryDeleteFile(fileName);
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("migrate old data(user_config.xml) error:", exception);
		}
	}
}
