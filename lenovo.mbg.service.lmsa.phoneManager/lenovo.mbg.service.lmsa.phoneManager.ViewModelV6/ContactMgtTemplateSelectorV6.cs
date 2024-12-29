using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ContactMgtTemplateSelectorV6 : DataTemplateSelector
{
	public DataTemplate DefaultRow { get; set; }

	public DataTemplate LastRow { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		ContactItemPhoneViewModelV6 contactItemPhoneViewModelV = (ContactItemPhoneViewModelV6)item;
		if (contactItemPhoneViewModelV == null)
		{
			return null;
		}
		if (contactItemPhoneViewModelV.IsLast)
		{
			return LastRow;
		}
		return DefaultRow;
	}
}
