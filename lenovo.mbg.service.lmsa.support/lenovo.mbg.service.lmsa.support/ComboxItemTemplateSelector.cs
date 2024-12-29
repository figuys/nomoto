using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.support;

public class ComboxItemTemplateSelector : DataTemplateSelector
{
	private DataTemplate SelectionTemplate { get; set; }

	private DataTemplate DropDownTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (VisualTreeHelper.GetParent(container) is ContentPresenter)
		{
			if (DropDownTemplate == null)
			{
				DropDownTemplate = ((FrameworkElement)container).FindResource("dropdownSelectionItemDataTemplate") as DataTemplate;
			}
			return DropDownTemplate;
		}
		if (SelectionTemplate == null)
		{
			SelectionTemplate = ((FrameworkElement)container).FindResource("SelectionBoxItemDataTemplate") as DataTemplate;
		}
		return SelectionTemplate;
	}
}
