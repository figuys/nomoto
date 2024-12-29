using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class CustomerFlashData
{
	public string customerid { get; set; }

	public List<FlashDeviceData> devices { get; set; }
}
