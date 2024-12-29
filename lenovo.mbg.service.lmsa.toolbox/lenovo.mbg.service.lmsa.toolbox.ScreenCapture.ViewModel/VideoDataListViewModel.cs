using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.BLL;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.Model;
using lenovo.themes.generic.Attributes;
using lenovo.themes.generic.Component.DataListControl;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.Interactivity.Ex;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenCapture.ViewModel;

public class VideoDataListViewModel : DataListViewModel<VideoDetailListItemViewModel, string>
{
	private class SortingStatus
	{
		public Dictionary<string, SortingInfo> SortMapping { get; set; }

		public string CurrentSortMemberPath { get; set; }
	}

	private class SortingInfo
	{
		public string SortProtertyName { get; set; }

		public bool SortDataIsLoaded { get; set; }

		public string SortMemberPath { get; set; }
	}

	private VideoBLL videoBLL = new VideoBLL();

	private ReplayCommand detailListScrollChangedCommand;

	private int ICON_VIEW_COLUMN_COUNT = 4;

	private int ICON_VIEW_ITEM_PANEL_HEIGHT = 166;

	private SortingStatus mSortingStatus;

	private ReplayCommand sortingCommand;

	public ReplayCommand DetailListScrollChangedCommand
	{
		get
		{
			return detailListScrollChangedCommand;
		}
		set
		{
			if (detailListScrollChangedCommand != value)
			{
				detailListScrollChangedCommand = value;
				OnPropertyChanged("DetailListScrollChangedCommand");
			}
		}
	}

	public ReplayCommand SortingCommand
	{
		get
		{
			return sortingCommand;
		}
		set
		{
			if (sortingCommand != value)
			{
				sortingCommand = value;
				OnPropertyChanged("SortingCommand");
			}
		}
	}

	public VideoDataListViewModel()
	{
		base.ExportEnable = false;
		base.DeleteEnable = false;
		base.RefreshEnable = true;
		base.ImportVisibility = Visibility.Collapsed;
		base.CheckAllBoxVisibility = Visibility.Collapsed;
		base.CurrentLoadingPageSize = 25;
		DetailListScrollChangedCommand = new ReplayCommand(DetailListScrollChangedCommandHandler);
		SortingCommand = new ReplayCommand(SortingCommandHandler);
	}

	public override void LoadData()
	{
		base.LoadData();
		BeginLoadIdList(delegate(object p1, Exception exception1)
		{
			if (exception1 == null)
			{
				BeginLoadBigPropertyData(null);
			}
		});
	}

