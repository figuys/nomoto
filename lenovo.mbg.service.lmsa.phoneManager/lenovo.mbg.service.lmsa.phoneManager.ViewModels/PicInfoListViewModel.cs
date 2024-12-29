using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PicInfoListViewModel : ViewModelBase
{
	private Action<bool> _selectionHandler;

	private ObservableCollection<PicInfoViewModel> _pics;

	private volatile bool _isListSelected;

	private string _groupKey = string.Empty;

	private long _picCount;

	public int DataVersion { get; set; }

	public double HeadExtendHeight => 30.0;

	public double ItemExtendHeight => 115.0;

	public double ItemExtendWidth => 115.0;

	public double BodyExtendHeight
	{
		get
		{
			int num = (int)(BodyExtendWidth / ItemExtendWidth);
			int count = Pics.Count;
			return (double)(count / num + ((count % num > 0) ? 1 : 0)) * ItemExtendHeight;
		}
	}

	public double BodyExtendWidth
	{
		get
		{
			double i = 0.0;
			if (Thread.CurrentThread.IsBackground)
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					i = Application.Current.MainWindow.ActualWidth - 200.0;
				});
			}
			else
			{
				i = Application.Current.MainWindow.ActualWidth - 200.0;
			}
			return i;
		}
	}

	public double VisibleAreaHeight
	{
		get
		{
			double i = 0.0;
			if (Thread.CurrentThread.IsBackground)
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					i = Application.Current.MainWindow.Height - 200.0;
				});
			}
			else
			{
				i = Application.Current.MainWindow.Height - 200.0;
			}
			return i;
		}
	}

	public int NumberOfVisibleAreaImages
	{
		get
		{
			int num = (int)(VisibleAreaHeight / ItemExtendHeight) + ((VisibleAreaHeight % ItemExtendHeight > 0.0) ? 1 : 0);
			int num2 = (int)(BodyExtendWidth / ItemExtendWidth) + ((BodyExtendWidth % ItemExtendWidth > 0.0) ? 1 : 0);
			return num * num2;
		}
	}

	public ObservableCollection<PicInfoViewModel> Pics => _pics;

	public bool IsListSelected
	{
		get
		{
			return _isListSelected;
		}
		set
		{
			_isListSelected = value;
			_selectionHandler?.BeginInvoke(_isListSelected, null, null);
			OnPropertyChanged("IsListSelected");
		}
	}

	public ReplayCommand PicSelectAllCommand { get; set; }

	public string GroupKey
	{
		get
		{
			return _groupKey;
		}
		set
		{
			if (!(_groupKey == value))
			{
				_groupKey = value;
				OnPropertyChanged("GroupKey");
			}
		}
	}

	public long PicCount
	{
		get
		{
			return _picCount;
		}
		set
		{
			if (_picCount != value)
			{
				_picCount = value;
				OnPropertyChanged("PicCount");
			}
		}
	}

	public PicInfoListViewModel(Action<bool> selectionHandler)
	{
		_selectionHandler = selectionHandler;
		PicSelectAllCommand = new ReplayCommand(PicSelectAllCommandHandler);
		_pics = new ObservableCollection<PicInfoViewModel>();
		_pics.CollectionChanged += delegate(object s, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
			{
				PicCount = Pics.Count;
			}
		};
	}

	public void SelectAll(bool selected)
	{
		foreach (PicInfoViewModel pic in Pics)
		{
			pic.IsSelected = selected;
		}
	}

	private void PicSelectAllCommandHandler(object parameter)
	{
		bool value = (parameter as CheckBox).IsChecked.Value;
		SelectAll(value);
	}

	public void PicSclectionHandler(bool result)
	{
		ObservableCollection<PicInfoViewModel> pics = Pics;
		if (pics == null || pics.Count == 0)
		{
			IsListSelected = false;
			return;
		}
		IsListSelected = pics.Where((PicInfoViewModel m) => !m.IsSelected).FirstOrDefault() == null;
	}
}
