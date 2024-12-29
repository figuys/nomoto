using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyServiceInfo
{
	public List<IBaseWarrantyServiceItemInfo> WarrantyServiceItems { get; set; }
}
