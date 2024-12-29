using System.Windows.Media;

namespace lenovo.themes.generic.ViewModelV6;

public class DeviceInfoModel : ViewModelBase
{
	private string _Item1;

	private string _Item2;

	private int _Item3;

	private int _Item4;

	private bool _Item5;

	private ImageSource _Item6;

	public string Item1
	{
		get
		{
			return _Item1;
		}
		set
		{
			_Item1 = value;
			OnPropertyChanged("Item1");
		}
	}

	public string Item2
	{
		get
		{
			return _Item2;
		}
		set
		{
			_Item2 = value;
			OnPropertyChanged("Item2");
		}
	}

	public int Item3
	{
		get
		{
			return _Item3;
		}
		set
		{
			_Item3 = value;
			OnPropertyChanged("Item3");
		}
	}

	public int Item4
	{
		get
		{
			return _Item4;
		}
		set
		{
			_Item4 = value;
			OnPropertyChanged("Item4");
		}
	}

	public bool Item5
	{
		get
		{
			return _Item5;
		}
		set
		{
			_Item5 = value;
			OnPropertyChanged("Item5");
		}
	}

	public ImageSource Item6
	{
		get
		{
			return _Item6;
		}
		set
		{
			_Item6 = value;
			OnPropertyChanged("Item6");
		}
	}

	public DeviceInfoModel(ImageSource item6, string item1, string item2, int item3, int item4, bool item5 = false)
	{
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
		Item4 = item4;
		Item5 = item5;
		Item6 = item6;
	}
}
