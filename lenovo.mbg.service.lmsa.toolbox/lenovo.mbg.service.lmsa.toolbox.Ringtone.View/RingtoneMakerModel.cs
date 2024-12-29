using System.Windows.Input;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.View;

public class RingtoneMakerModel : ViewModelBase
{
	private double _start;

	private double _end;

	private double _max;

	private double _current;

	private bool _isPlay;

	private string _mediaFile;

	private string _fileName;

	private bool _isMessage;

	private bool _isFileOk;

	private bool _isDevOnLine;

	private bool _isFadein;

	private bool _isFadeout;

	private double _playTime;

	public ICommand CloseCommand { get; set; }

	public ICommand StartCommand { get; set; }

	public ICommand SetEndCommand { get; set; }

	public ICommand PlayCommand { get; set; }

	public ICommand LoadCommand { get; set; }

	public ICommand SaveCommand { get; set; }

	public ICommand SelModelCommand { get; set; }

	public ICommand RingtoneCommand { get; set; }

	public ICommand IncreaseCommand { get; set; }

	public ICommand DecreaseCommand { get; set; }

	public double Start
	{
		get
		{
			return _start;
		}
		set
		{
			_start = value;
			OnPropertyChanged("Start");
		}
	}

	public double End
	{
		get
		{
			return _end;
		}
		set
		{
			_end = value;
			OnPropertyChanged("End");
		}
	}

	public double Max
	{
		get
		{
			return _max;
		}
		set
		{
			_max = value;
			OnPropertyChanged("Max");
		}
	}

	public double Current
	{
		get
		{
			return _current;
		}
		set
		{
			_current = value;
			OnPropertyChanged("Current");
		}
	}

	public bool IsPlay
	{
		get
		{
			return _isPlay;
		}
		set
		{
			_isPlay = value;
			OnPropertyChanged("IsPlay");
		}
	}

	public bool IsPause { get; set; }

	public string MediaFile
	{
		get
		{
			return _mediaFile;
		}
		set
		{
			_mediaFile = value;
			IsFileOk = !string.IsNullOrEmpty(_mediaFile);
			OnPropertyChanged("MediaFile");
		}
	}

	public string FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			_fileName = value;
			OnPropertyChanged("FileName");
		}
	}

	public bool IsMessage
	{
		get
		{
			return _isMessage;
		}
		set
		{
			_isMessage = value;
			OnPropertyChanged("IsMessage");
		}
	}

	public bool IsFileOk
	{
		get
		{
			return _isFileOk;
		}
		set
		{
			_isFileOk = value;
			OnPropertyChanged("IsFileOk");
		}
	}

	public bool IsDevOnLine
	{
		get
		{
			return _isDevOnLine;
		}
		set
		{
			_isDevOnLine = value;
			OnPropertyChanged("IsDevOnLine");
		}
	}

	public bool IsFadein
	{
		get
		{
			return _isFadein;
		}
		set
		{
			_isFadein = value;
			OnPropertyChanged("IsFadein");
		}
	}

	public bool IsFadeout
	{
		get
		{
			return _isFadeout;
		}
		set
		{
			_isFadeout = value;
			OnPropertyChanged("IsFadeout");
		}
	}

	public double PlayTime
	{
		get
		{
			return _playTime;
		}
		set
		{
			_playTime = value;
			OnPropertyChanged("PlayTime");
		}
	}

	public RingtoneMakerModel()
	{
		Start = 0.0;
		Current = 0.0;
		End = 0.0;
		Max = 0.0;
		PlayTime = 0.0;
		IsPlay = false;
		IsMessage = false;
		IsFadein = false;
		IsFadeout = false;
		CloseCommand = new RoutedCommand();
		StartCommand = new RoutedCommand();
		SetEndCommand = new RoutedCommand();
		PlayCommand = new RoutedCommand();
		LoadCommand = new RoutedCommand();
		SaveCommand = new RoutedCommand();
		SelModelCommand = new RoutedCommand();
		RingtoneCommand = new RoutedCommand();
		IncreaseCommand = new RoutedCommand();
		DecreaseCommand = new RoutedCommand();
	}
}
