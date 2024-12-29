using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ModelV6;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class RegisterDevView : Window, IUserMsgControl, IComponentConnector, IStyleConnector
{
	private RegisterDevicesViewModel _VM;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public RegisterDevView(string title, bool IsOnlySel = true)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		if (IsOnlySel)
		{
			listbox.ItemContainerStyle = TryFindResource("RegisterDevListBoxItemStyle") as Style;
		}
		else
		{
			btnViewOK.Visibility = Visibility.Collapsed;
			listbox.ItemContainerStyle = TryFindResource("RegisterDevListBoxItemStyle2") as Style;
		}
		_VM = new RegisterDevicesViewModel(this);
		_VM.Title = title;
		base.DataContext = _VM;
	}

	public bool IsExistRegistedDev(string category = null)
	{
		bool flag = _VM.LoadLocalRegistedDev(category);
		if (category == null && !flag)
		{
			txtEmptyWarn.Visibility = Visibility.Visible;
			listbox.Visibility = Visibility.Collapsed;
		}
		return flag;
	}

	public RegistedDevModel GetSelRegistedDev()
	{
		return _VM.RegDevArr.FirstOrDefault((RegistedDevModel p) => p.IsSelected);
	}

	private void OnDetailClicked(object sender, RoutedEventArgs e)
	{
		if (!(e.OriginalSource is RadioButton))
		{
			Button button = e.OriginalSource as Button;
			RegistedDevModel registedDevModel = button.Tag as RegistedDevModel;
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(registedDevModel.IMEI2))
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K0460") + " " + registedDevModel.IMEI + "\n\n" + HostProxy.LanguageService.Translate("K0461") + " " + registedDevModel.IMEI2 + "\n\n");
			}
			else if (!string.IsNullOrEmpty(registedDevModel.IMEI))
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K0459") + " " + registedDevModel.IMEI + "\n\n");
			}
			else
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K0459") + " " + HostProxy.LanguageService.Translate("K0470") + "\n\n");
			}
			if (!string.IsNullOrEmpty(registedDevModel.SN))
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K0462") + " " + registedDevModel.SN + "\n\n");
			}
			else
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K0462") + " " + HostProxy.LanguageService.Translate("K0470") + "\n\n");
			}
			if (!string.IsNullOrEmpty(registedDevModel.PN))
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K1118") + " " + registedDevModel.PN + "\n\n");
			}
			else
			{
				stringBuilder.Append(HostProxy.LanguageService.Translate("K1118") + " " + HostProxy.LanguageService.Translate("K0470") + "\n\n");
			}
			ApplcationClass.ApplcationStartWindow.ShowMessage("K0519", stringBuilder.ToString().Trim('\n'));
		}
	}

	private void OnDeleteClicked(object sender, RoutedEventArgs e)
	{
		if (ApplcationClass.ApplcationStartWindow.ShowMessage("K0071", "K0708", "K0571", "K0570") == true)
		{
			Button button = e.OriginalSource as Button;
			RegistedDevModel device = button.Tag as RegistedDevModel;
			_VM.RemoveDevice(device);
			DeviceDataCollection.Instance.GetLocalDevices();
		}
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(false);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(true);
	}
}
