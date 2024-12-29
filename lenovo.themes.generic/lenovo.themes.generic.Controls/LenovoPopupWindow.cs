using System.Windows;
using System.Windows.Markup;

namespace lenovo.themes.generic.Controls;

public partial class LenovoPopupWindow : Window, IComponentConnector
{
	public LenovoPopupWindowModel WindowModel { get; set; }

	public LenovoPopupWindow()
	{
		InitializeComponent();
	}
}
