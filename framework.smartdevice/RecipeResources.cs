using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.resources;

namespace lenovo.mbg.service.framework.smartdevice;

public class RecipeResources
{
	public static string RecipeUrl = "RecipeUrl";

	public static string ModelName = "ModelName";

	public static string TooL = "Tool";

	public static string ToolZip = "ToolZip";

	public static string Rom = "Rom";

	public static string CountryCode = "CountryCode";

	public const string Platform = "Platform";

	public const string IsFastboot = "IsFastboot";

	public Dictionary<string, string> Resources { get; private set; }

	public ResultLogger Log { get; set; }

	public RecipeResources()
	{
		Resources = new Dictionary<string, string>();
	}

	public void Clear()
	{
		Resources.Clear();
	}

	public void Add(string key, string value)
	{
		if (Resources.ContainsKey(key))
		{
			Resources[key] = value;
		}
		else
		{
			Resources.Add(key, value);
		}
	}

	public string Get(string key)
	{
		Resources.TryGetValue(key, out var value);
		return value;
	}

	public void AddResource(string key, string url)
	{
		Rsd.Instance.GetDownloadedResource(url, out var filePath);
		if (key == TooL)
		{
			Resources.Add(TooL, Path.Combine(Configurations.ToolPath, Guid.NewGuid().ToString("N")));
			Resources.Add(ToolZip, filePath);
		}
		else
		{
			Resources.Add(key, filePath);
		}
	}

