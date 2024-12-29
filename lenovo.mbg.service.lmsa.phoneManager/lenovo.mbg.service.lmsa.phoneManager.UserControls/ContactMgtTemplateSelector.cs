using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class ContactMgtTemplateSelector : DataTemplateSelector
{
	public DataTemplate DefaultRow { get; set; }

	public DataTemplate LastRow { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		ContactItemPhoneViewModel contactItemPhoneViewModel = (ContactItemPhoneViewModel)item;
		if (contactItemPhoneViewModel == null)
		{
			return null;
		}
		if (contactItemPhoneViewModel.IsLast)
		{
			return LastRow;
		}
		return DefaultRow;
	}
}
