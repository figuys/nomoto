using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Component.ProgressEx;

public class ProgressTipsItemViewModel : ViewModelBase
{
	private string _message = string.Empty;

	private Brush _foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#494949"));

	private double _fontSize = 15.0;

	public string Message
	{
		get
		{
			return _message;
		}
		set
		{
			if (!(_message == value))
			{
				_message = value;
				OnPropertyChanged("Message");
			}
		}
	}

	public Brush Foreground
	{
		get
		{
			return _foreground;
		}
		set
		{
			if (_foreground != value)
			{
				_foreground = value;
				OnPropertyChanged("Foreground");
			}
		}
	}

	public double FontSize
	{
		get
		{
			return _fontSize;
		}
		set
		{
			if (_fontSize != value)
			{
				_fontSize = value;
				OnPropertyChanged("FontSize");
			}
		}
	}

	public object Tag { get; set; }
}
