using System.Collections.Generic;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

public class OrderItem
{
	public int orderId { get; set; }

	public string rid { get; set; }

	public string orderStatus { get; set; }

	public string type { get; set; }

	public long? usingDate { get; set; }

	public int? purchaseAmount { get; set; }

	public long? effectiveDate { get; set; }

	public long? expiredDate { get; set; }

	public int? imeiCount { get; set; }

	public int imeiUsedCount { get; set; }

	public string macAddressRsa { get; set; }

	public string unit { get; set; }

	public bool display { get; set; }

	public bool refund { get; set; }

	public bool enable { get; set; }

	public string orderLevelDesc { get; set; }

	public string serverOrderStatus { get; set; }

	public List<FlashedDevModel> imeiDtos { get; set; }
}
