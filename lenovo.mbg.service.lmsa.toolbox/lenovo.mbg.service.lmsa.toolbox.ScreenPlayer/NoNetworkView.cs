using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public partial class NoNetworkView : Window, IComponentConnector
{
	public NoNetworkViewModel VM { get; set; }

	public NoNetworkView()
	{
		InitializeComponent();
		VM = new NoNetworkViewModel(this);
		base.DataContext = VM;
	}
}
