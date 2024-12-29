using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ModelV6;

public class LeftNavigationModelV6 : ViewModelBase
{
	private bool _IsEnable = true;

	public ImageSource IconImageSource { get; private set; }

	public ImageSource IconHoverImageSource { get; private set; }

	public string Text { get; private set; }

	public ViewType Key { get; private set; }

	public bool Reload { get; set; }

	public bool IsEnable
	{
		get
		{
			return _IsEnable;
		}
		set
		{
			_IsEnable = value;
			OnPropertyChanged("IsEnable");
		}
	}

	public LeftNavigationModelV6(ViewType _viewType, string _text, string _icon, string _iconHover, bool reload = false)
	{
		Key = _viewType;
		Text = _text;
		IconImageSource = GetIconImage(_icon);
		IconHoverImageSource = GetIconImage(_iconHover);
		Reload = reload;
	}

	private ImageSource GetIconImage(string _iconName)
	{
		return LeftNavResources.SingleInstance.GetResource(_iconName) as ImageSource;
	}
}
