using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupStorage : IBackupStorage, IDisposable
{
	private class BRWriterReaderFactory
	{
		private BackupStorage _outer;

		public IBackupResourceReader BackupResourceReader { get; private set; }

		public IBackupResourceWriter BackupResourceWriter { get; private set; }

		public BRWriterReaderFactory(BackupStorage outer)
		{
			_outer = outer;
		}

		public void Load()
		{
		}
	}

	private FileInfo _fileInfo;

	private FileStream _fs;

	private IBackupResourceWriter _backupResourceWriter;

	private IBackupResourceReader _backupResourceReader;

	private string _version = string.Empty;

	public long Position
	{
		get
		{
			if (_fs != null)
			{
				return _fs.Position;
			}
			return 0L;
		}
	}

	public string StoragePath => _fileInfo.FullName;

	public long Size
	{
		get
		{
			if (_fs == null)
			{
				return 0L;
			}
			return _fs.Length;
		}
	}

	public BackupStorage(FileInfo fileInfo)
	{
		_fileInfo = fileInfo;
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		if (_fs == null)
		{
			throw new BackupRestoreException("Read stream is null,should open write");
		}
		return _fs.Read(buffer, offset, count);
	}

	public void Seek(long offset, SeekOrigin seekOrigin)
	{
		if (_fs == null)
		{
			throw new BackupRestoreException("Write/read stream is null,should open write/read");
		}
		_fs.Seek(offset, seekOrigin);
	}

	public void Write(byte[] buffer, int offset, int count)
	{
		if (_fs == null)
		{
			throw new BackupRestoreException("Write stream is null,should open write");
		}
		_fs.Write(buffer, offset, count);
	}

	public IBackupResourceWriter OpenWrite(string version)
	{
		if (_fs == null)
		{
			if (!_fileInfo.Directory.Exists)
			{
				_fileInfo.Directory.Create();
			}
			_fs = _fileInfo.OpenWrite();
			if (_fs.Length == 0L)
			{
				byte[] buffer = new BackupStorageVersion().EncryptVersion(version);
				_fs.Write(buffer, 0, 48);
			}
			_backupResourceWriter = LoadBRWriterReaderVersionType<IBackupResourceWriter>(version);
		}
		return _backupResourceWriter;
	}

	public IBackupResourceReader OpenRead(out string version)
	{
		version = _version;
		if (_fs == null)
		{
			_fs = _fileInfo.OpenRead();
			byte[] array = new byte[48];
			if (_fs.Read(array, 0, 48) != 48)
			{
				throw new BackupRestoreException("Get storage version failed");
			}
			BackupStorageVersion backupStorageVersion = new BackupStorageVersion();
			_version = (version = backupStorageVersion.DecryptVersion(array));
			_backupResourceReader = LoadBRWriterReaderVersionType<IBackupResourceReader>(version);
		}
		return _backupResourceReader;
	}

	private T LoadBRWriterReaderVersionType<T>(string version)
	{
		Type typeFromHandle = typeof(T);
		Type[] types = Assembly.GetAssembly(typeFromHandle).GetTypes();
		foreach (Type type in types)
		{
			if (!type.IsClass || !typeFromHandle.IsAssignableFrom(type))
			{
				continue;
			}
			object[] customAttributes = type.GetCustomAttributes(typeof(SupportedFormatVersionAttribute), inherit: false);
			if (customAttributes.Count() == 0)
			{
				continue;
			}
			object[] array = customAttributes;
			for (int j = 0; j < array.Length; j++)
			{
				if (((SupportedFormatVersionAttribute)array[j]).Version.Equals(version))
				{
					return (T)Activator.CreateInstance(type, this);
				}
			}
		}
		return default(T);
	}

	public void Flush()
	{
		if (_fs == null)
		{
			throw new BackupRestoreException("Write/read stream is null,should open write/read");
		}
		_fs.Flush();
	}

	public void Dispose()
	{
		if (_fs != null)
		{
			try
			{
				_fs.Close();
				_fs.Dispose();
				_fs = null;
			}
			catch (Exception)
			{
			}
		}
		try
		{
			_backupResourceReader?.Dispose();
		}
		catch (Exception)
		{
		}
		try
		{
			_backupResourceWriter?.Dispose();
		}
		catch (Exception)
		{
		}
	}

	public void Delete()
	{
		_fileInfo?.Delete();
	}

	public void SetLength(long length)
	{
		if (_fs == null)
		{
			throw new BackupRestoreException("Write/read stream is null,should open write/read");
		}
		_fs.SetLength(length);
	}
}
