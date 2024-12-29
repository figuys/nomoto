using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ModelV6;

namespace lenovo.themes.generic.ViewModelV6;

public class TreeViewModel : ViewModelBase
{
	private int _ChildrenCount;

	private ImageSource _Image;

	private bool _IsSelected;

	private bool? _IsChecked = false;

	private bool _IsExpanded;

	private TreeViewModel _ParentModel;

	private ObservableCollection<TreeViewModel> _Childrens = new ObservableCollection<TreeViewModel>();

	public string ParentID
	{
		get
		{
			return Model.ParentID;
		}
		set
		{
			Model.ParentID = value;
			OnPropertyChanged("ParentID");
		}
	}

	public int ChildrenCount
	{
		get
		{
			return _ChildrenCount;
		}
		set
		{
			_ChildrenCount = value;
			OnPropertyChanged("ChildrenCount");
		}
	}

	public string Name
	{
		get
		{
			return Model.Name;
		}
		set
		{
			Model.Name = value;
			OnPropertyChanged("Name");
		}
	}

	public ImageSource Image
	{
		get
		{
			return _Image;
		}
		set
		{
			_Image = value;
			OnPropertyChanged("Image");
		}
	}

	public string ID
	{
		get
		{
			return Model.ID;
		}
		set
		{
			Model.ID = value;
			OnPropertyChanged("ID");
		}
	}

	public string Data => Model.Data;

	public bool IsSelected
	{
		get
		{
			return _IsSelected;
		}
		set
		{
			if (_IsSelected != value)
			{
				_IsSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public bool? IsChecked
	{
		get
		{
			return _IsChecked;
		}
		set
		{
			if (_IsChecked != value)
			{
				_IsChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}

	public bool IsExpanded
	{
		get
		{
			return _IsExpanded;
		}
		set
		{
			if (_IsExpanded != value)
			{
				_IsExpanded = value;
				FireExpand(this, _IsExpanded);
				OnPropertyChanged("IsExpanded");
			}
		}
	}

	public bool HasChildrens
	{
		get
		{
			if (Childrens != null)
			{
				return Childrens.Count != 0;
			}
			return false;
		}
	}

	public TreeViewModel ParentModel
	{
		get
		{
			return _ParentModel;
		}
		set
		{
			_ParentModel = value;
			OnPropertyChanged("ParentModel");
		}
	}

	public TreeModel Model { get; private set; }

	public virtual ObservableCollection<TreeViewModel> Childrens
	{
		get
		{
			return _Childrens;
		}
		set
		{
			_Childrens = value;
			OnPropertyChanged("Childrens");
		}
	}

	public virtual void FireExpand(TreeViewModel treeViewModel, bool expanded)
	{
	}

	public TreeViewModel(TreeModel model, TreeViewModel parentModel)
	{
		Model = model;
		ParentModel = parentModel;
		if (!string.IsNullOrEmpty(Model.ImageKey))
		{
			Image = Application.Current.Resources[Model.ImageKey] as ImageSource;
		}
		Childrens = new ObservableCollection<TreeViewModel>();
	}
}
