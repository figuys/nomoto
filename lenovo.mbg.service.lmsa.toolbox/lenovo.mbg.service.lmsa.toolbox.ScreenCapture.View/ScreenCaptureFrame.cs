using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenCapture.View;

public partial class ScreenCaptureFrame : UserControl, IComponentConnector
{
	public ScreenCaptureFrame()
	{
		InitializeComponent();
	}

	private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
	{
	}
}
