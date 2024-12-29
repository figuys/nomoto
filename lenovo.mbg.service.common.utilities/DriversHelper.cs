using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;
using Microsoft.Win32;

namespace lenovo.mbg.service.common.utilities;

public class DriversHelper
{
	public static readonly Dictionary<string, List<DriverType>> Driver_White_List = new Dictionary<string, List<DriverType>>
	{
		{
			"XT2433",
			new List<DriverType>
			{
				DriverType.Motorola,
				DriverType.Unisoc
			}
		},
		{
			"XT2423",
			new List<DriverType>
			{
				DriverType.Motorola,
				DriverType.MTK
			}
		},
		{
			"XT2425",
			new List<DriverType>
			{
				DriverType.Motorola,
				DriverType.MTK
			}
		}
	};

	private static readonly Dictionary<string, string> MTK_Driver_Mapping = new Dictionary<string, string>
	{
		{
			"cdc-acm",
			Path.Combine(DRIVERS_SAVE_PATH, "MTK_20240103\\CDC\\cdc-acm.inf")
		},
		{
			"android_winusb",
			Path.Combine(DRIVERS_SAVE_PATH, "MTK_20240103\\Android\\android_winusb.inf")
		}
	};

	private static readonly Dictionary<string, string> Motorola_Driver_Mapping = new Dictionary<string, string> { 
	{
		"motoandroid2",
		Path.Combine(DRIVERS_SAVE_PATH, "Motorola\\motoandroid2.inf")
	} };

	private static readonly Dictionary<string, string> Lenovo_Driver_Mapping = new Dictionary<string, string>
	{
		{
			"Lenovo_adb",
			Path.Combine(DRIVERS_SAVE_PATH, "Lenovo_20240103\\ADB\\Lenovo_adb.inf")
		},
		{
			"Lenovo_mdm",
			Path.Combine(DRIVERS_SAVE_PATH, "Lenovo_20240103\\MODEM\\Lenovo_mdm.inf")
		},
		{
			"Lenovo_ser",
			Path.Combine(DRIVERS_SAVE_PATH, "Lenovo_20240103\\SERIAL\\Lenovo_ser.inf")
		}
	};

	private static readonly Dictionary<string, string> Unisoc_Window10_Driver_Mapping = new Dictionary<string, string>
	{
		{
			"DriversForWin10_rdavcom",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin10\\rdavcom.inf")
		},
		{
			"DriversForWin10_sprdadb",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin10\\sprdadb.inf")
		},
		{
			"DriversForWin10_sprdvcom",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin10\\sprdvcom.inf")
		},
		{
			"DriversForWin10_sprdvmdm",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin10\\sprdvmdm.inf")
		}
	};

	private static readonly Dictionary<string, string> Unisoc_Window78_Driver_Mapping = new Dictionary<string, string>
	{
		{
			"DriversForWin78_rdavcom",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin78\\rdavcom.inf")
		},
		{
			"DriversForWin78_sprdadb",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin78\\sprdadb.inf")
		},
		{
			"DriversForWin78_sprdvcom",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin78\\sprdvcom.inf")
		},
		{
			"DriversForWin78_sprdvmdm",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\DriversForWin78\\sprdvmdm.inf")
		}
	};

	private static readonly Dictionary<string, string> Unisoc_L19111_Driver_Mapping = new Dictionary<string, string>
	{
		{
			"L19111drivers_sprdadb",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\L19111drivers\\sprdadb.inf")
		},
		{
			"L19111drivers_sprdvcom",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\L19111drivers\\sprdvcom.inf")
		}
	};

	private static readonly Dictionary<string, string> Unisoc_SPRD_Driver_Mapping = new Dictionary<string, string>
	{
		{
			"SPRD_NPI_drivers_sprdvcom",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\SPRD_NPI_drivers\\sprdvcom.inf")
		},
		{
			"SPRD_NPI_drivers_sprdadb",
			Path.Combine(DRIVERS_SAVE_PATH, "Unisoc\\SPRD_NPI_drivers\\sprdadb.inf")
		}
	};

