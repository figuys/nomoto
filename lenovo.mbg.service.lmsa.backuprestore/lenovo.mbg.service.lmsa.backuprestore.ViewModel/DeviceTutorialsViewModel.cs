using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class DeviceTutorialsViewModel : ViewModelBase
{
	protected List<Uri> UriList;

	private ListViewItemModel _SelectedCategory;

	private ListViewItemModel _SelectedSubCategory;

	private ListViewItemModel _SelectedNote;

	private Visibility _SubCategoryVisibility = Visibility.Collapsed;

	private bool _PrevEnable;

	private bool _NextEnable = true;

	private Uri _TutorialImageUri;

	private int _CurrentCategoryId;

	public ReplayCommand PrevCommand { get; }

	public ReplayCommand NextCommand { get; }

	public ObservableCollection<ListViewItemModel> CategoryList { get; set; }

	public ObservableCollection<ListViewItemModel> SubCategoryList { get; set; }

	public ObservableCollection<ListViewItemModel> NoteList { get; set; }

	public ListViewItemModel SelectedCategory
	{
		get
		{
			return _SelectedCategory;
		}
		set
		{
			_SelectedCategory = value;
			if (_SelectedCategory != null)
			{
				CurrentCategoryId = _SelectedCategory.Id;
				if (_SelectedCategory.Id == 0)
				{
					SelectedSubCategory = SubCategoryList.First();
				}
				else
				{
					SelectedSubCategory = null;
					ChangeNotes("tablet");
				}
			}
			OnPropertyChanged("SelectedCategory");
		}
	}

	public ListViewItemModel SelectedSubCategory
	{
		get
		{
			return _SelectedSubCategory;
		}
		set
		{
			_SelectedSubCategory = value;
			if (_SelectedSubCategory == null)
			{
				SubCategoryVisibility = Visibility.Collapsed;
			}
			else
			{
				if (_SelectedSubCategory.Id == 0)
				{
					ChangeNotes("phone-normal");
				}
				else
				{
					ChangeNotes("phone-legion");
				}
				SubCategoryVisibility = Visibility.Visible;
			}
			OnPropertyChanged("SelectedSubCategory");
		}
	}

	public ListViewItemModel SelectedNote
	{
		get
		{
			return _SelectedNote;
		}
		set
		{
			_SelectedNote = value;
			if (_SelectedNote != null)
			{
				Task.Run(() => TutorialImageUri = UriList[_SelectedNote.Id]);
				if (_SelectedNote.Id == NoteList.Count - 1)
				{
					NextEnable = false;
					PrevEnable = NoteList.Count > 1;
				}
				else if (_SelectedNote.Id == 0)
				{
					PrevEnable = false;
					NextEnable = NoteList.Count > 1;
				}
				else
				{
					PrevEnable = true;
					NextEnable = true;
				}
			}
			OnPropertyChanged("SelectedNote");
		}
	}

	public Visibility SubCategoryVisibility
	{
		get
		{
			return _SubCategoryVisibility;
		}
		set
		{
			_SubCategoryVisibility = value;
			OnPropertyChanged("SubCategoryVisibility");
		}
	}

	public bool PrevEnable
	{
		get
		{
			return _PrevEnable;
		}
		set
		{
			_PrevEnable = value;
			OnPropertyChanged("PrevEnable");
		}
	}

	public bool NextEnable
	{
		get
		{
			return _NextEnable;
		}
		set
		{
			_NextEnable = value;
			OnPropertyChanged("NextEnable");
		}
	}

	public Uri TutorialImageUri
	{
		get
		{
			return _TutorialImageUri;
		}
		set
		{
			_TutorialImageUri = value;
			OnPropertyChanged("TutorialImageUri");
		}
	}

	public int CurrentCategoryId
	{
		get
		{
			return _CurrentCategoryId;
		}
		set
		{
			_CurrentCategoryId = value;
			OnPropertyChanged("CurrentCategoryId");
		}
	}

	public DeviceTutorialsViewModel()
	{
		PrevCommand = new ReplayCommand(PrevCommandHandler);
		NextCommand = new ReplayCommand(NextCommandHandler);
		NoteList = new ObservableCollection<ListViewItemModel>();
		CategoryList = new ObservableCollection<ListViewItemModel>();
		CategoryList.Add(new ListViewItemModel(0, "K0688", "v6_icon_samll_phone", "v6_icon_samll_phone_selected"));
		CategoryList.Add(new ListViewItemModel(1, "K0929", "v6_icon_samll_tablet", "v6_icon_samll_tablet_selected"));
		SubCategoryList = new ObservableCollection<ListViewItemModel>();
		SubCategoryList.Add(new ListViewItemModel(0, "K0688"));
		SubCategoryList.Add(new ListViewItemModel(1, "K1356"));
	}

	public override void LoadData(object data)
	{
		SelectedCategory = CategoryList.First();
		base.LoadData(data);
	}

	private void PrevCommandHandler(object data)
	{
		int id = SelectedNote.Id;
		SelectedNote = NoteList[--id];
	}

	private void NextCommandHandler(object data)
	{
		int id = SelectedNote.Id;
		SelectedNote = NoteList[++id];
	}

	private void ChangeNotes(string type)
	{
		NoteList.Clear();
		if (type == "phone-normal")
		{
			UriList = new List<Uri>
			{
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone11.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone12.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone13.gif")
			};
			NoteList.Add(new ListViewItemModel(0, "K1097"));
			NoteList.Add(new ListViewItemModel(1, "K1098"));
			NoteList.Add(new ListViewItemModel(2, "K1099"));
		}
		else if (type == "tablet")
		{
			UriList = new List<Uri>
			{
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet31.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet32.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet33.gif")
			};
			NoteList.Add(new ListViewItemModel(0, "K1102"));
			NoteList.Add(new ListViewItemModel(1, "K1103"));
			NoteList.Add(new ListViewItemModel(2, "K1107"));
		}
		else
		{
			UriList = new List<Uri>
			{
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone21.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone22.gif"),
				new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone23.gif")
			};
			NoteList.Add(new ListViewItemModel(0, "K1100"));
			NoteList.Add(new ListViewItemModel(1, "K1101"));
			NoteList.Add(new ListViewItemModel(2, "K1108"));
		}
		SelectedNote = NoteList.First();
	}
}
