using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicInfoListViewModelV7 : ViewModelBase
{
	private Action<bool?> _selectionHandler;

	private ObservableCollection<PicInfoViewModelV7> _pics;

	private bool? _isListSelected = false;

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

	public ObservableCollection<PicInfoViewModelV7> Pics
	{
		get
		{
			return _pics;
		}
		set
		{
			if (_pics != value)
			{
				_pics = value;
				OnPropertyChanged("Pics");
			}
		}
	}

	public bool? IsListSelected
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

	public PicInfoListViewModelV7()
	{
		PicSelectAllCommand = new ReplayCommand(PicSelectAllCommandHandler);
		_pics = new ObservableCollection<PicInfoViewModelV7>();
		_pics.CollectionChanged += delegate(object s, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
			{
				PicCount = Pics.Count;
			}
		};
	}

	public void SetSelectionHandler(Action<bool?> selectionHandler)
	{
		_selectionHandler = selectionHandler;
	}

	public void SetPics(ObservableCollection<PicInfoViewModelV7> _setPics)
	{
		Pics = _setPics;
		PicCount = _setPics.Count;
	}

	public void SelectAll(bool selected)
	{
		foreach (PicInfoViewModelV7 pic in Pics)
		{
			pic.IsSelected = selected;
		}
	}

	private void PicSelectAllCommandHandler(object parameter)
	{
		bool value = (parameter as CheckBox).IsChecked.Value;
		SelectAll(value);
	}
}