	public string GetLocalFilePath(string name, string filename = null)
	{
		if (name.Equals("xmlFile", StringComparison.CurrentCultureIgnoreCase))
		{
			if (string.IsNullOrEmpty(filename))
			{
				filename = "flashfile.xml";
			}
			return GetXmlFlashFile(filename);
		}
		if (name.Equals("upgradeXmlFile", StringComparison.CurrentCultureIgnoreCase))
		{
			return GetXmlFlashFile("servicefile.xml");
		}
		if (name.StartsWith("flashinfo", StringComparison.CurrentCultureIgnoreCase) || name.Equals("softwareupgrade", StringComparison.CurrentCultureIgnoreCase))
		{
			return GetFlashInfoFile(name);
		}
		if (name.Equals("recoveryFile", StringComparison.CurrentCultureIgnoreCase))
		{
			return GetRecoveryFile();
		}
		if (name.Equals("recoveryFile2", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(Rom, out var value);
			string fullPathName = GetFullPathName(value, "*.img");
			if (!GlobalFun.Exists(fullPathName))
			{
				Log.AddLog(value + ": *.img not exists", upload: true);
			}
			return fullPathName;
		}
		if (name.Equals("recoveryExe", StringComparison.CurrentCultureIgnoreCase))
		{
			return GetRecoveryExe();
		}
		if (name.Equals("recoveryExe2", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(TooL, out var value2);
			string fullPathName2 = GetFullPathName(value2, "flash_tool.exe");
			if (string.IsNullOrEmpty(fullPathName2))
			{
				fullPathName2 = GetFullPathName(value2, "SPFlashToolV6.exe");
			}
			if (!GlobalFun.Exists(fullPathName2))
			{
				Log.AddLog(value2 + ": flash_tool.exe|SPFlashToolV6.exe not exists", upload: true);
			}
			return fullPathName2;
		}
		if (name.Equals("fastbootExe", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(TooL, out var value3);
			if (!GlobalFun.Exists(GetFullPathName(value3, "fastboot.exe")))
			{
				Log.AddLog(value3 + ": fastboot.exe not exists", upload: true);
			}
			return string.Empty;
		}
		if (name.Equals("countryCodes", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(CountryCode, out var value4);
			if (!GlobalFun.Exists(value4))
			{
				Log.AddLog(value4 + ": country code path not exists", upload: true);
			}
			return value4;
		}
		if (name.Equals("progFile", StringComparison.CurrentCultureIgnoreCase))
		{
			return GetProgFile();
		}
		if (name.Equals("recoveryFolder", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(Rom, out var value5);
			return value5;
		}
		if (name.StartsWith("cfc_flash", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(Rom, out var value6);
			string fullPathName3 = GetFullPathName(value6, name);
			if (!GlobalFun.Exists(fullPathName3))
			{
				Log.AddLog(value6 + ": " + name + " not exists", upload: true);
			}
			return fullPathName3;
		}
		if (name.StartsWith("Check_rollbackid_update", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(Rom, out var value7);
			string fullPathName4 = GetFullPathName(value7, name);
			if (!GlobalFun.Exists(fullPathName4))
			{
				Log.AddLog(value7 + ": " + name + " not exists", upload: true);
			}
			return fullPathName4;
		}
		if (name.StartsWith("root", StringComparison.CurrentCultureIgnoreCase))
		{
			return Environment.CurrentDirectory;
		}
		if (name.Equals("toolFolder", StringComparison.CurrentCultureIgnoreCase))
		{
			Resources.TryGetValue(TooL, out var value8);
			while (Directory.GetFiles(value8).Length == 0)
			{
				string[] directories = Directory.GetDirectories(value8);
				if (directories.Length == 0)
				{
					break;
				}
				value8 = directories[0];
			}
			if (!Directory.Exists(value8))
			{
				Log.AddLog(value8 + ": tool directory not exists", upload: true);
			}
			return value8;
		}
		if (name.ToUpper().EndsWith(".BAT"))
		{
			Resources.TryGetValue(Rom, out var value9);
			string fullPathName5 = GetFullPathName(value9, name);
			if (!GlobalFun.Exists(fullPathName5))
			{
				Log.AddLog(value9 + ": " + name + " not exists", upload: true);
			}
			return fullPathName5;
		}
		Resources.TryGetValue(Rom, out var value10);
		string fullPathName6 = GetFullPathName(value10, name);
		if (string.IsNullOrEmpty(fullPathName6))
		{
			Resources.TryGetValue(TooL, out value10);
			fullPathName6 = GetFullPathName(value10, name);
		}
		if (!GlobalFun.Exists(fullPathName6))
		{
			Log.AddLog(value10 + ": " + name + " not exists", upload: true);
		}
		return fullPathName6;
	}

	private string GetXmlFlashFile(string filename)
	{
		Resources.TryGetValue(Rom, out var value);
		string[] files = Directory.GetFiles(value, filename, SearchOption.AllDirectories);
		if (files != null && files.Length != 0)
		{
			return files.First();
		}
		string fileName = Path.GetFileName(value);
		string text = Path.Combine(value, fileName);
		if (File.Exists(text))
		{
			return text;
		}
		files = Directory.GetFiles(value, "*.xml", SearchOption.AllDirectories);
		string[] array = files;
		foreach (string text2 in array)
		{
			if (File.ReadAllText(text2).Contains("operation=\"flash\""))
			{
				return text2;
			}
		}
		Log.AddLog(value + ": " + filename + " not exists", upload: true);
		return string.Empty;
	}

	private string GetFlashInfoFile(string name)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string empty = string.Empty;
		string value = string.Empty;
		Resources.TryGetValue(Rom, out value);
		Resources.TryGetValue(TooL, out var value2);
		name = name.ToLower();
		if (name.Contains("flashinfo"))
		{
			string text3 = "FlashInfo";
			if (name.Equals("flashinfo3", StringComparison.CurrentCultureIgnoreCase))
			{
				text3 = "FlashInfo_RSA";
			}
			text = GetFullPathName(value, text3 + ".tmp");
			if (string.IsNullOrEmpty(text))
			{
				Log.AddLog(value + ": " + text3 + ".tmp not exists", upload: true);
				return string.Empty;
			}
			value = Path.GetDirectoryName(text);
			text2 = Path.Combine(value, text3 + ".xml");
			empty = GetFullPathName(value2, "flash*.exe");
			if (string.IsNullOrEmpty(empty))
			{
				Log.AddLog(value2 + ": flash*.exe not exists", upload: true);
				return string.Empty;
			}
			value2 = Path.GetDirectoryName(empty);
		}
		else if (name.Contains("softwareupgrade"))
		{
			text = GetFullPathName(value, "SoftwareUpgrade.tmp");
			if (string.IsNullOrEmpty(text))
			{
				Log.AddLog(value + ": SoftwareUpgrade.tmp not exists", upload: true);
				return string.Empty;
			}
			value = Path.GetDirectoryName(text);
			text2 = Path.Combine(value, "SoftwareUpgrade.xml");
			empty = GetFullPathName(value2, "flash*.exe");
			if (string.IsNullOrEmpty(empty))
			{
				Log.AddLog(value2 + ": flash*.exe not exists", upload: true);
				return string.Empty;
			}
			value2 = Path.GetDirectoryName(empty);
		}
		else if (name.Contains("efuse"))
		{
			text = GetFullPathName(value, "Efuse.tmp");
			text2 = GetFullPathName(value, "Efuse.xml");
		}
		else if (name.Contains("lkbin"))
		{
			text = GetFullPathName(value, "LkBin.tmp");
			text2 = GetFullPathName(value, "LkBin.xml");
		}
		if (!File.Exists(text))
		{
			Log.AddLog(text + " not exists", upload: true);
			return string.Empty;
		}
		string text4 = File.ReadAllText(text);
		foreach (Match item in Regex.Matches(text4, "(?<=>)(SP_FLASH_TOOL_DIR|RECOVERY_FILE_DIR).*(?=</)", RegexOptions.IgnoreCase | RegexOptions.Multiline))
		{
			string text5 = item.Value.Replace("RECOVERY_FILE_DIR", value).Replace("SP_FLASH_TOOL_DIR", value2);
			if (!File.Exists(text5))
			{
				Log.AddLog(text5 + " not exists", upload: true);
				return string.Empty;
			}
		}
		text4 = text4.Replace("SP_FLASH_TOOL_DIR", value2);
		text4 = text4.Replace("RECOVERY_FILE_DIR", value);
		File.WriteAllText(text2, text4);
		return text2;
	}

	private string GetRecoveryFile()
	{
		Resources.TryGetValue(Rom, out var value);
		string fullPathName = GetFullPathName(value, "prog_emmc_firehose_*.mbn");
		fullPathName = ((!string.IsNullOrEmpty(fullPathName)) ? Path.GetDirectoryName(fullPathName) : GetFullPathName(value, "*_Android_scatter.txt"));
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "*.pac");
		}
		if (!GlobalFun.Exists(fullPathName))
		{
			Log.AddLog(value + ": prog_emmc_firehose_*.mbn|*_Android_scatter.txt|*.pac not exists", upload: true);
		}
		return fullPathName;
	}

	public string GetRecoveryExe()
	{
		string value = string.Empty;
		Resources.TryGetValue(TooL, out value);
		string fullPathName = GetFullPathName(value, "QcomDLoader.exe");
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "flash_tool.exe");
		}
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "QFIL.exe");
		}
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "CmdDloader.exe");
		}
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "upgrade_tool.exe");
		}
		if (!GlobalFun.Exists(fullPathName))
		{
			Log.AddLog(value + ": QcomDLoader.exe|flash_tool.exe|QFIL.exe|CmdDloader.exe|upgrade_tool.exe not exists", upload: true);
		}
		return fullPathName;
	}

	public string GetRecoveryCmd(string name)
	{
		Resources.TryGetValue(Rom, out var value);
		string fullPathName = GetFullPathName(value, name);
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "Rescue.cmd");
		}
		if (string.IsNullOrEmpty(fullPathName))
		{
			fullPathName = GetFullPathName(value, "Flash.cmd");
		}
		return fullPathName;
	}

	private string GetProgFile()
	{
		Resources.TryGetValue(Rom, out var value);
		string text = (from n in Directory.GetFiles(value, "*ddr.*", SearchOption.AllDirectories)
			where Regex.IsMatch(Path.GetFileName(n), "(.+ddr\\.mbn$)|(.+ddr\\.elf$)")
			select n).FirstOrDefault();
		if (!GlobalFun.Exists(text))
		{
			Log.AddLog(value + ": (.+ddr\\.mbn$)|(.+ddr\\.elf$) not exists", upload: true);
		}
		return text;
	}

	private string GetFullPathName(string dir, string filePattern)
	{
		if (!Directory.Exists(dir))
		{
			return string.Empty;
		}
		string[] files = Directory.GetFiles(dir, filePattern, SearchOption.AllDirectories);
		if (files.Length == 0)
		{
			return string.Empty;
		}
		return files[0];
	}

	public bool IsResourceDirExist()
	{
		Resources.TryGetValue(Rom, out var value);
		if (!Directory.Exists(value))
		{
			Log.AddLog(value + ": rom directory not exists", upload: true);
			return false;
		}
		Resources.TryGetValue(TooL, out var value2);
		if (!string.IsNullOrEmpty(value2) && !Directory.Exists(value2))
		{
			Log.AddLog(value2 + ": tool directory not exists", upload: true);
			return false;
		}
		return true;
	}
}
