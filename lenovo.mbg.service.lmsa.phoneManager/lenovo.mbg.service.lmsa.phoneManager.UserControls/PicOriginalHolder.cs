using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class PicOriginalHolder : Window, IComponentConnector
{
	public PicOriginalHolder()
	{
		Screen screen = Screen.FromHandle(HostProxy.Host.HostMainWindowHandle);
		base.AllowsTransparency = true;
		base.WindowStyle = WindowStyle.None;
		base.Left = screen.WorkingArea.Left;
		base.Top = screen.WorkingArea.Top;
		base.Width = screen.Bounds.Width;
		base.Height = screen.Bounds.Height;
		base.Loaded += PicOriginalHolder_Loaded;
		InitializeComponent();
	}

	private void PicOriginalHolder_Loaded(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Maximized;
	}
}
