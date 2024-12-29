using System;
using System.Windows;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class USBConnectHelperModel : ViewModelBase
{
	private bool isPhone;

	private int _Index;

	private string _Title;

	private string _AndroidVer7;

	private string _AndroidVer8;

	private string _AndroidVer10;

	private bool _IsNormalPhone;

	private string[] _NotArr;

	private Uri _TutorialImageUri;

	private Visibility _IsNoteVisible;

	private bool _IsBackBtnEnable;

	private bool _IsNextBtnEnable;

	public ReplayCommand DevTypeCommand { get; }

	public ReplayCommand PhoneCommand { get; }

	public ReplayCommand BtnCommand { get; }

	public ReplayCommand GifOverCommand { get; }

	public ReplayCommand GoStartCommand { get; }

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

	public USBConnectHelperModel()
	{
		DevTypeCommand = new ReplayCommand(DevTypeCommandHandler);
		PhoneCommand = new ReplayCommand(PhoneCommandHandler);
		GifOverCommand = new ReplayCommand(GifOverCommandHandler);
		BtnCommand = new ReplayCommand(BtnCommandHandler);
		GoStartCommand = new ReplayCommand(GoStartCommandHandler);
	}

	public override void LoadData(object data)
	{
		isPhone = true;
		IsNormalPhone = true;
		SetPhoneView();
		base.LoadData(data);
	}

	private void DevTypeCommandHandler(object data)
	{
		isPhone = Convert.ToBoolean(data as string);
		if (isPhone)
		{
			IsNormalPhone = true;
			SetPhoneView();
		}
		else
		{
			SetTabletView();
		}
	}

	private void PhoneCommandHandler(object data)
	{
		IsNormalPhone = Convert.ToBoolean(data as string);
		SetPhoneView();
	}

	private void GifOverCommandHandler(object data)
	{
		if (Index == 1 && !isPhone)
		{
			IsNoteVisible = ((!(bool)data) ? Visibility.Collapsed : Visibility.Visible);
		}
	}

	private void BtnCommandHandler(object data)
	{
		if (Convert.ToBoolean(data.ToString()))
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
	}

	private void GoStartCommandHandler(object data)
	{
		Context.Switch(ViewType.START);
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

	public void Initialize()
	{
		Index = 0;
		IsNoteVisible = Visibility.Collapsed;
		TutorialImageUri = UriArr[Index];
		IsBackBtnEnable = false;
		IsNextBtnEnable = true;
	}
}
