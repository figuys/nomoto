using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class ColumnViewModel : NotifyBase
{
	private string _StepNoteText;

	private string _StepTitle;

	private string _stepContext1;

	private string _stepContext2;

	private string _stepContext3;

	private GridLength _widthRatio1;

	private GridLength _widthRatio2;

	private GridLength _widthRatio3;

	public bool IsClosePopup { get; set; }

	public string StepNoteText
	{
		get
		{
			return _StepNoteText;
		}
		set
		{
			_StepNoteText = value;
			OnPropertyChanged("StepNoteText");
		}
	}

	public Visibility StepNoteVisibility
	{
		get
		{
			if (!string.IsNullOrEmpty(_StepNoteText))
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
	}

	public string StepTitle
	{
		get
		{
			return _StepTitle;
		}
		set
		{
			_StepTitle = value;
			OnPropertyChanged("StepTitle");
		}
	}

	public string StepContext1
	{
		get
		{
			return _stepContext1;
		}
		set
		{
			_stepContext1 = value;
			OnPropertyChanged("StepContext1");
		}
	}

	public string StepImage1 { get; set; }

	public string StepContext2
	{
		get
		{
			return _stepContext2;
		}
		set
		{
			_stepContext2 = value;
			OnPropertyChanged("StepContext2");
		}
	}

	public string StepImage2 { get; set; }

	public string StepContext3
	{
		get
		{
			return _stepContext3;
		}
		set
		{
			_stepContext3 = value;
			OnPropertyChanged("StepContext3");
		}
	}

	public string StepImage3 { get; set; }

	public GridLength WidthRatio1
	{
		get
		{
			return _widthRatio1;
		}
		set
		{
			_widthRatio1 = value;
			OnPropertyChanged("WidthRatio1");
		}
	}

	public GridLength WidthRatio2
	{
		get
		{
			return _widthRatio2;
		}
		set
		{
			_widthRatio2 = value;
			OnPropertyChanged("WidthRatio2");
		}
	}

	public GridLength WidthRatio3
	{
		get
		{
			return _widthRatio3;
		}
		set
		{
			_widthRatio3 = value;
			OnPropertyChanged("WidthRatio3");
		}
	}

	public ColumnViewModel()
	{
		IsClosePopup = true;
	}
}
