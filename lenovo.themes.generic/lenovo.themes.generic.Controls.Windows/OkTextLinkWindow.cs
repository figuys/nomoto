using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.Controls.Windows;

public partial class OkTextLinkWindow : Window, IComponentConnector
{
	public OkTextLinkWindow(string _title, string _content, string _link)
	{
		InitializeComponent();
		txtTitle.LangKey = _title;
		txtContent.LangKey = _content;
		txtLink.Text = _link;
	}

	private void btnOkClick(object sender, RoutedEventArgs e)
	{
		this?.Close();
	}

	private void GoToBrowerPageClick(object sender, RoutedEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser(txtLink.Text);
	}
}
