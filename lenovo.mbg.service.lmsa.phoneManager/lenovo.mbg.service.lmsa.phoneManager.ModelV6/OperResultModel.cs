using System.Collections.ObjectModel;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ModelV6;

public class OperResultModel : ViewModelBase
{
	private Visibility lineVisibility;

	private Visibility failedItemsPanelVisibility = Visibility.Collapsed;

	private bool isFailedItemsPanelClosed;

	private string resName;

	private string _ResNameCount;

	private int complete;

	private int total;

	private int _Failed;

	private bool isComplete;

	private bool isBackup;

	private ObservableCollection<FailedItem> failedItemList;

	private int missingCount;

	private string missintRowTital = "item(s) missed";

	private Visibility missingGroupVisibility = Visibility.Collapsed;

	public Visibility LineVisibility
	{
		get
		{
			return lineVisibility;
		}
		set
		{
			if (lineVisibility != value)
			{
				lineVisibility = value;
				OnPropertyChanged("LineVisibility");
			}
		}
	}

	public Visibility FailedItemsPanelVisibility
	{
		get
		{
			return failedItemsPanelVisibility;
		}
		set
		{
			if (failedItemsPanelVisibility != value)
			{
				failedItemsPanelVisibility = value;
				OnPropertyChanged("FailedItemsPanelVisibility");
			}
		}
	}

	public bool IsFailedItemsPanelClosed
	{
		get
		{
			return isFailedItemsPanelClosed;
		}
		set
		{
			if (isFailedItemsPanelClosed != value)
			{
				isFailedItemsPanelClosed = value;
				OnPropertyChanged("IsFailedItemsPanelClosed");
			}
		}
	}

	public string ResName
	{
		get
		{
			return resName;
		}
		set
		{
			resName = value;
			OnPropertyChanged("ResName");
		}
	}

	public string ResNameCount
	{
		get
		{
			return _ResNameCount;
		}
		set
		{
			_ResNameCount = value;
			OnPropertyChanged("ResNameCount");
		}
	}

	public int Complete
	{
		get
		{
			return complete;
		}
		set
		{
			complete = value;
			ResNameCount = $"({value}/{Total})";
			Failed = Total - value;
			OnPropertyChanged("Complete");
		}
	}

	public int Total
	{
		get
		{
			return total;
		}
		set
		{
			total = value;
			OnPropertyChanged("Total");
		}
	}

	public int Failed
	{
		get
		{
			return _Failed;
		}
		set
		{
			_Failed = value;
			OnPropertyChanged("Failed");
		}
	}

	public bool IsComplete
	{
		get
		{
			return isComplete;
		}
		set
		{
			if (isComplete != value)
			{
				isComplete = value;
				OnPropertyChanged("IsComplete");
			}
		}
	}

	public bool IsBackup
	{
		get
		{
			return isBackup;
		}
		set
		{
			isBackup = value;
			OnPropertyChanged("IsBackup");
		}
	}

	public ObservableCollection<FailedItem> FailedItemList
	{
		get
		{
			return failedItemList;
		}
		set
		{
			if (failedItemList != value)
			{
				failedItemList = value;
				OnPropertyChanged("FailedItemList");
			}
		}
	}

	public int MissingCount
	{
		get
		{
			return missingCount;
		}
		set
		{
			if (missingCount != value)
			{
				missingCount = value;
				OnPropertyChanged("MissingCount");
			}
		}
	}

	public string MissingRowTital
	{
		get
		{
			return missintRowTital;
		}
		set
		{
			if (!(missintRowTital == value))
			{
				missintRowTital = value;
				OnPropertyChanged("MissingRowTital");
			}
		}
	}

	public Visibility MissingGroupVisibility
	{
		get
		{
			return missingGroupVisibility;
		}
		set
		{
			if (missingGroupVisibility != value)
			{
				missingGroupVisibility = value;
				OnPropertyChanged("MissingGroupVisibility");
			}
		}
	}

	public void AddFailedItem(FailedItem item, bool isMissingType)
	{
		if (item == null)
		{
			return;
		}
		if (FailedItemList == null)
		{
			FailedItemList = new ObservableCollection<FailedItem>();
		}
		for (int i = 0; i < FailedItemList.Count; i++)
		{
			if (FailedItemList[i].Path != null && FailedItemList[i].Path.Equals(item.Path))
			{
				return;
			}
		}
		if (isMissingType)
		{
			int num = MissingCount + 1;
			MissingCount = num;
			MissingGroupVisibility = Visibility.Visible;
		}
		else
		{
			FailedItemList.Add(item);
		}
	}
}
