using System;
using System.IO;
using System.Security.Cryptography;

namespace lenovo.mbg.service.common.utilities;

public class DecryptRomFileHelper
{
	private const ulong FC_TAG = 18158797384510146255uL;

	private const int BUFFER_SIZE = 131072;

	private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

	private static bool CheckByteArrays(byte[] b1, byte[] b2)
	{
		if (b1.Length == b2.Length)
		{
			for (int i = 0; i < b1.Length; i++)
			{
				if (b1[i] != b2[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
	{
		PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, salt, "SHA256", 1000);
		Rijndael rijndael = Rijndael.Create();
		rijndael.KeySize = 256;
		rijndael.Key = passwordDeriveBytes.GetBytes(32);
		rijndael.Padding = PaddingMode.PKCS7;
		return rijndael;
	}

	private static byte[] GenerateRandomBytes(int count)
	{
		byte[] array = new byte[count];
		rand.GetBytes(array);
		return array;
	}

	public static void EncryptFile(string inFile, string outFile, string password = "OSD")
	{
		using (FileStream fileStream = File.OpenRead(inFile))
		{
			using FileStream fileStream2 = File.OpenWrite(outFile);
			long length = fileStream.Length;
			byte[] array = new byte[131072];
			int num = -1;
			int num2 = 0;
			byte[] array2 = GenerateRandomBytes(16);
			byte[] array3 = GenerateRandomBytes(16);
			SymmetricAlgorithm symmetricAlgorithm = CreateRijndael(password, array3);
			symmetricAlgorithm.IV = array2;
			fileStream2.Write(array2, 0, array2.Length);
			fileStream2.Write(array3, 0, array3.Length);
			HashAlgorithm hashAlgorithm = SHA256.Create();
			using CryptoStream cryptoStream = new CryptoStream(fileStream2, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			using CryptoStream cryptoStream2 = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write);
			BinaryWriter binaryWriter = new BinaryWriter(cryptoStream);
			binaryWriter.Write(length);
			binaryWriter.Write(18158797384510146255uL);
			while ((num = fileStream.Read(array, 0, array.Length)) != 0)
			{
				cryptoStream.Write(array, 0, num);
				cryptoStream2.Write(array, 0, num);
				num2 += num;
			}
			cryptoStream2.Flush();
			cryptoStream2.Close();
			byte[] hash = hashAlgorithm.Hash;
			cryptoStream.Write(hash, 0, hash.Length);
			cryptoStream.Flush();
			cryptoStream.Close();
		}
		File.Delete(inFile);
		File.SetAttributes(outFile, FileAttributes.Hidden);
	}

	public static void DecryptFile(string inFile, string outFile, string password = "OSD")
	{
		using FileStream fileStream = File.OpenRead(inFile);
		using FileStream fileStream2 = File.OpenWrite(outFile);
		_ = fileStream.Length;
		byte[] array = new byte[131072];
		int num = -1;
		int num2 = 0;
		int num3 = 0;
		byte[] array2 = new byte[16];
		fileStream.Read(array2, 0, 16);
		byte[] array3 = new byte[16];
		fileStream.Read(array3, 0, 16);
		SymmetricAlgorithm symmetricAlgorithm = CreateRijndael(password, array3);
		symmetricAlgorithm.IV = array2;
		num2 = 32;
		long num4 = -1L;
		HashAlgorithm hashAlgorithm = SHA256.Create();
		using (CryptoStream cryptoStream = new CryptoStream(fileStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
		{
			using CryptoStream cryptoStream2 = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write);
			BinaryReader binaryReader = new BinaryReader(cryptoStream);
			num4 = binaryReader.ReadInt64();
			ulong num5 = binaryReader.ReadUInt64();
			if (-287946689199405361L != (long)num5)
			{
				throw new Exception("file is corrupted!");
			}
			long num6 = num4 / 131072;
			long num7 = num4 % 131072;
			for (int i = 0; i < num6; i++)
			{
				num = cryptoStream.Read(array, 0, array.Length);
				fileStream2.Write(array, 0, num);
				cryptoStream2.Write(array, 0, num);
				num2 += num;
				num3 += num;
			}
			if (num7 > 0)
			{
				num = cryptoStream.Read(array, 0, (int)num7);
				fileStream2.Write(array, 0, num);
				cryptoStream2.Write(array, 0, num);
				num2 += num;
				num3 += num;
			}
			cryptoStream2.Flush();
			cryptoStream2.Close();
			fileStream2.Flush();
			fileStream2.Close();
			new FileInfo(outFile).Attributes = FileAttributes.Hidden;
			byte[] hash = hashAlgorithm.Hash;
			byte[] array4 = new byte[hashAlgorithm.HashSize / 8];
			num = cryptoStream.Read(array4, 0, array4.Length);
			if (array4.Length != num || !CheckByteArrays(array4, hash))
			{
				throw new Exception("file is corrupted!");
			}
		}
		if (num3 != num4)
		{
			throw new Exception("file size mismatch!");
		}
	}
}
