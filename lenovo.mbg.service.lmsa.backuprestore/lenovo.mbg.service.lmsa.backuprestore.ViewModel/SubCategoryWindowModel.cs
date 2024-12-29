using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class SubCategoryWindowModel : ViewModelBase
{
	protected bool FireAllChanged = true;

	private bool? _AllSelected = false;

	protected CategoryViewModel Parent { get; }

	public ObservableCollection<ComboBoxModel> Categories { get; set; }

	public ReplayCommand OnCheckCommand { get; }

	public bool? AllSelected
	{
		get
		{
			return _AllSelected;
		}
		set
		{
			_AllSelected = value;
			FireAllCheck(value);
			OnPropertyChanged("AllSelected");
		}
	}

	public string Title { get; set; }

	public SubCategoryWindowModel(CategoryViewModel parent, ObservableCollection<ComboBoxModel> categories)
	{
		OnCheckCommand = new ReplayCommand(OnCheckCommandHandler);
		Title = parent.SubTitle ?? "K1516";
		Parent = parent;
		Categories = categories;
		FireAllSelected();
	}

	public void ChangeParentTitle()
	{
		int? num = Categories.Where((ComboBoxModel n) => (n as CategoryViewModel).IsSelected)?.Sum((ComboBoxModel n) => n.CountNum);
		Parent.ShowSubWindow = false;
		Parent.IsSelected = num > 0;
		if (Parent.IsSelected && num < Parent.CountNum)
		{
			Parent.Value = $"{Parent.Key}({num}/{Parent.CountNum})";
		}
		else
		{
			Parent.Value = $"{Parent.Key}({Parent.CountNum})";
		}
		Parent.ShowSubWindow = true;
	}

	private void OnCheckCommandHandler(object data)
	{
		CategoryViewModel obj = data as CategoryViewModel;
		obj.IsSelected = !obj.IsSelected;
		FireAllChanged = false;
		FireAllSelected();
		FireAllChanged = true;
	}

	private void FireAllSelected()
	{
		if (Categories.Count == 0)
		{
			AllSelected = null;
			return;
		}
		bool flag = true;
		bool flag2 = true;
		foreach (CategoryViewModel item in Categories.OfType<CategoryViewModel>().ToList())
		{
			if (item.SubItemVisible == Visibility.Visible)
			{
				if (item.IsSelected)
				{
					flag2 = false;
				}
				else
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			AllSelected = true;
		}
		else if (flag2)
		{
			AllSelected = false;
		}
		else
		{
			AllSelected = null;
		}
	}

	private void FireAllCheck(bool? selected)
	{
		if (!FireAllChanged)
		{
			return;
		}
		foreach (CategoryViewModel category in Categories)
		{
			if (category.SubItemVisible == Visibility.Visible)
			{
				category.IsSelected = selected ?? category.IsSelected;
			}
			else
			{
				category.IsSelected = false;
			}
		}
	}
}
