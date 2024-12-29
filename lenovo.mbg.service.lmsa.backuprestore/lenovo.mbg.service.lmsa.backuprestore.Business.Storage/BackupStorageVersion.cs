using System;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public class BackupStorageVersion
{
	public const int ENCRYPT_BYTES_LNGTH = 48;

	private AES _aes;

	public BackupStorageVersion()
	{
		_aes = new AES();
	}

	public byte[] EncryptVersion(string version)
	{
		if (!Guid.TryParse(version, out var _))
		{
			throw new BackupRestoreException("Storage version format is error");
		}
		byte[] array = _aes.Encrypt(Constants.Encoding.GetBytes(version));
		if (array.Length != 48)
		{
			throw new BackupRestoreException("Version encrypt failed");
		}
		return array;
	}

	public string DecryptVersion(byte[] bytes)
	{
		string @string = Constants.Encoding.GetString(_aes.Decrypt(bytes));
		if (!Guid.TryParse(@string, out var _))
		{
			throw new BackupRestoreException("Storage version format is error");
		}
		return @string;
	}
}
