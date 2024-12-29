using System.Windows;
using System.Windows.Markup;

namespace lenovo.themes.generic.Controls.Windows;

public partial class AnimationProgressWindow : Window, IComponentConnector
{
	public string Message
	{
		set
		{
			txtInfo.LangKey = value;
		}
	}

	public bool IsBtnVisible
	{
		set
		{
			btnFinish.Visibility = ((!value) ? Visibility.Collapsed : Visibility.Visible);
			border.Visibility = (value ? Visibility.Collapsed : Visibility.Visible);
			finishborder.Visibility = btnFinish.Visibility;
		}
	}

	public AnimationProgressWindow(string message)
	{
		InitializeComponent();
		txtInfo.LangKey = message;
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
