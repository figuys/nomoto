using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class MultiRomsSelView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public MultiRomsSelViewModel VM { get; private set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public MultiRomsSelView()
	{
		InitializeComponent();
		VM = new MultiRomsSelViewModel(this);
		base.DataContext = VM;
		base.Owner = Application.Current.MainWindow;
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnBack(object sender, RoutedEventArgs e)
	{
		panel1.Visibility = Visibility.Visible;
		panel2.Visibility = Visibility.Collapsed;
	}

	private void OnBtnNext(object sender, RoutedEventArgs e)
	{
		panel1.Visibility = Visibility.Collapsed;
		panel2.Visibility = Visibility.Visible;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		if (MainFrameV6.Instance.IMsgManager.ShowMessage("K0969", MessageBoxButton.OKCancel) == true)
		{
			Close();
			CloseAction?.Invoke(false);
		}
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		Result = true;
		Close();
		CloseAction?.Invoke(true);
	}

	public static ResourceResponseModel SelectOneFormRomArr(List<ResourceResponseModel> arr)
	{
		return Application.Current.Dispatcher.Invoke(delegate
		{
			MultiRomsSelView multiRomsSelView = new MultiRomsSelView();
			multiRomsSelView.VM.RomArr.AddRange(arr);
			MainFrameV6.Instance.IMsgManager.ShowMessage(multiRomsSelView);
			return (multiRomsSelView.Result != true) ? null : multiRomsSelView.VM.SelectedRom;
		});
	}
}
