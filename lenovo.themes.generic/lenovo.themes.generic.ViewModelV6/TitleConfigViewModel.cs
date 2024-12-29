using System.Windows.Media;
using lenovo.mbg.service.framework.lang;

namespace lenovo.themes.generic.ViewModelV6;

public class TitleConfigViewModel : ViewModelBase
{
	private string _Title;

	private int _PositionStart;

	private int _PositionCount;

	private Brush _Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2393B9"));

	public string Title
	{
		get
		{
			return _Title;
		}
		set
		{
			_Title = value;
			OnPropertyChanged("Title");
		}
	}

	public int PositionStart
	{
		get
		{
			return _PositionStart;
		}
		set
		{
			_PositionStart = value;
			OnPropertyChanged("PositionStart");
		}
	}

	public int PositionCount
	{
		get
		{
			return _PositionCount;
		}
		set
		{
			_PositionCount = value;
			OnPropertyChanged("PositionCount");
		}
	}

	public Brush Foreground
	{
		get
		{
			return _Foreground;
		}
		set
		{
			_Foreground = value;
			OnPropertyChanged("Foreground");
		}
	}

	public TitleConfigViewModel(string title, string foreground = null)
		: this(title, null, foreground)
	{
	}

	public TitleConfigViewModel(string title, string modelName, string foreground)
	{
		if (!string.IsNullOrEmpty(modelName))
		{
			Title = string.Format(LangTranslation.Translate(title), modelName);
			PositionStart = Title.IndexOf(modelName);
			PositionCount = modelName.Length;
		}
		else
		{
			Title = title;
		}
		if (!string.IsNullOrEmpty(foreground))
		{
			Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foreground));
		}
	}
}
