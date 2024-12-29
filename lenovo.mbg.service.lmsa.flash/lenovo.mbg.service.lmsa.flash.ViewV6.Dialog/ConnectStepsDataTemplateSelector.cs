using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class ConnectStepsDataTemplateSelector : DataTemplateSelector
{
	public DataTemplate StartPageTemplate { get; set; }

	public DataTemplate VerticalAlignmentTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is ConnectStepsModel connectStepsModel)
		{
			if (connectStepsModel.Layout == "S")
			{
				return StartPageTemplate;
			}
			if (connectStepsModel.Layout == "V")
			{
				return VerticalAlignmentTemplate;
			}
		}
		return base.SelectTemplate(item, container);
	}
}
