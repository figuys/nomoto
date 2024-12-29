namespace lenovo.themes.generic.ViewModelV6;

public class NormalPermissionViewModel : ViewModelBase
{
	private string _Text;

	private string _SubText;

	public string Text
	{
		get
		{
			return _Text;
		}
		set
		{
			_Text = value;
			OnPropertyChanged("Text");
		}
	}

	public string SubText
	{
		get
		{
			return _SubText;
		}
		set
		{
			_SubText = value;
			OnPropertyChanged("SubText");
		}
	}

	public NormalPermissionViewModel(string text, string subText)
	{
		Text = text;
		SubText = subText;
	}
}
