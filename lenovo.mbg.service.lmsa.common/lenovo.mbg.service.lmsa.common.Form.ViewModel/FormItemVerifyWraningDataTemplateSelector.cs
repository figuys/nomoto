using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.common.Form.ViewModel;

public class FormItemVerifyWraningDataTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item == null || container == null)
		{
			return new DataTemplate();
		}
		FormItemVerifyWraningViewModel formItemVerifyWraningViewModel = item as FormItemVerifyWraningViewModel;
		FrameworkElement frameworkElement = container as FrameworkElement;
		if (formItemVerifyWraningViewModel == null)
		{
			return (DataTemplate)frameworkElement.FindResource("emptyWraningDataTemplate");
		}
		return formItemVerifyWraningViewModel.WraningCode switch
		{
			1 => (DataTemplate)frameworkElement.FindResource("errorWraningTextDataTemplate"), 
			2 => (DataTemplate)frameworkElement.FindResource("correctWraningImageDataTemplate"), 
			3 => (DataTemplate)frameworkElement.FindResource("errorWraningProgressDataTemplate"), 
			_ => (DataTemplate)frameworkElement.FindResource("emptyWraningDataTemplate"), 
		};
	}
}
