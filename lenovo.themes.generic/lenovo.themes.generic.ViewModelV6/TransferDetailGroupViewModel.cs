using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace lenovo.themes.generic.ViewModelV6;

public class TransferDetailGroupViewModel : ViewModelBase
{
	private List<TransferFailedItemViewModel> MissItems = new List<TransferFailedItemViewModel>();

	private int _Count;

	private int _CurrentCount;

	private ImageSource _IMG;

	private int _SuccessCount;

	private int _FailCount;

	public string _DeatilTitleCount;

	private ObservableCollection<TransferFailedItemViewModel> _FailedItems;

	private int _MissingCount;

	private string _MissMessage;

	private Visibility _MissingGroupVisibility = Visibility.Collapsed;

	private string errorKey = "K1892";

	private bool _IsComplete;

	private bool? _IsComplete1;

	public string Id { get; set; }

	public string DeatilTitle { get; set; }

	public int Count
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

	public int CurrentCount
	{
		get
		{
			return _CurrentCount;
		}
		set
		{
			_CurrentCount = value;
			OnPropertyChanged("CurrentCount");
		}
	}

	public ImageSource IMG
	{
		get
		{
			return _IMG;
		}
		set
		{
			_IMG = value;
			OnPropertyChanged("IMG");
		}
	}

	public int SuccessCount
	{
		get
		{
			return _SuccessCount;
		}
		set
		{
			_SuccessCount = value;
			IsComplete = _SuccessCount >= Count;
			DeatilTitleCount = $"({_SuccessCount}/{Count})";
			OnPropertyChanged("SuccessCount");
		}
	}

	public int FailCount
	{
		get
		{
			return _FailCount;
		}
		set
		{
			_FailCount = value;
			OnPropertyChanged("FailCount");
		}
	}

	public string DeatilTitleCount
	{
		get
		{
			return _DeatilTitleCount;
		}
		set
		{
			_DeatilTitleCount = value;
			OnPropertyChanged("DeatilTitleCount");
		}
	}

	public ObservableCollection<TransferFailedItemViewModel> FailedItems
	{
		get
		{
			return _FailedItems;
		}
		set
		{
			_FailedItems = value;
			OnPropertyChanged("FailedItems");
		}
	}

	public int MissingCount
	{
		get
		{
			return _MissingCount;
		}
		set
		{
			_MissingCount = value;
			if (_MissingCount > 0)
			{
				MissMessage = "K0218";
				MissingGroupVisibility = Visibility.Visible;
			}
			else
			{
				MissingGroupVisibility = Visibility.Collapsed;
			}
			OnPropertyChanged("MissingCount");
		}
	}

	public string MissMessage
	{
		get
		{
			return _MissMessage;
		}
		set
		{
			_MissMessage = value;
			OnPropertyChanged("MissMessage");
		}
	}

	public Visibility MissingGroupVisibility
	{
		get
		{
			return _MissingGroupVisibility;
		}
		set
		{
			_MissingGroupVisibility = value;
			if (_MissingGroupVisibility == Visibility.Visible)
			{
				ErrorKey = "K1892";
			}
			OnPropertyChanged("MissingGroupVisibility");
		}
	}

	public string ErrorKey
	{
		get
		{
			return errorKey;
		}
		set
		{
			if (!(errorKey == value))
			{
				errorKey = value;
				OnPropertyChanged("ErrorKey");
			}
		}
	}

	public bool IsComplete
	{
		get
		{
			return _IsComplete;
		}
		set
		{
			_IsComplete = value;
			OnPropertyChanged("IsComplete");
		}
	}

	public bool? IsComplete1
	{
		get
		{
			return _IsComplete1;
		}
		set
		{
			_IsComplete1 = value;
			OnPropertyChanged("IsComplete1");
		}
	}

	public TransferDetailGroupViewModel(string id, string title, int count, int success)
	{
		switch (id)
		{
		case "Videos":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-Videos"] as ImageSource;
			break;
		case "Call log":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-Calllog"] as ImageSource;
			break;
		case "Contacts":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-Contacts"] as ImageSource;
			break;
		case "Files":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-Files"] as ImageSource;
			break;
		case "Songs":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-Music"] as ImageSource;
			break;
		case "Pictures":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-Pictures"] as ImageSource;
			break;
		case "SMS":
			IMG = Application.Current.Resources["v6_Icon-BackupRestore-SMS"] as ImageSource;
			break;
		}
		Id = id;
		DeatilTitle = title;
		Count = count;
		SuccessCount = success;
		FailedItems = new ObservableCollection<TransferFailedItemViewModel>();
	}

	public void AddMissedItem(int missedCount, TransferFailedItemViewModel missitem)
	{
		MissItems.Add(missitem);
		MissingCount += missedCount;
		FailCount += missedCount;
		SuccessCount -= missedCount;
	}

	public void AddFailedItem(TransferFailedItemViewModel item, int failedCount)
	{
		if (item != null)
		{
			FailedItems.Add(item);
		}
		FailCount += failedCount;
		SuccessCount -= failedCount;
	}

	public void RemoveWhenSuccess(string id, string path)
	{
		SuccessCount++;
		if (MissItems.Where((TransferFailedItemViewModel x) => x.Id == id && path.Contains(x.Message)).Count() != 0)
		{
			MissingCount--;
		}
		FailCount--;
		TransferFailedItemViewModel transferFailedItemViewModel = FailedItems.FirstOrDefault((TransferFailedItemViewModel n) => n.Id == id);
		if (transferFailedItemViewModel != null)
		{
			FailedItems.Remove(transferFailedItemViewModel);
		}
	}
}
