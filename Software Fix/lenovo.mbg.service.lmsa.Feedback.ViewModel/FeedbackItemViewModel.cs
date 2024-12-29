using System;
using System.Collections.Generic;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.Feedback.Business;
using lenovo.mbg.service.lmsa.Feedback.Converter;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackItemViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private bool isReplay;

	private List<FeedbackSubContentItemViewModel> feedbackContentItmes;

	private string title;

	private DateTime date;

	private long? id;

	private string type;

	private int? helpfulCode;

	private ReplayCommand helpfulCommand;

	private ReplayCommand noHelpfulCommand;

	private DataTemplateSelector dataTemplateSelector = new HelpfulTemplateSelector();

	public bool IsReplay
	{
		get
		{
			return isReplay;
		}
		set
		{
			if (isReplay != value)
			{
				isReplay = value;
				OnPropertyChanged("IsReplay");
			}
		}
	}

	public List<FeedbackSubContentItemViewModel> FeedbackContentItmes
	{
		get
		{
			return feedbackContentItmes;
		}
		set
		{
			if (feedbackContentItmes != value)
			{
				feedbackContentItmes = value;
				OnPropertyChanged("FeedbackContentItmes");
			}
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			if (!(title == value))
			{
				title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public DateTime Date
	{
		get
		{
			return date;
		}
		set
		{
			if (!(date == value))
			{
				date = value;
				OnPropertyChanged("Date");
			}
		}
	}

	public long? Id
	{
		get
		{
			return id;
		}
		set
		{
			if (id != value)
			{
				id = value;
				OnPropertyChanged("Id");
			}
		}
	}

	public string Type
	{
		get
		{
			return type;
		}
		set
		{
			if (!(type == value))
			{
				type = value;
				OnPropertyChanged("Type");
			}
		}
	}

	public int? HelpfulCode
	{
		get
		{
			return helpfulCode;
		}
		set
		{
			if (helpfulCode != value)
			{
				helpfulCode = value;
				OnPropertyChanged("HelpfulCode");
			}
		}
	}

	public ReplayCommand HelpfulCommand
	{
		get
		{
			return helpfulCommand;
		}
		set
		{
			if (helpfulCommand != value)
			{
				helpfulCommand = value;
				OnPropertyChanged("HelpfulCommand");
			}
		}
	}

	public ReplayCommand NoHelpfulCommand
	{
		get
		{
			return noHelpfulCommand;
		}
		set
		{
			if (noHelpfulCommand != value)
			{
				noHelpfulCommand = value;
				OnPropertyChanged("NoHelpfulCommand");
			}
		}
	}

	public DataTemplateSelector DataTemplateSelector
	{
		get
		{
			return dataTemplateSelector;
		}
		set
		{
			if (dataTemplateSelector != value)
			{
				dataTemplateSelector = value;
				OnPropertyChanged("DataTemplateSelector");
			}
		}
	}

	public FeedbackItemViewModel()
	{
		NoHelpfulCommand = new ReplayCommand(NoHelpfulCommandHandler);
		HelpfulCommand = new ReplayCommand(HelpfulCommandHandler);
	}

	private void HelpfulCommandHandler(object args)
	{
		SubmitHelpfulAsync(1);
	}

	private void NoHelpfulCommandHandler(object args)
	{
		SubmitHelpfulAsync(-1);
	}

	private async void SubmitHelpfulAsync(int helpfulCode)
	{
		if (await new FeedBackBLL().SubmitReplyIsHelpfull(Id, helpfulCode))
		{
			HelpfulCode = helpfulCode;
			DataTemplateSelector = new HelpfulTemplateSelector();
		}
	}
}
