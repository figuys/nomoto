using System;
using System.IO;
using System.Threading;
using lenovo.mbg.service.common.log;
using SevenZip;

namespace lenovo.mbg.service.common.utilities;

public class SevenZipHelper
{
	private static SevenZipHelper _instance;

	public static SevenZipHelper Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			return _instance = new SevenZipHelper();
		}
	}

	private SevenZipHelper()
	{
		SevenZipBase.SetLibraryPath(Configurations.SevenZipDllPath);
	}

	public int Extractor(string zipFile, string directory)
	{
		return Extractor(zipFile, directory, 3);
	}

	public int Extractor(string zipFile, string directory, int tryCount)
	{
		return Extractor(zipFile, directory, null, tryCount);
	}

	public bool Check7zAndZipFiles(string zipFilePath)
	{
		try
		{
			SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(zipFilePath);
			_ = string.Empty;
			foreach (ArchiveFileInfo archiveFileDatum in sevenZipExtractor.ArchiveFileData)
			{
				if (!IsSafePath(archiveFileDatum.FileName))
				{
					LogHelper.LogInstance.Warn("Unsafe file names:[" + archiveFileDatum.FileName + "], detected in ZIP file:[" + zipFilePath + "].");
					return false;
				}
			}
			return true;
		}
		catch (InvalidDataException arg)
		{
			LogHelper.LogInstance.Error($"Unable to open ZIP file:[{zipFilePath}], exception:[{arg}].");
			return false;
		}
	}

	public bool IsSafePath(string entryName)
	{
		string currentDirectory = Directory.GetCurrentDirectory();
		return Path.GetFullPath(Path.Combine(currentDirectory, entryName)).StartsWith(currentDirectory, StringComparison.OrdinalIgnoreCase);
	}

	public int Extractor(string zipFile, string directory, Action<int> progress, int tryCount = 3)
	{
		if (!Check7zAndZipFiles(zipFile))
		{
			return 3;
		}
		int code = 0;
		do
		{
			SevenZipExtractor sevenZipExtractor = null;
			try
			{
				sevenZipExtractor = new SevenZipExtractor(zipFile);
				sevenZipExtractor.Extracting += delegate(object sender, ProgressEventArgs e)
				{
					progress?.Invoke(e.PercentDone);
				};
				sevenZipExtractor.ExtractionFinished += delegate
				{
					LogHelper.LogInstance.Info(zipFile + " already extractor finished");
					code = 0;
				};
				sevenZipExtractor.ExtractArchive(directory);
			}
			catch (SevenZipException ex)
			{
				code = (ex.Message.Contains("not enough space") ? 1 : 2);
				LogHelper.LogInstance.Error("Extractor file " + zipFile + " occur an 7zip exception.\nmsg: " + ex.Message);
			}
			catch (Exception ex2)
			{
				code = 3;
				LogHelper.LogInstance.Error("Extractor file " + zipFile + " occur an exception.\nmsg: " + ex2.Message);
			}
			finally
			{
				sevenZipExtractor?.Dispose();
			}
			if (code == 0)
			{
				break;
			}
			GlobalFun.DeleteDirectoryEx(directory);
			if (code == 1)
			{
				break;
			}
			Thread.Sleep(500);
		}
		while (--tryCount > 0);
		return code;
	}

	public bool ExtractorWithPwd(string _zipFile, string _dir, string _zipPwd)
	{
		if (string.IsNullOrEmpty(_zipPwd))
		{
			LogHelper.LogInstance.Warn("Zip password is empty. Extractor with pwd file [" + _zipFile + "] failed.");
			return false;
		}
		if (!Check7zAndZipFiles(_zipFile))
		{
			return false;
		}
		if (!Directory.Exists(_dir))
		{
			Directory.CreateDirectory(_dir);
		}
		SevenZipExtractor sevenZipExtractor = null;
		try
		{
			string password = Security.Instance.DecryptAseString(_zipPwd);
			sevenZipExtractor = new SevenZipExtractor(_zipFile, password);
			sevenZipExtractor.ExtractionFinished += delegate
			{
				LogHelper.LogInstance.Info("[" + _zipFile + "] already extractor with password finished.");
			};
			sevenZipExtractor.ExtractArchive(_dir);
			if (!ExtractFileSuccessedWithPwd(_dir))
			{
				LogHelper.LogInstance.Info("Unzip folder size is zero. Extractor with pwd file [" + _zipFile + "] failed.");
				return false;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error($"Extractor file [{_zipFile}] occur an exception", exception);
			return false;
		}
		finally
		{
			sevenZipExtractor?.Dispose();
		}
	}

	public bool CheckExtractorWithPwd(string _zipFile, string _zipPwd)
	{
		string text = Path.Combine(Path.GetDirectoryName(_zipFile), Path.GetFileNameWithoutExtension(_zipFile));
		SevenZipExtractor sevenZipExtractor = null;
		if (string.IsNullOrEmpty(_zipPwd))
		{
			LogHelper.LogInstance.Warn("Zip password is empty. Check extractor with pwd file [" + _zipFile + "] failed.");
			return false;
		}
		try
		{
			string password = Security.Instance.DecryptAseString(_zipPwd);
			LogHelper.LogInstance.Debug("begin CheckExtractorWithPwd file [" + _zipFile + "].");
			sevenZipExtractor = new SevenZipExtractor(_zipFile, password);
			string text2 = string.Empty;
			ulong num = ulong.MaxValue;
			foreach (ArchiveFileInfo archiveFileDatum in sevenZipExtractor.ArchiveFileData)
			{
				if (!archiveFileDatum.IsDirectory && archiveFileDatum.Size != 0 && archiveFileDatum.Size < num)
				{
					num = archiveFileDatum.Size;
					text2 = archiveFileDatum.FileName;
				}
			}
			sevenZipExtractor.ExtractFiles(text, text2);
			if (!ExtractFileSuccessedWithPwd(text))
			{
				LogHelper.LogInstance.Info("Unzip folder size is zero. Check extractor with pwd file [" + _zipFile + "] failed.");
				return false;
			}
			return true;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error("check extractor with pwd file [" + _zipFile + "] failed.");
			LogHelper.LogInstance.Debug($"check extractor with pwd file [{_zipFile}] failed, exception:[{arg}].");
			return false;
		}
		finally
		{
			sevenZipExtractor?.Dispose();
			GlobalFun.DeleteDirectory(text);
		}
	}

	private bool ExtractFileSuccessedWithPwd(string _dir)
	{
		string[] files = Directory.GetFiles(_dir, "*.*", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			if (new FileInfo(files[i]).Length > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool CompressWithPwd(string directory, string archiveName, string _pwd)
	{
		try
		{
			string password = Security.Instance.DecryptAseString(_pwd);
			string extension = Path.GetExtension(archiveName);
			SevenZipCompressor sevenZipCompressor = new SevenZipCompressor
			{
				FastCompression = true
			};
			if (extension == ".7z")
			{
				sevenZipCompressor.ArchiveFormat = OutArchiveFormat.SevenZip;
			}
			else
			{
				sevenZipCompressor.ArchiveFormat = OutArchiveFormat.Zip;
			}
			sevenZipCompressor.CompressionFinished += delegate
			{
				LogHelper.LogInstance.Info("[" + archiveName + "] already compress finished.");
				GlobalFun.DeleteDirectory(directory);
			};
			sevenZipCompressor.CompressionMethod = CompressionMethod.Lzma;
			sevenZipCompressor.CompressDirectory(directory, archiveName, password);
			return true;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"compress with password failed, directory:[{directory}] archiveName:[{archiveName}], exception:[{arg}]");
			return false;
		}
	}

	public bool Compress(string directory, string archiveName, Action<int> progress)
	{
		try
		{
			SevenZipCompressor sevenZipCompressor = new SevenZipCompressor();
			sevenZipCompressor.Compressing += delegate(object s, ProgressEventArgs e)
			{
				progress?.Invoke(e.PercentDone);
			};
			sevenZipCompressor.FastCompression = false;
			sevenZipCompressor.ArchiveFormat = OutArchiveFormat.SevenZip;
			sevenZipCompressor.CompressionMethod = CompressionMethod.Lzma;
			sevenZipCompressor.CompressDirectory(directory, archiveName);
			return true;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"compress directory: {directory} failed, {arg}");
			return false;
		}
	}
}
