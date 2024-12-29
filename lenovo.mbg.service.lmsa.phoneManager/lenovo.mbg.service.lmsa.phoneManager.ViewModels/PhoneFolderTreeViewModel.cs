using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PhoneFolderTreeViewModel : FolderTreeViewModel
{
	private DeviceFileSystemManager _deviceFileSystemManager;

	public Action<FolderFileViewModel> SelectionHandler { get; private set; }

	public bool InternalStroageFocused
	{
		get
		{
			if (base.SelectedNode == null)
			{
				return true;
			}
			_ = string.Empty;
			FolderFileViewModel parent = base.SelectedNode;
			while (parent.Parent != null)
			{
				parent = parent.Parent;
			}
			return parent.UpperName == "INTERNAL_STORAGE";
		}
	}

	public PhoneFolderTreeViewModel(Action<FolderFileViewModel> selectionHandler)
	{
		SelectionHandler = selectionHandler;
		_deviceFileSystemManager = new DeviceFileSystemManager();
	}

	protected override List<FolderFileViewModel> DoGetChildNodes(FolderFileViewModel node)
	{
		string empty = string.Empty;
		if (node == null || string.IsNullOrEmpty(empty = node.Path))
		{
			return null;
		}
		List<FileInfo> filesInfo = _deviceFileSystemManager.GetFilesInfo(empty);
		return createFolderFileViewModel(filesInfo);
	}

	private List<FolderFileViewModel> createFolderFileViewModel(List<FileInfo> files)
	{
		if (files == null || files.Count == 0)
		{
			return null;
		}
		List<FolderFileViewModel> list = new List<FolderFileViewModel>();
		foreach (FileInfo item in from m in files
			where m.BooleanIsFolder
			orderby m.RawFileName
			select m)
		{
			FolderFileViewModel folderFileViewModel = new FolderFileViewModel(isFolder: true);
			folderFileViewModel.IsExpanded = false;
			folderFileViewModel.IsSelected = false;
			folderFileViewModel.Path = item.RawFilePath;
			folderFileViewModel.DisplayName = item.RawFileName;
			folderFileViewModel.UpperName = item.RawFileName;
			folderFileViewModel.FileSize = item.LongFileSize;
			folderFileViewModel.UpdateDate = item.DateTimeModifiedDate;
			folderFileViewModel.FileType = item.RawFileType;
			list.Add(folderFileViewModel);
		}
		foreach (FileInfo item2 in from m in files
			where !m.BooleanIsFolder
			orderby m.RawFileName
			select m)
		{
			FolderFileViewModel folderFileViewModel2 = new FolderFileViewModel(isFolder: false);
			folderFileViewModel2.IsExpanded = false;
			folderFileViewModel2.IsSelected = false;
			folderFileViewModel2.Path = item2.RawFilePath;
			folderFileViewModel2.DisplayName = item2.RawFileName;
			folderFileViewModel2.UpperName = item2.RawFileName;
			folderFileViewModel2.FileSize = item2.LongFileSize;
			folderFileViewModel2.UpdateDate = item2.DateTimeModifiedDate;
			folderFileViewModel2.FileType = item2.RawFileType;
			list.Add(folderFileViewModel2);
		}
		return list;
	}

	private FolderFileViewModel createFolderFileViewModel(FileInfo file)
	{
		if (file == null)
		{
			return null;
		}
		if (file.BooleanIsFolder)
		{
			return new FolderFileViewModel(isFolder: true)
			{
				IsExpanded = false,
				IsSelected = false,
				Path = file.RawFilePath,
				DisplayName = file.RawFileName,
				UpperName = file.RawFileName,
				FileSize = file.LongFileSize,
				UpdateDate = file.DateTimeModifiedDate,
				FileType = file.RawFileType
			};
		}
		return new FolderFileViewModel(isFolder: false)
		{
			IsExpanded = false,
			IsSelected = false,
			Path = file.RawFilePath,
			DisplayName = file.RawFileName,
			UpperName = file.RawFileName,
			FileSize = file.LongFileSize,
			UpdateDate = file.DateTimeModifiedDate,
			FileType = file.RawFileType
		};
	}

	public override char GetDirectorySeparatorChar()
	{
		return '/';
	}

	public override List<FolderFileViewModel> GetRootDirectories()
	{
		string internalPath = string.Empty;
		string externalPath = string.Empty;
		_deviceFileSystemManager.GetInternalAndExternamPath(out internalPath, out externalPath);
		List<FolderFileViewModel> list = new List<FolderFileViewModel>();
		if (!string.IsNullOrEmpty(internalPath) && internalPath.Length > 0 && internalPath[0].Equals('/'))
		{
			FolderFileViewModel folderFileViewModel = new FolderFileViewModel(isFolder: true);
			folderFileViewModel.UpperName = internalPath;
			folderFileViewModel.DisplayName = LangTranslation.Translate("K0592");
			folderFileViewModel.Parent = null;
			folderFileViewModel.IsExpanded = false;
			folderFileViewModel.IsSelected = false;
			folderFileViewModel.Path = internalPath;
			list.Add(folderFileViewModel);
		}
		externalPath = string.Empty;
		if (!string.IsNullOrEmpty(externalPath) && externalPath.Length > 0 && externalPath[0].Equals('/'))
		{
			FolderFileViewModel folderFileViewModel2 = new FolderFileViewModel(isFolder: true);
			folderFileViewModel2.UpperName = externalPath;
			folderFileViewModel2.DisplayName = "K0593";
			folderFileViewModel2.Parent = null;
			folderFileViewModel2.IsExpanded = false;
			folderFileViewModel2.IsSelected = false;
			folderFileViewModel2.Path = externalPath;
			list.Add(folderFileViewModel2);
		}
		return list;
	}

	protected override void OnSelectedChanged(FolderFileViewModel item, bool prevSelected)
	{
		base.OnSelectedChanged(item, prevSelected);
		SelectionHandler?.Invoke(item);
	}

	protected override void DoSearch(FolderFileViewModel node, string keywords, CancellationTokenSource cancellationTokenSource, Action<FolderFileViewModel> itemFoundCallback)
	{
		try
		{
			string empty = string.Empty;
			if (node != null && !string.IsNullOrEmpty(empty = node.Path))
			{
				_deviceFileSystemManager.SearchPhone(empty, keywords, delegate(FileInfo item)
				{
					itemFoundCallback?.Invoke(createFolderFileViewModel(item));
				});
			}
		}
		catch (Exception)
		{
		}
	}
}
