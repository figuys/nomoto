using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class RescuingViewV6 : UserControl, IDisposable, IComponentConnector
{
	private RescuingViewModel VM { get; }

	public RescuingViewV6()
	{
		InitializeComponent();
		VM = new RescuingViewModel();
		base.DataContext = VM;
	}

	public void Init(string modelName, string imei)
	{
		modelnameimeistp.Visibility = Visibility.Visible;
		modelnamectrl.Text = modelName;
		if (!string.IsNullOrEmpty(imei))
		{
			imeistap.Visibility = Visibility.Visible;
			imeictrl.Text = imei;
		}
	}

	public void ChangeData(string message, double percentage)
	{
		VM.FlashInfoText = message;
		if (percentage > 0.0)
		{
			VM.Percentage = percentage;
		}
	}

	public void Dispose()
	{
		VM.TickTimer.Stop();
		VM.TickTimer.Dispose();
	}
}
