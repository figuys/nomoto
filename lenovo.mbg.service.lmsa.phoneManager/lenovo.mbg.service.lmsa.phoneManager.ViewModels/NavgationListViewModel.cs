using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class NavgationListViewModel : NotifyBase
{
	private ObservableCollection<CenterNavButtonViewModel> mItems;

	public ObservableCollection<CenterNavButtonViewModel> Items => mItems;

	public CenterNavButtonViewModel SelectedItem { get; set; }

	public NavgationListViewModel()
	{
		mItems = new ObservableCollection<CenterNavButtonViewModel>();
	}

	public void AddRange(List<CenterNavButtonViewModel> items)
	{
		if (items == null)
		{
			return;
		}
		foreach (CenterNavButtonViewModel item in items)
		{
			Items.Add(item);
		}
	}

	public void Add(CenterNavButtonViewModel item)
	{
		if (item != null)
		{
			Items.Add(item);
		}
	}

	public ImageSource GetImageSource()
	{
		return XamlReader.Parse(ResourcesHelper.XamlResources.contactDrawingImage) as DrawingImage;
	}

	public ImageSource GetImageSource(string content)
	{
		return XamlReader.Parse(content) as DrawingImage;
	}
}
