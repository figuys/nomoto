using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ModelV6;

public class PluginModel : NotifyBase
{
	private bool _IsSelected;

	private FrameworkElement _UiElement;

	private bool isEnabled = true;

	public string PluginName { get; set; }

	public string NormalIcon { get; set; }

	public string HoverIcon { get; set; }

	public string Icon4B2B { get; set; }

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

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public PluginInfo Info { get; set; }

	public IPlugin Plugin { get; set; }
}
