using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class CategoryViewModel : ComboBoxModel
{
	private ImageSource _centerIconSelectedSource;

	private ImageSource _centerIconUnSelectedSource;

	private bool _IsOpen;

	private string _title = string.Empty;

	public int _count;

	private long totalSize;

	private string totalSizeWithUnit;

	private Dictionary<string, long> idAndSizeMapping;

	private int _transferCount;

	private bool _isTransferring;

	private bool _isSelected;

	private bool _isEnabled;

	private double _Opacity = 1.0;

	public bool ShowSubWindow = true;

	private Visibility _SubItemVisible;

	public ImageSource CenterIconSelectedSource
	{
		get
		{
			return _centerIconSelectedSource;
		}
		set
		{
			if (_centerIconSelectedSource != value)
			{
				_centerIconSelectedSource = value;
				OnPropertyChanged("CenterIconSelectedSource");
			}
		}
	}

	public ImageSource CenterIconUnSelectedSource
	{
		get
		{
			return _centerIconUnSelectedSource;
		}
		set
		{
			if (_centerIconUnSelectedSource != value)
			{
				_centerIconUnSelectedSource = value;
				OnPropertyChanged("CenterIconUnSelectedSource");
			}
		}
	}

	public bool IsOpen
	{
		get
		{
			return _IsOpen;
		}
		set
		{
			_IsOpen = value;
			OnPropertyChanged("IsOpen");
		}
	}

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			if (!(_title == value))
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public int Count
	{
		get
		{
			return _count;
		}
		set
		{
			if (_count != value)
			{
				_count = value;
				IsEnabled = _count > 0;
				OnPropertyChanged("Count");
			}
		}
	}

	public long TotalSize
	{
		get
		{
			return totalSize;
		}
		set
		{
			if (totalSize != value)
			{
				totalSize = value;
				if (value == 0L)
				{
					TotalSizeWithUnit = string.Empty;
				}
				else
				{
					TotalSizeWithUnit = "(" + GlobalFun.ConvertLong2String(value) + ")";
				}
				OnPropertyChanged("TotalSize");
			}
		}
	}

	public string TotalSizeWithUnit
	{
		get
		{
			return totalSizeWithUnit;
		}
		set
		{
			if (!(totalSizeWithUnit == value))
			{
				totalSizeWithUnit = value;
				OnPropertyChanged("TotalSizeWithUnit");
			}
		}
	}

	public Dictionary<string, long> IdAndSizeMapping
	{
		get
		{
			return idAndSizeMapping;
		}
		set
		{
			if (idAndSizeMapping != value)
			{
				idAndSizeMapping = value;
				OnPropertyChanged("IdAndSizeMapping");
			}
		}
	}

	public int TransferCount
	{
		get
		{
			return _transferCount;
		}
		set
		{
			if (_transferCount != value)
			{
				_transferCount = value;
				OnPropertyChanged("TransferCount");
			}
		}
	}

	public bool IsTransferring
	{
		get
		{
			return _isTransferring;
		}
		set
		{
			if (_isTransferring != value)
			{
				_isTransferring = value;
				OnPropertyChanged("IsTransferring");
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
			if (_isSelected == value)
			{
				return;
			}
			if (Opacity == 0.5)
			{
				CategoryClickAction?.Invoke(this);
				return;
			}
			if ((ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}" || ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}") && Context.CurrentDevice?.Property.Category == "tablet")
			{
				string msg = "K1611";
				if (Context.CurrentViewType == ViewType.RESTORE)
				{
					msg = "K1612";
				}
				Context.MessageBox.ShowMessage(msg, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (ShowSubWindow && base.Parent == null && base.Childs != null && base.Childs.Count > 1)
			{
				List<CategoryViewModel> list = (from p in base.Childs.OfType<CategoryViewModel>().ToList()
					where p.SubItemVisible == Visibility.Visible
					select p).ToList();
				if (list.Count == 1)
				{
					_isSelected = value;
					list[0].IsSelected = value;
				}
				else
				{
					IUserMsgControl userUi = new SubCategoryWindow
					{
						DataContext = new SubCategoryWindowModel(this, base.Childs)
					};
					Context.MessageBox.ShowMessage(userUi);
				}
			}
			else
			{
				_isSelected = value;
				if (base.Childs != null && base.Childs.Count == 1)
				{
					(base.Childs[0] as CategoryViewModel).IsSelected = value;
				}
			}
			OnPropertyChanged("IsSelected");
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public double Opacity
	{
		get
		{
			return _Opacity;
		}
		set
		{
			if (_Opacity != value)
			{
				_Opacity = value;
				OnPropertyChanged("Opacity");
			}
		}
	}

	public string ResourceType { get; set; }

	public object Tag { get; set; }

	public bool IsInternalRes { get; set; }

	public Visibility SubItemVisible
	{
		get
		{
			return _SubItemVisible;
		}
		set
		{
			_SubItemVisible = value;
			OnPropertyChanged("SubItemVisible");
		}
	}

	public string SubTitle { get; set; }

	public Action<CategoryViewModel> CategoryClickAction { get; set; }

	public List<CategoryViewModel> SubCategoryViewModelList { get; set; }

	public virtual bool DoProcess()
	{
		return true;
	}
}
