using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.themes.generic.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class PhoneAMatchViewV6 : UserControl, IAMatchView, IComponentConnector
{
	public FrameworkElement ParentView { get; private set; }

	public AutoMatchViewModel VM { get; private set; }

	public FrameworkElement RescueView { get; set; }

	public PhoneAMatchViewV6()
	{
		InitializeComponent();
		VM = new PhoneAMatchViewModel(this);
		base.DataContext = VM;
	}

	public void Init(AutoMatchResource data, object wModel, FrameworkElement parentView)
	{
		ParentView = parentView;
		VM.Init(data, wModel as WarrantyInfoBaseModel);
	}

	private void OnBtnCopy(object sender, RoutedEventArgs e)
	{
		if (e.OriginalSource is Button { Tag: not null } button)
		{
			Clipboard.SetText(button.Tag.ToString());
		}
	}

	private void OnBtnFlash(object sender, RoutedEventArgs e)
	{
		VM.OnRescue();
	}

	private void OnHelpClick(object sender, RoutedEventArgs e)
	{
		Plugin.IMsgManager.ShowMutilTutorials(show: true);
	}
}
