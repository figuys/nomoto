using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackItemTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement frameworkElement = container as FrameworkElement;
		if (!(item is FeedbackItemViewModel feedbackItemViewModel) || frameworkElement == null)
		{
			return null;
		}
		string empty = string.Empty;
		empty = ((!"Q".Equals(feedbackItemViewModel.Type)) ? "feedbackForAnswer" : "feedbackForQuestion");
		return frameworkElement.FindResource(empty) as DataTemplate;
	}
}
