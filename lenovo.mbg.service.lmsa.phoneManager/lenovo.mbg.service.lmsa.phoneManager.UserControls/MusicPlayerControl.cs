using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class MusicPlayerControl : UserControl, IComponentConnector
{
	public MusicPlayerControl()
	{
		InitializeComponent();
		base.DataContext = MusicPlayerViewModel.SingleInstance;
		MusicPlayerViewModel.SingleInstance.InitializeView(this);
	}
}
