using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public class DrapDropFileParameter
{
	public int Owner { get; private set; }

	public TreeViewModel FolderFile { get; private set; }

	public DrapDropFileParameter(int owner, TreeViewModel file)
	{
		Owner = owner;
		FolderFile = file;
	}
}
