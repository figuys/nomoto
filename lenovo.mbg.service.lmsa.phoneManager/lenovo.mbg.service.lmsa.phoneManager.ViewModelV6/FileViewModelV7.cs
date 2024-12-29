using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.ModelV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class FileViewModelV7 : ViewModelBase
{
	private bool IsDeviceTreeLoading;

	public Stack<FileDataViewModel> forwardStack = new Stack<FileDataViewModel>();

	public Stack<FileDataViewModel> backStack = new Stack<FileDataViewModel>();

	public List<FileDataViewModel> allDeviceChildrens = new List<FileDataViewModel>();

	private int count;

	private Dictionary<string, List<FileDataViewModel>> _childs = new Dictionary<string, List<FileDataViewModel>>();

	protected DeviceFileManager fileManager;

	protected TreeViewModel CurPcSelected;

	protected TreeViewModel CurPhoneSelected;

	protected bool IsFireChanged = true;

	private long _transferLocker;

	private int _DeviceFileCount;

	private int _DeviceSelectFileCount;

	private string _CurPcPath;

	private bool? _IsAllCheckedForCheckBox = false;

	private bool? _IsCurrentCheckBoxChecked = false;

	private string _CurDevicePath;

	private ObservableCollection<TreeViewModel> _PcTreeSources = new ObservableCollection<TreeViewModel>();

	private ObservableCollection<TreeViewModel> _DeviceTreeSources = new ObservableCollection<TreeViewModel>();

	private ObservableCollection<TreeViewModel> _PcDataGridSources = new ObservableCollection<TreeViewModel>();

	private ObservableCollection<TreeViewModel> _DeviceDataGridSources = new ObservableCollection<TreeViewModel>();

	protected SdCardViewModel sdCarVm;

	private Visibility _storageSelectPanelVisibility;

	private bool importToolBtnEnable = true;

	private bool fileRefreshToolBtnEnable = true;

	public ReplayCommand PcFolderClickCommand { get; }

	public ReplayCommand DeviceFolderClickCommand { get; }

	public ReplayCommand GobackCommand { get; }

	public ReplayCommand GoForwardCommand { get; }

	public ReplayCommand RefreshCommand { get; }

	public ReplayCommand SearchPcCommand { get; }

	public ReplayCommand SearchDeviceCommand { get; }

	public ReplayCommand PcEnterPathCommand { get; }

	public ReplayCommand DeviceEnterPathCommand { get; }

	public ReplayCommand OnCheckedCommand { get; }

	public ReplayCommand DeleteFileCommand { get; }

	public TreeViewModel PCDataGridSelected { get; set; }

	public TreeViewModel PhoneDataGridSelected { get; set; }

	private bool transferLocker
	{
		get
		{
			return Interlocked.Read(ref _transferLocker) == 1;
		}
		set
		{
			Interlocked.Exchange(ref _transferLocker, value ? 1 : 0);
		}
	}

	public int DeviceFileCount
	{
		get
		{
			return _DeviceFileCount;
		}
		set
		{
			_DeviceFileCount = value;
			OnPropertyChanged("DeviceFileCount");
		}
	}

	public int DeviceSelectFileCount
	{
		get
		{
			return _DeviceSelectFileCount;
		}
		set
		{
			_DeviceSelectFileCount = value;
			OnPropertyChanged("DeviceSelectFileCount");
		}
	}

	public string CurPcPath
	{
		get
		{
			return _CurPcPath;
		}
		set
		{
			_CurPcPath = value;
			OnPropertyChanged("CurPcPath");
		}
	}

	public bool? IsAllCheckedForCheckBox
	{
		get
		{
			return _IsAllCheckedForCheckBox;
		}
		set
		{
			_IsAllCheckedForCheckBox = value;
			OnPropertyChanged("IsAllCheckedForCheckBox");
		}
	}

	public bool? IsCurrentCheckBoxChecked
	{
		get
		{
			return _IsCurrentCheckBoxChecked;
		}
		set
		{
			_IsCurrentCheckBoxChecked = value;
			OnPropertyChanged("IsCurrentCheckBoxChecked");
		}
	}

	public string CurDevicePath
	{
		get
		{
			return _CurDevicePath;
		}
		set
		{
			_CurDevicePath = value;
			OnPropertyChanged("CurDevicePath");
		}
	}

	public virtual ObservableCollection<TreeViewModel> PcTreeSources
	{
		get
		{
			return _PcTreeSources;
		}
		set
		{
			_PcTreeSources = value;
			OnPropertyChanged("PcTreeSources");
		}
	}

	public virtual ObservableCollection<TreeViewModel> DeviceTreeSources
	{
		get
		{
			return _DeviceTreeSources;
		}
		set
		{
			_DeviceTreeSources = value;
			OnPropertyChanged("DeviceTreeSources");
		}
	}

	public virtual ObservableCollection<TreeViewModel> PcDataGridSources
	{
		get
		{
			return _PcDataGridSources;
		}
		set
		{
			_PcDataGridSources = value;
			OnPropertyChanged("PcDataGridSources");
		}
	}

	public virtual ObservableCollection<TreeViewModel> DeviceDataGridSources
	{
		get
		{
			return _DeviceDataGridSources;
		}
		set
		{
			_DeviceDataGridSources = value;
			OnPropertyChanged("DeviceDataGridSources");
		}
	}

	public SdCardViewModel SdCarVm
	{
		get
		{
			return sdCarVm;
		}
		set
		{
			sdCarVm = value;
			OnPropertyChanged("SdCarVm");
		}
	}

	public Visibility StorageSelectPanelVisibility
	{
		get
		{
			return _storageSelectPanelVisibility;
		}
		set
		{
			if (_storageSelectPanelVisibility != value)
			{
				_storageSelectPanelVisibility = value;
				OnPropertyChanged("StorageSelectPanelVisibility");
			}
		}
	}

	public bool ImportToolBtnEnable
	{
		get
		{
			return importToolBtnEnable;
		}
		set
		{
			importToolBtnEnable = value;
			OnPropertyChanged("ImportToolBtnEnable");
		}
	}

	public bool FileRefreshToolBtnEnable
	{
		get
		{
			return fileRefreshToolBtnEnable;
		}
		set
		{
			fileRefreshToolBtnEnable = value;
			OnPropertyChanged("FileRefreshToolBtnEnable");
		}
	}

	public FileViewModelV7()
	{
		PcFolderClickCommand = new ReplayCommand(PcFolderClickCommandHandler);
		DeviceFolderClickCommand = new ReplayCommand(DeviceFolderClickCommandHandler);
		GobackCommand = new ReplayCommand(GobackCommandHandler);
		GoForwardCommand = new ReplayCommand(GoForwardCommandHandler);
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		SearchPcCommand = new ReplayCommand(SearchPcCommandHandler);
		SearchDeviceCommand = new ReplayCommand(SearchDeviceCommandHandler);
		PcEnterPathCommand = new ReplayCommand(PcEnterPathCommandHandler);
		DeviceEnterPathCommand = new ReplayCommand(DeviceEnterPathCommandHandler);
		OnCheckedCommand = new ReplayCommand(OnCheckedCommandHandler);
		DeleteFileCommand = new ReplayCommand(DeleteFileCommandHandler);
		fileManager = new DeviceFileManager();
		SdCarVm = new SdCardViewModel();
	}

	public override void LoadData(object data)
	{
		CurPcSelected = null;
		CurPhoneSelected = null;
		IsAllCheckedForCheckBox = false;
		PcDataGridSources.Clear();
		DeviceDataGridSources.Clear();
		forwardStack.Clear();
		LoadDeviceTreeAsync();
		base.LoadData(data);
		SdCarVm.LoadData(Context.CurrentDevice);
	}

	public bool Import(FileDataViewModel fileInfo)
	{
		if (transferLocker)
		{
			return false;
		}
		transferLocker = true;
		if (fileInfo == null)
		{
			return false;
		}
		string data = fileInfo.Data;
		if (string.IsNullOrEmpty(data))
		{
			return false;
		}
		if (!(CurPhoneSelected is FileDataViewModel { Data: var data2 }))
		{
			return false;
		}
		string text = Path.GetFileName(data);
		string localFileFullName = data;
		string targetFileFullName = $"{data2}{'/'}{text}";
		TransferProgressWiew wnd = new TransferProgressWiew();
		string text2 = HostProxy.LanguageService.Translate("K0632");
		if (text.Length >= 45)
		{
			text = text.Substring(0, 42) + "...";
		}
		wnd.SetProgressTitle(text + " ({0}/{1})", text2 + "...");
		Task.Run(delegate
		{
			AsyncDataLoader.BeginLoading(delegate
			{
				if (fileManager.Import(localFileFullName, targetFileFullName, wnd.ProgressUpdate))
				{
					RefreshCommandHandler("2");
				}
			}, wnd);
			transferLocker = false;
		});
		return true;
	}

	public bool Export(FileDataViewModel fileInfo)
	{
		if (fileInfo == null)
		{
			return false;
		}
		string data = fileInfo.Data;
		if (string.IsNullOrEmpty(data))
		{
			return false;
		}
		try
		{
			string targetFileFullName = data;
			TransferProgressWiew wnd = new TransferProgressWiew();
			string text = HostProxy.LanguageService.Translate("K0629");
			string text2 = ((fileInfo.Name.Length >= 45) ? (fileInfo.Name.Substring(0, 42) + "...") : fileInfo.Name);
			wnd.SetProgressTitle(text2 + " ({0}/{1})", text + "...");
			AsyncDataLoader.BeginLoading(delegate
			{
				if (fileManager.Export(targetFileFullName, "D:\\新建文件夹", wnd.ProgressUpdate))
				{
					Application.Current.Dispatcher.Invoke(delegate
					{
						RefreshCommandHandler("1");
					});
				}
			}, wnd);
			return true;
		}
		finally
		{
			transferLocker = false;
		}
	}

	private Task LoadPcTreeAsync()
	{
		return Task.Run(delegate
		{
			List<TreeViewModel> list = new List<TreeViewModel>();
			string[] logicalDrives = Directory.GetLogicalDrives();
			foreach (string text in logicalDrives)
			{
				list.Add(new FileDataViewModel(new TreeModel(text, null, text.Trim(Path.DirectorySeparatorChar), text, "v6_icon_disk"), null, isFolder: true, 0L, null, null, PcFolderExpand));
			}
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			if (!string.IsNullOrEmpty(folderPath))
			{
				list.Add(new FileDataViewModel(new TreeModel(folderPath, null, folderPath.Trim(Path.DirectorySeparatorChar), folderPath, "v6_icon_disk"), null, isFolder: true, 0L, null, null, PcFolderExpand));
			}
			HostProxy.CurrentDispatcher.Invoke(() => PcTreeSources = new ObservableCollection<TreeViewModel>(list));
		});
	}

	private void LoadDeviceTreeAsync()
	{
		if (IsDeviceTreeLoading)
		{
			return;
		}
		IsDeviceTreeLoading = true;
		Task.Run(delegate
		{
			DeviceTreeSources = new ObservableCollection<TreeViewModel>();
			string internalPath = string.Empty;
			string externalPath = string.Empty;
			forwardStack.Clear();
			backStack.Clear();
			fileManager.GetInternalAndExternamPath(out internalPath, out externalPath);
			List<TreeViewModel> list = new List<TreeViewModel>();
			if (SdCarVm.StorageSelIndex == 0)
			{
				if (!string.IsNullOrEmpty(internalPath) && internalPath.Length > 0 && internalPath[0].Equals('/'))
				{
					list.Add(new FileDataViewModel(new TreeModel(internalPath, null, "Download", internalPath), null, isFolder: true, 0L, null, null, DeviceFolderExpand));
				}
			}
			else if (!string.IsNullOrEmpty(externalPath) && externalPath.Length > 0 && externalPath[0].Equals('/'))
			{
				list.Add(new FileDataViewModel(new TreeModel(externalPath, null, "Download", externalPath), null, isFolder: true, 0L, null, null, DeviceFolderExpand));
			}
			HostProxy.CurrentDispatcher.Invoke(() => DeviceTreeSources = new ObservableCollection<TreeViewModel>(list));
			if (DeviceTreeSources.Count >= 1)
			{
				allDeviceChildrens = new List<FileDataViewModel>();
				TraverseChildren(DeviceTreeSources[0] as FileDataViewModel);
				for (int i = 0; i < allDeviceChildrens.Count; i++)
				{
					TraverseChildren(allDeviceChildrens[i]);
				}
				DeviceFileCount = allDeviceChildrens.Where((FileDataViewModel x) => !x.IsFolder).Count();
				DeviceTreeSources[0].ChildrenCount = DeviceFileCount;
				if (string.IsNullOrEmpty(CurDevicePath) || (CurPhoneSelected != null && CurPhoneSelected.Name == "Download"))
				{
					LoadDeviceChildrens(DeviceTreeSources[0], expand: false, refresh: true);
				}
				else
				{
					(DeviceTreeSources[0] as FileDataViewModel).IsExpanded = true;
					if (allDeviceChildrens.Where((FileDataViewModel x) => x.Data == CurDevicePath && x.IsFolder).Count() != 0)
					{
						LoadDeviceChildrens(allDeviceChildrens.Where((FileDataViewModel x) => x.Data == CurDevicePath && x.IsFolder).First(), expand: false, refresh: true);
					}
				}
				backStack.Push(DeviceTreeSources[0] as FileDataViewModel);
				FileRefreshToolBtnEnable = true;
				IsDeviceTreeLoading = false;
			}
		});
	}

	private void PcFolderExpand(object data)
	{
		if (IsFireChanged)
		{
			LoadPcChildrens(data, expand: true, refresh: false);
		}
	}

	private void PcFolderClickCommandHandler(object data)
	{
		if (IsFireChanged)
		{
			LoadPcChildrens(data, expand: false, refresh: false);
		}
	}

	private void DeleteFileCommandHandler(object parameter)
	{
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0585", "K0562", "K0208", "K0583", new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Assets/Images/PicPopup/delete.png")));
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		if (!win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
		{
			return;
		}
		AsyncDataLoader.Loading(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			Dictionary<string, int> result = new Dictionary<string, int>();
			BusinessData businessData = new BusinessData(BusinessType.PICTURE_DELETE, Context.CurrentDevice);
			stopwatch.Start();
			List<string> list = new List<string>();
			List<FileDataViewModel> list2 = allDeviceChildrens.Where((FileDataViewModel x) => x.IsChecked == true && x.Name != "Download").ToList();
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].ID);
			}
			bool num = new DeviceCommonManagement().DeleteDevFilesWithConfirm("deleteVideosById", list, ref result);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.VIDEO_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, (result["success"] > 0) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, result));
			if (num)
			{
				RefreshCommandHandler(CurDevicePath);
			}
		});
	}

	private void LoadPcChildrens(object data, bool expand, bool refresh)
	{
		FileDataViewModel selected = data as FileDataViewModel;
		if (selected == null || !selected.IsFolder)
		{
			return;
		}
		List<FileDataViewModel> childrens = new List<FileDataViewModel>();
		if (selected == null || (!refresh && CurPcSelected != null && CurPcSelected.ID.Equals(selected.ID) && CurPcSelected.Childrens.Count > 0))
		{
			return;
		}
		if (!expand)
		{
			CurPcSelected = selected;
		}
		if (selected.IsFolder)
		{
			string data2 = selected.Data;
			if (Directory.Exists(data2))
			{
				DirectoryInfo[] directories = new DirectoryInfo(data2).GetDirectories();
				foreach (DirectoryInfo directoryInfo in directories)
				{
					if (!directoryInfo.Attributes.HasFlag(FileAttributes.System) && !directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
					{
						childrens.Add(new FileDataViewModel(new TreeModel(directoryInfo.FullName, selected.ID, directoryInfo.Name, directoryInfo.FullName), selected, isFolder: true, 0L, "Directory", directoryInfo.LastWriteTime, PcFolderExpand));
					}
				}
				foreach (string item in (from m in Directory.GetFiles(data2)
					orderby m
					select m).ToList())
				{
					FileInfo fileInfo = new FileInfo(item);
					if (!fileInfo.Attributes.HasFlag(FileAttributes.System) && !fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
					{
						childrens.Add(new FileDataViewModel(new TreeModel(item, selected.ID, Path.GetFileName(item), item), selected, isFolder: false, fileInfo.Length, fileInfo.Extension, fileInfo.LastWriteTime, PcFolderExpand));
					}
				}
			}
		}
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			selected.Childrens = new ObservableCollection<TreeViewModel>(childrens);
			if (!expand)
			{
				selected.IsSelected = true;
				PcDataGridSources = new ObservableCollection<TreeViewModel>(childrens);
				CurPcPath = selected.Data;
			}
			selected.IsExpanded = true;
		});
	}

	private void SetIsCheckForCheckbox()
	{
		if (string.IsNullOrEmpty(CurDevicePath))
		{
			return;
		}
		List<FileDataViewModel> list = allDeviceChildrens.Where((FileDataViewModel x) => x.Data.Contains(CurDevicePath)).ToList();
		for (int i = 0; i < list.Count(); i++)
		{
			if (list.Where((FileDataViewModel x) => x.IsChecked == true).Count() == 0)
			{
				IsAllCheckedForCheckBox = false;
			}
			else if (list.Where((FileDataViewModel x) => x.IsChecked == true).Count() == list.Count)
			{
				IsAllCheckedForCheckBox = true;
			}
			else
			{
				IsAllCheckedForCheckBox = null;
			}
		}
	}

	private void DeviceFolderExpand(object data)
	{
		if (IsFireChanged)
		{
			LoadDeviceChildrens(data, expand: true, refresh: false);
		}
	}

	private void DeviceFolderClickCommandHandler(object data)
	{
		if (IsFireChanged)
		{
			if (data != null && data is FileDataViewModel && (backStack == null || backStack.Count == 0 || (backStack.Count > 0 && backStack.Peek() != data as FileDataViewModel)))
			{
				backStack.Push(data as FileDataViewModel);
			}
			LoadDeviceChildrens(data, expand: false, refresh: false);
		}
	}

	private void LoadDeviceChildrens(object data, bool expand, bool refresh)
	{
		FileDataViewModel selected = data as FileDataViewModel;
		if (selected == null || !selected.IsFolder)
		{
			return;
		}
		List<FileDataViewModel> childrens = new List<FileDataViewModel>();
		if (selected != null && !string.IsNullOrEmpty(selected.Data))
		{
			if (!refresh && CurPhoneSelected != null && CurPhoneSelected.ID.Equals(selected.ID) && CurPhoneSelected.Childrens.Count > 0)
			{
				return;
			}
			if (!expand)
			{
				CurPhoneSelected = selected;
			}
			if (selected.IsFolder)
			{
				List<DeviceFileInfo> filesInfo = fileManager.GetFilesInfo(selected.Data);
				if (filesInfo != null && filesInfo.Count > 0)
				{
					childrens = new List<FileDataViewModel>();
					foreach (DeviceFileInfo item in (from m in filesInfo
						orderby !m.BooleanIsFolder descending
						orderby m.RawFileName
						select m).ToList())
					{
						if (allDeviceChildrens.FindLast((FileDataViewModel x) => x.Data == item.RawFilePath && x.Name == item.RawFileName) != null)
						{
							childrens.Add(allDeviceChildrens.FindLast((FileDataViewModel x) => x.Data == item.RawFilePath && x.Name == item.RawFileName));
						}
					}
				}
			}
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				selected.Childrens = new ObservableCollection<TreeViewModel>(childrens);
				if (!expand)
				{
					selected.IsSelected = true;
					DeviceDataGridSources = new ObservableCollection<TreeViewModel>(childrens.Where((FileDataViewModel x) => !x.IsFolder));
					CurDevicePath = selected.Data;
				}
				selected.IsExpanded = true;
			});
			SetIsCheckForCheckbox();
		}
		if (DeviceDataGridSources.Where((TreeViewModel x) => x.IsChecked == true).Count() == 0)
		{
			IsCurrentCheckBoxChecked = false;
		}
		else if (DeviceDataGridSources.Where((TreeViewModel x) => x.IsChecked == false).Count() == 0)
		{
			IsCurrentCheckBoxChecked = true;
		}
		else
		{
			IsCurrentCheckBoxChecked = null;
		}
	}

	public void GoForwardCommandHandler(object data)
	{
		if (forwardStack.Count > 0)
		{
			forwardStack.Peek().IsSelected = true;
			FileDataViewModel fileDataViewModel = forwardStack.Pop();
			if ((backStack.Count > 0 && backStack.Peek() != fileDataViewModel) || backStack.Count == 0)
			{
				backStack.Push(fileDataViewModel);
			}
		}
	}

	private void GobackCommandHandler(object data)
	{
		if (backStack.Count > 1)
		{
			FileDataViewModel fileDataViewModel = backStack.Pop();
			if ((forwardStack.Count > 0 && forwardStack.Peek() != fileDataViewModel) || (forwardStack.Count == 0 && fileDataViewModel.ParentModel != null))
			{
				forwardStack.Push(fileDataViewModel);
			}
			if (backStack.Count > 0)
			{
				backStack.Peek().IsSelected = true;
			}
		}
	}

	public void RefreshCommandHandler(object data)
	{
		lock (this)
		{
			if (!FileRefreshToolBtnEnable)
			{
				return;
			}
			FileRefreshToolBtnEnable = false;
		}
		DeviceDataGridSources.Clear();
		DeviceFileCount = 0;
		DeviceSelectFileCount = 0;
		LoadDeviceTreeAsync();
		LoadDeviceChildrens(CurPhoneSelected, expand: true, refresh: true);
	}

	private void SearchPcCommandHandler(object data)
	{
		if (data == null || CurPcSelected == null || !CurPcSelected.HasChildrens)
		{
			return;
		}
		string searchText = data.ToString();
		List<TreeViewModel> list = CurPcSelected.Childrens.ToList();
		if (!string.IsNullOrEmpty(searchText))
		{
			list = CurPcSelected.Childrens.Where((TreeViewModel n) => Regex.IsMatch(n.Name, searchText, RegexOptions.IgnoreCase)).ToList();
		}
		PcDataGridSources = new ObservableCollection<TreeViewModel>(list);
	}

	public void SearchDeviceCommandHandler(object data)
	{
		if (data == null || CurPhoneSelected == null || !CurPhoneSelected.HasChildrens)
		{
			return;
		}
		string searchText = data.ToString();
		List<TreeViewModel> list = CurPhoneSelected.Childrens.ToList();
		if (!string.IsNullOrEmpty(searchText))
		{
			list = CurPhoneSelected.Childrens.Where((TreeViewModel n) => Regex.IsMatch(n.Name, searchText, RegexOptions.IgnoreCase)).ToList();
		}
		DeviceDataGridSources = new ObservableCollection<TreeViewModel>(list);
	}

	private void PcEnterPathCommandHandler(object data)
	{
		string path = data?.ToString();
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		IsFireChanged = false;
		TreeViewModel treeViewModel = PcTreeSources.FirstOrDefault((TreeViewModel n) => path.StartsWith(n.Data, StringComparison.CurrentCultureIgnoreCase));
		LoadPcChildrens(treeViewModel, expand: false, refresh: true);
		string[] array = path.Substring(treeViewModel.Data.Length).Split(new char[1] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
		List<TreeViewModel> list = treeViewModel.Childrens.ToList();
		string[] array2 = array;
		foreach (string text in array2)
		{
			foreach (TreeViewModel item in list)
			{
				if (text.Equals(item.Model.Name, StringComparison.CurrentCultureIgnoreCase))
				{
					item.ParentModel.IsSelected = false;
					LoadPcChildrens(item, expand: false, refresh: true);
					list = item.Childrens.ToList();
					break;
				}
				if (list.Count == 0)
				{
					break;
				}
			}
		}
		IsFireChanged = true;
	}

	private void TraverseChildren(FileDataViewModel parent)
	{
		if (parent == null || string.IsNullOrEmpty(parent.Data) || !parent.IsFolder)
		{
			return;
		}
		List<DeviceFileInfo> filesInfo = fileManager.GetFilesInfo(parent.Data);
		if (filesInfo == null || filesInfo.Count <= 0)
		{
			return;
		}
		foreach (DeviceFileInfo item in (from m in filesInfo
			orderby !m.BooleanIsFolder descending
			orderby m.RawFileName
			select m).ToList())
		{
			FileDataViewModel fileDataViewModel = new FileDataViewModel(new TreeModel(item.RawFilePath, parent.ID, item.RawFileName, item.RawFilePath), parent, item.BooleanIsFolder, item.LongFileSize, item.RawFileType, item.DateTimeModifiedDate, DeviceFolderExpand);
			if (allDeviceChildrens.Contains(fileDataViewModel))
			{
				continue;
			}
			if (!fileDataViewModel.IsFolder)
			{
				allDeviceChildrens.Add(fileDataViewModel);
			}
			if (fileDataViewModel.IsFolder)
			{
				count = 0;
				FindChild(fileDataViewModel);
				fileDataViewModel.ChildrenCount = count;
				if (fileDataViewModel.ChildrenCount > 0)
				{
					allDeviceChildrens.Add(fileDataViewModel);
				}
				if ((from x in fileManager.GetFilesInfo(fileDataViewModel.Data)
					where x.BooleanIsFolder
					select x).Count() == 0)
				{
					fileDataViewModel.IsVisible = Visibility.Hidden;
				}
			}
		}
	}

	public void FindChild(FileDataViewModel model)
	{
		if (!model.IsFolder)
		{
			return;
		}
		List<DeviceFileInfo> list = fileManager.GetFilesInfo(model.Data).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].BooleanIsFolder)
			{
				(from x in fileManager.GetFilesInfo(model.Data)
					where !x.BooleanIsFolder
					select x).ToList();
				FileDataViewModel model2 = new FileDataViewModel(new TreeModel(list[i].RawFilePath, model.ID, list[i].RawFileName, list[i].RawFilePath), model, list[i].BooleanIsFolder, list[i].LongFileSize, list[i].RawFileType, list[i].DateTimeModifiedDate, DeviceFolderExpand);
				FindChild(model2);
			}
			else
			{
				count++;
			}
		}
	}

	private void OnCheckedCommandHandler(object data)
	{
		FileDataViewModel selected = null;
		if (data is CheckBox)
		{
			selected = (data as CheckBox).DataContext as FileDataViewModel;
			selected.IsChecked = (data as CheckBox).IsChecked;
		}
		else
		{
			selected = data as FileDataViewModel;
		}
		selected.IsExpanded = true;
		selected.IsSelected = true;
		if (selected.Name == "Download")
		{
			for (int i = 0; i < allDeviceChildrens.Count; i++)
			{
				allDeviceChildrens[i].IsChecked = selected.IsChecked;
			}
		}
		if (selected != null && selected.IsFolder)
		{
			List<FileDataViewModel> list = allDeviceChildrens.Where((FileDataViewModel x) => x.Data.Contains(selected.Data + "/")).ToList();
			for (int j = 0; j < list.Count; j++)
			{
				list[j].IsChecked = selected.IsChecked;
			}
		}
		if (selected != null && selected.IsChecked != true)
		{
			TreeViewModel parent = null;
			do
			{
				parent = selected.ParentModel;
				if (parent != null)
				{
					if (parent.Childrens.Where((TreeViewModel x) => x.IsChecked != false).Count() > 0)
					{
						parent.IsChecked = null;
					}
					else
					{
						parent.IsChecked = false;
					}
					selected = allDeviceChildrens.FindLast((FileDataViewModel x) => x.Data == parent.Data && x.Name == parent.Name);
				}
			}
			while (parent != null && parent.Name != "Download");
		}
		if (selected != null && selected.IsChecked == true)
		{
			TreeViewModel parent2 = null;
			do
			{
				parent2 = selected.ParentModel;
				if (parent2 == null)
				{
					continue;
				}
				bool flag = true;
				for (int k = 0; k < parent2.Childrens.Count; k++)
				{
					if (parent2.Childrens[k].IsChecked != true)
					{
						flag = false;
					}
				}
				if (flag)
				{
					parent2.IsChecked = true;
				}
				else
				{
					parent2.IsChecked = null;
				}
				selected = allDeviceChildrens.FindLast((FileDataViewModel x) => x.Data == parent2.Data && x.Name == parent2.Name);
			}
			while (parent2 != null && parent2.Name != "Download");
		}
		SetIsCheckForCheckbox();
		DeviceSelectFileCount = allDeviceChildrens.Where((FileDataViewModel x) => !x.IsFolder && x.IsChecked == true).Count();
		if (DeviceDataGridSources.Where((TreeViewModel x) => x.IsChecked == true).Count() == 0)
		{
			IsCurrentCheckBoxChecked = false;
		}
		else if (DeviceDataGridSources.Where((TreeViewModel x) => x.IsChecked == false).Count() == 0)
		{
			IsCurrentCheckBoxChecked = true;
		}
		else
		{
			IsCurrentCheckBoxChecked = null;
		}
	}

	private void DeviceEnterPathCommandHandler(object data)
	{
		string path = data?.ToString();
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		IsFireChanged = false;
		TreeViewModel treeViewModel = DeviceTreeSources.FirstOrDefault((TreeViewModel n) => path.StartsWith(n.Data, StringComparison.CurrentCultureIgnoreCase));
		if (backStack == null || backStack.Count == 0 || (backStack.Count > 0 && backStack.Peek() != allDeviceChildrens.Where((FileDataViewModel x) => x.Data == path).SingleOrDefault()))
		{
			backStack.Push(allDeviceChildrens.Where((FileDataViewModel x) => x.Data == path).SingleOrDefault());
		}
		LoadDeviceChildrens(treeViewModel, expand: false, refresh: true);
		string[] array = path.Substring(treeViewModel.Data.Length).Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		List<TreeViewModel> list = treeViewModel.Childrens.ToList();
		string[] array2 = array;
		foreach (string text in array2)
		{
			foreach (TreeViewModel item in list)
			{
				if (text.Equals(item.Model.Name, StringComparison.CurrentCultureIgnoreCase))
				{
					item.ParentModel.IsSelected = false;
					LoadDeviceChildrens(item, expand: false, refresh: true);
					list = item.Childrens.ToList();
					break;
				}
				if (list.Count == 0)
				{
					break;
				}
			}
		}
		IsFireChanged = true;
	}
}
