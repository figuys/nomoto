using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class DeleteResultWindowV6 : Window, IComponentConnector
{
	public DeleteResultWindowV6(int success, int fail, int confirm)
	{
		InitializeComponent();
		string text = string.Format(HostProxy.LanguageService.Translate("K1095"), success, fail);
		string text2 = string.Format(HostProxy.LanguageService.Translate("K1096"), confirm);
		txtContent.Text = text;
		txtContent.TextEffects.Clear();
		txtContent.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(0, 93, 127)),
			PositionStart = text.IndexOf(success.ToString()),
			PositionCount = success.ToString().Length
		});
		txtContent.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 64, 66)),
			PositionStart = text.LastIndexOf(fail.ToString()),
			PositionCount = fail.ToString().Length
		});
		txtWarning.Text = text2;
		txtWarning.TextEffects.Clear();
		txtWarning.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 64, 66)),
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