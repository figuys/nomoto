namespace lenovo.themes.generic.ViewModelV6;

public class ComboboxViewModel : ViewModelBase
{
	private object key;

	private string content;

	private bool isSelected;

	private bool isEnabled = true;

	public object Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
			OnPropertyChanged("Key");
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

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public object ExtendData { get; set; }
}
