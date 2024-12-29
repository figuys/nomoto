using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public partial class WifiConnectView : Window, IComponentConnector
{
	public WifiConnectViewModel VM { get; set; }

	public WifiConnectView()
	{
		InitializeComponent();
		VM = new WifiConnectViewModel(this);
		base.DataContext = VM;
	}
}
