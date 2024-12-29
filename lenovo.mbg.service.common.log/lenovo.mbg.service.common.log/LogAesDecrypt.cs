using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace lenovo.mbg.service.common.log;

public class LogAesDecrypt : ILogDecrypt
{
	public string Decrypt(string cipherText)
	{
		if (string.IsNullOrEmpty(cipherText))
		{
			return string.Empty;
		}
		return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(cipherText), null, DataProtectionScope.LocalMachine));
	}

	public bool Decrypt2File(string encryptFile, string decryptSaveFile)
	{
		if (File.Exists(encryptFile))
		{
			try
			{
				using (StreamReader streamReader = new StreamReader(encryptFile, Encoding.UTF8))
				{
					using StreamWriter streamWriter = new StreamWriter(decryptSaveFile, append: true, Encoding.UTF8);
					while (!streamReader.EndOfStream)
					{
						string value = Decrypt(streamReader.ReadLine());
						streamWriter.WriteLine(value);
					}
				}
				return true;
			}
			catch (Exception)
			{
			}
		}
		return false;
	}
}
