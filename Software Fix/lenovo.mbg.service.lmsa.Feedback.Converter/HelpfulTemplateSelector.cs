using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.Feedback.ViewModel;

namespace lenovo.mbg.service.lmsa.Feedback.Converter;

public class HelpfulTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement frameworkElement = container as FrameworkElement;
		if (!(item is FeedbackItemViewModel feedbackItemViewModel) || frameworkElement == null)
		{
			return frameworkElement.FindResource("helpfulEmptyDataTemplate") as DataTemplate;
		}
		int? helpfulCode = feedbackItemViewModel.HelpfulCode;
		if (!helpfulCode.HasValue)
		{
			return frameworkElement.FindResource("helpfulEmptyDataTemplate") as DataTemplate;
		}
		string resourceKey = string.Empty;
		switch (helpfulCode.Value)
		{
		case -1:
			resourceKey = "helpfulNoDataTemplate";
			break;
		case 0:
			resourceKey = "helpfulReadyOptionDataTemplate";
			break;
		case 1:
			resourceKey = "helpfulYesDataTemplate";
			break;
		}
		return frameworkElement.FindResource(resourceKey) as DataTemplate;
	}
}
