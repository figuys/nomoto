using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.socket;

public class RsaSocketDataSecurity
{
	public static string AESEncrypt(string text, bool isRandomKey = false)
	{
		try
		{
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.KeySize = 128;
			rijndaelManaged.BlockSize = 128;
			byte[] keyBytes = RandomAesKeyHelper.Instance.GetKeyBytes(isRandomKey);
			byte[] array = new byte[16];
			int num = keyBytes.Length;
			if (num > array.Length)
			{
				num = array.Length;
			}
			Array.Copy(keyBytes, array, num);
			rijndaelManaged.Key = array;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Debug("aes encrypt text throw exception: ", exception);
			return text;
		}
	}

	public static byte[] AESEncrypt(byte[] data, bool isRandomKey = false)
	{
		try
		{
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.KeySize = 128;
			rijndaelManaged.BlockSize = 128;
			byte[] keyBytes = RandomAesKeyHelper.Instance.GetKeyBytes(isRandomKey);
			byte[] array = new byte[16];
			int num = keyBytes.Length;
			if (num > array.Length)
			{
				num = array.Length;
			}
			Array.Copy(keyBytes, array, num);
			rijndaelManaged.Key = array;
			byte[] result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write);
				cryptoStream.Write(data, 0, data.Length);
				cryptoStream.FlushFinalBlock();
				result = memoryStream.ToArray();
			}
			return result;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Debug("aes encrypt byte throw exception: ", exception);
			return data;
		}
	}

	public static string AESDecrypt(string text, bool isRandomKey = false)
	{
		try
		{
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.KeySize = 128;
			rijndaelManaged.BlockSize = 128;
			byte[] array = Convert.FromBase64String(text);
			byte[] keyBytes = RandomAesKeyHelper.Instance.GetKeyBytes(isRandomKey);
			byte[] array2 = new byte[16];
			int num = keyBytes.Length;
			if (num > array2.Length)
			{
				num = array2.Length;
			}
			Array.Copy(keyBytes, array2, num);
			rijndaelManaged.Key = array2;
			byte[] bytes = rijndaelManaged.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Debug("aes decrypt text throw exception: ", exception);
			return text;
		}
	}

	public static byte[] AESDecrypt(byte[] data, bool isRandomKey = false)
	{
		try
		{
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.KeySize = 128;
			rijndaelManaged.BlockSize = 128;
			byte[] keyBytes = RandomAesKeyHelper.Instance.GetKeyBytes(isRandomKey);
			byte[] array = new byte[16];
			int num = keyBytes.Length;
			if (num > array.Length)
			{
				num = array.Length;
			}
			Array.Copy(keyBytes, array, num);
			rijndaelManaged.Key = array;
			byte[] result = null;
			using (MemoryStream stream = new MemoryStream(data))
			{
				using CryptoStream cryptoStream = new CryptoStream(stream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read);
				using MemoryStream memoryStream = new MemoryStream();
				byte[] array2 = new byte[1024];
				int num2 = 0;
				while ((num2 = cryptoStream.Read(array2, 0, array2.Length)) > 0)
				{
					memoryStream.Write(array2, 0, num2);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Debug($"aes decrypt byte[{data.Length}] throw exception: {arg}");
			return data;
		}
	}
}
