using System;
using System.IO;

namespace lenovo.mbg.service.framework.socket;

public class TransferFileInfo
{
	private string _virtualFileName = string.Empty;

	public string LogicFileName { get; set; }

	public string VirtualFileName
	{
		get
		{
			if (!string.IsNullOrEmpty(_virtualFileName))
			{
				return _virtualFileName;
			}
			if (!string.IsNullOrEmpty(FilePath))
			{
				string fileName = Path.GetFileName(FilePath);
				if (string.IsNullOrEmpty(fileName))
				{
					_virtualFileName = LogicFileName;
				}
				else
				{
					_virtualFileName = fileName;
				}
			}
			return _virtualFileName;
		}
	}

	public string FilePath { get; set; }

	public DateTime ModifiedDateTime { get; set; }

	public long FileSize { get; set; }

	public string localFilePath { get; set; }
}
