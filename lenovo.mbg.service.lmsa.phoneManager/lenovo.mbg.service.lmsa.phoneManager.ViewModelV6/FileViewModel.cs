using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.ModelV6;
using lenovo.themes.generic;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class FileViewModel : ViewModelBase
{
	protected DeviceFileManager fileManager;

	protected TreeViewModel CurPcSelected;

	protected TreeViewModel CurPhoneSelected;

	protected bool IsFireChanged = true;

	private long _transferLocker;

	private string _CurPcPath;

	private string _CurDevicePath;

	private ObservableCollection<TreeViewModel> _PcTreeSources = new ObservableCollection<TreeViewModel>();

	private ObservableCollection<TreeViewModel> _DeviceTreeSources = new ObservableCollection<TreeViewModel>();

	private ObservableCollection<TreeViewModel> _PcDataGridSources = new ObservableCollection<TreeViewModel>();

	private ObservableCollection<TreeViewModel> _DeviceDataGridSources = new ObservableCollection<TreeViewModel>();

	public ReplayCommand PcFolderClickCommand { get; }

	public ReplayCommand DeviceFolderClickCommand { get; }

	public ReplayCommand GobackCommand { get; }

	public ReplayCommand RefreshCommand { get; }

	public ReplayCommand SearchPcCommand { get; }

	public ReplayCommand SearchDeviceCommand { get; }

	public ReplayCommand PcEnterPathCommand { get; }

	public ReplayCommand DeviceEnterPathCommand { get; }

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

	public FileViewModel()
	{
		PcFolderClickCommand = new ReplayCommand(PcFolderClickCommandHandler);
		DeviceFolderClickCommand = new ReplayCommand(DeviceFolderClickCommandHandler);
		GobackCommand = new ReplayCommand(GobackCommandHandler);
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		SearchPcCommand = new ReplayCommand(SearchPcCommandHandler);
		SearchDeviceCommand = new ReplayCommand(SearchDeviceCommandHandler);
		PcEnterPathCommand = new ReplayCommand(PcEnterPathCommandHandler);
		DeviceEnterPathCommand = new ReplayCommand(DeviceEnterPathCommandHandler);
		fileManager = new DeviceFileManager();
	}

	public override void LoadData(object data)
	{
		CurPcSelected = null;
		CurPhoneSelected = null;
		PcDataGridSources.Clear();
		DeviceDataGridSources.Clear();
		LoadPcTreeAsync();
		LoadDeviceTreeAsync();
		base.LoadData(data);
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
		if (!(CurPcSelected is FileDataViewModel fileDataViewModel))
		{
			return false;
		}
		string pcFocusedDir = fileDataViewModel.Data;
		try
		{
			string targetFileFullName = data;
			TransferProgressWiew wnd = new TransferProgressWiew();
			string text = HostProxy.LanguageService.Translate("K0629");
			string text2 = ((fileInfo.Name.Length >= 45) ? (fileInfo.Name.Substring(0, 42) + "...") : fileInfo.Name);
			wnd.SetProgressTitle(text2 + " ({0}/{1})", text + "...");
			AsyncDataLoader.BeginLoading(delegate
			{
				if (fileManager.Export(targetFileFullName, pcFocusedDir, wnd.ProgressUpdate))
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

	private Task LoadDeviceTreeAsync()
	{
		return Task.Run(delegate
		{
			DeviceTreeSources = new ObservableCollection<TreeViewModel>();
			string internalPath = string.Empty;
			string externalPath = string.Empty;
			fileManager.GetInternalAndExternamPath(out internalPath, out externalPath);
			List<TreeViewModel> list = new List<TreeViewModel>();
			if (!string.IsNullOrEmpty(internalPath) && internalPath.Length > 0 && internalPath[0].Equals('/'))
			{
				list.Add(new FileDataViewModel(new TreeModel(internalPath, null, "Internal storage", internalPath), null, isFolder: true, 0L, null, null, DeviceFolderExpand));
			}
			HostProxy.CurrentDispatcher.Invoke(() => DeviceTreeSources = new ObservableCollection<TreeViewModel>(list));
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
		if (selected == null || string.IsNullOrEmpty(selected.Data) || (!refresh && CurPhoneSelected != null && CurPhoneSelected.ID.Equals(selected.ID) && CurPhoneSelected.Childrens.Count > 0))
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
				foreach (DeviceFileInfo item in (from m in filesInfo
					orderby !m.BooleanIsFolder descending
					orderby m.RawFileName
					select m).ToList())
				{
					childrens.Add(new FileDataViewModel(new TreeModel(item.RawFilePath, selected.ID, item.RawFileName, item.RawFilePath), selected, item.BooleanIsFolder, item.LongFileSize, item.RawFileType, item.DateTimeModifiedDate, DeviceFolderExpand));
				}
			}
		}
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			selected.Childrens = new ObservableCollection<TreeViewModel>(childrens);
			if (!expand)
			{
				selected.IsSelected = true;
				DeviceDataGridSources = new ObservableCollection<TreeViewModel>(childrens);
				CurDevicePath = selected.Data;
			}
			selected.IsExpanded = true;
		});
	}

	private void GobackCommandHandler(object data)
	{
		int num = int.Parse(data.ToString());
		if (num == 1 && CurPcSelected != null && CurPcSelected.ParentModel != null)
		{
			CurPcSelected.ParentModel.IsSelected = true;
		}
		else if (num == 2 && CurPhoneSelected != null && CurPhoneSelected.ParentModel != null)
		{
			CurPhoneSelected.ParentModel.IsSelected = true;
		}
	}

	private void RefreshCommandHandler(object data)
	{
		int num = int.Parse(data.ToString());
		if (num == 1 && CurPcSelected != null)
		{
			LoadPcChildrens(CurPcSelected, expand: false, refresh: true);
		}
		else if (num == 2 && CurPhoneSelected != null)
		{
			LoadDeviceChildrens(CurPhoneSelected, expand: false, refresh: true);
		}
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

	private void DeviceEnterPathCommandHandler(object data)
	{
		string path = data?.ToString();
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		IsFireChanged = false;
		TreeViewModel treeViewModel = DeviceTreeSources.FirstOrDefault((TreeViewModel n) => path.StartsWith(n.Data, StringComparison.CurrentCultureIgnoreCase));
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
