using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackSubContentItemViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private string dataType;

	private string content;

	public string DataType
	{
		get
		{
			return dataType;
		}
		set
		{
			if (!(dataType == value))
			{
				dataType = value;
				OnPropertyChanged("DataType");
			}
		}
	}

	public string Content
	{
		get
		{
			return content;
		}
		set
		{
			if (!(content == value))
			{
				content = value;
				OnPropertyChanged("Content");
			}
		}
	}
}
