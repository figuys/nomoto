using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.ControlsV6;

public partial class DefaultSmsAppAuthorizeWindow : Window, IUserMsgControl, IComponentConnector
{
	public Func<bool?> OnCheckSMSAuthorize;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public DefaultSmsAppAuthorizeWindow()
	{
		InitializeComponent();
	}

	public DefaultSmsAppAuthorizeWindow(int type)
		: this()
	{
		Init(type);
	}

	private void Init(int type)
	{
		switch (type)
		{
		case 0:
			textblock1.LangKey = "K1604";
			textblock2.Visibility = Visibility.Collapsed;
			img.Source = Application.Current.Resources["v6_SMSMessagesAuthorize"] as ImageSource;
			break;
		case 2:
			textblock1.LangKey = "K1604";
			textblock2.Visibility = Visibility.Collapsed;
			img.Source = Application.Current.Resources["v6_SMSMessagesAuthorizeLt29"] as ImageSource;
			break;
		case 3:
			btnClose.Visibility = Visibility.Collapsed;
			textblock1.LangKey = "K1605";
			textblock2.LangKey = "K0572";
			img.Source = Application.Current.Resources["v6_SMSAppAuthorizeLt29"] as ImageSource;
			break;
		default:
			btnClose.Visibility = Visibility.Collapsed;
			break;
		}
		base.Tag = UserMsgWndData.CanCloseByOthers;
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

	private async void Button_Click(object sender, RoutedEventArgs e)
	{
		if (OnCheckSMSAuthorize != null)
		{
			Button btn = sender as Button;
			btn.IsEnabled = false;
			Result = await Task.Run(() => OnCheckSMSAuthorize());
			btn.IsEnabled = true;
			if (false == Result)
			{
				return;
			}
		}
		Close();
		CloseAction?.Invoke(false);
	}

	private void Run_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
