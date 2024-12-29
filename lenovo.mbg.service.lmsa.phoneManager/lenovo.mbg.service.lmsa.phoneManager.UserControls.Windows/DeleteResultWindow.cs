using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class DeleteResultWindow : Window, IComponentConnector
{
	public DeleteResultWindow(int success, int fail, int confirm)
	{
		InitializeComponent();
		string text = string.Format(HostProxy.LanguageService.Translate("K1095"), success, fail);
		string text2 = string.Format(HostProxy.LanguageService.Translate("K1096"), confirm);
		txtContent.Text = text;
		txtContent.TextEffects.Clear();
		txtContent.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(67, 181, 226)),
			PositionStart = text.IndexOf(success.ToString()),
			PositionCount = success.ToString().Length
		});
		txtContent.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0)),
			PositionStart = text.LastIndexOf(fail.ToString()),
			PositionCount = fail.ToString().Length
		});
		txtWarning.Text = text2;
		txtWarning.TextEffects.Clear();
		txtWarning.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0)),
			PositionStart = text2.IndexOf(confirm.ToString()),
			PositionCount = confirm.ToString().Length
		});
	}

	private void OnBtnGoOn(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}
}