	protected override bool FillBasicProperty(IEnumerable<VideoDetailListItemViewModel> target)
	{
		List<string> idList = target.Select((VideoDetailListItemViewModel m) => m.Id).ToList();
		List<VideoDetailModel> basicInfoList = videoBLL.GetVideoInfoList(idList);
		if (basicInfoList != null && basicInfoList.Count > 0)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				foreach (VideoDetailModel item in basicInfoList)
				{
					foreach (VideoDetailListItemViewModel item2 in target)
					{
						if (item2.Id.Equals(item.Id))
						{
							item2.Id = item.Id;
							item2.Name = item.Name;
							item2.LongDuration = item.Duration;
							item2.LongSize = item.Size;
							item2.LongModifiedDate = item.ModifiedDate;
							item2.ModifiedDate = item.ModifiedDateDisplayString;
							item2.BasicPropertyIsLoaded = true;
							break;
						}
					}
				}
			});
		}
		return true;
	}

	protected override bool FillBigProperty(IEnumerable<VideoDetailListItemViewModel> target, CancellationTokenSource cancel)
	{
		List<string> idList = target.Select((VideoDetailListItemViewModel m) => m.Id).ToList();
		string exportFolder = Path.Combine(Configurations.PicCacheDir, "ScreenCAP");
		videoBLL.ExportVideoThumbnailList(idList, exportFolder, delegate(string id, bool success, string path)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				foreach (VideoDetailListItemViewModel item in target)
				{
					if (item.Id.Equals(id))
					{
						try
						{
							BitmapImage bitmapImage = new BitmapImage();
							bitmapImage.BeginInit();
							bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
							bitmapImage.UriSource = new Uri(path, UriKind.Absolute);
							bitmapImage.EndInit();
							item.IconSource = bitmapImage;
							item.BigPropertyIsLoaded = true;
							break;
						}
						catch
						{
							break;
						}
					}
				}
			});
		});
		return true;
	}

	protected override IEnumerable<string> PrepareIdList()
	{
		return videoBLL.GetIdList("RecordScreen", "id", isSortDesc: false);
	}

	protected override int PrepareTotalCount()
	{
		return 1000;
	}

	private void DetailListScrollChangedCommandHandler(object e)
	{
		InvokeCommandActionParameters obj = e as InvokeCommandActionParameters;
		ScrollChangedEventArgs scrollChangedEventArgs = obj.InvokeParameter as ScrollChangedEventArgs;
		string value = obj.CommandParameter.ToString();
		double verticalOffset = scrollChangedEventArgs.VerticalOffset;
		if ("detail".Equals(value))
		{
			base.CurrentLoadingStartIndex = (int)verticalOffset;
			BeginLoadBasicPropertyData(null);
		}
		else
		{
			base.CurrentLoadingStartIndex = ((!(verticalOffset < (double)ICON_VIEW_ITEM_PANEL_HEIGHT)) ? ((int)Math.Ceiling(verticalOffset / (double)ICON_VIEW_ITEM_PANEL_HEIGHT) - 1) : 0) * ICON_VIEW_COLUMN_COUNT;
			BeginLoadBigPropertyData(null);
		}
	}

	protected override void OnItemInitialized(VideoDetailListItemViewModel model)
	{
		base.OnItemInitialized(model);
		model.CheckCammand = new ReplayCommand(delegate(object e)
		{
			if ((bool)e)
			{
				base.IsAllChecked = base.DataItemsSource.Where((VideoDetailListItemViewModel m) => !m.IsChecked).Count() == 0;
				bool flag2 = (base.DeleteEnable = true);
				base.ExportEnable = flag2;
			}
			else
			{
				base.IsAllChecked = false;
				bool flag2 = (base.DeleteEnable = base.DataItemsSource.Where((VideoDetailListItemViewModel m) => m.IsChecked).Count() > 0);
				base.ExportEnable = flag2;
			}
		});
	}

	protected override void OnStartLoadingId(Dictionary<string, object> tag)
	{
		base.RefreshEnable = false;
		base.ExportEnable = false;
		base.DeleteEnable = false;
		base.ImportEnable = false;
	}

	protected override void OnStopLoadingId(Dictionary<string, object> tag)
	{
		base.RefreshEnable = true;
		bool flag2 = (base.DeleteEnable = base.DataItemsSource.Where((VideoDetailListItemViewModel m) => m.IsChecked).Count() > 0);
		base.ExportEnable = flag2;
		base.IsAllChecked = base.DataItemsSource.Count > 0 && base.DataItemsSource.Where((VideoDetailListItemViewModel m) => !m.IsChecked).Count() == 0;
	}

	protected override void OnShowModeChanged(bool isDetailMode)
	{
		base.CheckAllBoxVisibility = (isDetailMode ? Visibility.Collapsed : Visibility.Visible);
	}

	protected override void CheckAllCommandHandler(object e)
	{
		bool flag = (bool)e;
		foreach (VideoDetailListItemViewModel item in base.DataItemsSource)
		{
			item.IsChecked = flag;
		}
		bool flag3 = (base.DeleteEnable = flag && base.DataItemsSource.Where((VideoDetailListItemViewModel m) => m.IsChecked).Count() > 0);
		base.ExportEnable = flag3;
	}

	protected override void SyncUIDataFromCache(IEnumerable<VideoDetailListItemViewModel> news, IEnumerable<VideoDetailListItemViewModel> removed)
	{
		base.SyncUIDataFromCache(news, removed);
		if (news != null && news.Count() > 0)
		{
			ClearSortStatus();
		}
	}

	protected override void ExportCommandHandler(object e)
	{
		FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
		if (folderBrowser.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			_ = folderBrowser.SelectedPath;
			List<string> idList = (from m in base.DataItemsSource
				where m.IsChecked
				select m.Id).ToList();
			string saveDir = folderBrowser.SelectedPath.Trim();
			new ImportAndExportWrapper().ExportFile(BusinessType.SCREEN_RECORDER_EXPORT, 20, idList, "K0567", "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "Videos", saveDir, null);
		});
	}

	protected override void RefreshCommandHandler(object e)
	{
		BeginLoadIdList(delegate(object p1, Exception exception1)
		{
			if (exception1 == null && !base.IsDetailMode)
			{
				BeginLoadBigPropertyData(null);
			}
		});
	}

	protected override void DeleteCommandHandler(object e)
	{
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0585", "K0569", "K0208", "K0583", null);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				win.ShowDialog();
			});
			if (win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
			{
				OnStartLoadingId(null);
				DoProcessAsync(delegate
				{
					try
					{
						List<string> idList = (from m in base.DataItemsSource
							where m.IsChecked
							select m.Id).ToList();
						if (videoBLL.DeleteVideo(idList))
						{
							UpdateDataCollection(PrepareIdList());
						}
					}
					finally
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							OnStopLoadingId(null);
						});
					}
				}, null);
			}
		});
	}

	private void ClearSortStatus()
	{
		if (mSortingStatus != null)
		{
			foreach (KeyValuePair<string, SortingInfo> item in mSortingStatus.SortMapping)
			{
				item.Value.SortDataIsLoaded = false;
			}
			mSortingStatus.CurrentSortMemberPath = string.Empty;
		}
		foreach (VideoDetailListItemViewModel item2 in base.DataItemsSource)
		{
			item2.ClearSortStatus();
		}
	}

	private void SortingCommandHandler(object e)
	{
		DataGridSortingEventArgs dataGridSortingEventArgs = (e as InvokeCommandActionParameters).InvokeParameter as DataGridSortingEventArgs;
		DataGridColumn column = dataGridSortingEventArgs.Column;
		ListSortDirection listSortDirection = ((column.SortDirection == ListSortDirection.Descending) ? ListSortDirection.Descending : ListSortDirection.Ascending);
		string sortMemberPath = column.SortMemberPath;
		if (string.IsNullOrEmpty(sortMemberPath))
		{
			return;
		}
		if (mSortingStatus == null)
		{
			mSortingStatus = new SortingStatus();
			Dictionary<string, SortingInfo> dictionary = new Dictionary<string, SortingInfo>();
			Type typeFromHandle = typeof(VideoDetailListItemViewModel);
			Type typeFromHandle2 = typeof(SortProtertyNameAttribute);
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				object[] customAttributes = properties[i].GetCustomAttributes(typeFromHandle2, inherit: true);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					SortProtertyNameAttribute sortProtertyNameAttribute = (SortProtertyNameAttribute)customAttributes[j];
					dictionary[sortProtertyNameAttribute.SortMemberPath] = new SortingInfo
					{
						SortDataIsLoaded = false,
						SortMemberPath = sortProtertyNameAttribute.SortMemberPath,
						SortProtertyName = sortProtertyNameAttribute.SortPropertyName
					};
				}
			}
			mSortingStatus.SortMapping = dictionary;
		}
		if (PreparSorting(sortMemberPath, listSortDirection))
		{
			mSortingStatus.CurrentSortMemberPath = sortMemberPath;
			column.SortDirection = ((listSortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending);
		}
		dataGridSortingEventArgs.Handled = true;
	}

	private bool PreparSorting(string sortMemberPath, ListSortDirection currentDirection)
	{
		if (!mSortingStatus.SortMapping[sortMemberPath].SortDataIsLoaded)
		{
			List<string> idList = videoBLL.GetIdList("RecordScreen", mSortingStatus.SortMapping[sortMemberPath].SortProtertyName, isSortDesc: false);
			if (idList == null)
			{
				return false;
			}
			for (int i = 0; i < idList.Count; i++)
			{
				foreach (VideoDetailListItemViewModel item in base.DataItemsSource)
				{
					if (idList[i].Equals(item.Id))
					{
						item.AddSortMapping(sortMemberPath, i);
						break;
					}
				}
			}
		}
		if (!sortMemberPath.Equals(mSortingStatus.CurrentSortMemberPath))
		{
			foreach (VideoDetailListItemViewModel item2 in base.DataItemsSource)
			{
				item2.SetCurrentSortIndex(sortMemberPath);
			}
		}
		if (currentDirection == ListSortDirection.Descending)
		{
			base.DataItemsSource = new ObservableCollection<VideoDetailListItemViewModel>(base.DataItemsSource.OrderByDescending((VideoDetailListItemViewModel m) => m.SortIndex));
		}
		else
		{
			base.DataItemsSource = new ObservableCollection<VideoDetailListItemViewModel>(base.DataItemsSource.OrderBy((VideoDetailListItemViewModel m) => m.SortIndex));
		}
		return true;
	}
}
