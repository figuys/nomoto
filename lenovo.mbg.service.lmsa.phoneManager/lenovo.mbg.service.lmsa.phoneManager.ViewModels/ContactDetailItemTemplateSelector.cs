using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ContactDetailItemTemplateSelector : DataTemplateSelector
{
	public DataTemplate RowTemplate { get; set; }

	public DataTemplate SubRowTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		ContactDetailListItemViewModel contactDetailListItemViewModel = (ContactDetailListItemViewModel)item;
		if (contactDetailListItemViewModel == null)
		{
			return null;
		}
		if (contactDetailListItemViewModel.IsTop)
		{
			return RowTemplate;
		}
		return SubRowTemplate;
	}
}
