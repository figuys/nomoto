using System;
using System.Security.Cryptography;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

internal class PW
{
	internal class EncryptionInfo
	{
		public byte[] AesKey { get; set; }

		public byte[] AesSalt { get; set; }

		public byte[] AesVector { get; set; }
	}

	internal class PWInfo
	{
		public byte[] AesSalt { get; set; }

		public byte[] AesVetcor { get; set; }

		public byte[] PK { get; set; }

		public bool IsSetPassword { get; set; }

		public int PWBlockSize { get; set; }

		public PWInfo()
		{
			PWBlockSize = 1;
			IsSetPassword = false;
		}
	}

	private const int AesKeyBlockSize = 32;

	private const int AesSaltBlockSize = 16;

	private const int AesVectorBlockSize = 16;

	private const int PKBlockSize = 64;

	private const int Iterations = 20000;

	private const int PasswordSetFlag = 1;

	internal const int PWBlockSize = 97;

	internal EncryptionInfo CreateAesEncryptionInfo(string password)
	{
		try
		{
			EncryptionInfo encryptionInfo = new EncryptionInfo();
			using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				byte[] array = new byte[16];
				rNGCryptoServiceProvider.GetBytes(array);
				encryptionInfo.AesVector = array;
				byte[] array2 = new byte[16];
				rNGCryptoServiceProvider.GetBytes(array2);
				encryptionInfo.AesSalt = array2;
			}
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, encryptionInfo.AesSalt, 20000))
			{
				encryptionInfo.AesKey = rfc2898DeriveBytes.GetBytes(32);
				rfc2898DeriveBytes.Reset();
			}
			return encryptionInfo;
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal byte[] CreatePWBytes(EncryptionInfo encryptionInfo)
	{
		try
		{
			byte[] array = new byte[97];
			int num = 0;
			Array.Copy(encryptionInfo.AesSalt, array, 16);
			num += 16;
			Array.Copy(encryptionInfo.AesVector, 0, array, num, 16);
			num += 16;
			byte[] array2 = new byte[32];
			Array.Copy(encryptionInfo.AesSalt, array2, 16);
			Array.Copy(encryptionInfo.AesVector, 0, array2, 16, 16);
			using (HMACSHA512 hMACSHA = new HMACSHA512(encryptionInfo.AesKey))
			{
				Array.Copy(hMACSHA.ComputeHash(array2), 0, array, num, 64);
				num += 64;
				array[num] = 1;
				hMACSHA.Clear();
			}
			return array;
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal EncryptionInfo GetEncryptionInfo(PWInfo pwInfo, string password)
	{
		if (pwInfo == null || string.IsNullOrEmpty(password))
		{
			return null;
		}
		try
		{
			byte[] aesSalt = pwInfo.AesSalt;
			byte[] aesVetcor = pwInfo.AesVetcor;
			byte[] pK = pwInfo.PK;
			byte[] array = null;
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, aesSalt, 20000))
			{
				array = rfc2898DeriveBytes.GetBytes(32);
				rfc2898DeriveBytes.Reset();
			}
			byte[] array2 = new byte[32];
			Array.Copy(aesSalt, array2, 16);
			Array.Copy(aesVetcor, 0, array2, 16, 16);
			byte[] array3 = null;
			using (HMACSHA512 hMACSHA = new HMACSHA512(array))
			{
				array3 = hMACSHA.ComputeHash(array2);
				hMACSHA.Clear();
			}
			if (pK != null && array3 != null && pK.Length != 0 && pK.Length == array3.Length)
			{
				bool flag = true;
				for (int i = 0; i < pK.Length; i++)
				{
					flag &= pK[i] == array3[i];
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					return new EncryptionInfo
					{
						AesKey = array,
						AesVector = aesVetcor,
						AesSalt = aesSalt
					};
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	internal PWInfo ConvertToPwInfo(byte[] pwInfoBytes)
	{
		if (pwInfoBytes.Length != 97)
		{
			return null;
		}
		try
		{
			PWInfo pWInfo = new PWInfo();
			byte[] array = new byte[16];
			Array.Copy(pwInfoBytes, array, 16);
			int num = 16;
			pWInfo.AesSalt = array;
			byte[] array2 = new byte[16];
			Array.Copy(pwInfoBytes, num, array2, 0, 16);
			num += 16;
			pWInfo.AesVetcor = array2;
			byte[] array3 = new byte[64];
			Array.Copy(pwInfoBytes, num, array3, 0, 64);
			num += 64;
			pWInfo.PK = array3;
			int num2 = pwInfoBytes[num];
			pWInfo.IsSetPassword = num2 == 1;
			pWInfo.PWBlockSize = 97;
			return pWInfo;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
