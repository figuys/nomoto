using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;

public class PlotSetModel : ViewModelBase
{
	private string _FontFamily;

	private bool _IsBold;

	private bool _IsItalic;

	private double _FontSize;

	private Color _Color;

	private double _LineWeight;

	public string FontFamily
	{
		get
		{
			return _FontFamily;
		}
		set
		{
			_FontFamily = value;
			OnPropertyChanged("FontFamily");
		}
	}

	public bool IsBold
	{
		get
		{
			return _IsBold;
		}
		set
		{
			_IsBold = value;
			OnPropertyChanged("IsBold");
		}
	}

	public bool IsItalic
	{
		get
		{
			return _IsItalic;
		}
		set
		{
			_IsItalic = value;
			OnPropertyChanged("IsItalic");
		}
	}

	public double FontSize
	{
		get
		{
			return _FontSize;
		}
		set
		{
			_FontSize = value;
			OnPropertyChanged("FontSize");
		}
	}

	public Color Color
	{
		get
		{
			return _Color;
		}
		set
		{
			_Color = value;
			OnPropertyChanged("Color");
		}
	}

	public double LineWeight
	{
		get
		{
			return _LineWeight;
		}
		set
		{
			_LineWeight = value;
			OnPropertyChanged("LineWeight");
		}
	}

	public PlotSetModel()
	{
		FontFamily = "Arial";
		IsBold = false;
		IsItalic = false;
		FontSize = 12.0;
		Color = Colors.Black;
		LineWeight = 1.0;
	}

	public PlotSetModel Clone()
	{
		return new PlotSetModel
		{
			LineWeight = LineWeight,
			FontFamily = FontFamily,
			IsBold = IsBold,
			IsItalic = IsItalic,
			FontSize = FontSize,
			Color = Color
		};
	}
}
