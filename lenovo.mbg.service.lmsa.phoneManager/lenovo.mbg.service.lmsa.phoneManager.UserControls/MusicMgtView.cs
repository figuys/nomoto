using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class MusicMgtView : System.Windows.Controls.UserControl, IComponentConnector, IStyleConnector
{
	private NavigationManagementBLL bll = new NavigationManagementBLL();

	public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(Window), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(Window), new PropertyMetadata("All|*.*"));

	public string Path
	{
		get
		{
			return (string)GetValue(PathProperty);
		}
		set
		{
			SetValue(PathProperty, value);
		}
	}

	public string Filter
	{
		get
		{
			return (string)GetValue(FilterProperty);
		}
		set
		{
			SetValue(FilterProperty, value);
		}
	}

	public MusicMgtView()
	{
		InitializeComponent();
	}

	private void CheckAll_Click(object sender, RoutedEventArgs e)
	{
		System.Windows.Controls.CheckBox checkBox = e.OriginalSource as System.Windows.Controls.CheckBox;
		bool flag = !checkBox.IsChecked.HasValue || checkBox.IsChecked.Value;
		IEnumerable<MusicInfoViewModel> enumerable = MusicListData.ItemsSource.OfType<MusicInfoViewModel>();
		if (enumerable == null || enumerable.Count() <= 0)
		{
			return;
		}
		foreach (MusicInfoViewModel item in enumerable)
		{
			item.IsSelected = flag;
		}
		SetExportDeleteLabColor(flag);
	}

	public void SetExportDeleteLabColor(bool hasChecked)
	{
		if (AlbumScrolls.Visibility == Visibility.Visible)
		{
			btnExport.IsEnabled = false;
			btnDelete.IsEnabled = false;
		}
		else
		{
			IconButton iconButton = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = hasChecked);
			iconButton.IsEnabled = isEnabled;
		}
	}

	private void CheckOne_Click(object sender, RoutedEventArgs e)
	{
		if (!(sender is System.Windows.Controls.CheckBox checkBox) || !(base.DataContext is MusicViewModel musicViewModel))
		{
			return;
		}
		if (checkBox.IsChecked == true)
		{
			musicViewModel.IsAllSelected = musicViewModel.SongList.FirstOrDefault((MusicInfoViewModel p) => !p.IsSelected) == null;
		}
		else
		{
			musicViewModel.IsAllSelected = false;
		}
		int count = new List<MusicInfoViewModel>(musicViewModel.SongList).FindAll((MusicInfoViewModel p) => p.IsSelected).Count;
		SetExportDeleteLabColor(count > 0);
		if (count == MusicListData.Items.Count)
		{
			MusicViewModel.SingleInstance.IsAllSelected = true;
		}
		else
		{
			MusicViewModel.SingleInstance.IsAllSelected = false;
		}
	}

	private void orderMusicByName_MouseUp(object sender, MouseButtonEventArgs e)
	{
		if (MusicListData.Columns[0].SortDirection == ListSortDirection.Descending)
		{
			MusicListData.Columns[0].SortDirection = ListSortDirection.Ascending;
		}
		else
		{
			MusicListData.Columns[0].SortDirection = ListSortDirection.Descending;
		}
	}

	private void MusicViewChange(object sender, SelectionChangedEventArgs e)
	{
		object selectedItem = (sender as System.Windows.Controls.ListBox).SelectedItem;
		if (selectedItem == null)
		{
			return;
		}
		MusicViewModel.SingleInstance.IsAllSelected = false;
		selectedItem.ToString();
		if (MusicListData == null)
		{
			return;
		}
		if (musictopmenulist.SelectedIndex == 0)
		{
			MusicViewModel.SingleInstance.FocuseAlbum = null;
			MusicListData.Visibility = Visibility.Visible;
			AlbumScrolls.Visibility = Visibility.Hidden;
			albumBack.Visibility = Visibility.Collapsed;
			if (!MusicViewModel.SingleInstance.FirstLoadData)
			{
				List<MusicInfo> music = MusicViewModel.SingleInstance.Service.LoadMusicFormCache("-1");
				MusicViewModel.SingleInstance.UpdateMusicListAfterClear(music);
			}
		}
		else
		{
			MusicListData.Visibility = Visibility.Hidden;
			AlbumScrolls.Visibility = Visibility.Visible;
			SetExportDeleteLabColor(hasChecked: false);
		}
	}

	private void AddSong(object sender, RoutedEventArgs e)
	{
		List<string> fileNames = new List<string>();
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = string.Format("{0}|*.mp3;*.wav;*.ogg;*.aac;*.midi;*.amr", "Songs");
		openFileDialog.Multiselect = true;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() == true)
		{
			int length = openFileDialog.SafeFileNames.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				fileNames.Add(openFileDialog.FileNames[i]);
			}
		}
		if (fileNames.Count() != 0)
		{
			TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			string appSaveDir = tcpAndroidDevice.Property.InternalStoragePath + "/LMSA/Music/";
			new ImportAndExportWrapper().ImportFile(BusinessType.SONG_IMPORT, 19, ResourcesHelper.StringResources.SingleInstance.MUSIC_IMPORT_MESSAGE, "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", BackupRestoreStaticResources.SingleInstance.MUSIC, () => fileNames, (string sourcePath) => appSaveDir + System.IO.Path.GetFileName(sourcePath));
			RefreshList(null, null);
		}
	}

	private void SongList_DBClick(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Controls.DataGrid dataGrid = sender as System.Windows.Controls.DataGrid;
		Point position = e.GetPosition(dataGrid);
		DependencyObject dependencyObject = dataGrid.InputHitTest(position) as DependencyObject;
		bool flag = false;
		while (dependencyObject != null)
		{
			if (dependencyObject is DataGridRow)
			{
				flag = true;
				break;
			}
			dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
		}
		e.Handled = !flag;
		if (flag)
		{
			MusicInfoViewModel music = MusicListData.SelectedItem as MusicInfoViewModel;
			MusicPlayerViewModel.SingleInstance.SongDBClick(music);
		}
	}

	private void BacktoMusicAlbum(object sender, RoutedEventArgs e)
	{
		MusicViewModel.SingleInstance.FocuseAlbum = null;
		MusicListData.Visibility = Visibility.Hidden;
		AlbumScrolls.Visibility = Visibility.Visible;
		albumBack.Visibility = Visibility.Collapsed;
		SetExportDeleteLabColor(hasChecked: false);
	}

	private void DeleteSong(object sender, RoutedEventArgs e)
	{
		List<MusicInfoViewModel> list = MusicViewModel.SingleInstance.SongList.Where((MusicInfoViewModel m) => m.IsSelected).ToList();
		if (list == null || list.Count == 0)
		{
			return;
		}
		if (list.Exists((MusicInfoViewModel n) => n.ID == MusicPlayerViewModel.SingleInstance.CurrentPlayId))
		{
			MusicPlayerViewModel.SingleInstance.Stop();
			MusicPlayerViewModel.SingleInstance.ResetSongText();
		}
		if (MessageBoxHelper.DeleteConfirmMessagebox(ResourcesHelper.StringResources.SingleInstance.CONTACT_DELETE_TITLE, ResourcesHelper.StringResources.SingleInstance.MUSIC_DELETE_CONTENT))
		{
			List<string> idArr = list.Select((MusicInfoViewModel p) => p.ID.ToString()).ToList();
			AsyncDataLoader.Loading(delegate
			{
				Stopwatch stopwatch = new Stopwatch();
				Dictionary<string, int> result = new Dictionary<string, int>();
				BusinessData businessData = new BusinessData(BusinessType.SONG_DELETE, Context.CurrentDevice);
				stopwatch.Start();
				new DeviceCommonManagement().DeleteDevFilesWithConfirm("deleteAudiosById", idArr, ref result);
				stopwatch.Stop();
				HostProxy.BehaviorService.Collect(BusinessType.SONG_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, BusinessStatus.SUCCESS, result));
			});
			int num = MusicViewModel.SingleInstance.SongList.Count((MusicInfoViewModel p) => p.IsSelected);
			SetExportDeleteLabColor(num > 0);
			MusicViewModel.SingleInstance.IsAllSelected = num == MusicListData.Items.Count;
		}
		RefreshList(null, null);
	}

	private void ExportMusic(object sender, RoutedEventArgs e)
	{
		if (AlbumScrolls.Visibility == Visibility.Visible)
		{
			return;
		}
		List<MusicInfoViewModel> list = MusicViewModel.SingleInstance.SongList.Where((MusicInfoViewModel m) => m.IsSelected).ToList();
		if (list == null || list.Count == 0)
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel)
		{
			string saveDir = folderBrowserDialog.SelectedPath.Trim();
			ImportAndExportWrapper importAndExportWrapper = new ImportAndExportWrapper();
			List<string> idList = list.Select((MusicInfoViewModel m) => m.ID.ToString()).ToList();
			importAndExportWrapper.ExportFile(BusinessType.SONG_EXPORT, 19, idList, ResourcesHelper.StringResources.SingleInstance.MUSIC_EXPORT_MESSAGE, "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", ResourcesHelper.StringResources.SingleInstance.MUSIC_CONTENT, saveDir, null);
		}
	}

	private void AlbumListboxMouseDoubleClick(object sender, RoutedEventArgs e)
	{
		if (albumListBox.SelectedItem != null)
		{
			MusicListData.Visibility = Visibility.Visible;
			AlbumScrolls.Visibility = Visibility.Hidden;
			albumBack.Visibility = Visibility.Visible;
			MusicViewModel.SingleInstance.FocuseAlbum = (MusicAlbumViewModel)albumListBox.SelectedItem;
			string albumId = MusicViewModel.SingleInstance.FocuseAlbum.AlbumID;
			AsyncDataLoader.BeginLoading(delegate
			{
				List<MusicInfo> music = MusicViewModel.SingleInstance.Service.LoadMusicFormCache(albumId);
				MusicViewModel.SingleInstance.UpdateMusicListAfterClear(music);
			}, ViewContext.SingleInstance.MainViewModel);
		}
	}

	private void SearchMusicByKey(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			string text = txbSearch.Text;
			string albumID = GetAlbumID();
			List<MusicInfo> music = MusicViewModel.SingleInstance.Service.SearchMusicList(text, albumID);
			AsyncDataLoader.BeginLoading(delegate
			{
				MusicViewModel.SingleInstance.UpdateMusicListAfterClear(music);
			}, ViewContext.SingleInstance.MainViewModel);
		}
	}

	private void SetMusicNotify(object sender, RoutedEventArgs e)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		string type = (sender as System.Windows.Controls.MenuItem).Tag.ToString();
		MusicInfoViewModel model = MusicListData.SelectedItem as MusicInfoViewModel;
		HostProxy.PermissionService.BeginConfirmAppIsReady(HostProxy.deviceManager.MasterDevice, "SetRingTone", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				int code = 1;
				HostProxy.BehaviorService.Collect(BusinessType.SONG_SET_RINGTONE, null);
				AsyncDataLoader.BeginLoading(delegate
				{
					string[] names = Enum.GetNames(typeof(MusicType));
					foreach (string value in names)
					{
						if (type.Equals(value))
						{
							MusicType type2 = (MusicType)Enum.Parse(typeof(MusicType), value);
							code = MusicViewModel.SingleInstance.Service.SetMusicAsRingtone(model.RawMusicInfo, type2);
							break;
						}
					}
					if (code == 0)
					{
						return new Tuple<bool, string, string>(item1: true, "K0711", "K0763");
					}
					if (code == 2)
					{
						return (Tuple<bool, string, string>)null;
					}
					if (currentDevice != null && currentDevice.Property != null && currentDevice.Property.AndroidVersion.Contains("6.0"))
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							RegrantAppPermissionTips win1 = new RegrantAppPermissionTips();
							HostProxy.HostMaskLayerWrapper.New(win1, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
							{
								win1.ShowDialog();
							});
						});
						return (Tuple<bool, string, string>)null;
					}
					return new Tuple<bool, string, string>(item1: true, "K0071", "K0553");
				}, ViewContext.SingleInstance.MainViewModel);
			}
		});
	}

	private void RefreshList(object sender, RoutedEventArgs e)
	{
		txbSearch.Text = string.Empty;
		SetExportDeleteLabColor(hasChecked: false);
		MusicViewModel.SingleInstance.IsAllSelected = false;
		if (MusicViewModel.SingleInstance.SelectedIndex == 0)
		{
			MusicViewModel.SingleInstance.Load();
		}
		else
		{
			if (MusicViewModel.SingleInstance.SelectedIndex != 1)
			{
				return;
			}
			if (MusicViewModel.SingleInstance.FocuseAlbum == null || string.IsNullOrEmpty(MusicViewModel.SingleInstance.FocuseAlbum.AlbumID))
			{
				MusicViewModel.SingleInstance.Load(1);
				return;
			}
			MusicViewModel.SingleInstance.LoadAlbumMusic(MusicViewModel.SingleInstance.FocuseAlbum.AlbumID, delegate(List<MusicInfo> arr)
			{
				HostProxy.CurrentDispatcher.Invoke(delegate
				{
					if (arr == null || arr.Count == 0)
					{
						BacktoMusicAlbum(null, null);
					}
					else
					{
						MusicViewModel.SingleInstance.SongList.Clear();
						arr.ForEach(delegate(MusicInfo p)
						{
							MusicViewModel.SingleInstance.SongList.Add(new MusicInfoViewModel(p));
						});
					}
				});
			});
		}
	}

	private string GetAlbumID()
	{
		string result = "-1";
		if (MusicViewModel.SingleInstance.FocuseAlbum != null)
		{
			result = MusicViewModel.SingleInstance.FocuseAlbum.AlbumID;
		}
		return result;
	}

	private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
