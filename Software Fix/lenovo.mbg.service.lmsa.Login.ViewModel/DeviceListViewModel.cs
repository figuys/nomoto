using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.View;
using lenovo.mbg.service.lmsa.UserControls.MessageBoxWindow;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Login.ViewModel;

public class DeviceListViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private ObservableCollection<DeviceModel> _devices;

	public ReplayCommand DetailCommand { get; set; }

	public ReplayCommand DeleteCommand { get; set; }

	public ObservableCollection<DeviceModel> Devices
	{
		get
		{
			return _devices;
		}
		set
		{
			_devices = value;
			OnPropertyChanged("Devices");
		}
	}

	public DeviceListViewModel()
	{
		Devices = new DeviceModelCollection();
		DetailCommand = new ReplayCommand(DetailCommandHandler);
		DeleteCommand = new ReplayCommand(DeleteCommandHandler);
		DeviceDataCollection.Instance.OnDevicesChanged += OnDevicesChangedHandler;
	}

	private void OnDevicesChangedHandler(object sender, UserDeviceModel e)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			LoadDevices(e.Devices);
		});
	}

	public void Initialize()
	{
		List<DeviceModel> userDevices = DeviceDataCollection.Instance.GetUserDevices();
		LoadDevices(userDevices);
	}

	private void DetailCommandHandler(object parameter)
	{
		ListViewItem listViewItem = parameter as ListViewItem;
		DeviceModel device = listViewItem.DataContext as DeviceModel;
		LenovoWindow win = new LenovoWindow();
		win.SizeToContent = SizeToContent.WidthAndHeight;
		DeviceDetailViewModel dataContext = new DeviceDetailViewModel(device);
		win.Content = new DeviceDetailView
		{
			DataContext = dataContext
		};
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
	}

	private void DeleteCommandHandler(object parameter)
	{
		CommonWindowModel commonWindowModel = new CommonWindowModel(2);
		LenovoPopupWindow win = commonWindowModel.CreateWindow(LangTranslation.Translate("K0071"), LangTranslation.Translate("K0708"), "K0570", "K0571", null);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		}, delegate(Window s)
		{
			s.Topmost = true;
		});
		if (commonWindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
		{
			ListViewItem listViewItem = parameter as ListViewItem;
			DeviceModel deviceModel = listViewItem.DataContext as DeviceModel;
			Devices.Remove(deviceModel);
			DeviceDataCollection.Instance.Remove(deviceModel);
		}
	}

	private void LoadDevices(List<DeviceModel> devices)
	{
		Devices.Clear();
		devices?.ForEach(delegate(DeviceModel n)
		{
			Devices.Add(n);
		});
	}
}
