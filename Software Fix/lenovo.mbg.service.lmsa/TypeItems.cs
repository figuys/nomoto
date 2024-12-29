namespace lenovo.mbg.service.lmsa;

public class TypeItems
{
	public enum PlugInsUpdateType
	{
		Force,
		Recommend,
		InstallFirst,
		Other
	}

	public enum MessageBoxType
	{
		OK,
		OKCancel,
		YesNo,
		YesNoCancel,
		Cash_Load,
		Cash_Wait,
		Other
	}

	public enum UpdateType
	{
		Success,
		Downloaded,
		Install,
		Other
	}
}
