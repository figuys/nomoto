using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using lenovo.themes.generic.Component;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public abstract class PageLoadingViewModelBase<TModel, TId> : ViewModelBase where TModel : PageLoadingItemViewModelBase<TId>, new()
{
	protected readonly Dictionary<TId, TModel> cache = new Dictionary<TId, TModel>();

	private ObservableCollection<TModel> dataItemsSource;

	private int totalCount;

	private volatile int currentLoadingStartIndex;

	private volatile int currentLoadingPageSize;

	private long mDataVersion;

	private SingleSyncTask bigSingleSyncTask;

	public Dispatcher CurrentDispatcher { get; private set; }

	public ObservableCollection<TModel> DataItemsSource
	{
		get
		{
			return dataItemsSource;
		}
		set
		{
			if (dataItemsSource != value)
			{
				dataItemsSource = value;
				OnPropertyChanged("DataItemsSource");
			}
		}
	}

	public int TotalCount
	{
		get
		{
			return totalCount;
		}
		set
		{
			if (totalCount != value)
			{
				totalCount = value;
				OnPropertyChanged("TotalCount");
			}
		}
	}

	protected int CurrentLoadingStartIndex
	{
		get
		{
			return currentLoadingStartIndex;
		}
		set
		{
			currentLoadingStartIndex = value;
		}
	}

	protected int CurrentLoadingPageSize
	{
		get
		{
			return currentLoadingPageSize;
		}
		set
		{
			currentLoadingPageSize = value;
		}
	}

	public long CurrentDataVersion => Interlocked.Read(ref mDataVersion);

	public PageLoadingViewModelBase()
	{
		DataItemsSource = new ObservableCollection<TModel>();
		CurrentDispatcher = Dispatcher.CurrentDispatcher;
		bigSingleSyncTask = new SingleSyncTask();
	}

	protected abstract int PrepareTotalCount();

	protected abstract IEnumerable<TId> PrepareIdList();

	protected internal void ClearPropertyLoadIdentification()
	{
		foreach (KeyValuePair<TId, TModel> item in cache)
		{
			item.Value.BasicPropertyIsLoaded = false;
			item.Value.BigPropertyIsLoaded = false;
		}
	}

	public override void Reset()
	{
		base.Reset();
		dataItemsSource?.Clear();
		cache.Clear();
	}

	protected long IncrementDataVersion()
	{
		return Interlocked.Increment(ref mDataVersion);
	}

	protected void ZeroDataVersion()
	{
		Interlocked.Exchange(ref mDataVersion, 0L);
	}

	public virtual void BeginLoadIdList(Action<object, Exception> callback)
	{
		Dictionary<string, object> startLoadingTag = new Dictionary<string, object>();
		OnStartLoadingId(startLoadingTag);
		DoProcessAsync(delegate
		{
			try
			{
				IEnumerable<TId> idList = PrepareIdList();
				UpdateDataCollection(idList);
				callback?.Invoke(null, null);
			}
			catch (Exception arg)
			{
				callback?.Invoke(null, arg);
			}
			finally
			{
				OnStopLoadingId(startLoadingTag);
			}
		}, null);
	}

	protected virtual void UpdateDataCollection(IEnumerable<TId> idList)
	{
		if (idList == null)
		{
			return;
		}
		IEnumerable<TModel> news = null;
		IEnumerable<TModel> removed = null;
		if (idList.Count() == 0)
		{
			cache.Clear();
			CurrentDispatcher.Invoke(delegate
			{
				DataItemsSource.Clear();
			});
		}
		else
		{
			long dataVersion = IncrementDataVersion();
			if (cache.Count == 0)
			{
				foreach (TId id in idList)
				{
					TModel val = new TModel
					{
						Id = id,
						IdIsLoaded = true,
						DataVersion = dataVersion
					};
					OnItemInitialized(val);
					cache.Add(id, val);
				}
				news = cache.Values;
			}
			else
			{
				List<TModel> list = new List<TModel>();
				List<TModel> list2 = new List<TModel>();
				foreach (KeyValuePair<TId, TModel> item in cache.Where((KeyValuePair<TId, TModel> m) => idList.Contains(m.Key)))
				{
					item.Value.DataVersion = dataVersion;
				}
				foreach (KeyValuePair<TId, TModel> item2 in cache.Where((KeyValuePair<TId, TModel> m) => m.Value.DataVersion != dataVersion).ToList())
				{
					list2.Add(item2.Value);
					cache.Remove(item2.Key);
				}
				foreach (TId item3 in idList.Where((TId m) => !cache.Keys.Contains(m)))
				{
					TModel val2 = new TModel
					{
						Id = item3,
						IdIsLoaded = true,
						DataVersion = dataVersion
					};
					list.Add(val2);
					cache.Add(item3, val2);
					OnItemInitialized(val2);
				}
				news = list;
				removed = list2;
			}
		}
		if (news != null || removed != null)
		{
			CurrentDispatcher.Invoke(delegate
			{
				SyncUIDataFromCache(news, removed);
			});
		}
	}

	protected virtual void OnItemInitialized(TModel model)
	{
	}

	protected virtual void BeginLoadBasicPropertyData(Action<object, Exception> callback)
	{
		if (currentLoadingStartIndex < 0 || CurrentLoadingPageSize <= 0)
		{
			return;
		}
		List<TModel> list = (from m in DataItemsSource.Skip(CurrentLoadingStartIndex).Take(currentLoadingPageSize)
			where m.IdIsLoaded && !m.BasicPropertyIsLoaded
			select m).ToList();
		if (list.Count() <= 0)
		{
			return;
		}
		DoProcessAsync(delegate(object target)
		{
			try
			{
				IEnumerable<TModel> target2 = target as IEnumerable<TModel>;
				FillBasicProperty(target2);
				callback?.Invoke(null, null);
			}
			catch (Exception arg)
			{
				callback?.Invoke(null, arg);
			}
		}, list);
	}

	protected virtual void BeginLoadBigPropertyData(Action<object, Exception> callback)
	{
		if (currentLoadingStartIndex < 0 || CurrentLoadingPageSize <= 0)
		{
			return;
		}
		BeginLoadBasicPropertyData(null);
		List<TModel> list = (from m in DataItemsSource.Skip(CurrentLoadingStartIndex).Take(currentLoadingPageSize)
			where m.IdIsLoaded && !m.BigPropertyIsLoaded
			select m).ToList();
		if (list.Count() > 0)
		{
			bigSingleSyncTask.Load(list, delegate(IEnumerable<TModel> inputDataList, CancellationTokenSource cancel)
			{
				FillBigProperty(inputDataList, cancel);
			}, delegate
			{
			});
		}
	}

	protected abstract bool FillBasicProperty(IEnumerable<TModel> target);

	protected abstract bool FillBigProperty(IEnumerable<TModel> target, CancellationTokenSource cancel);

	protected virtual void SyncUIDataFromCache(IEnumerable<TModel> news, IEnumerable<TModel> removed)
	{
		if (news != null && news.Count() > 0)
		{
			foreach (TModel item in news)
			{
				DataItemsSource.Insert(0, item);
			}
		}
		if (removed == null || removed.Count() <= 0)
		{
			return;
		}
		foreach (TModel item2 in removed)
		{
			dataItemsSource.Remove(item2);
		}
	}

	protected void DoProcessAsync(Action<object> task, object param)
	{
		if (task != null)
		{
			Task.Factory.StartNew(delegate
			{
				task(param);
			});
		}
	}

	protected virtual void OnStartLoadingId(Dictionary<string, object> tag)
	{
	}

	protected virtual void OnStopLoadingId(Dictionary<string, object> tag)
	{
	}
}
