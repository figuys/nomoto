using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class BackupViewModel : BaseNotify
{
	private ObservableCollection<BackupRestoreTable> _BackupRestoreTables = new ObservableCollection<BackupRestoreTable>();

	private ObservableCollection<BackupCategoryModel> _BackupCategorys = new ObservableCollection<BackupCategoryModel>();

	private bool _IsCheckedProtect;

	private BackupRestoreTable _SelectedBackUpRestoreTable;

	private BackupCategoryModel _SelectedCategory;

	public ObservableCollection<BackupRestoreTable> BackupRestoreTables
	{
		get
		{
			return _BackupRestoreTables;
		}
		set
		{
			_BackupRestoreTables = value;
			OnPropertyChanged("BackupRestoreTables");
		}
	}

	public ObservableCollection<BackupCategoryModel> BackupCategorys
	{
		get
		{
			return _BackupCategorys;
		}
		set
		{
			_BackupCategorys = value;
			OnPropertyChanged("BackupCategorys");
		}
	}

	public bool IsCheckedProtect
	{
		get
		{
			return _IsCheckedProtect;
		}
		set
		{
			_IsCheckedProtect = value;
			OnPropertyChanged("IsCheckedProtect");
		}
	}

	public BackupRestoreTable SelectedBackUpRestoreTable
	{
		get
		{
			return _SelectedBackUpRestoreTable;
		}
		set
		{
			_SelectedBackUpRestoreTable = value;
			OnPropertyChanged("SelectedBackUpRestoreTable");
		}
	}

	public BackupCategoryModel SelectedCategory
	{
		get
		{
			return _SelectedCategory;
		}
		set
		{
			_SelectedCategory = value;
			OnPropertyChanged("SelectedCategory");
		}
	}

	public ICommand BackupCommand { get; private set; }

	public ICommand CancelCommand { get; private set; }

	public BackupViewModel()
	{
		BackupRestoreTables.Add(BackupRestoreTable.Backup);
		BackupRestoreTables.Add(BackupRestoreTable.Restore);
		BackupCategorys.Add(new BackupCategoryModel
		{
			UnChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_pic.png", UriKind.Relative)),
			ChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_pic_choose.png", UriKind.Relative)),
			ImageTitle = "K0475"
		});
		BackupCategorys.Add(new BackupCategoryModel
		{
			UnChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_music.png", UriKind.Relative)),
			ChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_music_choose.png", UriKind.Relative)),
			ImageTitle = "K0476",
			FinishedCount = 0,
			TotalCount = 512
		});
		BackupCategorys.Add(new BackupCategoryModel
		{
			UnChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_video.png", UriKind.Relative)),
			ChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_video_choose.png", UriKind.Relative)),
			ImageTitle = "K0477"
		});
		BackupCategorys.Add(new BackupCategoryModel
		{
			UnChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_contact.png", UriKind.Relative)),
			ChooseImageUrl = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/backuprestore/backup_contact_choose.png", UriKind.Relative)),
			ImageTitle = "K0478",
			TotalCount = 123,
			FinishedCount = 123
		});
		BackupCommand = new RelayCommand<object>(BackupClick);
		CancelCommand = new RelayCommand<object>(CancelClick);
	}

	private void BackupClick(object obj)
	{
	}

	private void CancelClick(object obj)
	{
	}
}
