using System;
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

public class IBaseSearchResultViewModel : ViewModelBase
{
	private class WarrantyStatusItemViewModelComparer : IComparer<IBaseWarrantyStatusItemViewModel>
	{
		public int Compare(IBaseWarrantyStatusItemViewModel x, IBaseWarrantyStatusItemViewModel y)
		{
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			if (x != null && y != null)
			{
				DateTime result = DateTime.Now;
				DateTime result2 = DateTime.Now;
				bool flag = DateTime.TryParse(x.StartDate, out result);
				bool flag2 = DateTime.TryParse(y.StartDate, out result2);
				if (flag && !flag2)
				{
					return 1;
				}
				if (!flag && flag2)
				{
					return -1;
				}
				if (!flag && !flag2)
				{
					return 1;
				}
				if (flag && flag2)
				{
					if (!(result > result2))
					{
						return -1;
					}
					return 1;
				}
			}
			return 1;
		}
	}

	private ObservableCollection<IBaseWarrantyStatusItemViewModel> warrantyStatusItemViewModel;

	private string productName;

	private string serialNumber;

	private string machineTypeModel;

	private string imei;

	private string shipToLocation;

	private ReplayCommand backHomeCommand;

	private ReplayCommand copyCommand;

	private bool copiedIMEIVisibility;

	private bool copiedSNVisibility;

	public ObservableCollection<IBaseWarrantyStatusItemViewModel> WarrantyStatusItemViewModel
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

	public string MachineTypeModel
	{
		get
		{
			return machineTypeModel;
		}
		set
		{
			if (!(machineTypeModel == value))
			{
				machineTypeModel = value;
				OnPropertyChanged("MachineTypeModel");
			}
		}
	}

	public string IMEI
	{
		get
		{
			return imei;
		}
		set
		{
			if (!(imei == value))
			{
				imei = value;
				OnPropertyChanged("IMEI");
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

	public IBaseSearchResultViewModel()
	{
		WarrantyStatusItemViewModel = new ObservableCollection<IBaseWarrantyStatusItemViewModel>();
		BackHomeCommand = new ReplayCommand(BackHomeCommandHandler);
		CopyCommand = new ReplayCommand(CopyCommandHandler);
	}

	public override void LoadData(object data)
	{
		if (!(data is IBaseWarrantyInfo baseWarrantyInfo))
		{
			return;
		}
		base.LoadData(baseWarrantyInfo);
		WarrantyStatusItemViewModel.Clear();
		ProductName = string.Empty;
		SerialNumber = string.Empty;
		MachineTypeModel = string.Empty;
		IMEI = string.Empty;
		List<IBaseWarrantyStatusItemViewModel> list = new List<IBaseWarrantyStatusItemViewModel>();
		bool flag = false;
		foreach (IBaseWarrantyServiceItemInfo warrantyServiceItem in baseWarrantyInfo.WarrantyServiceInfo.WarrantyServiceItems)
		{
			if (!flag)
			{
				flag = true;
				ShipTlLocation = warrantyServiceItem.CountryDesc;
			}
			list.Add(new IBaseWarrantyStatusItemViewModel
			{
				StartDate = warrantyServiceItem.Warstart,
				StopDate = warrantyServiceItem.Wed,
				Description = SdfDescFilter(warrantyServiceItem.SdfDesc),
				IsShowDottedLine = true
			});
		}
		foreach (IBaseWarrantyUpmaItemInfo warrantyUpmaItem in baseWarrantyInfo.WarrantyUpmaInfo.WarrantyUpmaItems)
		{
			list.Add(new IBaseWarrantyStatusItemViewModel
			{
				StartDate = warrantyUpmaItem.StartDate,
				StopDate = warrantyUpmaItem.EndDate,
				Description = SdfDescFilter(warrantyUpmaItem.SdfDesc),
				IsShowDottedLine = true
			});
		}
		if (list.Count > 0)
		{
			list = list.OrderByDescending((IBaseWarrantyStatusItemViewModel n) => DateTime.Parse(n.StopDate)).ToList();
			list.Last().IsShowDottedLine = false;
			foreach (IBaseWarrantyStatusItemViewModel item in list)
			{
				WarrantyStatusItemViewModel.Add(item);
			}
		}
		WarrantyStatusItemViewModel = WarrantyStatusItemViewModel;
		IBaseWarrantyMachineInfo warrantyMachineInfo = baseWarrantyInfo.WarrantyMachineInfo;
		ProductName = warrantyMachineInfo.ProductName;
		SerialNumber = warrantyMachineInfo.Serial;
		MachineTypeModel = warrantyMachineInfo.Product;
		IMEI = SpellImei(baseWarrantyInfo);
		if (IMEI.Contains("000000000000000"))
		{
			IMEI = IMEI.Replace("000000000000000", "not found");
		}
	}

	private string SpellImei(IBaseWarrantyInfo warrantyInfo)
	{
		List<string> list = (from m in warrantyInfo.WarrantyAodInfo.WarrantyAodItems
			where m.AodType.ToUpper().Contains("IMEI")
			select m.AodDescription).ToList();
		string iMEI = warrantyInfo.WarrantyMachineInfo.IMEI;
		if (!string.IsNullOrWhiteSpace(iMEI))
		{
			string[] array = iMEI.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string item in array)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		string text = string.Empty;
		for (int j = 0; j < list.Count; j++)
		{
			text = text + ((j == 0) ? string.Empty : " | ") + list[j];
		}
		return text;
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
				if (args.ToString() == IMEI)
				{
					CopiedIMEIVisibility = true;
				}
				else
				{
					CopiedSNVisibility = true;
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
