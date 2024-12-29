using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class TipsItemViewModel : ViewModelBase
{
	private string _tips = string.Empty;

	private Brush _foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9b9b9b"));

	private double _fontSize = 15.0;

	public string Tips
	{
		get
		{
			return _tips;
		}
		set
		{
			if (!(_tips == value))
			{
				_tips = value;
				OnPropertyChanged("Tips");
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
}
