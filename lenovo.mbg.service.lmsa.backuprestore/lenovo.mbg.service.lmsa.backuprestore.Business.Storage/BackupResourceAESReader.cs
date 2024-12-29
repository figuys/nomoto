using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

[SupportedFormatVersion("{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}")]
public class BackupResourceAESReader : IBackupResourceReader, IDisposable
{
	private string _path = string.Empty;

	protected long _storageSize;

	protected long _resourceInfoStorageOffset;

	protected IBackupStorage _storage;

	internal List<BackupStorageIndex> _indexs;

	protected bool _canRead;

	protected AES _aes;

	public BackupResourceAESReader(IBackupStorage storage)
	{
		_storage = storage;
		_storageSize = storage.Size;
		_aes = new AES();
		Prepare();
	}

	internal virtual List<BackupStorageIndex> LoadIndexs()
	{
		try
		{
			int num = 1616;
			if (_storageSize < num)
			{
				throw new BackupRestoreException("File format error!");
			}
			byte[] array = new byte[num];
			int num2 = 0;
			_storage.Seek(-num, SeekOrigin.End);
			if (_storage.Read(array, 0, num) != num)
			{
				throw new BackupRestoreException("File format error!");
			}
			byte[] array2 = _aes.Decrypt(array);
			num2 = JsonUtils.Parse<BackupStorageHead>(Constants.Encoding.GetString(array2, 4, BitConverter.ToInt32(array2, 0))).IndexByteLength;
			if (num2 + num > _storageSize)
			{
				throw new BackupRestoreException("File format error!");
			}
			byte[] array3 = new byte[num2];
			int num3 = 1024;
			int num4 = 0;
			int num5 = 0;
			long num6 = 0L;
			long num7 = num2;
			_storage.Seek(-(num2 + num), SeekOrigin.End);
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

	protected virtual void Prepare()
	{
		_indexs = LoadIndexs();
		_canRead = true;
	}

	public bool CanRead()
	{
		return _canRead;
	}

	public IBackupResourceStreamReader Seek(BackupResource resource)
	{
		if (!_canRead)
		{
			throw new CanNotReadException("Can not read");
		}
		BackupStorageIndex backupStorageIndex = _indexs.FirstOrDefault((BackupStorageIndex m) => m.Id == resource.Id);
		if (backupStorageIndex == null)
		{
			return null;
		}
		return new BackupResourceStreamAESReader(_storage, _aes, backupStorageIndex.ResourceStreamOffset, backupStorageIndex.ResourceStreamLength);
	}

	public List<BackupResource> GetChildResources(BackupResource parent)
	{
		if (!_canRead)
		{
			throw new CanNotReadException("Can not read");
		}
		if (_indexs == null)
		{
			return null;
		}
		int parentId = 0;
		if (parent != null)
		{
			parentId = parent.Id;
		}
		IEnumerable<BackupStorageIndex> enumerable = _indexs.Where((BackupStorageIndex m) => m.ParentId == parentId);
		List<BackupResource> list = new List<BackupResource>();
		int num = 1024;
		int num2 = 0;
		int num3 = 0;
		long num4 = 0L;
		foreach (BackupStorageIndex item in enumerable)
		{
			num2 = 0;
			num3 = 0;
			num4 = 0L;
			_storage.Seek(_resourceInfoStorageOffset + item.ResourceOffset, SeekOrigin.Begin);
			long encryptByteLength = AES.GetEncryptByteLength(item.ResourceLength, partBlockForPlaintext: false);
			byte[] array = new byte[encryptByteLength];
			do
			{
				num4 = encryptByteLength - num2;
				num = (int)((num4 > num) ? num : num4);
				num3 = _storage.Read(array, num2, num);
				num2 += num3;
			}
			while (num2 < encryptByteLength);
			if (num3 > 0)
			{
				byte[] array2 = _aes.Decrypt(array);
				BackupResource backupResource = JsonUtils.Parse<BackupResource>(Constants.Encoding.GetString(array2, 0, array2.Length));
				if (backupResource == null)
				{
					throw new BackupRestoreException("Parse json to object failed");
				}
				list.Add(backupResource);
			}
		}
		return list;
	}

	public void Foreach(BackupResource resource, Action<BackupResource> callback)
	{
		if (!_canRead)
		{
			throw new CanNotReadException("Can not read");
		}
		List<BackupResource> childResources = GetChildResources(resource);
		if (childResources == null || childResources.Count == 0)
		{
			return;
		}
		foreach (BackupResource item in childResources)
		{
			callback(item);
			Foreach(item, callback);
		}
	}

	public virtual bool CheckPassword(string password)
	{
		return true;
	}

	public virtual bool IsSetPassword()
	{
		return false;
	}

	public void Dispose()
	{
	}
}
