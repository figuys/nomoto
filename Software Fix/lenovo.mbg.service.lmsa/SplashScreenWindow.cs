using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.hostcontroller;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.LenovoId;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.ModelV6;

namespace lenovo.mbg.service.lmsa;

public partial class SplashScreenWindow : Window, IComponentConnector
{
	public SplashScreenWindow()
	{
		InitializeComponent();
		CheckClientVersion.Instance.CheckToolVersion(delegate
		{
		});
		versionTextblock.Text = HostProxy.LanguageService.Translate("K1794") + ": " + LMSAContext.MainProcessVersion;
		bool value = "TRUE".Equals(UserService.Single.GetLoginSetting("AgreePolicy"));
		cbxAgreePolicy.IsChecked = value;
		DeviceReadConfig.Instance.LoadTask();
		DotNetBrowserHelper.Instance.InitEngineAsync();
		LoadBroadcastAsync();
		LoadPluginAsync();
		base.Loaded += delegate
		{
			UserService.Single.AutoLoginIfNeed(delegate(Window win)
			{
				base.Dispatcher.Invoke(delegate
				{
					win?.Close();
				});
			});
		};
	}

	private Task LoadBroadcastAsync()
	{
		return Task.Run(() => WebServiceProxy.SingleInstance.GetNotice()).ContinueWith(delegate(Task<List<NoticeInfo>> ar)
		{
			if (ar.Result != null && ar.Result.Count > 0)
			{
				List<string> contents = (from n in ar.Result
					where !string.IsNullOrEmpty(n.noticeContent)
					select n.noticeContent).ToList();
				if (contents != null && contents.Count > 0)
				{
					base.Dispatcher.Invoke(delegate
					{
						broadcast_border.Visibility = Visibility.Visible;
						string input = string.Join("\t\t\t\t\t", contents);
						input = Regex.Replace(input, "[\\r\\n]", " ");
						broadcast_text.Text = string.Join("\t\t\t\t\t", input);
					});
				}
			}
		});
	}

	private Task LoadPluginAsync()
	{
		return Task.Run(delegate
		{
			List<PluginModel> list = new List<PluginModel>();
			List<PluginInfo> list2 = MainWindowControl.Instance.LoadPlguinInfo();
			foreach (PluginInfo item2 in list2)
			{
				if (!string.IsNullOrEmpty(item2.PluginID))
				{
					PluginModel item = new PluginModel
					{
						Info = item2,
						PluginName = item2.DisplayName,
						NormalIcon = "pack://application:,,,/Software Fix;component/ResourceV6/" + item2.Description + ".png",
						HoverIcon = "pack://application:,,,/Software Fix;component/ResourceV6/" + item2.Description + "Hover.png",
						Icon4B2B = "pack://application:,,,/lenovo.themes.generic;component/ResourceV6/p" + item2.Description + ".png"
					};
					list.Add(item);
				}
			}
			PluginContainer.Instance.LoadPuginDir(list.Select((PluginModel n) => n.Info.PluginDir).ToList());
			ApplcationClass.AvailablePlugins = list;
		});
	}

	private void OnBtnLogin(object sender, RoutedEventArgs e)
	{
		if (!UserService.Single.IsOnline)
		{
			LenovoIdWindow.ShowDialogEx(isRegister: false, delegate(Window win)
			{
				base.Dispatcher.Invoke(delegate
				{
					ShowHostWindow();
					win?.Close();
				});
			});
		}
		else
		{
			ShowHostWindow();
		}
	}

	private void ShowHostWindow()
	{
		if (UserService.Single.IsOnline)
		{
			Application.Current.MainWindow = new MainWindow();
			Application.Current.MainWindow.Show();
			Close();
		}
	}

	private async Task ClearResoucesAysnc()
	{
		await Task.Run(delegate
		{
			GlobalFun.KillProcess("adb");
		});
		await Task.Run(delegate
		{
			GlobalFun.KillProcess("qcomdloader");
		});
	}

	private void InitDeviceListener()
	{
		ClearResoucesAysnc().ContinueWith(delegate
		{
			Thread.Sleep(50);
			global::Smart.DeviceManagerEx.Start();
		});
	}

	private void GoToPrivacyPage_Click(object sender, RoutedEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}

	private void MouseLeftButtonDownDrag(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	private void OnCbxAgreePolicy(object sender, RoutedEventArgs e)
	{
		bool value = cbxAgreePolicy.IsChecked.Value;
		UserService.Single.AddOrUpdateAgreePolicySetting("AgreePolicy", value.ToString().ToUpper());
	}

	private void CloseClick(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		if (UserService.Single.IsOnline)
		{
			popupUser.IsOpen = true;
			return;
		}
		LenovoIdWindow.ShowDialogEx(isRegister: false, delegate(Window win)
		{
			base.Dispatcher.Invoke(delegate
			{
				win?.Close();
			});
		});
	}

	private void OnBtnLogout(object sender, RoutedEventArgs e)
	{
		popupUser.IsOpen = false;
		UserService.Single.Logout(delegate
		{
			DotNetBrowserHelper.Instance.ClearCookies();
		});
	}

	private void OnMouseLeave(object sender, MouseEventArgs e)
	{
		popupUser.IsOpen = false;
	}
}
