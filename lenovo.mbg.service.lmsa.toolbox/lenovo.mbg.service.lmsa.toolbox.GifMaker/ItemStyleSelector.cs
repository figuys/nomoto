using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker;

public class ItemStyleSelector : StyleSelector
{
	public Style NoramlStyle { get; set; }

	public Style AddBtnStyle { get; set; }

	public override Style SelectStyle(object item, DependencyObject container)
	{
		if (item != null && item is ImageEntitiy imageEntitiy)
		{
			if (!imageEntitiy.IsImage)
			{
				return AddBtnStyle;
			}
			return NoramlStyle;
		}
		return base.SelectStyle(item, container);
	}
}