	private static readonly Dictionary<string, string> PNP_Driver_Mapping = new Dictionary<string, string> { 
	{
		"ElinkAdb_android_winusb",
		Path.Combine(DRIVERS_SAVE_PATH, "Elink\\ADBDriver\\android_winusb.inf")
	} };

	private static readonly Dictionary<string, string> PNP_Window10_Driver_Mapping = new Dictionary<string, string> { 
	{
		"ElinkWin10_rockusb",
		Path.Combine(DRIVERS_SAVE_PATH, "Elink\\Driver\\x64\\win10\\rockusb.inf")
	} };

	private static readonly Dictionary<string, string> PNP_Window78_Driver_Mapping = new Dictionary<string, string> { 
	{
		"ElinkWin7_rockusb",
		Path.Combine(DRIVERS_SAVE_PATH, "Elink\\Driver\\x64\\win7\\rockusb.inf")
	} };

	private static readonly Dictionary<string, string> ADB_Driver_Mapping = new Dictionary<string, string> { 
	{
		"android_winusb",
		Path.Combine(DRIVERS_SAVE_PATH, "ADBDriver\\android_winusb.inf")
	} };

	private static readonly Tuple<DriverType, string, string> Motorola_Driver_Exe_Install_Info = new Tuple<DriverType, string, string>(DriverType.MTK, "Motorola Mobile Drivers Installation 6.4.0", Path.Combine(DRIVERS_SAVE_PATH, "Motorola\\Motorola_End_User_Driver_Installation_6.4.0_64bit.msi"));

	private static readonly Dictionary<DriverType, Dictionary<string, string>> DriverType_Driver_Mapping = new Dictionary<DriverType, Dictionary<string, string>>
	{
		{
			DriverType.MTK,
			MTK_Driver_Mapping
		},
		{
			DriverType.Motorola,
			Motorola_Driver_Mapping
		},
		{
			DriverType.Lenovo,
			Lenovo_Driver_Mapping
		},
		{
			DriverType.Unisoc_Window10,
			Unisoc_Window10_Driver_Mapping
		},
		{
			DriverType.Unisoc_Window78,
			Unisoc_Window78_Driver_Mapping
		},
		{
			DriverType.Unisoc_L19111,
			Unisoc_L19111_Driver_Mapping
		},
		{
			DriverType.Unisoc_SPRD,
			Unisoc_SPRD_Driver_Mapping
		},
		{
			DriverType.PNP,
			PNP_Driver_Mapping
		},
		{
			DriverType.PNP_Window10,
			PNP_Window10_Driver_Mapping
		},
		{
			DriverType.PNP_Window78,
			PNP_Window78_Driver_Mapping
		},
		{
			DriverType.ADBDRIVER,
			ADB_Driver_Mapping
		}
	};

	private static readonly string PNPUTIL_INSTALL_FORMAT_COMMAND = "pnputil /add-driver \"{0}\" /install";

	private static readonly string DRIVERS_REGISTRY_PATH = "SYSTEM\\Software\\Microsoft\\LmsaDrivers\\drivers";

	private static string DRIVERS_SAVE_PATH => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LMSA\\drivers\\");

	[DllImport("USER32.DLL")]
	public static extern bool SetForegroundWindow(IntPtr hWnd);

