using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

[SupportedFormatVersion("{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}")]
public class BackupResourceAESWriterV3 : IBackupResourceWriter, IDisposable
{
	private PW pw = new PW();

	private PW.EncryptionInfo encryptionInfo;

	private IBackupStorage _storage;

	private AES _aes;

	private List<BackupStorageIndex> _newIndexsInfo;

	private FileInfo _indexsFileInfo;

	private FileStream _indexsFs;

	protected bool _canWrite;

	private int _idBase;

	public void SetPassword(string password)
	{
		if (!string.IsNullOrEmpty(password))
		{
			encryptionInfo = pw.CreateAesEncryptionInfo(password);
			_aes = new AES(encryptionInfo.AesKey, encryptionInfo.AesVector);
		}
	}

	public BackupResourceAESWriterV3(IBackupStorage storage)
	{
		_storage = storage;
		_aes = new AES();
		Prepare();
	}

	public bool CanWrite()
	{
		return _canWrite;
	}

	private void Prepare()
	{
		if (_indexsFs == null)
		{
			string phoneBackupCacheDir = Configurations.PhoneBackupCacheDir;
			_indexsFileInfo = new FileInfo(Path.Combine(phoneBackupCacheDir, Guid.NewGuid().ToString() + ".mabk.temp"));
			_indexsFs = _indexsFileInfo.Open(FileMode.CreateNew, FileAccess.ReadWrite);
			_newIndexsInfo = new List<BackupStorageIndex>();
			_canWrite = true;
		}
	}

	public void ReserveDiskSpace(int resourceItemsCount, long reservereSourceStreamGross)
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
		long encryptByteLength = AES.GetEncryptByteLength(40 * resourceItemsCount, partBlockForPlaintext: true);
		_indexsFs.SetLength(encryptByteLength);
		_storage.SetLength(reservereSourceStreamGross);
	}

	public void BeginWrite()
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
	}

	private int GetNewId()
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
		return ++_idBase;
	}

	public void Write(BackupResource resource)
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
		if (_indexsFs == null)
		{
			throw new BackupRestoreException("Show call BeginWrite method begin write");
		}
		resource.Id = GetNewId();
		string s = JsonUtils.Stringify(resource);
		byte[] bytes = Constants.Encoding.GetBytes(s);
		int num = bytes.Length;
		byte[] array = _aes.Encrypt(bytes);
		BackupStorageIndex item = new BackupStorageIndex
		{
			Id = resource.Id,
			ParentId = resource.ParentId,
			ResourceLength = num,
			ResourceOffset = _indexsFs.Position,
			ResourceStreamOffset = _storage.Position,
			ResourceStreamLength = resource.AssociatedStreamSize
		};
		_newIndexsInfo.Add(item);
		int count = array.Length;
		_indexsFs.Write(array, 0, count);
	}

	public IBackupResourceStreamWriter Seek(BackupResource resource)
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
		BackupStorageIndex backupStorageIndex = _newIndexsInfo.FirstOrDefault((BackupStorageIndex m) => m.Id == resource.Id);
		if (backupStorageIndex == null)
		{
			return null;
		}
		return new BackupResourceStreamAESWriter(_storage, _aes, backupStorageIndex.ResourceStreamOffset, backupStorageIndex.ResourceStreamLength);
	}

	public void EndWrite()
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
		try
		{
			_indexsFs.SetLength(_indexsFs.Position);
			_storage.SetLength(_storage.Position);
			_indexsFs.Seek(0L, SeekOrigin.Begin);
			byte[] buffer = new byte[1024];
			int num = 0;
			do
			{
				num = _indexsFs.Read(buffer, 0, 1024);
				_storage.Write(buffer, 0, num);
			}
			while (num != 0);
			string s = JsonUtils.Stringify(_newIndexsInfo);
			byte[] bytes = Constants.Encoding.GetBytes(s);
			byte[] array = _aes.Encrypt(bytes);
			_storage.Write(array, 0, array.Length);
			byte[] array2 = new byte[1600];
			BackupStorageHead jsonObject = new BackupStorageHead
			{
				IndexByteLength = array.Length
			};
			byte[] bytes2 = Constants.Encoding.GetBytes(JsonUtils.Stringify(jsonObject));
			Array.Copy(BitConverter.GetBytes(bytes2.Length), 0, array2, 0, 4);
			Array.Copy(bytes2, 0, array2, 4, bytes2.Length);
			byte[] array3 = _aes.Encrypt(array2);
			_storage.Write(array3, 0, array3.Length);
			if (encryptionInfo != null)
			{
				byte[] array4 = pw.CreatePWBytes(encryptionInfo);
				_storage.Write(array4, 0, array4.Length);
			}
			else
			{
				_storage.Write(new byte[1], 0, 1);
			}
			_storage.Flush();
		}
		finally
		{
			DisposeIndexFileHandler();
		}
	}

	private void DisposeIndexFileHandler()
	{
		try
		{
			if (_indexsFs != null)
			{
				_indexsFs.Close();
				_indexsFs.Dispose();
				_indexsFs = null;
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Close index fs throw exception:" + ex.ToString());
		}
		try
		{
			if (_indexsFileInfo != null)
			{
				_indexsFileInfo.Delete();
				_indexsFileInfo = null;
			}
		}
		catch (Exception ex2)
		{
			LogHelper.LogInstance.Error("Delete index file info throw exception:" + ex2.ToString());
		}
	}

	public void RemoveEnd()
	{
		if (!_canWrite)
		{
			throw new CanNotWriteException("Can not write");
		}
		if (_newIndexsInfo != null && _newIndexsInfo.Count != 0)
		{
			BackupStorageIndex backupStorageIndex = _newIndexsInfo.Last();
			if (backupStorageIndex != null)
			{
				_newIndexsInfo.Remove(backupStorageIndex);
				_indexsFs?.Seek(backupStorageIndex.ResourceOffset, SeekOrigin.Begin);
				_indexsFs?.SetLength(backupStorageIndex.ResourceOffset);
				_storage?.Seek(backupStorageIndex.ResourceStreamOffset, SeekOrigin.Begin);
				_storage?.SetLength(backupStorageIndex.ResourceStreamOffset);
			}
		}
	}

	public void Dispose()
	{
		DisposeIndexFileHandler();
	}
}
