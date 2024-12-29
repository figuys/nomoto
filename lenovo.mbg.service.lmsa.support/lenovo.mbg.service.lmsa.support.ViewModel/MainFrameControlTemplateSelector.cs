using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class MainFrameControlTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement obj = (FrameworkElement)container;
		string resourceKey = (string.IsNullOrEmpty(FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, MainFrameViewModel.DEVICE_TYPE_STORAGE_KEY)) ? "startPage" : "ucListNavigation");
		return (DataTemplate)obj.FindResource(resourceKey);
	}
}
