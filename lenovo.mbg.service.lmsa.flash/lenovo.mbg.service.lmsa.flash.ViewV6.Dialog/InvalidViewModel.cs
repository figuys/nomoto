using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class InvalidViewModel : NotifyBase
{
	private string _LeftBtnImage;

	private string _RightBtnImage;

	private string _LeftBtnText;

	private string _RightBtnText;

	private string _LeftLabelText;

	private string _RightLabelText;

	public string LeftBtnImage
	{
		get
		{
			return _LeftBtnImage;
		}
		set
		{
			_LeftBtnImage = value;
			OnPropertyChanged("LeftBtnImage");
		}
	}

	public string RightBtnImage
	{
		get
		{
			return _RightBtnImage;
		}
		set
		{
			_RightBtnImage = value;
			OnPropertyChanged("RightBtnImage");
		}
	}

	public string LeftBtnText
	{
		get
		{
			return _LeftBtnText;
		}
		set
		{
			_LeftBtnText = value;
			OnPropertyChanged("LeftBtnText");
		}
	}

	public string RightBtnText
	{
		get
		{
			return _RightBtnText;
		}
		set
		{
			_RightBtnText = value;
			OnPropertyChanged("RightBtnText");
		}
	}

	public string LeftLabelText
	{
		get
		{
			return _LeftLabelText;
		}
		set
		{
			_LeftLabelText = value;
			OnPropertyChanged("LeftLabelText");
		}
	}

	public string RightLabelText
	{
		get
		{
			return _RightLabelText;
		}
		set
		{
			_RightLabelText = value;
			OnPropertyChanged("RightLabelText");
		}
	}
}
