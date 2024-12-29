using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ModelV6;

public class DevCategoryModel : NotifyBase
{
	private bool _IsSelected;

	private FrameworkElement _UiElement;

	public string CategoryName { get; set; }

	public DrawingImage NormalIcon { get; set; }

	public DrawingImage HoverIcon { get; set; }

	public DevCategory Category { get; set; }

	public bool IsLoaded { get; set; }

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

	public PageIndex Index { get; set; }

	public FrameworkElement UiElement
	{
		get
		{
			return _UiElement;
		}
		set
		{
			_UiElement = value;
			OnPropertyChanged("UiElement");
		}
	}
}
