using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public partial class MAHideNotifyView : Window, IComponentConnector
{
	public MAHideNotifyViewModel VM { get; private set; }

	public MAHideNotifyView()
	{
		InitializeComponent();
		VM = new MAHideNotifyViewModel(this);
		base.DataContext = VM;
	}
}
