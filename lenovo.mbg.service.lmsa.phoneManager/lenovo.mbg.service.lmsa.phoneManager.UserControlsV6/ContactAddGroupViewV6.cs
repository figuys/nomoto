using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class ContactAddGroupViewV6 : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public ContactAddGroupViewV6()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnSave(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(txtGroupName.Text))
		{
			Context.FindViewModel<ContactViewModelV6>(typeof(ContactMgtViewV6)).AddNewGroup(txtGroupName.Text);
		}
		Result = true;
		CloseAction?.Invoke(true);
		Close();
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Result = false;
		CloseAction?.Invoke(false);
		Close();
	}

	private void txtGroupName_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(txtGroupName.Text))
		{
			btn_Save.IsEnabled = true;
		}
		else
		{
			btn_Save.IsEnabled = false;
		}
	}
}
