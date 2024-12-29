using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ContactDetailItemTemplateSelectorV6 : DataTemplateSelector
{
	public DataTemplate RowTemplate { get; set; }

	public DataTemplate SubRowTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		ContactDetailListItemViewModelV6 contactDetailListItemViewModelV = (ContactDetailListItemViewModelV6)item;
		if (contactDetailListItemViewModelV == null)
		{
			return null;
		}
		if (contactDetailListItemViewModelV.IsTop)
		{
			return RowTemplate;
		}
		return SubRowTemplate;
	}
}
