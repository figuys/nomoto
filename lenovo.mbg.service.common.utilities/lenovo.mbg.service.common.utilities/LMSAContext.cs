using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using lenovo.mbg.service.common.log;
using Microsoft.Win32;

namespace lenovo.mbg.service.common.utilities;

public class LMSAContext
{
	public static string LANGUAGE = "lang";

	public static string DEF_LANGUAGE = "en-US";

	public static string UPDATE_XML_FILE = "update";

	public static string USER_CONFIG_FILE = "user_config.xml";

	public static string SERVER_CATALOG_FILE = "catalog.xml";

	public static string LANGUAGE_PACKAGE_URI = "https://rsddownload-cloud.motorola.com/RSALanguage";

	public static string PC_BACKUP_PATH = "C:\\ProgramData\\LMSA\\Backup";

	public static string GIF_SAVE_PATH = AppDomain.CurrentDomain.BaseDirectory;

	private static string currentlanguage;

	private static string sMainProcessVersion;

	protected static Dictionary<string, string> LanguageMap = new Dictionary<string, string>
	{
		{ "1033", "en-US" },
		{ "1034", "es-ES" },
		{ "1041", "ja-JP" },
		{ "1045", "pl-PL" },
		{ "1046", "pt-BR" },
		{ "1049", "ru-RU" },
		{ "1040", "it-IT" },
		{ "2052", "zh-CN" },
		{ "1031", "de-DE" },
		{ "1051", "sk-SK" },
		{ "3098", "sr-RS" },
		{ "1048", "ro-RO" },
		{ "1026", "bg-BG" },
		{ "1029", "cs-CZ" },
		{ "1036", "fr-FR" },
		{ "1081", "hi-IN" },
		{ "1057", "id-ID" }
	};

	public static string CurrentLanguage
	{
		get
		{
			if (string.IsNullOrEmpty(currentlanguage))
			{
				currentlanguage = GetCurrentLanguage();
			}
			return currentlanguage;
		}
	}

	public static string OsVersionName
	{
		get
		{
			object obj = (from ManagementObject x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get()
				select x.GetPropertyValue("Caption")).FirstOrDefault();
			if (obj == null)
			{
				return "unknown";
			}
			return obj.ToString();
		}
	}

	public static string LanguagePackageRootPath => Path.Combine(Environment.CurrentDirectory, LANGUAGE);

	public static string MainProcessVersion
	{
		get
		{
			if (string.IsNullOrEmpty(sMainProcessVersion))
			{
				sMainProcessVersion = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).FileVersion;
			}
			return sMainProcessVersion;
		}
	}

	public static string GetLanguageId(string lang)
	{
		return LanguageMap.FirstOrDefault((KeyValuePair<string, string> n) => n.Value == lang).Key;
	}

	private static string GetCurrentLanguage()
	{
		string value = "en-US";
		GlobalFun.TryGetRegistryKey(RegistryHive.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Rescue and Smart Assistant", "NSIS:language", out var value2);
		if (value2 == null)
		{
			return value;
		}
		LanguageMap.TryGetValue(value2.ToString(), out value);
		return value ?? "en-US";
	}

	public static void SetCurrentLanguage(string selectLanguage)
	{
		LogHelper.LogInstance.Info("Set current language [" + GetLanguageId(selectLanguage) + "].");
		GlobalFun.WriteRegistryKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Rescue and Smart Assistant", "NSIS:language", GetLanguageId(selectLanguage));
	}
}
