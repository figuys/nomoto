using System;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.framework.lang;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

public class OkCancelWindowModel : LenovoPopupWindowModel
{
	protected override ViewModelBase GetDefaultViewModel()
	{
		return OKCancelViewModel.DefaultValues();
	}

	protected override ControlTemplate GetDefaultControlTemplate()
	{
		return ComponentResources.SingleInstance.GetResource("defaultOKCancelWindowTemplate") as ControlTemplate;
	}

	public LenovoPopupWindow CreateWindow(string contentKey)
	{
		OKCancelViewModel obj = base.ViewModel as OKCancelViewModel;
		obj.Title = "K0711";
		obj.Content = LangTranslation.Translate(contentKey);
		obj.OKButtonText = "K0571";
		obj.CancelButtonText = "K0570";
		return CreateWindow();
	}

	public LenovoPopupWindow CreateWindow(string title, string content, string cancelButtonText, string okButtonText, ImageSource iamge)
	{
		OKCancelViewModel obj = base.ViewModel as OKCancelViewModel;
		obj.Title = title;
		obj.Content = content;
		obj.CancelButtonText = cancelButtonText;
		obj.OKButtonText = okButtonText;
		obj.ContentIconImageSource = iamge;
		return CreateWindow();
	}

	public LenovoPopupWindow CreateWindow(string title, string content, string cancelButtonText, string okButtonText)
	{
		OKCancelViewModel obj = base.ViewModel as OKCancelViewModel;
		obj.Title = title;
		obj.Content = content;
		obj.CancelButtonText = cancelButtonText;
		obj.OKButtonText = okButtonText;
		base.ContentControlTemplate = ComponentResources.SingleInstance.GetResource("defaultOKCancelNoImageWindowTemplate") as ControlTemplate;
		return CreateWindow();
	}

	public LenovoPopupWindow CreateWindow(IntPtr ownerHandler, string title, string content, string cancelButtonText, string okButtonText, ImageSource iamge)
	{
		OKCancelViewModel obj = base.ViewModel as OKCancelViewModel;
		obj.Title = title;
		obj.Content = content;
		obj.CancelButtonText = cancelButtonText;
		obj.OKButtonText = okButtonText;
		obj.ContentIconImageSource = iamge;
		return CreateWindow();
	}

	public LenovoPopupWindow CreateWindow(string title, string content, string cancelButtonText, string okButtonText, ImageSource iamge, ref OKCancelViewModel vm)
	{
		vm = base.ViewModel as OKCancelViewModel;
		vm.Title = title;
		vm.Content = content;
		vm.CancelButtonText = cancelButtonText;
		vm.OKButtonText = okButtonText;
		vm.ContentIconImageSource = iamge;
		return CreateWindow();
	}

	public LenovoPopupWindow CreateWindow(IntPtr ownerHandler, string title, string content, string cancelButtonText, string okButtonText, ImageSource iamge, ref OKCancelViewModel vm, ref LenovoPopupWindow win)
	{
		vm = base.ViewModel as OKCancelViewModel;
		vm.Title = title;
		vm.Content = content;
		vm.CancelButtonText = cancelButtonText;
		vm.OKButtonText = okButtonText;
		vm.ContentIconImageSource = iamge;
		return CreateWindow();
	}
}
