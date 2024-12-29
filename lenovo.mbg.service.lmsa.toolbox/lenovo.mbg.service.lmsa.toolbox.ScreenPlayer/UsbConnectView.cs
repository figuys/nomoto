using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public partial class UsbConnectView : Window, IComponentConnector
{
	public UsbConnectViewModel VM { get; set; }

	public UsbConnectView()
	{
		InitializeComponent();
		VM = new UsbConnectViewModel(this);
		base.DataContext = VM;
	}
}
