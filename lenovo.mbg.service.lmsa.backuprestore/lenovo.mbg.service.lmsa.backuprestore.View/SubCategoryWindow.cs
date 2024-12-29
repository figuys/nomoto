using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class SubCategoryWindow : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public SubCategoryWindow()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	public void CloseCallback(bool? status)
	{
		CloseAction?.Invoke(status);
	}

	protected override void OnClosed(EventArgs e)
	{
		CloseAction?.Invoke(false);
		base.OnClosed(e);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnClose(object sender, RoutedEventArgs e)
	{
		(base.DataContext as SubCategoryWindowModel).ChangeParentTitle();
		Close();
	}
}
