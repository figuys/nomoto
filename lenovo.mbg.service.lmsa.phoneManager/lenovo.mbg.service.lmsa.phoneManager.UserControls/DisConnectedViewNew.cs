using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class DisConnectedViewNew : UserControl, IComponentConnector
{
	public DisConnectedViewNew()
	{
		InitializeComponent();
		DesignerProperties.GetIsInDesignMode(this);
		base.DataContext = DisConnectedViewModel.SingleInstance;
	}
}
