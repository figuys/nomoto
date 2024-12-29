using System;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class SupportWarrantyItemInfo
{
	public string Code { get; set; }

	public string Type { get; set; }

	public string Category { get; set; }

	public string DeliveryType { get; set; }

	public int Duration { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public DateTime Start { get; set; }

	public DateTime End { get; set; }

	public object Country { get; set; }
}
