using System.Windows.Media;

namespace lenovo.themes.generic.ViewModels;

public class IconButtonViewModel : ButtonViewModel
{
	private ImageSource _iconSource;

	private ImageSource _iconMouseOverSource;

	private ImageSource _iconDisabledSource;

	public ImageSource IconSource
	{
		get
		{
			return _iconSource;
		}
		set
		{
			if (_iconSource != value)
			{
				_iconSource = value;
				OnPropertyChanged("IconSource");
			}
		}
	}

	public ImageSource IconMouseOverSource
	{
		get
		{
			if (_iconMouseOverSource == null)
			{
				return IconSource;
			}
			return _iconMouseOverSource;
		}
		set
		{
			if (_iconMouseOverSource != value)
			{
				_iconMouseOverSource = value;
				OnPropertyChanged("IconMouseOverSource");
			}
		}
	}

	public ImageSource IconDisabledSource
	{
		get
		{
			if (_iconDisabledSource == null)
			{
				return IconSource;
			}
			return _iconDisabledSource;
		}
		set
		{
			if (_iconDisabledSource != value)
			{
				_iconDisabledSource = value;
				OnPropertyChanged("IconDisabledSource");
			}
		}
	}
}
