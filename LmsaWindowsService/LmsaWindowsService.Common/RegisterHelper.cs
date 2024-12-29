using Microsoft.Win32;

namespace LmsaWindowsService.Common;

public class RegisterHelper
{
	public static bool TryGetRegistryKey(RegistryKey registrykey, string path, string key, out dynamic value)
	{
		value = null;
		try
		{
			using RegistryKey registryKey = registrykey.OpenSubKey(path);
			if (registryKey == null)
			{
				return false;
			}
			value = registryKey.GetValue(key);
			return value != null;
		}
		catch
		{
			return false;
		}
	}

	public static void WriteRegistry(string subkey, string name, object value)
	{
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey(subkey);
		if (registryKey == null)
		{
			registryKey = localMachine.CreateSubKey(subkey);
		}
		registryKey.SetValue(name, value);
	}

	public static void DeleteRegistry(string subkey)
	{
		RegistryKey localMachine = Registry.LocalMachine;
		if (localMachine.OpenSubKey(subkey) != null)
		{
			localMachine.DeleteSubKeyTree(subkey);
		}
	}
}
