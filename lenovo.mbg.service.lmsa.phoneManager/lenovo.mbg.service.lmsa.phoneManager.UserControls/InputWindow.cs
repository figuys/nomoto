using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class InputWindow : LenovoPopupWindowModel
{
	protected override ViewModelBase GetDefaultViewModel()
	{
		InputWindowViewModel inputWindowViewModel = InputWindowViewModel.DefaultValues();
		inputWindowViewModel.TipForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#494949"));
		return inputWindowViewModel;
	}

	protected override ControlTemplate GetDefaultControlTemplate()
	{
		return PopupWindowResources.SingleInstance.GetResource("addContactGroupWindowTemplate") as ControlTemplate;
	}

	public LenovoPopupWindow CreateWindow(string title, string tipText, string cancelButtonText, string okButtonText, ImageSource iamge)
	{
		InputWindowViewModel obj = base.ViewModel as InputWindowViewModel;
		obj.Title = title;
		obj.TipText = tipText;
		obj.CancelButtonText = cancelButtonText;
		obj.OKButtonText = okButtonText;
		obj.ContentIconImageSource = iamge;
		return CreateWindow();
	}
}
