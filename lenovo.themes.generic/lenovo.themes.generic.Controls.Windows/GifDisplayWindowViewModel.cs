using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls.Windows;

public class GifDisplayWindowViewModel : NotifyBase
{
	private bool _IsBackBtnEnable;

	private bool _IsNextBtnEnable;

	private Visibility _IsMultiItemVisible;

	private Uri _TutorialImageUri;

	private string _GifNote;

	private string _Title;

	private int _Index;

	public ReplayCommand BtnCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

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

	public Visibility IsMultiItemVisible
	{
		get
		{
			return _IsMultiItemVisible;
		}
		set
		{
			_IsMultiItemVisible = value;
			OnPropertyChanged("IsMultiItemVisible");
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

	public string GifNote
	{
		get
		{
			return _GifNote;
		}
		set
		{
			_GifNote = value;
			OnPropertyChanged("GifNote");
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

	public Uri[] UriArr { get; set; }

	private string[] _NotArr { get; set; }

	public GifDisplayWindowViewModel(Window wnd)
	{
		GifDisplayWindowViewModel gifDisplayWindowViewModel = this;
		CloseCommand = new ReplayCommand(delegate
		{
			wnd.Close();
		});
		BtnCommand = new ReplayCommand(delegate(object param)
		{
			if (Convert.ToBoolean(param as string))
			{
				GifDisplayWindowViewModel gifDisplayWindowViewModel2 = gifDisplayWindowViewModel;
				Uri[] uriArr = gifDisplayWindowViewModel.UriArr;
				int num = gifDisplayWindowViewModel.Index + 1;
				gifDisplayWindowViewModel.Index = num;
				gifDisplayWindowViewModel2.TutorialImageUri = uriArr[num];
				gifDisplayWindowViewModel.GifNote = gifDisplayWindowViewModel._NotArr[gifDisplayWindowViewModel.Index];
				gifDisplayWindowViewModel.IsBackBtnEnable = true;
				gifDisplayWindowViewModel.IsNextBtnEnable = ((gifDisplayWindowViewModel.Index < gifDisplayWindowViewModel.UriArr.Length - 1) ? true : false);
			}
			else
			{
				GifDisplayWindowViewModel gifDisplayWindowViewModel3 = gifDisplayWindowViewModel;
				Uri[] uriArr2 = gifDisplayWindowViewModel.UriArr;
				int num = gifDisplayWindowViewModel.Index - 1;
				gifDisplayWindowViewModel.Index = num;
				gifDisplayWindowViewModel3.TutorialImageUri = uriArr2[num];
				gifDisplayWindowViewModel.GifNote = gifDisplayWindowViewModel._NotArr[gifDisplayWindowViewModel.Index];
				gifDisplayWindowViewModel.IsNextBtnEnable = true;
				gifDisplayWindowViewModel.IsBackBtnEnable = gifDisplayWindowViewModel.Index > 0;
			}
		});
	}

	public void Init(string title, Uri[] uriArr, string[] noteArr)
	{
		Index = 0;
		Title = title;
		UriArr = uriArr;
		_NotArr = noteArr;
		GifNote = _NotArr[Index];
		_TutorialImageUri = UriArr[Index];
		IsBackBtnEnable = false;
		IsNextBtnEnable = true;
		IsMultiItemVisible = ((uriArr.Length <= 1) ? Visibility.Hidden : Visibility.Visible);
	}
}
