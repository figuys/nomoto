using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class Configurations
{
	private static string _DownloadPath = DefaultDownloadPath;

	private static string _BackupPath = DefaultBackupPath;

	private static DateTime _BackupLastDateTime = DateTime.MinValue;

	private static string _GifSavePath = DefaultGifSavePath;

	private static string _languageVersion = DefaultLanguageVersion;

	private static string serviceBaseUrl;

	private static readonly object serviceBaseUrlLock = new object();

	private static string serviceInterfaceUrl = string.Empty;

	private static string _baseHttpUrl = string.Empty;

	private static int _apkVersionCode = 1;

	private static string _ProgramDataPath = null;

	private static Dictionary<bool, int> _RescueResultMap = new Dictionary<bool, int>
	{
		{ true, 0 },
		{ false, 0 }
	};

	public const long TRANSFER_FILE_MAX_SIZE = 4294967296L;

	public static string TRANSFER_FILE_ERROR_TXT_PATH = Path.Combine(TempDir, "larger_max_size_file_list.txt");

	public static string ToolPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "ToolFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string RomsPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "RomFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ApkPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "ApkFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string IconPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "IconFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string CountryCodePath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "CountryCodeFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string JsonPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "JsonFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string BannerPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "BannerFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string XamlPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "XamlFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string UnknownPath
	{
		get
		{
			string text = Path.Combine(DownloadPath, "UnknownFiles\\");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string DownloadingSavePath => Path.Combine(DownloadInfoSavePath, "download_resources.json");

	public static string DownloadedSavePath => Path.Combine(DownloadInfoSavePath, "downloaded_resources.json");

	public static string DownloadedMatchPath => Path.Combine(DownloadInfoSavePath, "downloaded_match.json");

	public static string DownloadSpeedPath => Path.Combine(DownloadInfoSavePath, "speed.json.dpapi");

	public static string NoticesPath => Path.Combine(ProgramDataPath, "notices.json.dpapi");

	public static string RescueManualMatchFile => "rescueddevice.json.dpapi";

	public static string DefaultOptionsFileName => "options.json.dpapi";

	public static string RescueRecordsFile => "flashrecords.json.dpapi";

	public static string UserRequestRecordsFile => "user_request_records.json.dpapi";

	public static string AppCacheDir => Path.Combine(ProgramDataPath, "App");

	public static string AppIconCacheDir => Path.Combine(ProgramDataPath, "AppIcon");

	public static string PicCacheDir => Path.Combine(ProgramDataPath, "Pic");

	public static string PicOriginalCacheDir => Path.Combine(ProgramDataPath, "PicOriginal");

	public static string MusicCacheDir => Path.Combine(ProgramDataPath, "Music");

	public static string VideoCacheDir => Path.Combine(ProgramDataPath, "Video");

	public static string ContactCacheDir => Path.Combine(ProgramDataPath, "Contact");

	public static string PhoneBackupCacheDir
	{
		get
		{
			string text = Path.Combine(ProgramDataPath, "Backup");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ScreencapDir => Path.Combine(ProgramDataPath, "Screencap");

	public static string TempDir
	{
		get
		{
			string text = Path.Combine(ProgramDataPath, "Temp");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ProgramDataPath
	{
		get
		{
			if (_ProgramDataPath == null)
			{
				_ProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				_ProgramDataPath = Path.Combine(_ProgramDataPath, "LMSA");
				if (StorageDirName != null && StorageDirName != string.Empty)
				{
					_ProgramDataPath = Path.Combine(_ProgramDataPath, StorageDirName);
				}
			}
			return _ProgramDataPath;
		}
	}

	public static string ProgramDataRSARootPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "RSA");

	public static string ChromiumLogFilePath { get; set; }

	public static string DownloadInfoSavePath
	{
		get
		{
			string text = Path.Combine(ProgramDataPath, "Download");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string DefaultDownloadPath
	{
		get
		{
			string text = FileHelper.ReadWithAesDecrypt(DefaultOptionsFileName, "downloadpath");
			if (text == null)
			{
				text = Path.Combine(ProgramDataRSARootPath, "Download");
			}
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string DownloadPath
	{
		get
		{
			if (!Directory.Exists(_DownloadPath))
			{
				Directory.CreateDirectory(_DownloadPath);
			}
			return _DownloadPath;
		}
		set
		{
			if (!(_DownloadPath == value))
			{
				_DownloadPath = value;
				GlobalFun.WriteRegistryKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Rescue and Smart Assistant", "downloadpath", value);
				FileHelper.WriteJsonWithAesEncrypt(DefaultOptionsFileName, "downloadpath", value);
			}
		}
	}

	private static string DefaultBackupPath
	{
		get
		{
			string text = FileHelper.ReadWithAesDecrypt(DefaultOptionsFileName, "BackupPath");
			if (text == null)
			{
				text = Path.Combine(ProgramDataRSARootPath, "Backup");
			}
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string BackupPath
	{
		get
		{
			if (!Directory.Exists(_BackupPath))
			{
				Directory.CreateDirectory(_BackupPath);
			}
			return _BackupPath;
		}
		set
		{
			if (!(_BackupPath == value))
			{
				_BackupPath = value;
				FileHelper.WriteJsonWithAesEncrypt(DefaultOptionsFileName, "BackupPath", value);
			}
		}
	}

	public static DateTime BackupLastDateTime
	{
		get
		{
			if (_BackupLastDateTime == DateTime.MinValue)
			{
				string value = FileHelper.ReadWithAesDecrypt(DefaultOptionsFileName, "BackupLastDateTime");
				if (!string.IsNullOrEmpty(value))
				{
					_BackupLastDateTime = Convert.ToDateTime(value);
				}
			}
			return _BackupLastDateTime;
		}
		set
		{
			if (!(_BackupLastDateTime == value))
			{
				_BackupLastDateTime = value;
				FileHelper.WriteJsonWithAesEncrypt(DefaultOptionsFileName, "BackupLastDateTime", value);
			}
		}
	}

	private static string DefaultGifSavePath
	{
		get
		{
			string text = FileHelper.ReadWithAesDecrypt(DefaultOptionsFileName, "GifSavePath");
			if (text == null)
			{
				text = Path.Combine(ProgramDataRSARootPath, "Gif");
			}
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string GifSavePath
	{
		get
		{
			if (!Directory.Exists(_GifSavePath))
			{
				Directory.CreateDirectory(_GifSavePath);
			}
			return _GifSavePath;
		}
		set
		{
			if (!(_GifSavePath == value))
			{
				_GifSavePath = value;
				FileHelper.WriteJsonWithAesEncrypt(DefaultOptionsFileName, "GifSavePath", value);
			}
		}
	}

	private static string DefaultLanguageVersion
	{
		get
		{
			string text = FileHelper.ReadWithAesDecrypt(DefaultOptionsFileName, "LanguagePackageVersion");
			if (text != null && text.Split('_')[0] == LMSAContext.MainProcessVersion)
			{
				return text;
			}
			return LMSAContext.MainProcessVersion + "_0";
		}
	}

	public static int LanguagePackageVersion
	{
		get
		{
			return Convert.ToInt32(_languageVersion.Split('_')[1]);
		}
		set
		{
			string text = $"{LMSAContext.MainProcessVersion}_{value}";
			if (!(_languageVersion == text))
			{
				_languageVersion = text;
				FileHelper.WriteJsonWithAesEncrypt(DefaultOptionsFileName, "LanguagePackageVersion", text);
			}
		}
	}

	public static Dictionary<string, string> FileSaveLocalPath => new Dictionary<string, string>
	{
		{ "ROM", RomsPath },
		{ "APK", ApkPath },
		{ "TOOL", ToolPath },
		{ "ICON", IconPath },
		{ "COUNTRYCODE", CountryCodePath },
		{ "JSON", JsonPath },
		{ "BANNER", BannerPath },
		{ "XAML", XamlPath },
		{ "UNKNOWN", UnknownPath }
	};

	public static string AdbPath => Path.Combine(".", "adb.exe");

	public static string RescueFailedLogPath => Path.Combine(".", "rescuefailedtmp.log");

	public static string FastbootPath => Path.Combine(".", "fastboot.exe");

	public static string SevenZipDllPath => Path.Combine(".", "7zSharp", Environment.Is64BitProcess ? "7z64.dll" : "7z.dll");

	public static bool IsReleaseVersion => "https://lsa.lenovo.com".Equals(ServiceBaseUrl, StringComparison.InvariantCultureIgnoreCase);

	public static string ServiceBaseUrl
	{
		get
		{
			if (string.IsNullOrEmpty(serviceBaseUrl))
			{
				lock (serviceBaseUrlLock)
				{
					serviceBaseUrl = GetExecuteConfig().AppSettings.Settings["BaseHttpUrl"].Value.TrimEnd('/');
				}
			}
			return serviceBaseUrl;
		}
	}

	public static string ServiceInterfaceUrl
	{
		get
		{
			if (string.IsNullOrEmpty(serviceInterfaceUrl))
			{
				serviceInterfaceUrl = ServiceBaseUrl + "/Interface";
			}
			return serviceInterfaceUrl;
		}
	}

	public static string BaseHttpUrl
	{
		get
		{
			if (string.IsNullOrEmpty(_baseHttpUrl))
			{
				_baseHttpUrl = GetExecuteConfig().AppSettings.Settings["BaseHttpUrl"].Value;
			}
			return _baseHttpUrl;
		}
	}

	public static int AppVersionCode
	{
		get
		{
			if (_apkVersionCode != 1)
			{
				return _apkVersionCode;
			}
			int.TryParse(ConfigurationManager.AppSettings["ApkVersionCode"], out _apkVersionCode);
			return _apkVersionCode;
		}
		set
		{
			Configuration executeConfig = GetExecuteConfig();
			executeConfig.AppSettings.Settings["ApkVersionCode"].Value = value.ToString();
			executeConfig.Save(ConfigurationSaveMode.Minimal);
			_apkVersionCode = value;
		}
	}

	public static int DefaultAppMinVersionCodeOfMotoStr
	{
		get
		{
			string text = ConfigurationManager.AppSettings["MotoApkMinVersionCode"];
			if (text != null)
			{
				return int.Parse(text);
			}
			return 0;
		}
	}

	public static int AppMinVersionCodeOfMoto { get; set; } = DefaultAppMinVersionCodeOfMotoStr;

	public static string MotoHelperSecurityVersion => ConfigurationManager.AppSettings["MotoHelperSecurityVersion"];

	public static string MotoApkRandomKeyVersion => ConfigurationManager.AppSettings["MotoApkRandomKeyVersion"];

	public static string StorageDirName { get; set; }

	private static bool defaultAppdomain => AppDomain.CurrentDomain.IsDefaultAppDomain();

	public static Dictionary<bool, int> RescueResultMap => _RescueResultMap;

	private static Configuration GetExecuteConfig()
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

	public static void AddRescueResult(bool success)
	{
		_RescueResultMap[success] = ++_RescueResultMap[success];
	}

	public static void ResetRescueResultMap()
	{
		_RescueResultMap = new Dictionary<bool, int>
		{
			{ true, 0 },
			{ false, 0 }
		};
	}
}
