using System.Windows;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class SwitchDeviceWindowViewModel : ViewModelBase
{
	private static object locker = new object();

	private static SwitchDeviceWindowViewModel _instance;

	private object _ComboBoxSelectedValue;

	public ConnectedDeviceViewModel ConnectedDeviceViewModel => ConnectedDeviceViewModel.Instance;

	public RelayCommand<object> ClickCommand { get; private set; }

	public RelayCommand<object> CloseCommand { get; private set; }

	public static SwitchDeviceWindowViewModel Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (locker)
				{
					if (_instance == null)
					{
						_instance = new SwitchDeviceWindowViewModel();
					}
				}
			}
			return _instance;
		}
	}

	public object ComboBoxSelectedValue
	{
		get
		{
			return _ComboBoxSelectedValue;
		}
		set
		{
			_ComboBoxSelectedValue = value;
			OnPropertyChanged("ComboBoxSelectedValue");
		}
	}

	public SwitchDeviceWindowViewModel()
	{
		ClickCommand = new RelayCommand<object>(ClickCommandHandler);
		CloseCommand = new RelayCommand<object>(CloseCommandhandler);
	}

	private void ClickCommandHandler(object parameter)
	{
		Close(parameter);
		if (ComboBoxSelectedValue == null)
		{
			CloseCommandhandler(null);
		}
		else
		{
			_ = ((ConnectedDeviceModel)ComboBoxSelectedValue).Key;
		}
	}

	private void CloseCommandhandler(object parameter)
	{
		Close(parameter);
	}

	public void Close(object parameter)
	{
		(parameter as Window)?.Close();
	}
}
