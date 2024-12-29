using System.Windows;
using System.Windows.Markup;

namespace lenovo.themes.generic.ControlsV6;

public partial class ItemRequestBringIntoView : ResourceDictionary, IComponentConnector, IStyleConnector
{
	private void Item_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
	{
		e.Handled = true;
	}
}
