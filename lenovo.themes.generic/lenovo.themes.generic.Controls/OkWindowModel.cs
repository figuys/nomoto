using System;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

public class OkWindowModel : LenovoPopupWindowModel
{
	protected override ViewModelBase GetDefaultViewModel()
	{
		return OKViewModel.DefaultValues();
	}

	protected override ControlTemplate GetDefaultControlTemplate()
	{
		return ComponentResources.SingleInstance.GetResource("defaultOKWindowTemplate") as ControlTemplate;
	}

	public LenovoPopupWindow CreateWindow(IntPtr ownerHandler, string title, string content, string okButtonText, ImageSource iamge, bool forceEnable = true)
	{
		OKViewModel obj = base.ViewModel as OKViewModel;
		obj.Title = title;
		obj.Content = content;
		obj.OKButtonText = okButtonText;
		obj.ContentIconImageSource = iamge;
		return CreateWindow();
	}

	public LenovoPopupWindow CreateCustomizeWindow(string title, string content, string okButtonText, bool showLine = true)
	{
		base.ContentControlTemplate = ComponentResources.SingleInstance.GetResource("defaultNoticeWindowTemplate") as ControlTemplate;
		OKViewModel obj = base.ViewModel as OKViewModel;
		obj.Title = title;
		obj.TitleForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		obj.Content = content;
		obj.OKButtonText = okButtonText;
		return CreateWindow();
	}

	public LenovoPopupWindow CreateWindow(string title, string content, string okButtonText, ImageSource iamge, int countdownSecondsClose)
	{
		OKViewModel vm = base.ViewModel as OKViewModel;
		vm.Title = title;
		vm.Content = content;
		vm.OKButtonText = okButtonText;
		vm.ContentIconImageSource = iamge;
		LenovoPopupWindow win = CreateWindow();
		win.Loaded += delegate
		{
			vm.Countdown(countdownSecondsClose, delegate
			{
				win.Dispatcher.Invoke(delegate
				{
					win.Close();
				});
			});
		};
		return win;
	}
}
