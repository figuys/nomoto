using System.Collections.Generic;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

public class RespOrders
{
	public string id { get; set; }

	public string username { get; set; }

	public string email { get; set; }

	public string vipName { get; set; }

	public int freeAmount { get; set; }

	public int usedFreeAmount { get; set; }

	public bool multiDevice { get; set; }

	public List<OrderItem> enableOrderDtos { get; set; }

	public List<OrderItem> unableOrderDtos { get; set; }

	public bool popUp { get; set; }

	public int popMode { get; set; }
}
