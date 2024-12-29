using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class MainWindowOperationPanelStyleViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private Brush controlPanelBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));

	private Brush controlPanelFreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));

	private Brush controlPanelMouseOverFreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));

	private Visibility navigationSpliteEffectVisibility = Visibility.Collapsed;

	public Brush ControlPanelBackground
	{
		get
		{
			return controlPanelBackground;
		}
		set
		{
			if (value != controlPanelBackground)
			{
				controlPanelBackground = value;
				OnPropertyChanged("ControlPanelBackground");
			}
		}
	}

	public Brush ControlPanelFreground
	{
		get
		{
			return controlPanelFreground;
		}
		set
		{
			if (controlPanelFreground != value)
			{
				controlPanelFreground = value;
				OnPropertyChanged("ControlPanelFreground");
			}
		}
	}

	public Brush ControlPanelMouseOverFreground
	{
		get
		{
			return controlPanelMouseOverFreground;
		}
		set
		{
			if (controlPanelMouseOverFreground != value)
			{
				controlPanelMouseOverFreground = value;
				OnPropertyChanged("ControlPanelMouseOverFreground");
			}
		}
	}

	public Visibility NavigationSpliteEffectVisibility
	{
		get
		{
			return navigationSpliteEffectVisibility;
		}
		set
		{
			if (navigationSpliteEffectVisibility != value)
			{
				navigationSpliteEffectVisibility = value;
				OnPropertyChanged("NavigationSpliteEffectVisibility");
			}
		}
	}
}
