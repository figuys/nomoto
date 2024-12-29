using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lenovo.mbg.service.common.utilities;

public class CustomFile
{
	private static CustomFile _instance;

	private string TAG => GetType().FullName;

	public static CustomFile Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			return _instance = new CustomFile();
		}
	}

	public string StorageDirName { get; set; }

	public Stream ReadStream(string filePath)
	{
		return new FileStream(filePath, FileMode.Open, FileAccess.Read);
	}

	public Stream WriteStream(string filePath)
	{
		return WriteStream(filePath, FileMode.Create);
	}

	public Stream WriteStream(string filePath, FileMode mode)
	{
		EnsureExists(filePath);
		return new FileStream(filePath, mode, FileAccess.Write);
	}

	public void CopyStream(Stream input, Stream output)
	{
		byte[] array = new byte[4096];
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, count);
		}
	}

	public void CopyStream(Stream input, Stream output, Func<byte[], byte[]> modifier)
	{
		byte[] array = new byte[4096];
		int num;
		while ((num = input.Read(array, 0, array.Length)) > 0)
		{
			byte[] array2 = array;
			if (modifier != null)
			{
				if (array2.Length != num)
				{
					array2 = new byte[num];
					Array.Copy(array, array2, num);
				}
				array2 = modifier(array2);
			}
			output.Write(array2, 0, num);
		}
	}

	public string ReadText(string filePath)
	{
		try
		{
			return File.ReadAllText(filePath, Encoding.UTF8);
		}
		catch
		{
			using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			return streamReader.ReadToEnd();
		}
	}

	public void WriteText(string filePath, string text)
	{
		EnsureExists(filePath);
		File.WriteAllText(filePath, text, Encoding.UTF8);
	}

	public string Uuid()
	{
		return Guid.NewGuid().ToString("N").ToLowerInvariant();
	}

	public string TempFolder()
	{
		string text = string.Empty;
		do
		{
			string text2 = PathJoin(Path.GetTempPath(), Uuid());
			if (!Exists(text2))
			{
				text = text2;
			}
		}
		while (text == string.Empty);
		return text;
	}

	public string PathJoin(string path1, string path2)
	{
		return Path.Combine(path1, path2);
	}

	public bool Exists(string path)
	{
		if (!File.Exists(path))
		{
			return Directory.Exists(path);
		}
		return true;
	}

	public long FileSize(string path)
	{
		if (!Exists(path))
		{
			return -1L;
		}
		return new FileInfo(path).Length;
	}

	public void Delete(string path)
	{
		File.Delete(path);
	}

	public void Remove(string folderPath)
	{
		Directory.Delete(folderPath, recursive: true);
	}

	public void MirrorFiles(string sourcePath, string destinationPath)
	{
		if (!Exists(sourcePath))
		{
			throw new DirectoryNotFoundException("Could not find " + sourcePath);
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);
		DirectoryInfo directoryInfo2 = new DirectoryInfo(destinationPath);
		if (!directoryInfo2.Exists)
		{
			directoryInfo2.Create();
		}
		foreach (string item in FindFiles("*.*", directoryInfo.FullName, recursive: true))
		{
			FileInfo fileInfo = new FileInfo(item);
			if (!item.StartsWith(directoryInfo.FullName))
			{
				continue;
			}
			string text = item.Substring(directoryInfo.FullName.Length);
			string text2 = directoryInfo2.FullName + text;
			if (Exists(text2))
			{
				FileInfo fileInfo2 = new FileInfo(text2);
				if (fileInfo2.Length == fileInfo.Length && fileInfo2.LastWriteTime.Equals(fileInfo.LastWriteTime))
				{
					continue;
				}
			}
			EnsureExists(text2);
			fileInfo.CopyTo(text2, overwrite: true);
		}
	}

	public List<string> FindFiles(string searchPattern, string path, bool recursive)
	{
		List<string> list = new List<string>();
		SearchOption searchOption = SearchOption.TopDirectoryOnly;
		if (recursive)
		{
			searchOption = SearchOption.AllDirectories;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			return list;
		}
		FileInfo[] files = directoryInfo.GetFiles(searchPattern, searchOption);
		foreach (FileInfo fileInfo in files)
		{
			list.Add(fileInfo.FullName);
		}
		list.Sort();
		return list;
	}

	private void EnsureExists(string filePath)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		if (!fileInfo.Directory.Exists)
		{
			Directory.CreateDirectory(fileInfo.Directory.FullName);
		}
	}

	public void FileCopy(string source, string destination, bool overwrite)
	{
		File.Copy(source, destination, overwrite);
	}
}
