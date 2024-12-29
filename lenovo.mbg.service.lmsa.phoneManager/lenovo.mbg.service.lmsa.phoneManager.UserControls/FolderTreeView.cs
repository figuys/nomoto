using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class FolderTreeView : UserControl, IComponentConnector, IStyleConnector
{
	public static readonly DependencyProperty ItemPaddingProperty = DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(FolderTreeView), new PropertyMetadata(null));

	public Thickness ItemPadding
	{
		get
		{
			return (Thickness)GetValue(ItemPaddingProperty);
		}
		set
		{
			SetValue(ItemPaddingProperty, value);
		}
	}

	public FolderTreeView()
	{
		InitializeComponent();
	}

	private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
	{
		if (sender is TreeViewItem treeViewItem)
		{
			treeViewItem.BringIntoView();
			e.Handled = true;
		}
	}
}
