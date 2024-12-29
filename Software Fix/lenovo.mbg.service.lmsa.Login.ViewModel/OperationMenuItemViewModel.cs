using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Login.ViewModel;

public abstract class OperationMenuItemViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private ReplayCommand mClickCommand;

	private Visibility mItemVisibility;

	private ImageSource m_iconSource;

	private ImageSource m_mouseOverIconSource;

	private string m_headerText;

	private bool isEnabled = true;

	public ReplayCommand ClickCommand
	{
		get
		{
			return mClickCommand;
		}
		set
		{
			if (mClickCommand != value)
			{
				mClickCommand = value;
				OnPropertyChanged("ClickCommand");
			}
		}
	}

	public Visibility ItemVisibility
	{
		get
		{
			return mItemVisibility;
		}
		set
		{
			if (mItemVisibility != value)
			{
				mItemVisibility = value;
				OnPropertyChanged("ItemVisibility");
			}
		}
	}

	public ImageSource IconSource
	{
		get
		{
			return m_iconSource;
		}
		set
		{
			if (m_iconSource != value)
			{
				m_iconSource = value;
				OnPropertyChanged("IconSource");
			}
		}
	}

	public ImageSource MouseOverIconSource
	{
		get
		{
			return m_mouseOverIconSource;
		}
		set
		{
			if (m_mouseOverIconSource != value)
			{
				m_mouseOverIconSource = value;
				OnPropertyChanged("MouseOverIconSource");
			}
		}
	}

	public string HeaderText
	{
		get
		{
			return m_headerText;
		}
		set
		{
			if (!(m_headerText == value))
			{
				m_headerText = value;
				OnPropertyChanged("HeaderText");
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

	public OperationMenuItemViewModel()
	{
		ClickCommand = new ReplayCommand(ClickCommandHandler);
	}

	protected abstract void ClickCommandHandler(object e);
}
