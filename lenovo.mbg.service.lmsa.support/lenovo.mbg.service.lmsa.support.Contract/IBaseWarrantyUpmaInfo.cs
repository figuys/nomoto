using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyUpmaInfo
{
	public List<IBaseWarrantyUpmaItemInfo> WarrantyUpmaItems { get; set; }
}
