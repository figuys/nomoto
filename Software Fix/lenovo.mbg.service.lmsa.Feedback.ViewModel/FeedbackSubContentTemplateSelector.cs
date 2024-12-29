using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackSubContentTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement frameworkElement = container as FrameworkElement;
		if (!(item is FeedbackSubContentItemViewModel feedbackSubContentItemViewModel) || frameworkElement == null)
		{
			return null;
		}
		string empty = string.Empty;
		empty = ((!"img".Equals(feedbackSubContentItemViewModel.DataType)) ? "feedbackSubContentForString" : "feedbackSubContentForImage");
		return frameworkElement.FindResource(empty) as DataTemplate;
	}
}