	public static bool CheckAndInstallInfDriver(DriverType drivertype, Action confrimAction, out string output)
	{
		Dictionary<string, string> dictionary;
		if (DriverType.Unisoc == drivertype)
		{
			dictionary = new Dictionary<string, string>(DriverType_Driver_Mapping[DriverType.Unisoc_SPRD]);
			Dictionary<string, string> dictionary2 = ((!IsWindow10Or11()) ? DriverType_Driver_Mapping[DriverType.Unisoc_Window78] : DriverType_Driver_Mapping[DriverType.Unisoc_Window10]);
			foreach (KeyValuePair<string, string> item in dictionary2)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		else if (DriverType.PNP == drivertype)
		{
			dictionary = new Dictionary<string, string>(DriverType_Driver_Mapping[DriverType.PNP]);
			Dictionary<string, string> dictionary3 = ((!IsWindow10Or11()) ? DriverType_Driver_Mapping[DriverType.PNP_Window78] : DriverType_Driver_Mapping[DriverType.PNP_Window10]);
			foreach (KeyValuePair<string, string> item2 in dictionary3)
			{
				dictionary.Add(item2.Key, item2.Value);
			}
		}
		else
		{
			dictionary = DriverType_Driver_Mapping[drivertype];
		}
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> item3 in dictionary)
		{
			if (!CheckDriverInstall(drivertype.ToString(), item3.Key))
			{
				if (File.Exists(item3.Value))
				{
					string text = ExecuteCommand(string.Format(PNPUTIL_INSTALL_FORMAT_COMMAND, item3.Value));
					stringBuilder.AppendLine(text);
					flag = text != null && text.IndexOf("failed", StringComparison.CurrentCultureIgnoreCase) < 0;
					if (flag)
					{
						WriteRegistry(drivertype.ToString(), item3.Key, item3.Value);
					}
				}
				else
				{
					stringBuilder.AppendLine($"the file: {item3.Value} not found");
				}
			}
			else
			{
				flag = true;
			}
		}
		output = stringBuilder.ToString();
		return flag;
	}

	public static void InstallDriver(List<string> drivers)
	{
		foreach (string driver in drivers)
		{
			if (File.Exists(driver))
			{
				string text = driver;
				if (Regex.IsMatch(text, "^*.msi$", RegexOptions.IgnoreCase))
				{
					text = "msiexec /i \"" + text + "\"";
				}
				LogHelper.LogInstance.Debug("install driver: " + text);
				ExecuteCommand(text);
			}
			else
			{
				LogHelper.LogInstance.Error("the file: " + driver + " not found");
			}
		}
	}

	public static void InstallDriver(List<Tuple<DriverType, string, string>> drivers, Action confrimAction, Action finisheAction)
	{
		confrimAction?.Invoke();
		foreach (Tuple<DriverType, string, string> driver in drivers)
		{
			if (File.Exists(driver.Item3))
			{
				string text = driver.Item3;
				if (Regex.IsMatch(text, "^*.msi$", RegexOptions.IgnoreCase))
				{
					text = "msiexec /i \"" + text + "\"";
				}
				ExecuteCommand(text);
				if (finisheAction != null)
				{
					finisheAction();
				}
				else
				{
					WriteRegistry(driver.Item1.ToString(), driver.Item2, driver.Item3);
				}
			}
			else
			{
				LogHelper.LogInstance.Error("the file: " + driver.Item3 + " not found");
			}
		}
	}

	public static bool CheckDriverInstall(string subkey, string key)
	{
		GlobalFun.TryGetRegistryKey(RegistryHive.LocalMachine, Path.Combine(DRIVERS_REGISTRY_PATH, subkey), key, out var value);
		if (value == null)
		{
			return false;
		}
		return true;
	}

	public static bool CheckDriverInstall(string displayPattern)
	{
		string name = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
		RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(name, RegistryKeyPermissionCheck.ReadWriteSubTree);
		string[] subKeyNames = registryKey.GetSubKeyNames();
		foreach (string name2 in subKeyNames)
		{
			RegistryKey registryKey2 = registryKey.OpenSubKey(name2);
			object value = registryKey2.GetValue("DisplayName");
			object value2 = registryKey2.GetValue("DisplayVersion");
			if (value != null && Regex.IsMatch(value.ToString(), displayPattern, RegexOptions.IgnoreCase))
			{
				LogHelper.LogInstance.Info($"driver : {value}, version: {value2}");
				return true;
			}
		}
		registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(name, RegistryKeyPermissionCheck.ReadWriteSubTree);
		subKeyNames = registryKey.GetSubKeyNames();
		foreach (string name3 in subKeyNames)
		{
			RegistryKey registryKey3 = registryKey.OpenSubKey(name3);
			object value = registryKey3.GetValue("DisplayName");
			object value2 = registryKey3.GetValue("DisplayVersion");
			if (value != null && Regex.IsMatch(value.ToString(), displayPattern, RegexOptions.IgnoreCase))
			{
				LogHelper.LogInstance.Info($"driver : {value}, version: {value2}");
				return true;
			}
		}
		LogHelper.LogInstance.Info(displayPattern + " driver not exists");
		return false;
	}

