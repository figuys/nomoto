using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.ViewModels;

public class CustomDataTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (!string.IsNullOrEmpty(item as string))
		{
			return ComponentResources.SingleInstance.GetResource("defaultContentPresenterTemplateForTextBlock") as DataTemplate;
		}
		return base.SelectTemplate(item, container);
	}
}
