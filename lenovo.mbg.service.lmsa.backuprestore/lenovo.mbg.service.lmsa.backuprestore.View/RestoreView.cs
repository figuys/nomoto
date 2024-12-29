using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class RestoreView : UserControl, IComponentConnector
{
	public RestoreView()
	{
		InitializeComponent();
	}

	private void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
	}

	private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		(base.DataContext as RestoreViewModel).FireCategorySelectionChanged();
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		grid1.Visibility = Visibility.Collapsed;
	}
}
