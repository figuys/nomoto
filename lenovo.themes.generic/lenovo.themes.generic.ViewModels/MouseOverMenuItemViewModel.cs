using System.Collections.ObjectModel;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class MouseOverMenuItemViewModel : ViewModelBase
{
	private ObservableCollection<MouseOverMenuItemViewModel> subMenuItems;

	private bool isEnabled = true;

	private object icon;

	private object mouseOverIcon;

	private object disabledIcon;

	private object header;

	private object mouseOverHeader;

	private object disabledHeader;

	private Visibility mItemVisibility;

	private ReplayCommand clickCommand;

	public ObservableCollection<MouseOverMenuItemViewModel> SubMenuItems
	{
		get
		{
			return subMenuItems;
		}
		set
		{
			if (subMenuItems != value)
			{
				subMenuItems = value;
				OnPropertyChanged("SubMenuItems");
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

	public object Icon
	{
		get
		{
			return icon;
		}
		set
		{
			if (icon != value)
			{
				icon = value;
				OnPropertyChanged("Icon");
			}
		}
	}

	public object MouseOverIcon
	{
		get
		{
			if (mouseOverIcon == null)
			{
				return Icon;
			}
			return mouseOverIcon;
		}
		set
		{
			if (mouseOverIcon != value)
			{
				mouseOverIcon = value;
				OnPropertyChanged("MouseOverIcon");
			}
		}
	}

	public object DisabledIcon
	{
		get
		{
			if (disabledIcon == null)
			{
				return Icon;
			}
			return disabledIcon;
		}
		set
		{
			if (disabledIcon != value)
			{
				disabledIcon = value;
				OnPropertyChanged("DisabledIcon");
			}
		}
	}

	public object Header
	{
		get
		{
			return header;
		}
		set
		{
			if (header != value)
			{
				header = value;
				OnPropertyChanged("Header");
			}
		}
	}

	public object MouseOverHeader
	{
		get
		{
			if (mouseOverHeader == null)
			{
				return Header;
			}
			return mouseOverHeader;
		}
		set
		{
			if (mouseOverHeader != value)
			{
				mouseOverHeader = value;
				OnPropertyChanged("MouseOverHeader");
			}
		}
	}

	public object DisabledHeader
	{
		get
		{
			if (disabledHeader == null)
			{
				return Header;
			}
			return disabledHeader;
		}
		set
		{
			if (disabledHeader != value)
			{
				disabledHeader = value;
				OnPropertyChanged("DisabledHeader");
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

	public ReplayCommand ClickCommand
	{
		get
		{
			return clickCommand;
		}
		set
		{
			if (clickCommand != value)
			{
				clickCommand = value;
				OnPropertyChanged("ClickCommand");
			}
		}
	}

	public MouseOverMenuItemViewModel()
	{
		ClickCommand = new ReplayCommand(ClickCommandHandler);
	}

	public virtual void ClickCommandHandler(object args)
	{
	}
}
