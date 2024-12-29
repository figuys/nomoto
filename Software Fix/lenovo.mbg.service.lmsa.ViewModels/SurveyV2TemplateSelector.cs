using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class SurveyV2TemplateSelector : DataTemplateSelector
{
	public DataTemplate SroceTemplate { get; set; }

	public DataTemplate SingleChoiceTemplate { get; set; }

	public DataTemplate MultipleChoiceTemplate { get; set; }

	public DataTemplate TextTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		QuestionItem questionItem = (QuestionItem)item;
		if (questionItem == null)
		{
			return null;
		}
		return questionItem.QuestionType switch
		{
			"Sroce" => SroceTemplate, 
			"SingleChoice" => SingleChoiceTemplate, 
			"MultipleChoice" => MultipleChoiceTemplate, 
			"Text" => TextTemplate, 
			_ => null, 
		};
	}
}
