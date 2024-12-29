using System;
using System.Windows;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.ViewModels;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.Login.ViewModel.UserOperation;

public class LogoutMenuItemViewModel : MouseOverMenuItemViewModel
{
	public LogoutMenuItemViewModel()
	{
		base.Icon = Application.Current.FindResource("userLogoutNormal") as BitmapImage;
		base.MouseOverIcon = Application.Current.FindResource("userLogoutMouseover") as BitmapImage;
		base.Header = "K0019";
	}

	public override void ClickCommandHandler(object e)
	{
		LogOut(force: false);
	}

	public static void LogOut(bool force)
	{
		if (force || !MainWindowViewModel.SingleInstance.IsExecuteWork())
		{
			WebApiContext.GUID = Guid.NewGuid().ToString();
			UserService.Single.Dispose();
			UserService.Single.Logout(delegate
			{
				DotNetBrowserHelper.Instance.ClearCookies();
			});
			global::Smart.DeviceManagerEx.Dispose();
			DeviceConnectViewModel.Instance.Reset();
			Application.Current.MainWindow = new SplashScreenWindow();
			ApplcationClass.ApplcationStartWindow.Close();
			Application.Current.MainWindow.Show();
			MainWindowViewModel.SingleInstance.PluginArr.Clear();
			Configurations.ResetRescueResultMap();
			ViewContext.Reset();
			AppContext.IsLogIn = false;
		}
	}
}
