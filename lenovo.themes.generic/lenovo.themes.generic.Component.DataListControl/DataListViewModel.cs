using System.Collections.Generic;
using System.Windows;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Component.DataListControl;

public abstract class DataListViewModel<TModel, TId> : PageLoadingViewModelBase<TModel, TId> where TModel : PageLoadingItemViewModelBase<TId>, new()
{
	private bool isAllChecked;

	private Visibility checkAllBoxVisibility;

	private Visibility importVisibility;

	private ReplayCommand importCommand;

	private bool importEnable = true;

	private Visibility exportVisibility;

	private bool exportEnable = true;

	private ReplayCommand exportCommand;

	private Visibility deleteVisibility;

	private bool deleteEnable = true;

	private ReplayCommand deleteCommand;

	private bool refreshEnable = true;

	private Visibility refreshVisibility;

	private ReplayCommand refreshCommand;

	private bool isDeailMode = true;

	public bool IsAllChecked
	{
		get
		{
			return isAllChecked;
		}
		set
		{
			if (isAllChecked != value)
			{
				isAllChecked = value;
				OnPropertyChanged("IsAllChecked");
			}
		}
	}

	public ReplayCommand CheckAllCommand { get; set; }

	public Visibility CheckAllBoxVisibility
	{
		get
		{
			return checkAllBoxVisibility;
		}
		set
		{
			if (checkAllBoxVisibility != value)
			{
				checkAllBoxVisibility = value;
				OnPropertyChanged("CheckAllBoxVisibility");
			}
		}
	}

	public Visibility ImportVisibility
	{
		get
		{
			return importVisibility;
		}
		set
		{
			if (importVisibility != value)
			{
				importVisibility = value;
				OnPropertyChanged("ImportVisibility");
			}
		}
	}

	public ReplayCommand ImportCommand
	{
		get
		{
			return importCommand;
		}
		set
		{
			if (importCommand != value)
			{
				importCommand = value;
				OnPropertyChanged("ImportCommand");
			}
		}
	}

	public bool ImportEnable
	{
		get
		{
			return importEnable;
		}
		set
		{
			if (importEnable != value)
			{
				importEnable = value;
				OnPropertyChanged("ImportEnable");
			}
		}
	}

	public Visibility ExportVisibility
	{
		get
		{
			return exportVisibility;
		}
		set
		{
			if (exportVisibility != value)
			{
				exportVisibility = value;
				OnPropertyChanged("ExportVisibility");
			}
		}
	}

	public bool ExportEnable
	{
		get
		{
			return exportEnable;
		}
		set
		{
			if (exportEnable != value)
			{
				exportEnable = value;
				OnPropertyChanged("ExportEnable");
			}
		}
	}

	public ReplayCommand ExportCommand
	{
		get
		{
			return exportCommand;
		}
		set
		{
			if (exportCommand != value)
			{
				exportCommand = value;
				OnPropertyChanged("ExportCommand");
			}
		}
	}

	public Visibility DeleteVisibility
	{
		get
		{
			return deleteVisibility;
		}
		set
		{
			if (deleteVisibility != value)
			{
				deleteVisibility = value;
				OnPropertyChanged("DeleteVisibility");
			}
		}
	}

	public bool DeleteEnable
	{
		get
		{
			return deleteEnable;
		}
		set
		{
			if (deleteEnable != value)
			{
				deleteEnable = value;
				OnPropertyChanged("DeleteEnable");
			}
		}
	}

	public ReplayCommand DeleteCommand
	{
		get
		{
			return deleteCommand;
		}
		set
		{
			if (deleteCommand != value)
			{
				deleteCommand = value;
				OnPropertyChanged("DeleteCommand");
			}
		}
	}

	public bool RefreshEnable
	{
		get
		{
			return refreshEnable;
		}
		set
		{
			if (refreshEnable != value)
			{
				refreshEnable = value;
				OnPropertyChanged("RefreshEnable");
			}
		}
	}

	public Visibility RefreshVisbility
	{
		get
		{
			return refreshVisibility;
		}
		set
		{
			if (refreshVisibility != value)
			{
				refreshVisibility = value;
				OnPropertyChanged("RefreshVisbility");
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

	public bool IsDetailMode
	{
		get
		{
			return isDeailMode;
		}
		set
		{
			if (isDeailMode != value)
			{
				isDeailMode = value;
				OnPropertyChanged("IsDetailMode");
				OnShowModeChanged(value);
			}
		}
	}

	protected virtual void CheckAllCommandHandler(object e)
	{
	}

	protected virtual void ImportCommandHandler(object e)
	{
	}

	protected virtual void ExportCommandHandler(object e)
	{
	}

	protected virtual void DeleteCommandHandler(object e)
	{
	}

	protected virtual void RefreshCommandHandler(object e)
	{
		ClearPropertyLoadIdentification();
		BeginLoadIdList(null);
	}

	protected virtual void OnShowModeChanged(bool isDetailMode)
	{
	}

	public DataListViewModel()
	{
		ImportCommand = new ReplayCommand(ImportCommandHandler);
		ExportCommand = new ReplayCommand(ExportCommandHandler);
		DeleteCommand = new ReplayCommand(DeleteCommandHandler);
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		CheckAllCommand = new ReplayCommand(CheckAllCommandHandler);
	}

	protected override void OnStartLoadingId(Dictionary<string, object> tag)
	{
		RefreshEnable = false;
		tag.Add("ExportEnable", ExportEnable);
		tag.Add("DeleteEnable", DeleteEnable);
		ExportEnable = false;
		DeleteEnable = false;
		ImportEnable = false;
	}

	protected override void OnStopLoadingId(Dictionary<string, object> tag)
	{
		RefreshEnable = true;
		ImportEnable = true;
		ExportEnable = (bool)tag["ExportEnable"];
		DeleteEnable = (bool)tag["DeleteEnable"];
	}

	public override void Reset()
	{
		base.Reset();
		RefreshEnable = true;
		ExportEnable = false;
		DeleteEnable = false;
		ImportEnable = true;
		IsAllChecked = false;
	}
}
