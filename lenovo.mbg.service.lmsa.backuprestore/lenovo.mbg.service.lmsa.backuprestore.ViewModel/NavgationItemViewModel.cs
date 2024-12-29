using System.Windows.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class NavgationItemViewModel : ViewModelBase
{
	private string _title = string.Empty;

	private bool isDialog;

	private bool isSelected;

	private int? _Count;

	public int Index { get; set; }

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			if (!(_title == value))
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public bool IsDialog
	{
		get
		{
			return isDialog;
		}
		set
		{
			isDialog = value;
		}
	}

	public Control View { get; set; }

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public int? Count
	{
		get
		{
			return _Count;
		}
		set
		{
			_Count = value;
			OnPropertyChanged("Count");
		}
	}
}
