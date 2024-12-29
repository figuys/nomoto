using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ModelV6;

public class FailedItem : ViewModelBase
{
	private string id;

	private string path;

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			if (!(id == value))
			{
				id = value;
				OnPropertyChanged("Id");
			}
		}
	}

	public string Path
	{
		get
		{
			return path;
		}
		set
		{
			if (!(path == value))
			{
				path = value;
				OnPropertyChanged("Path");
			}
		}
	}
}
