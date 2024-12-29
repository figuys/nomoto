using System.IO;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class ServerAlbumInfo
{
	private string _albumName = string.Empty;

	public string AlbumPath { get; set; }

	public string AlbumName
	{
		get
		{
			if (!string.IsNullOrEmpty(AlbumPath))
			{
				_albumName = Path.GetFileName(AlbumPath);
			}
			return _albumName;
		}
		set
		{
			_albumName = value;
		}
	}

	public int FileCount { get; set; }
}
