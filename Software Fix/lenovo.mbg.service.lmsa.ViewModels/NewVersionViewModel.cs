namespace lenovo.mbg.service.lmsa.ViewModels;

public class NewVersionViewModel
{
	public string VersionID { get; set; }

	public string VersionName { get; set; }

	public string ReleaseDate { get; set; }

	public string FileSize { get; set; }

	public long Size { get; set; }

	public bool IsSelected { get; set; }

	public string CountSize(long Size)
	{
		string result = "";
		long num = 0L;
		num = Size;
		if ((double)num < 1024.0)
		{
			result = num.ToString("F2") + " Byte";
		}
		else if ((double)num >= 1024.0 && num < 1048576)
		{
			result = ((double)num / 1024.0).ToString("F2") + " K";
		}
		else if (num >= 1048576 && num < 1073741824)
		{
			result = ((double)num / 1024.0 / 1024.0).ToString("F2") + " M";
		}
		else if (num >= 1073741824)
		{
			result = ((double)num / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " G";
		}
		return result;
	}
}
