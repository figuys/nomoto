using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.hardwaretest.ViewModel;

public class TestItemViewModel : ViewModelBase
{
	private ImageSource _Image = Application.Current.Resources[""] as ImageSource;

	private string _Result = "K1491";

	private int _Status = -1;

	private int idx;

	public string Test { get; set; }

	public string Item { get; set; }

	public string Result
	{
		get
		{
			return _Result;
		}
		set
		{
			_Result = value;
			OnPropertyChanged("Result");
		}
	}

	public ImageSource Image
	{
		get
		{
			return _Image;
		}
		set
		{
			_Image = value;
			OnPropertyChanged("Image");
		}
	}

	public int Status
	{
		get
		{
			return _Status;
		}
		set
		{
			_Status = value;
			OnPropertyChanged("Status");
		}
	}

	public Thickness BorderThickness { get; set; }

	public int Idx
	{
		get
		{
			return idx;
		}
		set
		{
			idx = value;
			if (value % 2 == 0)
			{
				BorderThickness = new Thickness(1.0, 0.0, 1.0, 1.0);
			}
			else
			{
				BorderThickness = new Thickness(0.0, 0.0, 1.0, 1.0);
			}
		}
	}

	public TestItemViewModel(string item, string icon)
	{
		Item = item;
		Image = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/icons/" + icon));
		Status = -1;
	}
}
