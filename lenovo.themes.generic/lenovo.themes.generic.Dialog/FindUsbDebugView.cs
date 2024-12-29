using System.Windows;
using System.Windows.Markup;

namespace lenovo.themes.generic.Dialog;

public partial class FindUsbDebugView : Window, IComponentConnector
{
	public FindUsbDebugView(string image, string info)
	{
		InitializeComponent();
		base.DataContext = new FindUsbDebugViewMode(image, info);
	}
}
