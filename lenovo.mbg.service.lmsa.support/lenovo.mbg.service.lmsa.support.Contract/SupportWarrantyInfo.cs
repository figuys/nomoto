using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class SupportWarrantyInfo
{
	public string ID { get; set; }

	public string Name { get; set; }

	public string Brand { get; set; }

	public string Image { get; set; }

	public string Serial { get; set; }

	public string MachineType { get; set; }

	public string Model { get; set; }

	public string Description { get; set; }

	public string Status { get; set; }

	public string InWarranty { get; set; }

	public string PopDate { get; set; }

	public List<SupportWarrantyItemInfo> Warranties { get; set; }

	public string UpgradeURL { get; set; }

	public List<SupportCountry> Countries { get; set; }
}
