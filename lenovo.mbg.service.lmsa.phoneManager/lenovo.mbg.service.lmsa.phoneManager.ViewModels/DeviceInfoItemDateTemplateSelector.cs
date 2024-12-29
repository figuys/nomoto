using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class DeviceInfoItemDateTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement frameworkElement = container as FrameworkElement;
		if (item is DeviceInfoItemViewModel deviceInfoItemViewModel && frameworkElement != null)
		{
			string dateTemplateTag = deviceInfoItemViewModel.DateTemplateTag;
			if (!(dateTemplateTag == "Battery"))
			{
				if (dateTemplateTag == "Storage")
				{
					return frameworkElement.FindResource("storageDateTemplate") as DataTemplate;
				}
				return frameworkElement.FindResource("stringDateTemplate") as DataTemplate;
			}
			return frameworkElement.FindResource("batteryDateTemplate") as DataTemplate;
		}
		return base.SelectTemplate(item, container);
	}
}
