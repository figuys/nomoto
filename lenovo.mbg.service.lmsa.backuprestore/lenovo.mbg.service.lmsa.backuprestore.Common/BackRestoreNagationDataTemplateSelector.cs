using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;

namespace lenovo.mbg.service.lmsa.backuprestore.Common;

public class BackRestoreNagationDataTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement frameworkElement = container as FrameworkElement;
		if ((item as NavgationItemViewModel).Index == 0)
		{
			return frameworkElement.FindResource("leftRadiusNagativeDataTemplate") as DataTemplate;
		}
		return frameworkElement.FindResource("NormalNagativeDataTemplate") as DataTemplate;
	}
}
