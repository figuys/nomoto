using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public abstract class FolderTreeViewModel : ViewModelBase
{
	protected class SearchCacheModel : IDisposable
	{
		public string Path { get; set; }

		public string SearchKeywords { get; set; }

		public CancellationTokenSource CancellationToken { get; set; }

		public void Dispose()
		{
			CancellationToken?.Dispose();
		}

		public override string ToString()
		{
			return $"Path:{Path}, SearchKeywords:{SearchKeywords}, CancellationToken.IsCancellationRequested:{CancellationToken?.IsCancellationRequested}";
		}
	}

	private volatile bool isTraceBacking;

	private ConcurrentStack<string> nodeChangedTraceStack = new ConcurrentStack<string>();

	private ConcurrentDictionary<string, SearchCacheModel> searchCache = new ConcurrentDictionary<string, SearchCacheModel>();

	private volatile bool isSearching;

	private string searchKeywords;

	private FolderFileViewModel selectedNode;

	private bool navigationStoreyGobackIsEnabled;

	private ReplayCommand navigationStoreyGobackCommand;

	private bool navigationTraceGobackIsEnabled;

	private ReplayCommand navigationTraceGobackCommand;

	private ReplayCommand refreshCommand;

	private string focusedPath;

	private bool enterPathButtonIsEnabled;

	private ReplayCommand enterPathCommand;

	private ReplayCommand searchCommand;

	private Visibility dataGridPathColVisibility;

	public virtual ObservableCollection<FolderFileViewModel> TreeItems { get; private set; }

	public bool IsSearching
	{
		get
		{
			return isSearching;
		}
		set
		{
			if (isSearching != value)
			{
				isSearching = value;
				OnPropertyChanged("IsSearching");
			}
		}
	}

	public string SearchKeywords
	{
		get
		{
			return searchKeywords;
		}
		set
		{
			if (!(searchKeywords == value))
			{
				searchKeywords = value;
				OnPropertyChanged("SearchKeywords");
			}
		}
	}

	public FolderFileViewModel SelectedNode
	{
		get
		{
			return selectedNode;
		}
		set
		{
			if (selectedNode != value)
			{
				selectedNode = value;
				OnPropertyChanged("SelectedNode");
			}
		}
	}

	public bool NavigationStoreyGobackIsEnabled
	{
		get
		{
			return navigationStoreyGobackIsEnabled;
		}
		set
		{
			if (navigationStoreyGobackIsEnabled != value)
			{
				navigationStoreyGobackIsEnabled = value;
				OnPropertyChanged("NavigationStoreyGobackIsEnabled");
			}
		}
	}

	public ReplayCommand NavigationStoreyGobackCommand
	{
		get
		{
			return navigationStoreyGobackCommand;
		}
		set
		{
			if (navigationStoreyGobackCommand != value)
			{
				navigationStoreyGobackCommand = value;
				OnPropertyChanged("NavigationStoreyGobackCommand");
			}
		}
	}

	public bool NavigationTraceGobackIsEnabled
	{
		get
		{
			return navigationTraceGobackIsEnabled;
		}
		set
		{
			if (navigationTraceGobackIsEnabled != value)
			{
				navigationTraceGobackIsEnabled = value;
				OnPropertyChanged("NavigationTraceGobackIsEnabled");
			}
		}
	}

	public ReplayCommand NavigationTraceGobackCommand
	{
		get
		{
			return navigationTraceGobackCommand;
		}
		set
		{
			if (navigationTraceGobackCommand != value)
			{
				navigationTraceGobackCommand = value;
				OnPropertyChanged("NavigationTraceGobackCommand");
			}
		}
	}

	public ReplayCommand RefreshCommand
	{
		get
		{
			return refreshCommand;
		}
		set
		{
			if (refreshCommand != value)
			{
				refreshCommand = value;
				OnPropertyChanged("RefreshCommand");
			}
		}
	}

	public string FocusedPath
	{
		get
		{
			return focusedPath;
		}
		set
		{
			if (!(focusedPath == value))
			{
				focusedPath = value;
				OnPropertyChanged("FocusedPath");
			}
		}
	}

	public bool EnterPathButtonIsEnabled
	{
		get
		{
			return enterPathButtonIsEnabled;
		}
		set
		{
			if (enterPathButtonIsEnabled != value)
			{
				enterPathButtonIsEnabled = value;
				OnPropertyChanged("EnterPathButtonIsEnabled");
			}
		}
	}

	public ReplayCommand EnterPathCommand
	{
		get
		{
			return enterPathCommand;
		}
		set
		{
			if (enterPathCommand != value)
			{
				enterPathCommand = value;
				OnPropertyChanged("EnterPathCommand");
			}
		}
	}

	public ReplayCommand SearchCommand
	{
		get
		{
			return searchCommand;
		}
		set
		{
			if (searchCommand != value)
			{
				searchCommand = value;
				OnPropertyChanged("SearchCommand");
			}
		}
	}

	public Visibility DataGridPathColVisibility
	{
		get
		{
			return dataGridPathColVisibility;
		}
		set
		{
			if (dataGridPathColVisibility != value)
			{
				dataGridPathColVisibility = value;
				OnPropertyChanged("DataGridPathColVisibility");
			}
		}
	}

	public FolderTreeViewModel()
	{
		TreeItems = new ObservableCollection<FolderFileViewModel>();
		NavigationStoreyGobackCommand = new ReplayCommand(NavigationStoreyGobackCommandHandler);
		NavigationTraceGobackCommand = new ReplayCommand(NavigationTraceGobackCommandHandler);
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		EnterPathCommand = new ReplayCommand(EnterPathCommandHandler);
		SearchCommand = new ReplayCommand(SearchCommandHandler);
	}

	public abstract char GetDirectorySeparatorChar();

	public abstract List<FolderFileViewModel> GetRootDirectories();

	public List<FolderFileViewModel> GetChildNodes(FolderFileViewModel node)
	{
		LogHelper.LogInstance.Info($"GetChildNodes entered, params[node:{node}]");
		List<FolderFileViewModel> list = DoGetChildNodes(node);
		if (list != null)
		{
			foreach (FolderFileViewModel item in list)
			{
				item.Parent = node;
			}
		}
		LogHelper.LogInstance.Info($"GetChildNodes exited, result list size:[{list?.Count}]");
		return list;
	}

	protected abstract List<FolderFileViewModel> DoGetChildNodes(FolderFileViewModel node);

	public virtual void BindingRootNodes(List<FolderFileViewModel> nodes)
	{
		LogHelper.LogInstance.Info($"BindingRootNodes entered, params[nodes size:{nodes?.Count}]");
		if (nodes != null)
		{
			DataGridPathColVisibility = Visibility.Collapsed;
			TreeItems.Clear();
			foreach (FolderFileViewModel node in nodes)
			{
				node.SelectionHandler = OnSelectedChanged;
				node.ExpandHandler = OnExpandChanged;
				if (node.IsFolder)
				{
					TreeItems.Add(node);
				}
			}
		}
		LogHelper.LogInstance.Info("BindingRootNodes exited");
	}

	public virtual void BindingChildNodes(FolderFileViewModel parent, List<FolderFileViewModel> nodes)
	{
		LogHelper.LogInstance.Info($"BindingChildNodes entered, params[parent:{parent},nodes size:{nodes?.Count}]");
		if (parent == null || nodes == null)
		{
			return;
		}
		DataGridPathColVisibility = Visibility.Collapsed;
		parent.Clear();
		foreach (FolderFileViewModel node in nodes)
		{
			node.SelectionHandler = OnSelectedChanged;
			node.ExpandHandler = OnExpandChanged;
			parent.AddChild(node);
		}
		LogHelper.LogInstance.Info("BindingChildNodes exited");
	}

	public virtual void Refresh(FolderFileViewModel node)
	{
		LogHelper.LogInstance.Info($"Refresh entered, internal params[node:{node}]");
		if (node != null)
		{
			node.IsExpanded = false;
			string path = node?.Path;
			if (RemoveSearchCache(path) != null)
			{
				SearchKeywords = string.Empty;
			}
			BindingChildNodes(node, GetChildNodes(node));
		}
		LogHelper.LogInstance.Info("Refresh exited");
	}

	protected virtual void OnSelectedChanged(FolderFileViewModel node, bool prevSelected)
	{
		LogHelper.LogInstance.Info($"OnSelectedChanged entered, params[node:{node}, prevSelected:{prevSelected}]");
		if (!prevSelected)
		{
			SearchKeywords = string.Empty;
			string path = (FocusedPath = node?.Path);
			RecordNodeChangedTraceIfNeed(path);
			SelectedNode = node;
			if (node != null)
			{
				if (!node.IsSelected)
				{
					GetSearchCache(node.Path)?.CancellationToken?.Cancel();
				}
				NavigationStoreyGobackIsEnabled = node.Parent != null;
				if (node.IsFolder && node.IsSelected)
				{
					string keywords = string.Empty;
					if (isTraceBacking)
					{
						SearchCacheModel searchCacheModel = GetSearchCache(path);
						if (searchCacheModel != null)
						{
							string text3 = (SearchKeywords = searchCacheModel.SearchKeywords);
							keywords = text3;
						}
					}
					Search(node, keywords);
				}
			}
			else
			{
				NavigationStoreyGobackIsEnabled = false;
			}
		}
		LogHelper.LogInstance.Info("OnSelectedChanged exited");
	}

	protected virtual void OnExpandChanged(FolderFileViewModel node, bool prevExpanded)
	{
		LogHelper.LogInstance.Info($"OnExpandChanged entered, params[node:{node}, prevExpanded:{prevExpanded}]");
		if (node != null && node.IsFolder && node.IsExpanded && !prevExpanded)
		{
			if (RemoveSearchCache(node.Path) != null)
			{
				SearchKeywords = string.Empty;
			}
			BindingChildNodes(node, GetChildNodes(node));
		}
		LogHelper.LogInstance.Info("OnExpandChanged exited");
	}

	public virtual void ExpandAndSelectFolder(FolderFileViewModel node)
	{
		LogHelper.LogInstance.Info($"ExpandAndSelectFolder entered, params[node:{node}]");
		EnterFolder(node);
		LogHelper.LogInstance.Info("ExpandAndSelectFolder exited");
	}

	public virtual FolderFileViewModel EnterFolder(string path)
	{
		LogHelper.LogInstance.Info($"EnterFolder entered, params[path:{path},TreeItems size:{TreeItems?.Count}]");
		FolderFileViewModel folderFileViewModel = null;
		if (TreeItems != null && !string.IsNullOrEmpty(path))
		{
			string upperPath = path.ToUpper();
			List<FolderFileViewModel> list = TreeItems.Where((FolderFileViewModel m) => upperPath.StartsWith(m.UpperName)).ToList();
			LogHelper.LogInstance.Info($"matchedRootDir size{list?.Count}");
			if (list.Count == 1)
			{
				char directorySeparatorChar = GetDirectorySeparatorChar();
				FolderFileViewModel folderFileViewModel2 = list[0];
				folderFileViewModel2.IsExpanded = false;
				folderFileViewModel2.IsExpanded = true;
				List<FolderFileViewModel> list2 = folderFileViewModel2.ChildFolders?.ToList();
				string[] array = upperPath.Substring(folderFileViewModel2.UpperName.Length).Split(new char[1] { directorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0)
				{
					string[] array2 = array;
					foreach (string text in array2)
					{
						foreach (FolderFileViewModel item in list2)
						{
							if (text.Equals(item.UpperName))
							{
								folderFileViewModel = item;
								item.IsExpanded = false;
								item.IsExpanded = true;
								list2 = item.ChildFolders?.ToList();
								break;
							}
						}
						if (list2 == null || list2.Count == 0)
						{
							break;
						}
					}
				}
				else
				{
					folderFileViewModel = folderFileViewModel2;
				}
				if (folderFileViewModel != null)
				{
					folderFileViewModel.IsSelected = true;
				}
			}
		}
		LogHelper.LogInstance.Info($"EnterFolder exited, params[targetNode:{folderFileViewModel}]");
		return folderFileViewModel;
	}

	public virtual FolderFileViewModel EnterFolder(FolderFileViewModel node)
	{
		LogHelper.LogInstance.Info($"EnterFolder entered, params[node:{node}]");
		string empty = string.Empty;
		if (node == null || string.IsNullOrEmpty(empty = node.Path))
		{
			return null;
		}
		FolderFileViewModel folderFileViewModel = EnterFolder(empty);
		LogHelper.LogInstance.Info($"EnterFolder exited, result[{folderFileViewModel}]");
		return folderFileViewModel;
	}

	public virtual void NavigationTraceBack()
	{
		LogHelper.LogInstance.Info("TraceBack entered");
		try
		{
			isTraceBacking = true;
			string text = PopNodeFromTrace();
			string text2 = PopNodeFromTrace();
			string path = (string.IsNullOrEmpty(text2) ? text : text2);
			EnterFolder(path);
			SearchCacheModel searchCacheModel = GetSearchCache(path);
			if (searchCacheModel != null)
			{
				Search(SelectedNode, searchCacheModel.SearchKeywords);
			}
		}
		finally
		{
			isTraceBacking = false;
		}
		LogHelper.LogInstance.Info("TraceBack exited");
	}

	public virtual void NavigationStoreyBack()
	{
		LogHelper.LogInstance.Info("StoreyBack entered]");
		if (SelectedNode != null && SelectedNode.Parent != null)
		{
			RecordNodeChangedTraceIfNeed(SelectedNode.Path);
			SelectedNode.Parent.IsSelected = true;
			LogHelper.LogInstance.Info("StoreyBack exited");
		}
	}

	public virtual void Clear()
	{
		nodeChangedTraceStack.Clear();
		searchCache.Clear();
		TreeItems?.Clear();
		SelectedNode = null;
		isTraceBacking = false;
		FocusedPath = string.Empty;
	}

	protected void RecordNodeChangedTraceIfNeed(string path)
	{
		LogHelper.LogInstance.Info("RecordNodeChangedTrace entered, params[path:" + path + "]");
		bool flag = isTraceBacking;
		if (!flag && !string.IsNullOrEmpty(path))
		{
			nodeChangedTraceStack.Push(path);
			NavigationTraceGobackIsEnabled = true;
			LogHelper.LogInstance.Info($"RecordNodeChangedTrace exited, current isTraceBacking[{flag}]");
		}
		else
		{
			LogHelper.LogInstance.Info($"RecordNodeChangedTrace exited, current isTraceBacking[{flag}]");
		}
	}

	protected string PopNodeFromTrace()
	{
		LogHelper.LogInstance.Info("PopNodeFromTrace entered");
		string result = null;
		while (!nodeChangedTraceStack.IsEmpty && !nodeChangedTraceStack.TryPop(out result))
		{
		}
		NavigationTraceGobackIsEnabled = !nodeChangedTraceStack.IsEmpty;
		LogHelper.LogInstance.Info("PopNodeFromTrace exited,  params[node:" + result + "]");
		return result;
	}

	public bool NodeChangedTraceIsEmpty()
	{
		LogHelper.LogInstance.Info("NodeChangedTraceIsEmpty entered");
		bool isEmpty = nodeChangedTraceStack.IsEmpty;
		LogHelper.LogInstance.Info($"NodeChangedTraceIsEmpty exited, params[isEmpty:{isEmpty}]");
		return isEmpty;
	}

	protected void AddOrUpdateSearchCache(SearchCacheModel cache)
	{
		LogHelper.LogInstance.Info($"AddOrUpdateSearchCache entered, params[cache:{cache}]");
		searchCache[cache.Path] = cache;
		LogHelper.LogInstance.Info("AddOrUpdateSearchCache exited");
	}

	protected SearchCacheModel RemoveSearchCache(string path)
	{
		LogHelper.LogInstance.Info("RemoveSearchCache entered, params[path:" + path + "]");
		SearchCacheModel value = null;
		if (!string.IsNullOrEmpty(path))
		{
			while (searchCache.ContainsKey(path))
			{
				if (searchCache.TryRemove(path, out value))
				{
					value.CancellationToken?.Cancel();
					value.Dispose();
					break;
				}
			}
		}
		LogHelper.LogInstance.Info($"RemoveSearchCache exited, o[{value}]");
		return value;
	}

	protected SearchCacheModel GetSearchCache(string path)
	{
		LogHelper.LogInstance.Info("GetSearchCache entered, params[path:" + path + "]");
		SearchCacheModel value = null;
		if (!string.IsNullOrEmpty(path))
		{
			while (searchCache.ContainsKey(path) && !searchCache.TryGetValue(path, out value))
			{
			}
		}
		LogHelper.LogInstance.Info($"GetSearchCache exited, params[o:{value}]");
		return value;
	}

	public virtual void Search(FolderFileViewModel node, string keywords)
	{
		LogHelper.LogInstance.Info($"Search entered, params[node:{node}, keywords:{keywords}]");
		if (node != null && !string.IsNullOrEmpty(node.Path))
		{
			if (string.IsNullOrEmpty(keywords))
			{
				Refresh(node);
			}
			else
			{
				node.IsExpanded = false;
				IsSearching = true;
				node.Clear();
				new Task(delegate(object pNode)
				{
					FolderFileViewModel iNode = pNode as FolderFileViewModel;
					SearchCacheModel searchCacheModel = null;
					FolderTreeViewModel folderTreeViewModel = this;
					SearchCacheModel obj = new SearchCacheModel
					{
						Path = iNode.Path,
						SearchKeywords = keywords,
						CancellationToken = new CancellationTokenSource()
					};
					SearchCacheModel cache = obj;
					searchCacheModel = obj;
					folderTreeViewModel.AddOrUpdateSearchCache(cache);
					try
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							DataGridPathColVisibility = Visibility.Visible;
							iNode.Clear();
						});
						DoSearch(iNode, keywords, searchCacheModel.CancellationToken, delegate(FolderFileViewModel item)
						{
							if (!searchCacheModel.CancellationToken.IsCancellationRequested)
							{
								HostProxy.CurrentDispatcher?.Invoke(delegate
								{
									if (GetSearchCache(searchCacheModel.Path) == searchCacheModel && !searchCacheModel.CancellationToken.IsCancellationRequested)
									{
										iNode.AddSearchNodes(item);
									}
								});
							}
						});
					}
					finally
					{
						if (SelectedNode == iNode)
						{
							IsSearching = false;
						}
					}
				}, node).Start();
			}
		}
		LogHelper.LogInstance.Info("Search exited");
	}

	public virtual void CancelSearch()
	{
		FolderFileViewModel folderFileViewModel = SelectedNode;
		LogHelper.LogInstance.Info($"CancelSearch entered, internal params[selectedNode:{folderFileViewModel}]");
		Refresh(folderFileViewModel);
		LogHelper.LogInstance.Info("CancelSearch exited");
	}

	protected abstract void DoSearch(FolderFileViewModel node, string keywords, CancellationTokenSource cancellationTokenSource, Action<FolderFileViewModel> itemFoundCallback);

	private void NavigationStoreyGobackCommandHandler(object args)
	{
		NavigationStoreyBack();
	}

	private void NavigationTraceGobackCommandHandler(object args)
	{
		NavigationTraceBack();
	}

	private void RefreshCommandHandler(object args)
	{
		Refresh(SelectedNode);
	}

	private void EnterPathCommandHandler(object args)
	{
		EnterFolder(FocusedPath);
	}

	private void SearchCommandHandler(object args)
	{
		if (SelectedNode != null && IsSearching)
		{
			IsSearching = false;
			SearchKeywords = string.Empty;
			Refresh(SelectedNode);
		}
		else
		{
			Search(SelectedNode, SearchKeywords);
		}
	}
}
