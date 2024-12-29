using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyAodInfo
{
	public List<IBaseWarrantyAodItemInfo> WarrantyAodItems { get; set; }
}
