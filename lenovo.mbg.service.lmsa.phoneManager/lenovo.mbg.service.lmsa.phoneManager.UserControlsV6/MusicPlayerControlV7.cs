using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class MusicPlayerControlV7 : UserControl, IComponentConnector
{
	public MusicPlayerControlV7()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			base.DataContext = MusicPlayerViewModelV7.SingleInstance;
			MusicPlayerViewModelV7.SingleInstance.InitializeView(this);
		};
	}
}
