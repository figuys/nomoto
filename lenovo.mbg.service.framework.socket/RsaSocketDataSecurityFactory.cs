using System;
using System.Configuration;
using System.Text;

namespace lenovo.mbg.service.framework.socket;

public class RsaSocketDataSecurityFactory
{
	protected static int MotoHelperVersion => int.Parse(ConfigurationManager.AppSettings["MotoApkMinVersionCode"]);

	public bool IsEncryptRandomKey { get; private set; }

	public bool IsSecurity { get; private set; }

	public RsaSocketDataSecurityFactory(bool isSecutiry, bool isRandomKey)
	{
		IsSecurity = isSecutiry;
		IsEncryptRandomKey = isRandomKey;
	}

	public string DecryptFromBase64(string text)
	{
		string text2 = text;
		if (text2.EndsWith("\\r\\n"))
		{
			text2 = text2.Remove(text2.LastIndexOf("\\r\\n"));
		}
		if (IsSecurity && !string.IsNullOrEmpty(text2))
		{
			text2 = RsaSocketDataSecurity.AESDecrypt(text2, IsEncryptRandomKey);
		}
		if (text2.EndsWith("\\r\\n"))
		{
			text2 = text2.Remove(text2.LastIndexOf("\\r\\n"));
		}
		return text2;
	}

	public string EncryptToBase64(string text)
	{
		if (IsSecurity && !string.IsNullOrEmpty(text))
		{
			return RsaSocketDataSecurity.AESEncrypt(text, IsEncryptRandomKey);
		}
		return text;
	}

	public byte[] EncryptBase64(byte[] data)
	{
		if (IsSecurity && data.Length != 0)
		{
			return Encoding.UTF8.GetBytes(Convert.ToBase64String(RsaSocketDataSecurity.AESEncrypt(data, IsEncryptRandomKey)));
		}
		return data;
	}

	public byte[] Encrypt(byte[] data)
	{
		if (IsSecurity && data.Length != 0)
		{
			return RsaSocketDataSecurity.AESEncrypt(data, IsEncryptRandomKey);
		}
		return data;
	}

	public byte[] Decrypt(byte[] data)
	{
		if (IsSecurity && data.Length != 0)
		{
			return RsaSocketDataSecurity.AESDecrypt(data, IsEncryptRandomKey);
		}
		return data;
	}

	public byte[] DecryptBase64(byte[] data)
	{
		if (IsSecurity && data.Length != 0)
		{
			int num = Array.FindLastIndex(data, (byte b) => b != 0);
			Array.Resize(ref data, num + 1);
			string text = Encoding.UTF8.GetString(data);
			if (text.EndsWith("\\r\\n"))
			{
				text = text.Remove(text.LastIndexOf("\\r\\n"));
			}
			return RsaSocketDataSecurity.AESDecrypt(Convert.FromBase64String(text), IsEncryptRandomKey);
		}
		return data;
	}

	protected static string ConvertVersion(string version)
	{
		string text = "";
		if (string.IsNullOrEmpty(version))
		{
			return text;
		}
		string[] array = version.Split('.');
		foreach (string text2 in array)
		{
			text += text2.PadLeft(5, '0');
		}
		return text;
	}
}
