using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.common.Form.ViewModel;

public class FormItemVerifyWraningViewModel : ViewModelBase
{
	private object mWraningContent;

	private int mWraningCode;

	public object WraningContent
	{
		get
		{
			return mWraningContent;
		}
		set
		{
			if (mWraningContent != value)
			{
				mWraningContent = value;
				OnPropertyChanged("WraningContent");
			}
		}
	}

	public int WraningCode
	{
		get
		{
			return mWraningCode;
		}
		set
		{
			if (mWraningCode != value)
			{
				mWraningCode = value;
				OnPropertyChanged("WraningCode");
			}
		}
	}
}
