using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class RescuingSuccessViewV6 : UserControl, IComponentConnector
{
	public Action OkAction { get; set; }

	public RescuingSuccessViewV6()
	{
		InitializeComponent();
	}

	public void Init(string mesage, string modelName, string imei)
	{
		if (!string.IsNullOrEmpty(mesage))
		{
			panelNotify.Visibility = Visibility.Visible;
			message.LangKey = mesage;
		}
		if (Plugin.SupportMulti)
		{
			modelnameimeistp.Visibility = Visibility.Visible;
			modelnamectrl.Text = modelName;
			if (!string.IsNullOrEmpty(imei))
			{
				imeistap.Visibility = Visibility.Visible;
				imeictrl.Text = imei;
			}
		}
	}

	private void OkClick(object sender, RoutedEventArgs e)
	{
		OkAction();
	}
}
