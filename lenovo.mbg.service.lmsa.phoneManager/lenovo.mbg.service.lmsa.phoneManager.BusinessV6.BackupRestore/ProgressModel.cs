namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class ProgressModel
{
	public string ProgressBarTitle { get; set; }

	public int ProgressBarTotalCount { get; set; }

	public ProgressModel(string title, int count)
	{
		ProgressBarTitle = title;
		ProgressBarTotalCount = count;
	}
}
