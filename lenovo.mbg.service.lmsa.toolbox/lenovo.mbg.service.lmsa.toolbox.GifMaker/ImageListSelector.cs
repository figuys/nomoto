using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker;

public class ImageListSelector : DataTemplateSelector
{
	public DataTemplate ImageTemplate { get; set; }

	public DataTemplate BtnTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item != null && item is ImageEntitiy imageEntitiy)
		{
			if (!imageEntitiy.IsImage)
			{
				return BtnTemplate;
			}
			return ImageTemplate;
		}
		return base.SelectTemplate(item, container);
	}
}
