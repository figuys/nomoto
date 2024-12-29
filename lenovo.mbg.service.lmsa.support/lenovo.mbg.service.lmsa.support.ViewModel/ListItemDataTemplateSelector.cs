using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class ListItemDataTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (!(item is SupportSearchResultViewModel supportSearchResultViewModel))
		{
			return null;
		}
		_ = supportSearchResultViewModel.Brand;
		return (DataTemplate)((FrameworkElement)container).FindResource("lenovoDataTemplate");
	}
}
