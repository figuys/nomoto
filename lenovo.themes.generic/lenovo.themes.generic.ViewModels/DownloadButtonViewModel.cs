using System.Windows;
using System.Windows.Media;

namespace lenovo.themes.generic.ViewModels;

public class DownloadButtonViewModel : ButtonViewModel
{
	private Visibility m_DownloadIconShow;

	public object m_DownloadContent = "K0100";

	private bool _isDownloading;

	private Brush _topBackgroundBrush;

	private Brush _topMouseOverBackgroundBrush;

	private Brush _topDisabledBackgroundBrush;

	private Brush _bottomBackgroundBrush;

	private Brush _bottomMouseOverBackgroundBrush;

	private Brush _bottomDisabledBackgroundBrush;

	public Visibility DownloadIconShow
	{
		get
		{
			return m_DownloadIconShow;
		}
		set
		{
			m_DownloadIconShow = value;
			OnPropertyChanged("DownloadIconShow");
		}
	}

	public object DownloadContent
	{
		get
		{
			return m_DownloadContent;
		}
		set
		{
			m_DownloadContent = value;
			OnPropertyChanged("DownloadContent");
		}
	}

	public bool IsDownloading
	{
		get
		{
			return _isDownloading;
		}
		set
		{
			if (_isDownloading != value)
			{
				_isDownloading = value;
				if (!_isDownloading)
				{
					DownloadContent = "K0100";
					DownloadIconShow = Visibility.Visible;
				}
				else
				{
					DownloadContent = "K0101";
					DownloadIconShow = Visibility.Collapsed;
				}
				OnPropertyChanged("IsDownloading");
			}
		}
	}

	public Brush TopBackgroundBrush
	{
		get
		{
			return _topBackgroundBrush;
		}
		set
		{
			if (_topBackgroundBrush != value)
			{
				_topBackgroundBrush = value;
				OnPropertyChanged("TopBackgroundBrush");
			}
		}
	}

	public Brush TopMouseOverBackgroundBrush
	{
		get
		{
			return _topMouseOverBackgroundBrush;
		}
		set
		{
			if (_topMouseOverBackgroundBrush != value)
			{
				_topMouseOverBackgroundBrush = value;
				OnPropertyChanged("TopMouseOverBackgroundBrush");
			}
		}
	}

	public Brush TopDisabledBackgroundBrush
	{
		get
		{
			return _topDisabledBackgroundBrush;
		}
		set
		{
			if (_topDisabledBackgroundBrush != value)
			{
				_topDisabledBackgroundBrush = value;
				OnPropertyChanged("TopDisabledBackgroundBrush");
			}
		}
	}

	public Brush BottomBackgroundBrush
	{
		get
		{
			return _bottomBackgroundBrush;
		}
		set
		{
			if (_bottomBackgroundBrush != value)
			{
				_bottomBackgroundBrush = value;
				OnPropertyChanged("BottomBackgroundBrush");
			}
		}
	}

	public Brush BottomMouseOverBackgroundBrush
	{
		get
		{
			return _bottomMouseOverBackgroundBrush;
		}
		set
		{
			if (_bottomMouseOverBackgroundBrush != value)
			{
				_bottomMouseOverBackgroundBrush = value;
				OnPropertyChanged("BottomMouseOverBackgroundBrush");
			}
		}
	}

	public Brush BottomDisabledBackgroundBrush
	{
		get
		{
			return _bottomDisabledBackgroundBrush;
		}
		set
		{
			if (_bottomDisabledBackgroundBrush != value)
			{
				_bottomDisabledBackgroundBrush = value;
				OnPropertyChanged("BottomDisabledBackgroundBrush");
			}
		}
	}
}
