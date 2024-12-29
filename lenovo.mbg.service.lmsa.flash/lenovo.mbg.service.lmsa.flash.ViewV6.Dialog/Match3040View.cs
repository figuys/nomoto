using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class Match3040View : Window, IUserMsgControl, IComponentConnector
{
	private Match3040ViewModel _VM;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public Match3040View(ResourceResponseModel response, string matchText, DevCategory category, object wModel)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		_VM = new Match3040ViewModel(this, response, matchText, category, wModel);
		base.DataContext = _VM;
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		bool value = Convert.ToBoolean((sender as Button).Tag);
		Close();
		CloseAction?.Invoke(value);
	}

	public static bool? ProcMatch3040(ResourceResponseModel response, string matchText, DevCategory category, object wModel)
	{
		return Application.Current.Dispatcher.Invoke(delegate
		{
			Match3040View userUi = new Match3040View(response, matchText, category, wModel);
			return MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		});
	}
}
