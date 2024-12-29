using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyInfo
{
	public IBaseWarrantyServiceInfo WarrantyServiceInfo { get; set; }

	public IBaseWarrantyMachineInfo WarrantyMachineInfo { get; set; }

	public IBaseWarrantyAodInfo WarrantyAodInfo { get; set; }

	public IBaseWarrantyUpmaInfo WarrantyUpmaInfo { get; set; }

	public static IBaseWarrantyInfo New()
	{
		IBaseWarrantyInfo baseWarrantyInfo = new IBaseWarrantyInfo();
		baseWarrantyInfo.WarrantyMachineInfo = new IBaseWarrantyMachineInfo();
		baseWarrantyInfo.WarrantyServiceInfo = new IBaseWarrantyServiceInfo();
		baseWarrantyInfo.WarrantyServiceInfo.WarrantyServiceItems = new List<IBaseWarrantyServiceItemInfo>();
		baseWarrantyInfo.WarrantyAodInfo = new IBaseWarrantyAodInfo();
		baseWarrantyInfo.WarrantyAodInfo.WarrantyAodItems = new List<IBaseWarrantyAodItemInfo>();
		baseWarrantyInfo.WarrantyUpmaInfo = new IBaseWarrantyUpmaInfo();
		baseWarrantyInfo.WarrantyUpmaInfo.WarrantyUpmaItems = new List<IBaseWarrantyUpmaItemInfo>();
		return baseWarrantyInfo;
	}
}
