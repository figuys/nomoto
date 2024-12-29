using System.Windows.Media;

namespace lenovo.themes.generic.ViewModelV6;

public class StepViewModel : ViewModelBase
{
	private bool isSelected;

	private string firstTitle;

	private string content;

	private ImageSource tipImage;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public string FirstTitle
	{
		get
		{
			return firstTitle;
		}
		set
		{
			firstTitle = value;
			OnPropertyChanged("FirstTitle");
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
			content = value;
			OnPropertyChanged("Content");
		}
	}

	public ImageSource TipImage
	{
		get
		{
			return tipImage;
		}
		set
		{
			tipImage = value;
			OnPropertyChanged("TipImage");
		}
	}
}