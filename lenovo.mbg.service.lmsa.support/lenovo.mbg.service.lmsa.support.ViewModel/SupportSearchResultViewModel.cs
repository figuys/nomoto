using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.lmsa.support.Commons;
using lenovo.mbg.service.lmsa.support.Contract;
using lenovo.mbg.service.lmsa.support.UserControls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class SupportSearchResultViewModel : ViewModelBase
{
	private ObservableCollection<SupportWarrantyStatusItemViewModel> warrantyStatusItemViewModel;

	private string machineType;

	private string brand;

	private string productName;

	private string serialNumber;

	private string model;

	private string shipToLocation;

	private ReplayCommand copyCommand;

	private bool copiedIMEIVisibility;

	private bool copiedSNVisibility;

	private ReplayCommand backHomeCommand;

	public ObservableCollection<SupportWarrantyStatusItemViewModel> WarrantyStatusItemViewModel
	{
		get
		{
			return warrantyStatusItemViewModel;
		}
		set
		{
			warrantyStatusItemViewModel = value;
			OnPropertyChanged("WarrantyStatusItemViewModel");
		}
	}

	public string MachineType
	{
		get
		{
			return machineType;
		}
		set
		{
			if (!(machineType == value))
			{
				machineType = value;
				OnPropertyChanged("MachineType");
			}
		}
	}

	public string Brand
	{
		get
		{
			return brand;
		}
		set
		{
			if (!(brand == value))
			{
				brand = value;
				OnPropertyChanged("Brand");
			}
		}
	}

	public string ProductName
	{
		get
		{
			return productName;
		}
		set
		{
			if (!(productName == value))
			{
				productName = value;
				OnPropertyChanged("ProductName");
			}
		}
	}

	public string SerialNumber
	{
		get
		{
			return serialNumber;
		}
		set
		{
			if (!(serialNumber == value))
			{
				serialNumber = value;
				OnPropertyChanged("SerialNumber");
			}
		}
	}

	public string Model
	{
		get
		{
			return model;
		}
		set
		{
			if (!(model == value))
			{
				model = value;
				OnPropertyChanged("Model");
			}
		}
	}

	public string ShipTlLocation
	{
		get
		{
			return shipToLocation;
		}
		set
		{
			if (!(shipToLocation == value))
			{
				shipToLocation = value;
				OnPropertyChanged("ShipTlLocation");
			}
		}
	}

	public ReplayCommand CopyCommand
	{
		get
		{
			return copyCommand;
		}
		set
		{
			if (copyCommand != value)
			{
				copyCommand = value;
				OnPropertyChanged("CopyCommand");
			}
		}
	}

	public bool CopiedIMEIVisibility
	{
		get
		{
			return copiedIMEIVisibility;
		}
		set
		{
			if (copiedIMEIVisibility != value)
			{
				copiedIMEIVisibility = value;
				OnPropertyChanged("CopiedIMEIVisibility");
			}
		}
	}

	public bool CopiedSNVisibility
	{
		get
		{
			return copiedSNVisibility;
		}
		set
		{
			if (copiedSNVisibility != value)
			{
				copiedSNVisibility = value;
				OnPropertyChanged("CopiedSNVisibility");
			}
		}
	}

	public ReplayCommand BackHomeCommand
	{
		get
		{
			return backHomeCommand;
		}
		set
		{
			if (backHomeCommand != value)
			{
				backHomeCommand = value;
				OnPropertyChanged("BackHomeCommand");
			}
		}
	}

	public SupportSearchResultViewModel()
	{
		WarrantyStatusItemViewModel = new ObservableCollection<SupportWarrantyStatusItemViewModel>();
		BackHomeCommand = new ReplayCommand(BackHomeCommandHandler);
		CopyCommand = new ReplayCommand(CopyCommandHandler);
	}

	public override void LoadData(object data)
	{
		WarrantyStatusItemViewModel.Clear();
		if (!(data is SupportWarrantyInfo supportWarrantyInfo))
		{
			return;
		}
		base.LoadData(supportWarrantyInfo);
		ProductName = supportWarrantyInfo.Name;
		if (ProductName.LastIndexOf("Model") > 0)
		{
			ProductName = ProductName.Substring(0, ProductName.LastIndexOf("Model")).TrimEnd(' ', '-');
		}
		SerialNumber = supportWarrantyInfo.Serial;
		Model = supportWarrantyInfo.Model;
		Brand = supportWarrantyInfo.Brand;
		MachineType = supportWarrantyInfo.MachineType;
		if (!string.IsNullOrEmpty(Model) && !string.IsNullOrEmpty(MachineType))
		{
			int length = Model.Length;
			int length2 = MachineType.Length;
			if (length2 < length)
			{
				int i;
				for (i = 0; i < length2 && MachineType[i].Equals(Model[i]); i++)
				{
				}
				if (i == length2)
				{
					Model = Model.Substring(length2);
				}
			}
		}
		List<SupportCountry> countries = supportWarrantyInfo.Countries;
		string text = string.Empty;
		if (countries != null && countries.Count > 0)
		{
			for (int j = 0; j < countries.Count; j++)
			{
				text = text + ((j == 0) ? "" : ", ") + countries[j].Name;
			}
		}
		ShipTlLocation = text;
		List<SupportWarrantyItemInfo> warranties = supportWarrantyInfo.Warranties;
		if (warranties != null && warranties.Count > 0)
		{
			warranties = warranties.OrderByDescending((SupportWarrantyItemInfo m) => m.End).ToList();
			SupportWarrantyItemInfo supportWarrantyItemInfo = null;
			int count = warranties.Count;
			int num = count - 1;
			for (int k = 0; k < count; k++)
			{
				SupportWarrantyStatusItemViewModel supportWarrantyStatusItemViewModel = new SupportWarrantyStatusItemViewModel();
				supportWarrantyItemInfo = warranties[k];
				supportWarrantyStatusItemViewModel.SubTitle = supportWarrantyItemInfo.Name;
				supportWarrantyStatusItemViewModel.ID = supportWarrantyItemInfo.Code;
				supportWarrantyStatusItemViewModel.StartDate = supportWarrantyItemInfo.Start;
				supportWarrantyStatusItemViewModel.StopDate = supportWarrantyItemInfo.End;
				supportWarrantyStatusItemViewModel.DeliveryType = supportWarrantyItemInfo.DeliveryType;
				supportWarrantyStatusItemViewModel.Category = supportWarrantyItemInfo.Category;
				supportWarrantyStatusItemViewModel.GroupExpanded = k == 0;
				supportWarrantyStatusItemViewModel.IsShowDottedLine = num != k;
				supportWarrantyStatusItemViewModel.Description = supportWarrantyItemInfo.Description;
				WarrantyStatusItemViewModel.Add(supportWarrantyStatusItemViewModel);
			}
			WarrantyStatusItemViewModel = WarrantyStatusItemViewModel;
		}
		if (SerialNumber.Contains("000000000000000"))
		{
			SerialNumber = SerialNumber.Replace("000000000000000", "not found");
		}
	}

	private string SdfDescFilter(string sdfDesc)
	{
		if (!string.IsNullOrEmpty(sdfDesc))
		{
			sdfDesc = sdfDesc.Trim();
			if (sdfDesc.Length > 0)
			{
				return sdfDesc;
			}
			return string.Empty;
		}
		return sdfDesc;
	}

	protected virtual void CopyCommandHandler(object args)
	{
		try
		{
			if (args != null)
			{
				Clipboard.SetDataObject(args.ToString());
				if (args.ToString() == SerialNumber)
				{
					CopiedSNVisibility = true;
				}
				else
				{
					CopiedIMEIVisibility = true;
				}
				Task.Factory.StartNew(delegate
				{
					Thread.Sleep(800);
					CopiedIMEIVisibility = false;
					CopiedSNVisibility = false;
				});
			}
		}
		catch
		{
		}
	}

	private void BackHomeCommandHandler(object args)
	{
		((ViewModelBase)lenovo.mbg.service.lmsa.support.Commons.ViewContext.SwitchView<SearchViewEx>().DataContext).LoadData(null);
	}
}
