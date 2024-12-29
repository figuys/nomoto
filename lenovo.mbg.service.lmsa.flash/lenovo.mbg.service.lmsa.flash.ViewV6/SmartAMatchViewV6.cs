using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.themes.generic.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class SmartAMatchViewV6 : UserControl, IAMatchView, IComponentConnector
{
	public AutoMatchViewModel VM { get; private set; }

	public FrameworkElement ParentView { get; private set; }

	public FrameworkElement RescueView { get; set; }

	public SmartAMatchViewV6()
	{
		InitializeComponent();
		VM = new SmartAMatchViewModel(this);
		base.DataContext = VM;
	}

	public void Init(AutoMatchResource data, object wModel, FrameworkElement parentView)
	{
		ParentView = parentView;
		VM.Init(data, wModel as WarrantyInfoBaseModel);
	}

	private void OnBtnCopy(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText((e.OriginalSource as Button).Tag.ToString());
	}

	private void OnBtnFlash(object sender, RoutedEventArgs e)
	{
		VM.OnRescue();
	}
}