	public static void WriteRegistry(string key, string name, object value)
	{
		string text = Path.Combine(DRIVERS_REGISTRY_PATH, key);
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey(text, RegistryKeyPermissionCheck.ReadWriteSubTree);
		if (registryKey == null)
		{
			registryKey = localMachine.CreateSubKey(text, RegistryKeyPermissionCheck.ReadWriteSubTree);
		}
		registryKey.SetValue(name, value);
	}

	public static string CheckMotorolaDriverExeInstalled(Action<string> _action)
	{
		string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
		bool flag = false;
		string text = string.Empty;
		using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
		{
			string[] subKeyNames = registryKey.GetSubKeyNames();
			foreach (string name2 in subKeyNames)
			{
				object value = registryKey.OpenSubKey(name2).GetValue("DisplayName");
				if (value != null && value.ToString() == Motorola_Driver_Exe_Install_Info.Item2)
				{
					flag = true;
					LogHelper.LogInstance.Info("Motorola driver:[" + Motorola_Driver_Exe_Install_Info.Item2 + "] is installed.");
					text = "Motorola driver:[" + Motorola_Driver_Exe_Install_Info.Item2 + "] is installed.";
					break;
				}
			}
		}
		if (!flag)
		{
			_action("installing");
			LogHelper.LogInstance.Info("Installing Motorola driver:[" + Motorola_Driver_Exe_Install_Info.Item3 + "].");
			text = text + Environment.NewLine + "Installing Motorola driver:[" + Motorola_Driver_Exe_Install_Info.Item3 + "].";
			ExecuteCommand("msiexec /i \"" + Motorola_Driver_Exe_Install_Info.Item3 + "\" /quiet");
			_action("installed");
		}
		return text;
	}

	private static string ExecuteCommand(string command)
	{
		bool Wow64Process = false;
		IsWow64Process(Process.GetCurrentProcess().Handle, out Wow64Process);
		if (Wow64Process)
		{
			IntPtr OldValue = IntPtr.Zero;
			Wow64DisableWow64FsRedirection(out OldValue);
		}
		string result = null;
		try
		{
			command = command.Trim() + " &exit";
			using Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Verb = "runas";
			if (process.Start())
			{
				SetForegroundWindow(process.MainWindowHandle);
			}
			process.StandardInput.WriteLine(command);
			process.StandardInput.AutoFlush = true;
			result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			process.Close();
			process.Dispose();
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
		return result;
	}

	private static bool IsWindow10Or11()
	{
		GetOsInfo(out var caption, out var _);
		if (!string.IsNullOrEmpty(caption) && Regex.IsMatch(caption, "windows 10", RegexOptions.IgnoreCase))
		{
			return true;
		}
		if (!string.IsNullOrEmpty(caption) && Regex.IsMatch(caption, "windows 11", RegexOptions.IgnoreCase))
		{
			return true;
		}
		return false;
	}

	private static void GetOsInfo(out string caption, out string architecture)
	{
		caption = null;
		architecture = null;
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
		using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectSearcher.Get().GetEnumerator();
		if (managementObjectEnumerator.MoveNext())
		{
			ManagementObject managementObject = (ManagementObject)managementObjectEnumerator.Current;
			caption = managementObject["Caption"].ToString();
			architecture = managementObject["OSArchitecture"].ToString();
		}
	}

	[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

	[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);
}
