using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ModelV6;

public class LComboBoxViewModelV6 : NotifyBase
{
	private Func<object, string, int> _ComboBoxFilter;

	private Visibility _ComboBoxMoreButtonVisibility;

	private bool _IsDropDownOpened;

	private ObservableCollection<ManualComboboxViewModel> _StepComboBoxItemSource;

	private bool _IsDropDownEnabled = true;

	private bool _IsDataLoading;

	private int _ComboBoxSelectedIndex;

	private object _ComboBoxSelectedValue;

	private string _ComboBoxWatermark;

	private bool _IsCanShow;

	public bool IsEditable { get; set; }

	public dynamic Tag { get; set; }

	public ICommand SetTopClickCommand { get; set; }

	public ICommand MouseEnterCommand { get; set; }

	public ICommand ComboBoxMoreButtonCommand { get; set; }

	public ICommand SelectionChangedCommand { get; set; }

	public ICommand ComboBoxTextChangedCommand { get; set; }

	public Action<object> ItemSelChangedActon { get; set; }

	public Action<bool> DropDownOpenedChanged { get; set; }

	public Func<object, string, int> ComboBoxFilter
	{
		get
		{
			return _ComboBoxFilter;
		}
		set
		{
			_ComboBoxFilter = value;
			OnPropertyChanged("ComboBoxFilter");
		}
	}

	public Visibility ComboBoxMoreButtonVisibility
	{
		get
		{
			return _ComboBoxMoreButtonVisibility;
		}
		set
		{
			if (_ComboBoxMoreButtonVisibility != value)
			{
				_ComboBoxMoreButtonVisibility = value;
				OnPropertyChanged("ComboBoxMoreButtonVisibility");
			}
		}
	}

	public bool IsDropDownOpened
	{
		get
		{
			return _IsDropDownOpened;
		}
		set
		{
			if (_IsDropDownOpened != value)
			{
				_IsDropDownOpened = value;
				OnPropertyChanged("IsDropDownOpened");
				DropDownOpenedChanged?.Invoke(value);
			}
		}
	}

	public ObservableCollection<ManualComboboxViewModel> StepComboBoxItemSource
	{
		get
		{
			return _StepComboBoxItemSource;
		}
		set
		{
			_StepComboBoxItemSource = value;
			OnPropertyChanged("StepComboBoxItemSource");
		}
	}

	public bool IsDropDownEnabled
	{
		get
		{
			return _IsDropDownEnabled;
		}
		set
		{
			if (_IsDropDownEnabled != value)
			{
				_IsDropDownEnabled = value;
				OnPropertyChanged("IsDropDownEnabled");
			}
		}
	}

	public bool IsDataLoading
	{
		get
		{
			return _IsDataLoading;
		}
		set
		{
			_IsDataLoading = value;
			OnPropertyChanged("IsDataLoading");
		}
	}

	public int ComboBoxSelectedIndex
	{
		get
		{
			return _ComboBoxSelectedIndex;
		}
		set
		{
			_ComboBoxSelectedIndex = value;
			OnPropertyChanged("ComboBoxSelectedIndex");
		}
	}

	public object ComboBoxSelectedValue
	{
		get
		{
			return _ComboBoxSelectedValue;
		}
		set
		{
			IsDropDownOpened = false;
			if (_ComboBoxSelectedValue != value)
			{
				_ComboBoxSelectedValue = value;
				OnPropertyChanged("ComboBoxSelectedValue");
				ItemSelChangedActon?.Invoke(value);
			}
		}
	}

	public string ComboBoxWatermark
	{
		get
		{
			return _ComboBoxWatermark;
		}
		set
		{
			_ComboBoxWatermark = value;
			OnPropertyChanged("ComboBoxWatermark");
		}
	}

	public bool IsCanShow
	{
		get
		{
			return _IsCanShow;
		}
		set
		{
			_IsCanShow = value;
			OnPropertyChanged("IsCanShow");
		}
	}
}
