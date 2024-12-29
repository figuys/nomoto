using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;

public class PlotModel : ViewModelBase
{
	private double _Zoom;

	private double _Scale;

	private BitmapSource _DisplayImage;

	private Cursor _Cursor;

	public double Zoom
	{
		get
		{
			return _Zoom;
		}
		set
		{
			_Zoom = value;
			Scale = _Zoom / 100.0;
			OnPropertyChanged("Zoom");
		}
	}

	public double Scale
	{
		get
		{
			return _Scale;
		}
		set
		{
			_Scale = value;
			OnPropertyChanged("Scale");
		}
	}

	public BitmapSource DisplayImage
	{
		get
		{
			return _DisplayImage;
		}
		set
		{
			_DisplayImage = value;
			OnPropertyChanged("DisplayImage");
		}
	}

	public Cursor Cursor
	{
		get
		{
			return _Cursor;
		}
		set
		{
			if (Cursor != value)
			{
				_Cursor = value;
				OnPropertyChanged("Cursor");
			}
		}
	}

	public string NewFile { get; set; }

	public string TempDir { get; set; }

	public PlotSetModel Settings { get; set; }

	public List<double> ZoomArr { get; set; }

	public List<double> LineArr { get; set; }

	public PlotModel()
	{
		Zoom = 100.0;
		Scale = 1.0;
		ZoomArr = new List<double>
		{
			10.0, 20.0, 30.0, 40.0, 50.0, 75.0, 100.0, 110.0, 120.0, 130.0,
			140.0, 150.0, 200.0, 300.0, 400.0, 500.0
		};
		LineArr = new List<double> { 1.0, 3.0, 5.0, 8.0 };
		Cursor = Cursors.Arrow;
		Settings = new PlotSetModel();
	}
}
