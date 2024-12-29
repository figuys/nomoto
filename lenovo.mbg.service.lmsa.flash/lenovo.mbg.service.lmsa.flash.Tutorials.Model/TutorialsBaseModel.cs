using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.Model;

public class TutorialsBaseModel : ViewModelBase
{
	private bool isSelected;

	private string firstTitle;

	private string secondTitle;

	private string content;

	private Visibility radioTitleVisibility = Visibility.Collapsed;

	public ImageSource tipImage;

	public ImageSource tipImagePartDetail;

	public bool isMagnifyingGlass;

	private bool isManual;

	private bool haveSecondTitle;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public string FirstTitle
	{
		get
		{
			return firstTitle;
		}
		set
		{
			firstTitle = value;
			OnPropertyChanged("FirstTitle");
		}
	}

	public string SecondTitle
	{
		get
		{
			return secondTitle;
		}
		set
		{
			secondTitle = value;
			if (!string.IsNullOrEmpty(value))
			{
				HaveSecondTitle = true;
			}
			OnPropertyChanged("SecondTitle");
		}
	}

	public string Content
	{
		get
		{
			return content;
		}
		set
		{
			content = value;
			OnPropertyChanged("Content");
		}
	}

	public Visibility RadioTitleVisibility
	{
		get
		{
			return radioTitleVisibility;
		}
		set
		{
			radioTitleVisibility = value;
			OnPropertyChanged("RadioTitleVisibility");
		}
	}

	public ImageSource TipImage
	{
		get
		{
			return tipImage;
		}
		set
		{
			tipImage = value;
			OnPropertyChanged("TipImage");
		}
	}

	public ImageSource TipImagePartDetail
	{
		get
		{
			return tipImagePartDetail;
		}
		set
		{
			tipImagePartDetail = value;
			OnPropertyChanged("TipImagePartDetail");
		}
	}

	public bool IsMagnifyingGlass
	{
		get
		{
			return isMagnifyingGlass;
		}
		set
		{
			isMagnifyingGlass = value;
			OnPropertyChanged("IsMagnifyingGlass");
		}
	}

	public bool IsManual
	{
		get
		{
			return isManual;
		}
		set
		{
			isManual = value;
		}
	}

	public bool HaveSecondTitle
	{
		get
		{
			return haveSecondTitle;
		}
		set
		{
			haveSecondTitle = value;
			OnPropertyChanged("HaveSecondTitle");
		}
	}

	public TutorialDefineModel NextModel { get; set; }
}
