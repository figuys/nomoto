using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.common.utilities;

public class Security
{
	private static Security _instance;

	private const char HASH_SEPARATOR = '|';

	private string TAG => GetType().FullName;

	public static Security Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			return _instance = new Security();
		}
	}

	public void Encrypt(byte[] key, CryptoType type, Stream inputData, Stream outputData)
	{
		Encrypt(key, null, type, inputData, outputData);
	}

	public void Decrypt(byte[] key, CryptoType type, Stream inputData, Stream outputData)
	{
		Decrypt(key, null, type, inputData, outputData);
	}

	public void Encrypt(byte[] key, byte[] hardcodedIv, CryptoType type, Stream inputData, Stream outputData)
	{
		switch (type)
		{
		case CryptoType.Aes:
			Encrypt(key, hardcodedIv, 16, new RijndaelManaged(), inputData, outputData);
			break;
		case CryptoType.TripleDes:
			Encrypt(key, hardcodedIv, 8, new TripleDESCryptoServiceProvider(), inputData, outputData);
			break;
		case CryptoType.Arc4:
			CryptArc4(key, inputData, outputData);
			break;
		default:
			throw new NotSupportedException($"Encryption type {type} not supported");
		}
	}

	public void Decrypt(byte[] key, byte[] hardcodedIv, CryptoType type, Stream inputData, Stream outputData)
	{
		switch (type)
		{
		case CryptoType.Aes:
			Decrypt(key, hardcodedIv, 16, new RijndaelManaged(), inputData, outputData);
			break;
		case CryptoType.TripleDes:
			Decrypt(key, hardcodedIv, 8, new TripleDESCryptoServiceProvider(), inputData, outputData);
			break;
		case CryptoType.Arc4:
			CryptArc4(key, inputData, outputData);
			break;
		default:
			throw new NotSupportedException($"Decryption type {type} not supported");
		}
	}

	public static byte[] EncryptWithDPAPI(string content)
	{
		return ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.LocalMachine);
	}

	public static string DecryptWithDPAPI(byte[] content)
	{
		return Encoding.UTF8.GetString(ProtectedData.Unprotect(content, null, DataProtectionScope.LocalMachine));
	}

	private static void Encrypt(byte[] key, byte[] hardcodedIv, int blockSize, SymmetricAlgorithm algorithm, Stream inputData, Stream outputData)
	{
		byte[] array = hardcodedIv;
		if (array == null)
		{
			array = new byte[blockSize];
			new RNGCryptoServiceProvider().GetBytes(array);
			outputData.Write(array, 0, array.Length);
		}
		ICryptoTransform transform = algorithm.CreateEncryptor(key, array);
		CryptoStream cryptoStream = new CryptoStream(outputData, transform, CryptoStreamMode.Write);
		CustomFile.Instance.CopyStream(inputData, cryptoStream);
		cryptoStream.FlushFinalBlock();
	}

	private static void Decrypt(byte[] key, byte[] hardcodedIv, int blockSize, SymmetricAlgorithm algorithm, Stream inputData, Stream outputData)
	{
		byte[] array = hardcodedIv;
		if (array == null)
		{
			array = new byte[blockSize];
			inputData.Read(array, 0, array.Length);
		}
		ICryptoTransform transform = algorithm.CreateDecryptor(key, array);
		CryptoStream input = new CryptoStream(inputData, transform, CryptoStreamMode.Read);
		CustomFile.Instance.CopyStream(input, outputData);
	}

	private void CryptArc4(byte[] key, Stream inputData, Stream outputData)
	{
		long length = inputData.Length;
		byte[] keyStream = GenerateKeystream(key, length);
		ModifyWithKeystream(keyStream, inputData, outputData);
	}

	private byte[] GenerateKeystream(byte[] key, long length)
	{
		byte b = (byte)key.Length;
		byte[] array = new byte[256];
		foreach (int item in Enumerable.Range(0, array.Length))
		{
			array[item] = (byte)item;
		}
		int num = 0;
		foreach (int item2 in Enumerable.Range(0, array.Length))
		{
			num = (num + array[item2] + key[item2 % b]) % array.Length;
			byte b2 = array[item2];
			array[item2] = array[num];
			array[num] = b2;
		}
		byte[] array2 = new byte[length];
		int num2 = 0;
		int num3 = 0;
		foreach (int item3 in Enumerable.Range(0, array2.Length))
		{
			num2 = (num2 + 1) % array.Length;
			num3 = (num3 + array[num2]) % array.Length;
			byte b3 = array[num2];
			array[num2] = array[num3];
			array[num3] = b3;
			int num4 = (array[num2] + array[num3]) % array.Length;
			array2[item3] = array[num4];
		}
		return array2;
	}

	private void ModifyWithKeystream(byte[] keyStream, Stream inputData, Stream outputData)
	{
		Queue<byte> keyQueue = new Queue<byte>(keyStream);
		CustomFile.Instance.CopyStream(inputData, outputData, (byte[] input) => ModifyWithKeystream(keyQueue, input));
	}

	private byte[] ModifyWithKeystream(Queue<byte> keyStream, byte[] input)
	{
		if (keyStream.Count < input.Length)
		{
			throw new EndOfStreamException("Not enough key stream to continue");
		}
		byte[] array = new byte[input.Length];
		foreach (int item in Enumerable.Range(0, input.Length))
		{
			byte b = keyStream.Dequeue();
			array[item] = (byte)(input[item] ^ b);
		}
		return array;
	}

	public byte[] RandomBytes(int length)
	{
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		byte[] array = new byte[length];
		rNGCryptoServiceProvider.GetBytes(array);
		return array;
	}

	public string Hash(string input)
	{
		byte[] salt = RandomBytes(16);
		return Hash(input, 16, 32768, salt);
	}

	public bool HashCheck(string input, string existingHash)
	{
		existingHash = existingHash.Trim();
		if (existingHash == string.Empty)
		{
			return false;
		}
		string[] array = existingHash.Split('|');
		if (array.Length != 3)
		{
			throw new FormatException("Unrecognized hash format: " + existingHash);
		}
		ushort iterations = CustomConvert.Instance.BytesToUShort(CustomConvert.Instance.HexToBytes(array[0]));
		byte[] salt = CustomConvert.Instance.Base64ToBytes(array[1]);
		byte[] array2 = CustomConvert.Instance.Base64ToBytes(array[2]);
		string text = Hash(input, array2.Length, iterations, salt);
		LogHelper.LogInstance.Info($"Comparing hashes. Old: {existingHash}, New: {text}");
		return text == existingHash;
	}

	private string Hash(string input, int length, ushort iterations, byte[] salt)
	{
		byte[] bytes = new Rfc2898DeriveBytes(input, salt)
		{
			IterationCount = iterations
		}.GetBytes(length);
		string text = CustomConvert.Instance.BytesToBase64(bytes);
		string text2 = CustomConvert.Instance.BytesToHex(CustomConvert.Instance.UShortToBytes(iterations));
		string text3 = CustomConvert.Instance.BytesToBase64(salt);
		return text2 + "|" + text3 + "|" + text;
	}

	public string EncryptAseString(string _content)
	{
		try
		{
			using MemoryStream inputData = new MemoryStream(Encoding.UTF8.GetBytes(_content));
			using MemoryStream memoryStream = new MemoryStream();
			Encrypt(RandomAesKeyHelper.Instance.GetKeyBytes(), CryptoType.Aes, inputData, memoryStream);
			return Convert.ToBase64String(memoryStream.ToArray());
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"ecrypt failed [{_content}], exception: {arg}");
			return string.Empty;
		}
	}

	public string DecryptAseString(string _content)
	{
		try
		{
			if (string.IsNullOrEmpty(_content))
			{
				return string.Empty;
			}
			using MemoryStream inputData = new MemoryStream(Convert.FromBase64String(_content));
			using MemoryStream memoryStream = new MemoryStream();
			Decrypt(RandomAesKeyHelper.Instance.GetKeyBytes(), CryptoType.Aes, inputData, memoryStream);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"decrypt failed [{_content}], exception: {arg}");
			return string.Empty;
		}
	}

	public string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
	{
		byte[] array = new byte[4];
		new RNGCryptoServiceProvider().GetBytes(array);
		Random random = new Random(BitConverter.ToInt32(array, 0));
		string text = null;
		string text2 = custom;
		if (useNum)
		{
			text2 += "0123456789";
		}
		if (useLow)
		{
			text2 += "abcdefghijklmnopqrstuvwxyz";
		}
		if (useUpp)
		{
			text2 += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		}
		if (useSpe)
		{
			text2 += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
		}
		for (int i = 0; i < length; i++)
		{
			text += text2.Substring(random.Next(0, text2.Length - 1), 1);
		}
		return text;
	}
}
