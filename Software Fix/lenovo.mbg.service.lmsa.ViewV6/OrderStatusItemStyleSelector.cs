using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.ViewV6;

public class OrderStatusItemStyleSelector : StyleSelector
{
	public Style ActiveItemStyle { get; set; }

	public Style UsingItemStyle { get; set; }

	public Style InvalidItemStyle { get; set; }

	public Style RefundingItemStyle { get; set; }

	public Style ProcessItemStyle { get; set; }

	public override Style SelectStyle(object item, DependencyObject container)
	{
		OrderModel orderModel = item as OrderModel;
		if (orderModel?.ServerStatus == "ACTIVE")
		{
			if (orderModel.EnableRefund)
			{
				return ActiveItemStyle;
			}
			return UsingItemStyle;
		}
		if (orderModel?.ServerStatus == "USING")
		{
			return UsingItemStyle;
		}
		if (orderModel?.ServerStatus == "PROCESSING")
		{
			return ProcessItemStyle;
		}
		if (orderModel?.ServerStatus == "INVALID")
		{
			return InvalidItemStyle;
		}
		return RefundingItemStyle;
	}
}
