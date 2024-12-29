using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class DeviceInfoItemViewModel : ViewModelBase
{
	private string title;

	private string content = "K0470".ToString();

	private Visibility copyButtonVisibility = Visibility.Collapsed;

	private bool isOddRow;

	private Visibility itemVisibility;

	private bool copiedVisibility;

	private ReplayCommand copyCommand;

	public string DateTemplateTag { get; set; }

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			if (!(title == value))
			{
				title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public string Content
	{
		get
		{
			return content;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value.Equals("unknown", StringComparison.CurrentCultureIgnoreCase))
			{
				ItemVisibility = Visibility.Collapsed;
				content = "K0470".ToString();
			}
			else
			{
				ItemVisibility = Visibility.Visible;
				content = value;
			}
			OnPropertyChanged("Content");
		}
	}

	public Visibility CopyVisibility
	{
		get
		{
			return copyButtonVisibility;
		}
		set
		{
			if (copyButtonVisibility != value)
			{
				copyButtonVisibility = value;
				OnPropertyChanged("CopyVisibility");
			}
		}
	}

	public bool IsOddRow
	{
		get
		{
			return isOddRow;
		}
		set
		{
			if (isOddRow != value)
			{
				isOddRow = value;
				OnPropertyChanged("IsOddRow");
			}
		}
	}

	public Visibility ItemVisibility
	{
		get
		{
			return itemVisibility;
		}
		set
		{
			if (itemVisibility != value)
			{
				itemVisibility = value;
				OnPropertyChanged("ItemVisibility");
			}
		}
	}

	public bool CopiedVisibility
	{
		get
		{
			return copiedVisibility;
		}
		set
		{
			if (copiedVisibility != value)
			{
				copiedVisibility = value;
				OnPropertyChanged("CopiedVisibility");
			}
		}
	}

	public ReplayCommand CopyCommand
	{
		get
		{
			return copyCommand;
		}
		set
		{
			if (copyCommand != value)
			{
				copyCommand = value;
				OnPropertyChanged("CopyCommand");
			}
		}
	}

	public DeviceInfoItemViewModel()
	{
		CopyCommand = new ReplayCommand(CopyCommandHandler);
	}

	public virtual void Clear()
	{
		Content = string.Empty;
	}

	protected virtual void CopyCommandHandler(object args)
	{
		try
		{
			Clipboard.SetDataObject(Content);
			CopiedVisibility = true;
			Task.Factory.StartNew(delegate
			{
				Thread.Sleep(800);
				CopiedVisibility = false;
			});
		}
		catch
		{
		}
	}
}
