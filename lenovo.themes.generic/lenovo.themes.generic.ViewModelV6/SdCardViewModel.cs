using System.Collections.ObjectModel;
using System.Windows;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.themes.generic.ViewModelV6;

public class SdCardViewModel : ViewModelBase
{
	protected ObservableCollection<ComboboxViewModel> sources = new ObservableCollection<ComboboxViewModel>();

	private int _StorageIndex;

	private Visibility _StorageSelVisible;

	public ObservableCollection<ComboboxViewModel> Sources
	{
		get
		{
			return sources;
		}
		set
		{
			sources = value;
			OnPropertyChanged("Sources");
		}
	}

	public int StorageSelIndex
	{
		get
		{
			return _StorageIndex;
		}
		set
		{
			_StorageIndex = value;
			OnPropertyChanged("StorageSelIndex");
		}
	}

	public Visibility StorageSelVisible
	{
		get
		{
			return _StorageSelVisible;
		}
		set
		{
			_StorageSelVisible = value;
			OnPropertyChanged("StorageSelVisible");
		}
	}

	public override void LoadData(object data)
	{
		DeviceEx obj = data as DeviceEx;
		Sources.Clear();
		StorageSelIndex = 0;
		Sources.Add(new ComboboxViewModel
		{
			Content = "Internal"
		});
		if (obj != null && obj.Property?.TotalExternalStorage > 0)
		{
			Sources.Add(new ComboboxViewModel
			{
				Content = "SD Card"
			});
		}
		else
		{
			Sources.Add(new ComboboxViewModel
			{
				Content = "No SD Card found",
				IsEnabled = false
			});
		}
	}
}
