using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class HelpWindowItemViewModel : ViewModelBase
{
	private object _question;

	private object _answer;

	private object _answerLabel;

	public object Question
	{
		get
		{
			return _question;
		}
		set
		{
			if (_question != value)
			{
				_question = value;
				OnPropertyChanged("Question");
			}
		}
	}

	public object Answer
	{
		get
		{
			return _answer;
		}
		set
		{
			if (_answer != value)
			{
				_answer = value;
				OnPropertyChanged("Answer");
			}
		}
	}

	public object AnswerLabel
	{
		get
		{
			return _answerLabel;
		}
		set
		{
			if (_answerLabel != value)
			{
				_answerLabel = value;
				OnPropertyChanged("AnswerLabel");
			}
		}
	}
}
