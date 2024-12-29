using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.backuprestore.Business;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class BackupHistoryListItemViewModel : ViewModelBase
{
	private ImageSource _deviceNameIcon;

	private string _deviceName = string.Empty;

	private string _modelName = string.Empty;

	private string _version = string.Empty;

	private double _size;

	private string _sizeStrFormat = string.Empty;

	private string _backupDateStrFormat = string.Empty;

	private string _notes = string.Empty;

	private bool _isSelected;

	private bool _IsPhone;

	public ImageSource DeviceNameIcon
	{
		get
		{
			return _deviceNameIcon;
		}
		set
		{
			if (_deviceNameIcon != value)
			{
				_deviceNameIcon = value;
				OnPropertyChanged("DeviceNameIcon");
			}
		}
	}

	public string DeviceName
	{
		get
		{
			return _deviceName;
		}
		set
		{
			if (!(_deviceName == value))
			{
				_deviceName = value;
				OnPropertyChanged("DeviceName");
			}
		}
	}

	public string ModelName
	{
		get
		{
			return _modelName;
		}
		set
		{
			if (!(_modelName == value))
			{
				_modelName = value;
				OnPropertyChanged("ModelName");
			}
		}
	}

	public string Version
	{
		get
		{
			return _version;
		}
		set
		{
			if (!(_version == value))
			{
				_version = value;
				OnPropertyChanged("Version");
			}
		}
	}

	public double Size
	{
		get
		{
			return _size;
		}
		set
		{
			if (_size != value)
			{
				_size = value;
				OnPropertyChanged("Size");
			}
		}
	}

	public string SizeStrFormat
	{
		get
		{
			return _sizeStrFormat;
		}
		set
		{
			if (!(_sizeStrFormat == value))
			{
				_sizeStrFormat = value;
				OnPropertyChanged("SizeStrFormat");
			}
		}
	}

	private DateTime BackupDate { get; set; }

	public string BackupDateStrFormat
	{
		get
		{
			return _backupDateStrFormat;
		}
		set
		{
			if (!(_backupDateStrFormat == value))
			{
				_backupDateStrFormat = value;
				OnPropertyChanged("BackupDateStrFormat");
			}
		}
	}

	public string Notes
	{
		get
		{
			return _notes;
		}
		set
		{
			if (!(_notes == value))
			{
				_notes = value;
				OnPropertyChanged("Notes");
			}
		}
	}

	public string Storagepath { get; set; }

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
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public bool IsPhone
	{
		get
		{
			return _IsPhone;
		}
		set
		{
			_IsPhone = value;
			OnPropertyChanged("IsPhone");
		}
	}

	public ButtonViewModel OpenViewButtonViewModel { get; set; }

	public ButtonViewModel DeleteButtonViewModel { get; set; }

	public BackupDescription BackupDescriptionInfo { get; set; }

	public BackupHistoryListItemViewModel()
	{
		ImageSource imageSource = Application.Current.FindResource("searchDrawingImage") as ImageSource;
		OpenViewButtonViewModel = new ButtonViewModel
		{
			Background = new ImageBrush(imageSource)
			{
				Stretch = Stretch.None
			},
			Visibility = Visibility.Visible,
			Content = new Image
			{
				Source = imageSource,
				Stretch = Stretch.None
			},
			ClickCommand = new ReplayCommand(OpenViewButtonClickCommandHandler)
		};
		ImageSource imageSource2 = Application.Current.FindResource("delete_DrawingImage") as ImageSource;
		DeleteButtonViewModel = new ButtonViewModel
		{
			Background = new ImageBrush(imageSource2)
			{
				Stretch = Stretch.None
			},
			Visibility = Visibility.Visible,
			Content = new Image
			{
				Source = imageSource2,
				Stretch = Stretch.None
			},
			ClickCommand = new ReplayCommand(DeleteButtonClickCommandHandler)
		};
	}

	private void OpenViewButtonClickCommandHandler(object parameter)
	{
	}

	private void DeleteButtonClickCommandHandler(object parameter)
	{
	}
}
