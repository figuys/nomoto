using System.Collections.Generic;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class HelperInfoModel : ViewModelBase
{
	private bool _IsSelected;

	public string ItemText { get; set; }

	public string SelectionText { get; set; }

	public List<ImageSource> HelperImage { get; set; }

	public string Tips { get; set; }

	public bool IsSelected
	{
		get
		{
			return _IsSelected;
		}
		set
		{
			_IsSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}
}
