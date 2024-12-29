using System;
using System.IO;
using System.Security.Cryptography;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public class AES : IDisposable
{
	public const int EncryptionStateSize = 16;

	public const int PlaintextByteBlockSize = 1600;

	public const int PlainTextByteBlockHeadSize = 4;

	private readonly byte[] _key = Constants.Encoding.GetBytes("jdkei3ffkjijut46#$%6y7U8km4p<mdT");

	public readonly byte[] _iv = Constants.Encoding.GetBytes("52,*u^yhNjk<./O0");

	private Rijndael rijndael;

	public AES()
	{
		Aes.Create();
		rijndael = Rijndael.Create();
		rijndael.BlockSize = 128;
		rijndael.Mode = CipherMode.CBC;
		rijndael.Padding = PaddingMode.PKCS7;
		rijndael.Key = _key;
		rijndael.IV = _iv;
	}

	public AES(byte[] key, byte[] iv)
		: this()
	{
		_key = key;
		_iv = iv;
	}

	public static long GetEncryptByteLength(long plaintextByteLength, bool partBlockForPlaintext)
	{
		if (plaintextByteLength == 0L)
		{
			return 0L;
		}
		if (partBlockForPlaintext)
		{
			int num = 1596;
			long num2 = plaintextByteLength / num;
			int num3 = ((plaintextByteLength > num2 * num) ? 1 : 0);
			return (num2 + num3) * 1616;
		}
		return plaintextByteLength / 16 * 16 + 16;
	}

	public byte[] Encrypt(byte[] data)
	{
		using ICryptoTransform transform = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);
		using MemoryStream memoryStream = new MemoryStream();
		using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
		{
			cryptoStream.Write(data, 0, data.Length);
		}
		return memoryStream.ToArray();
	}

	public byte[] Decrypt(byte[] data)
	{
		using ICryptoTransform transform = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);
		int num = data.Length;
		byte[] array = new byte[num];
		int num2 = 1024;
		int num3 = 0;
		int num4 = 0;
		using (MemoryStream stream = new MemoryStream(data))
		{
			using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			do
			{
				num2 = ((num - num3 >= num2) ? num2 : (num - num3));
				num4 = cryptoStream.Read(array, num3, num2);
				num3 += num4;
			}
			while (num4 != 0);
		}
		byte[] array2 = new byte[num3];
		Array.Copy(array, array2, num3);
		return array2;
	}

	public void EncryptFile()
	{
	}

	public void Dispose()
	{
		rijndael?.Dispose();
	}
}
