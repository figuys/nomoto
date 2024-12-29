using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PCFolderTreeViewModel : FolderTreeViewModel
{
	public Action<FolderFileViewModel> SelectionHandler { get; private set; }

	public PCFolderTreeViewModel(Action<FolderFileViewModel> selectionHandler)
	{
		SelectionHandler = selectionHandler;
	}

	private FolderFileViewModel GetFavoriteDir()
	{
		try
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			FolderFileViewModel folderFileViewModel = new FolderFileViewModel(isFolder: true);
			new DirectoryInfo(folderPath);
			folderFileViewModel.Parent = null;
			folderFileViewModel.IsExpanded = false;
			folderFileViewModel.IsSelected = false;
			folderFileViewModel.Path = folderPath;
			return folderFileViewModel;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Get favorites folder throw exception:" + ex.ToString());
			return null;
		}
	}

	public override List<FolderFileViewModel> GetRootDirectories()
	{
		try
		{
			List<FolderFileViewModel> list = new List<FolderFileViewModel>();
			string[] logicalDrives = Directory.GetLogicalDrives();
			foreach (string text in logicalDrives)
			{
				FolderFileViewModel folderFileViewModel = new FolderFileViewModel(isFolder: true);
				string upperName = (folderFileViewModel.DisplayName = text.Trim(Path.DirectorySeparatorChar));
				folderFileViewModel.UpperName = upperName;
				folderFileViewModel.Parent = null;
				folderFileViewModel.IsExpanded = false;
				folderFileViewModel.IsSelected = false;
				folderFileViewModel.Path = text;
				list.Add(folderFileViewModel);
			}
			FolderFileViewModel favoriteDir = GetFavoriteDir();
			if (favoriteDir != null)
			{
				list.Add(favoriteDir);
			}
			return list;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Get root dir throw exception:" + ex.ToString());
		}
		return null;
	}

	protected override List<FolderFileViewModel> DoGetChildNodes(FolderFileViewModel node)
	{
		string empty = string.Empty;
		if (node == null || string.IsNullOrEmpty(empty = node.Path))
		{
			return null;
		}
		if (!Directory.Exists(empty))
		{
			return null;
		}
		try
		{
			IOrderedEnumerable<string> orderedEnumerable = from m in Directory.GetDirectories(empty)
				orderby m
				select m;
			List<FolderFileViewModel> list = new List<FolderFileViewModel>();
			foreach (string item in orderedEnumerable)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(item);
				if (directoryInfo.Exists)
				{
					FolderFileViewModel folderFileViewModel = new FolderFileViewModel(isFolder: true);
					folderFileViewModel.IsExpanded = false;
					folderFileViewModel.IsSelected = false;
					folderFileViewModel.Path = item;
					folderFileViewModel.FileSize = 0L;
					folderFileViewModel.UpdateDate = directoryInfo.LastWriteTime;
					folderFileViewModel.FileType = "File folder";
					list.Add(folderFileViewModel);
				}
			}
			foreach (string item2 in from m in Directory.GetFiles(empty)
				orderby m
				select m)
			{
				FileInfo fileInfo = new FileInfo(item2);
				if (fileInfo.Exists)
				{
					FolderFileViewModel folderFileViewModel2 = new FolderFileViewModel(isFolder: false);
					folderFileViewModel2.IsExpanded = false;
					folderFileViewModel2.IsSelected = false;
					folderFileViewModel2.Path = item2;
					folderFileViewModel2.FileSize = fileInfo.Length;
					folderFileViewModel2.UpdateDate = fileInfo.LastWriteTime;
					string extension = fileInfo.Extension;
					if (!string.IsNullOrEmpty(extension))
					{
						folderFileViewModel2.FileType = $"{extension.ToUpper().TrimStart('.')} file";
					}
					list.Add(folderFileViewModel2);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error($"Get [{empty}]sub dir throw exception:{ex.ToString()}");
		}
		return null;
	}

	protected override void OnSelectedChanged(FolderFileViewModel item, bool prevSelected)
	{
		base.OnSelectedChanged(item, prevSelected);
		SelectionHandler?.Invoke(item);
	}

	public override char GetDirectorySeparatorChar()
	{
		return Path.DirectorySeparatorChar;
	}

	protected override void DoSearch(FolderFileViewModel node, string keywords, CancellationTokenSource cancellationTokenSource, Action<FolderFileViewModel> itemFoundCallback)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(base.SelectedNode.Path);
		if (!directoryInfo.Exists)
		{
			return;
		}
		string value = keywords.ToUpper();
		Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>();
		queue.Enqueue(directoryInfo);
		while (queue.Count != 0)
		{
			DirectoryInfo directoryInfo2 = queue.Dequeue();
			try
			{
				DirectoryInfo[] directories = directoryInfo2.GetDirectories();
				foreach (DirectoryInfo directoryInfo3 in directories)
				{
					if (directoryInfo3.Name.ToUpper().Contains(value))
					{
						FolderFileViewModel folderFileViewModel = new FolderFileViewModel(isFolder: true);
						folderFileViewModel.IsExpanded = false;
						folderFileViewModel.IsSelected = false;
						folderFileViewModel.Path = directoryInfo3.FullName;
						folderFileViewModel.FileSize = 0L;
						folderFileViewModel.UpdateDate = directoryInfo3.LastWriteTime;
						folderFileViewModel.FileType = "File folder";
						itemFoundCallback?.Invoke(folderFileViewModel);
					}
					queue.Enqueue(directoryInfo3);
				}
				FileInfo[] files = directoryInfo2.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					if (fileInfo.Name.ToUpper().Contains(value))
					{
						FolderFileViewModel folderFileViewModel2 = new FolderFileViewModel(isFolder: false);
						folderFileViewModel2.IsExpanded = false;
						folderFileViewModel2.IsSelected = false;
						folderFileViewModel2.Path = fileInfo.FullName;
						folderFileViewModel2.FileSize = fileInfo.Length;
						folderFileViewModel2.UpdateDate = fileInfo.LastWriteTime;
						string extension = fileInfo.Extension;
						if (!string.IsNullOrEmpty(extension))
						{
							folderFileViewModel2.FileType = $"{extension.ToUpper().TrimStart('.')} file";
						}
						itemFoundCallback?.Invoke(folderFileViewModel2);
					}
				}
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Error($"PCFolderTreeViewModel.DoSearch failed, will skip this directory{directoryInfo2?.FullName}, exception:{arg}");
			}
		}
	}
}
