using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.flash.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class RescueHomeViewV6 : UserControl, IComponentConnector
{
	public RescueHomeViewModelV6 VM { get; private set; }

	public RescueHomeViewV6()
	{
		InitializeComponent();
		VM = new RescueHomeViewModelV6(this);
		base.DataContext = VM;
	}

	private void OnBtnPhone(object sender, RoutedEventArgs e)
	{
		ChangeView(PageIndex.PHONE_ENTRANCE);
	}

	private void OnBtnTablet(object sender, RoutedEventArgs e)
	{
		ChangeView(PageIndex.TABLET_ENTRANCE);
	}

	private void OnBtnSmart(object sender, RoutedEventArgs e)
	{
		ChangeView(PageIndex.SMART_SEARCH);
	}

	private void ChangeView(PageIndex page)
	{
		MainFrameV6.Instance.ChangeView(page);
	}
}
