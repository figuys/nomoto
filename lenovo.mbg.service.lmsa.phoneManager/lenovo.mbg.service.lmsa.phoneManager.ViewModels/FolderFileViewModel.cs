using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class FolderFileViewModel : ViewModelBase
{
	private ImageSource _iconSource;

	private string _displayName = string.Empty;

	private string _upperName = string.Empty;

	private bool _IsFolder;

	private bool _isExpanded;

	private bool _isSelected;

	private ObservableCollection<FolderFileViewModel> childFolders;

	private ObservableCollection<FolderFileViewModel> childNoess;

	private bool _isLeafNode;

	public ImageSource IconSource
	{
		get
		{
			return _iconSource;
		}
		set
		{
			if (_iconSource != value)
			{
				_iconSource = value;
				OnPropertyChanged("IconSource");
			}
		}
	}

	public FolderFileViewModel Parent { get; set; }

	public string DisplayName
	{
		get
		{
			if (string.IsNullOrEmpty(_displayName))
			{
				_ = string.Empty;
				try
				{
					_displayName = System.IO.Path.GetFileName(Path);
				}
				catch (Exception)
				{
				}
			}
			return _displayName;
		}
		set
		{
			if (!(_displayName == value))
			{
				_displayName = value;
				OnPropertyChanged("DisplayName");
			}
		}
	}

	public string UpperName
	{
		get
		{
			if (string.IsNullOrEmpty(_upperName))
			{
				string text = string.Empty;
				try
				{
					text = System.IO.Path.GetFileName(Path);
				}
				catch (Exception)
				{
				}
				if (!string.IsNullOrEmpty(text))
				{
					_upperName = text.ToUpper();
				}
			}
			return _upperName;
		}
		set
		{
			if (!(_upperName == value))
			{
				if (!string.IsNullOrEmpty(value))
				{
					_upperName = value.ToUpper();
				}
				else
				{
					_upperName = value;
				}
				OnPropertyChanged("UpperName");
			}
		}
	}

	public string SuffixName { get; set; }

	public string Path { get; set; }

	public bool IsFolder
	{
		get
		{
			return _IsFolder;
		}
		set
		{
			_IsFolder = value;
			OnPropertyChanged("IsFolder");
		}
	}

	public bool IsExpanded
	{
		get
		{
			return _isExpanded;
		}
		set
		{
			if (_isExpanded != value)
			{
				bool isExpanded = _isExpanded;
				_isExpanded = value;
				OnPropertyChanged("IsExpanded");
				ExpandHandler?.Invoke(this, isExpanded);
			}
		}
	}

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (_isSelected != value)
			{
				bool isSelected = _isSelected;
				_isSelected = value;
				OnPropertyChanged("IsSelected");
				SelectionHandler?.Invoke(this, isSelected);
			}
		}
	}

	public long FileSize { get; set; }

	public string FileSizeFormatString
	{
		get
		{
			if (IsFolder)
			{
				return "--";
			}
			return GlobalFun.ConvertLong2String(FileSize);
		}
	}

	public DateTime UpdateDate { get; set; }

	public string FileType { get; set; }

	public ObservableCollection<FolderFileViewModel> ChildFolders
	{
		get
		{
			return childFolders;
		}
		set
		{
			if (childFolders != value)
			{
				childFolders = value;
				OnPropertyChanged("ChildFolders");
			}
		}
	}

	public ObservableCollection<FolderFileViewModel> ChildNodes
	{
		get
		{
			return childNoess;
		}
		set
		{
			if (childNoess != value)
			{
				childNoess = value;
				OnPropertyChanged("ChildNodes");
			}
		}
	}

	public int Level
	{
		get
		{
			if (Parent == null)
			{
				return 0;
			}
			return Parent.Level + 1;
		}
	}

	public double MarginLeft
	{
		get
		{
			if (Level <= 0)
			{
				return 0.0;
			}
			return (double)Level * 10.0;
		}
	}

	public bool IsLeafNode
	{
		get
		{
			return _isLeafNode;
		}
		set
		{
			if (_isLeafNode != value)
			{
				_isLeafNode = value;
				OnPropertyChanged("IsLeafNode");
			}
		}
	}

	public Action<FolderFileViewModel, bool> SelectionHandler { get; set; }

	public Action<FolderFileViewModel, bool> ExpandHandler { get; set; }

	public FolderFileViewModel(bool isFolder)
	{
		IsFolder = isFolder;
		if (Thread.CurrentThread.IsBackground)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				ChildFolders = new ObservableCollection<FolderFileViewModel>();
				ChildNodes = new ObservableCollection<FolderFileViewModel>();
			});
		}
		else
		{
			ChildFolders = new ObservableCollection<FolderFileViewModel>();
			ChildNodes = new ObservableCollection<FolderFileViewModel>();
		}
	}

	public void AddChild(FolderFileViewModel item)
	{
		if (item != null)
		{
			item.Parent = this;
			ChildNodes.Add(item);
			if (item.IsFolder)
			{
				IsLeafNode = false;
				ChildFolders.Add(item);
			}
		}
	}

	public void AddSearchNodes(FolderFileViewModel item)
	{
		if (item != null)
		{
			item.Parent = this;
			ChildNodes.Add(item);
		}
	}

	public void Clear()
	{
		ChildNodes.Clear();
		ChildFolders.Clear();
		IsLeafNode = true;
	}

	public override string ToString()
	{
		return $"IsFolder:[{IsFolder}], UpperName:[{UpperName}], IsExpanded:[{IsExpanded}], IsSelected:[{IsSelected}], Level:[{Level}], Path:[{Path}], ChildNodes size:[{ChildNodes?.Count}]";
	}
}
