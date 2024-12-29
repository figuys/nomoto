using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Dialog;

public class UsbDebugTutorialViewModel : NotifyBase
{
	private int _Index;

	private string _Title;

	private string _AndroidVer7;

	private string _AndroidVer8;

	private string _AndroidVer10;

	private bool isPhone;

	private bool _IsNormalPhone;

	private bool _IsBackBtnEnable;

	private bool _IsNextBtnEnable;

	private Uri _TutorialImageUri;

	private int _AndroidVerIndex;

	private Visibility _IsNoteVisible;

	private string[] _NotArr;

	public ReplayCommand BtnCommand { get; set; }

	public ReplayCommand RBtnCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public ReplayCommand GifOverCommand { get; set; }

	public ReplayCommand VersionCommand { get; set; }

	public ReplayCommand DevTypeCommand { get; set; }

	public ReplayCommand PhoneCommand { get; set; }

	public int Index
	{
		get
		{
			return _Index;
		}
		set
		{
			_Index = value;
			OnPropertyChanged("Index");
		}
	}

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

	public string AndroidVer7
	{
		get
		{
			return _AndroidVer7;
		}
		set
		{
			_AndroidVer7 = value;
			OnPropertyChanged("AndroidVer7");
		}
	}

	public string AndroidVer8
	{
		get
		{
			return _AndroidVer8;
		}
		set
		{
			_AndroidVer8 = value;
			OnPropertyChanged("AndroidVer8");
		}
	}

	public string AndroidVer10
	{
		get
		{
			return _AndroidVer10;
		}
		set
		{
			_AndroidVer10 = value;
			OnPropertyChanged("AndroidVer10");
		}
	}

	public bool IsNormalPhone
	{
		get
		{
			return _IsNormalPhone;
		}
		set
		{
			_IsNormalPhone = value;
			OnPropertyChanged("IsNormalPhone");
		}
	}

	public bool IsBackBtnEnable
	{
		get
		{
			return _IsBackBtnEnable;
		}
		set
		{
			_IsBackBtnEnable = value;
			OnPropertyChanged("IsBackBtnEnable");
		}
	}

	public bool IsNextBtnEnable
	{
		get
		{
			return _IsNextBtnEnable;
		}
		set
		{
			_IsNextBtnEnable = value;
			OnPropertyChanged("IsNextBtnEnable");
		}
	}

	public Uri TutorialImageUri
	{
		get
		{
			return _TutorialImageUri;
		}
		set
		{
			_TutorialImageUri = value;
			OnPropertyChanged("TutorialImageUri");
		}
	}

	public int AndroidVerIndex
	{
		get
		{
			return _AndroidVerIndex;
		}
		set
		{
			if (_AndroidVerIndex != value)
			{
				_AndroidVerIndex = value;
				OnPropertyChanged("AndroidVerIndex");
			}
		}
	}

	public Visibility IsNoteVisible
	{
		get
		{
			return _IsNoteVisible;
		}
		set
		{
			if (_IsNoteVisible != value)
			{
				_IsNoteVisible = value;
				OnPropertyChanged("IsNoteVisible");
			}
		}
	}

	public Uri[] UriArr { get; set; }

	public string[] NoteArr
	{
		get
		{
			return _NotArr;
		}
		set
		{
			_NotArr = value;
			OnPropertyChanged("NoteArr");
		}
	}

	public UsbDebugTutorialViewModel()
	{
		isPhone = true;
		IsNormalPhone = true;
		SetPhoneView();
		GifOverCommand = new ReplayCommand(delegate(object param)
		{
			if (Index == 1 && !isPhone)
			{
				IsNoteVisible = ((!(bool)param) ? Visibility.Collapsed : Visibility.Visible);
			}
		});
		BtnCommand = new ReplayCommand(delegate(object param)
		{
			if (Convert.ToBoolean(param.ToString()))
			{
				TutorialImageUri = UriArr[++Index];
				IsBackBtnEnable = true;
				IsNextBtnEnable = ((Index < UriArr.Length - 1) ? true : false);
			}
			else
			{
				TutorialImageUri = UriArr[--Index];
				IsNextBtnEnable = true;
				IsBackBtnEnable = Index > 0;
			}
			IsNoteVisible = Visibility.Collapsed;
		});
		CloseCommand = new ReplayCommand(delegate(object param)
		{
			Window obj = param as Window;
			obj.DialogResult = true;
			obj?.Close();
		});
		DevTypeCommand = new ReplayCommand(delegate(object param)
		{
			isPhone = Convert.ToBoolean(param as string);
			if (isPhone)
			{
				IsNormalPhone = true;
				SetPhoneView();
			}
			else
			{
				SetTabletView();
			}
		});
		PhoneCommand = new ReplayCommand(delegate(object param)
		{
			IsNormalPhone = Convert.ToBoolean(param as string);
			SetPhoneView();
		});
	}

	private void SetTabletView()
	{
		Title = "K1066";
		UriArr = new Uri[3]
		{
			new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet31.gif"),
			new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet32.gif"),
			new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet33.gif")
		};
		NoteArr = new string[3] { "K1102", "K1103", "K1107" };
		Initialize();
	}

	private void SetPhoneView()
	{
		if (IsNormalPhone)
		{
			Title = "K1066";
			UriArr = new Uri[3]
			{
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone11.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone12.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone13.gif")
			};
			NoteArr = new string[3] { "K1097", "K1098", "K1099" };
		}
		else
		{
			Title = "K1066";
			UriArr = new Uri[3]
			{
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone21.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone22.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone23.gif")
			};
			NoteArr = new string[3] { "K1100", "K1101", "K1108" };
		}
		Initialize();
	}

	public void Initialize()
	{
		Index = 0;
		IsNoteVisible = Visibility.Collapsed;
		TutorialImageUri = UriArr[Index];
		IsBackBtnEnable = false;
		IsNextBtnEnable = true;
	}
}
