using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class BackupCategoryModel : BaseNotify
{
	private BitmapImage _ChooseImageUrl;

	private BitmapImage _UnChooseImageUrl;

	private string _ImageTitle;

	private int _Order;

	private int _TotalCount;

	private int _FinishedCount = -1;

	public BitmapImage ChooseImageUrl
	{
		get
		{
			return _ChooseImageUrl;
		}
		set
		{
			_ChooseImageUrl = value;
			OnPropertyChanged("ChooseImageUrl");
		}
	}

	public BitmapImage UnChooseImageUrl
	{
		get
		{
			return _UnChooseImageUrl;
		}
		set
		{
			_UnChooseImageUrl = value;
			OnPropertyChanged("UnChooseImageUrl");
		}
	}

	public string ImageTitle
	{
		get
		{
			return _ImageTitle;
		}
		set
		{
			_ImageTitle = value;
			OnPropertyChanged("ImageTitle");
		}
	}

	public int Order
	{
		get
		{
			return _Order;
		}
		set
		{
			_Order = value;
			OnPropertyChanged("Order");
		}
	}

	public int TotalCount
	{
		get
		{
			return _TotalCount;
		}
		set
		{
			_TotalCount = value;
			OnPropertyChanged("TotalCount");
		}
	}

	public int FinishedCount
	{
		get
		{
			return _FinishedCount;
		}
		set
		{
			_FinishedCount = value;
			OnPropertyChanged("FinishedCount");
		}
	}
}
