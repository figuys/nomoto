using System;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class FileDataViewModel : TreeViewModel
{
	protected Action<TreeViewModel> ExpandedAction;

	private bool _IsFolder;

	private Visibility _IsVisible;

	private long _FileSize;

	private string _FileType;

	private DateTime? _UpdateDate;

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

	public Visibility IsVisible
	{
		get
		{
			return _IsVisible;
		}
		set
		{
			_IsVisible = value;
			OnPropertyChanged("IsVisible");
		}
	}

	public long FileSize
	{
		get
		{
			return _FileSize;
		}
		set
		{
			_FileSize = value;
			OnPropertyChanged("FileSize");
		}
	}

	public string FileSizeFormatString
	{
		get
		{
			if (!IsFolder)
			{
				return GlobalFun.ConvertLong2String(FileSize);
			}
			return "--";
		}
	}

	public string FileType
	{
		get
		{
			return _FileType;
		}
		set
		{
			_FileType = value;
			OnPropertyChanged("FileType");
		}
	}

	public DateTime? UpdateDate
	{
		get
		{
			return _UpdateDate;
		}
		set
		{
			_UpdateDate = value;
			OnPropertyChanged("UpdateDate");
		}
	}

	public FileDataViewModel(TreeModel model, FileDataViewModel parentModel, bool isFolder, long fileSize, string fileType, DateTime? updateDate, Action<TreeViewModel> expandedAction)
		: base(model, parentModel)
	{
		IsFolder = isFolder;
		FileSize = fileSize;
		FileType = fileType;
		UpdateDate = updateDate;
		ExpandedAction = expandedAction;
	}

	public override void FireExpand(TreeViewModel treeViewModel, bool expanded)
	{
		if (expanded)
		{
			ExpandedAction?.Invoke(treeViewModel);
		}
		base.FireExpand(treeViewModel, expanded);
	}
}
