using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

[SupportedFormatVersion("{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}")]
public class BackupResourceAESReaderV3 : BackupResourceAESReader
{
	private PW pw = new PW();

	private PW.PWInfo pWInfo;

	public BackupResourceAESReaderV3(IBackupStorage storage)
		: base(storage)
	{
	}

	protected override void Prepare()
	{
		pWInfo = LoadPwInfo();
		if (_canRead = !pWInfo.IsSetPassword)
		{
			_indexs = LoadIndexs();
		}
	}

	private PW.PWInfo LoadPwInfo()
	{
		int num = 1;
		byte[] array = new byte[num];
		_storage.Seek(-num, SeekOrigin.End);
		if (_storage.Read(array, 0, num) != num)
		{
			throw new BackupRestoreException("File format error!");
		}
		PW.PWInfo pWInfo = new PW.PWInfo();
		if (array[0] == 1)
		{
			pWInfo.IsSetPassword = true;
			_storage.Seek(-97L, SeekOrigin.End);
			byte[] array2 = new byte[97];
			if (_storage.Read(array2, 0, 97) != 97)
			{
				throw new BackupRestoreException("File format error!");
			}
			pWInfo = pw.ConvertToPwInfo(array2);
		}
		return pWInfo;
	}

	internal override List<BackupStorageIndex> LoadIndexs()
	{
		try
		{
			int num = 1616;
			int pWBlockSize = pWInfo.PWBlockSize;
			if (_storageSize < num + pWBlockSize)
			{
				throw new BackupRestoreException("File format error!");
			}
			byte[] array = new byte[num];
			int num2 = 0;
			_storage.Seek(-(num + pWBlockSize), SeekOrigin.End);
			if (_storage.Read(array, 0, num) != num)
			{
				throw new BackupRestoreException("File format error!");
			}
			byte[] array2 = _aes.Decrypt(array);
			num2 = JsonUtils.Parse<BackupStorageHead>(Constants.Encoding.GetString(array2, 4, BitConverter.ToInt32(array2, 0))).IndexByteLength;
			if (num2 + num + pWBlockSize > _storageSize)
			{
				throw new BackupRestoreException("File format error!");
			}
			byte[] array3 = new byte[num2];
			int num3 = 1024;
			int num4 = 0;
			int num5 = 0;
			long num6 = 0L;
			long num7 = num2;
			_storage.Seek(-(num2 + num + pWBlockSize), SeekOrigin.End);
			do
			{
				num6 = num7 - num4;
				num3 = (int)((num6 > num3) ? num3 : num6);
				num5 = _storage.Read(array3, num4, num3);
				num4 += num5;
			}
			while (num4 < num7);
			byte[] array4 = _aes.Decrypt(array3);
			List<BackupStorageIndex> list = JsonUtils.Parse<List<BackupStorageIndex>>(Constants.Encoding.GetString(array4, 0, array4.Length));
			if (list == null)
			{
				_resourceInfoStorageOffset = -1L;
			}
			else
			{
				_resourceInfoStorageOffset = 48 + list.Sum((BackupStorageIndex m) => AES.GetEncryptByteLength(m.ResourceStreamLength, partBlockForPlaintext: true));
			}
			_storage.Seek(0L, SeekOrigin.Begin);
			return list;
		}
		catch (Exception innerException)
		{
			throw new Exception("Load storage head info failed! file format is error", innerException);
		}
	}

	public override bool CheckPassword(string password)
	{
		PW.EncryptionInfo encryptionInfo = pw.GetEncryptionInfo(pWInfo, password);
		if (encryptionInfo != null)
		{
			_aes = new AES(encryptionInfo.AesKey, encryptionInfo.AesVector);
			if (!_canRead)
			{
				_indexs = LoadIndexs();
				_canRead = true;
			}
			return true;
		}
		_canRead = false;
		return false;
	}

	public override bool IsSetPassword()
	{
		return pWInfo.IsSetPassword;
	}
}
