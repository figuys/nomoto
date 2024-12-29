using System.Collections.Generic;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class MultiRomsSelViewModel : NotifyBase
{
	private bool _IsNextEnable;

	private ResourceResponseModel _SelectedRom;

	public bool IsNextEnable
	{
		get
		{
			return _IsNextEnable;
		}
		set
		{
			_IsNextEnable = value;
			OnPropertyChanged("IsNextEnable");
		}
	}

	public ResourceResponseModel SelectedRom
	{
		get
		{
			return _SelectedRom;
		}
		set
		{
			_SelectedRom = value;
			IsNextEnable = true;
			OnPropertyChanged("SelectedRom");
		}
	}

	public List<ResourceResponseModel> RomArr { get; set; }

	public MultiRomsSelViewModel(MultiRomsSelView ui)
	{
		RomArr = new List<ResourceResponseModel>();
	}
}
