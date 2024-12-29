using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using GoogleAnalytics;
using lenovo.mbg.service.lmsa.Business;

namespace lenovo.mbg.service.lmsa.Login.View;

public partial class UserOperationMenu : UserControl, IComponentConnector
{
	public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(UserOperationMenu), new PropertyMetadata(null));

	public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(UserOperationMenu), new PropertyMetadata(null));

	public object Icon
	{
		get
		{
			return GetValue(IconProperty);
		}
		set
		{
			SetValue(IconProperty, value);
		}
	}

	public object Header
	{
		get
		{
			return GetValue(HeaderProperty);
		}
		set
		{
			SetValue(HeaderProperty, value);
		}
	}

	public UserOperationMenu()
	{
		InitializeComponent();
	}

	private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuClick", "menu button click", 0L).Build());
	}

	private void winmenu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (MainWindowControl.Instance.IsExecuteWork())
		{
			e.Handled = true;
		}
	}
}
