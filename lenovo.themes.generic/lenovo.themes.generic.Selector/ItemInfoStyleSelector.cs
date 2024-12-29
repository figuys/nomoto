using System.Windows;
using System.Windows.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Selector;

internal class ItemInfoStyleSelector : StyleSelector
{
	public Style TextItemStyle { get; set; }

	public Style BatteryItemStyle { get; set; }

	public Style CopyItemStyle { get; set; }

	public override Style SelectStyle(object item, DependencyObject container)
	{
		DeviceInfoModel deviceInfoModel = item as DeviceInfoModel;
		if (deviceInfoModel != null && deviceInfoModel.Item3 == 1)
		{
			return CopyItemStyle;
		}
		if (deviceInfoModel != null && deviceInfoModel.Item3 == 2)
		{
			return BatteryItemStyle;
		}
		return TextItemStyle;
	}
}
