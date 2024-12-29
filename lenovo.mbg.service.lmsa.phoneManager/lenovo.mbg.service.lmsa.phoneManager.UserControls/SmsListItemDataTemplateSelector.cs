using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class SmsListItemDataTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement frameworkElement = container as FrameworkElement;
		SMS sMS = item as SMS;
		if (!string.IsNullOrEmpty(sMS.type) && "1".Equals(sMS.type))
		{
			return frameworkElement.FindResource("receivedSmsDataTemplate") as DataTemplate;
		}
		return frameworkElement.FindResource("sendedSmsDataTemplate") as DataTemplate;
	}
}
