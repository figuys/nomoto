using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class LeftNavigationItemViewModel : ViewModelBase
{
	private ImageSource iconImageSource;

	private ImageSource iconHoverImageSource;

	private string text;

	private bool isEnabled = true;

	private Visibility _ItemVisibility;

	public ImageSource IconImageSource
	{
		get
		{
			return iconImageSource;
		}
		set
		{
			if (iconImageSource != value)
			{
				iconImageSource = value;
				OnPropertyChanged("IconImageSource");
			}
		}
	}

	public ImageSource IconHoverImageSource
	{
		get
		{
			return iconHoverImageSource;
		}
		set
		{
			if (iconHoverImageSource != value)
			{
				iconHoverImageSource = value;
				OnPropertyChanged("IconHoverImageSource");
			}
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (!(text == value))
			{
				text = value;
				OnPropertyChanged("Text");
			}
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
			if (isEnabled != value)
			{
				isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public Visibility ItemVisibility
	{
		get
		{
			return _ItemVisibility;
		}
		set
		{
			if (_ItemVisibility != value)
			{
				_ItemVisibility = value;
				OnPropertyChanged("ItemVisibility");
			}
		}
	}

	public object Key { get; set; }

	public FrameworkElement View { get; set; }
}
