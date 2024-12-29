using System.Collections.ObjectModel;
using System.Windows;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class SurveyDataModel
{
	public string Id { get; private set; }

	public Visibility IsRequired { get; set; }

	public string VerifyErrorText { get; set; }

	public string QuestionTitle { get; set; }

	public Visibility IsSatisfied { get; set; }

	public ObservableCollection<QuestionItem> QuestionItems { get; set; }

	public SurveyDataModel(string _id, string _questionTitle, ObservableCollection<QuestionItem> _questionContent, bool _required = false, string _errorText = "")
	{
		Id = _id;
		IsSatisfied = Visibility.Collapsed;
		IsRequired = ((!_required) ? Visibility.Collapsed : Visibility.Visible);
		VerifyErrorText = _errorText;
		QuestionTitle = _questionTitle;
		QuestionItems = _questionContent;
	}
}
