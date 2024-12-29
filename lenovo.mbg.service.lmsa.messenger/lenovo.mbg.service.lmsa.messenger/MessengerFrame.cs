using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using DotNetBrowser.Navigation;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.messenger;

public partial class MessengerFrame : System.Windows.Controls.UserControl, IComponentConnector
{
	protected string MoliRequestBasicUrl { get; set; }

	public MessengerFrame()
	{
		InitializeComponent();
	}

	public void LoadMoliOrLenaUrl(string uri, string _country)
	{
		Task.Run(delegate
		{
			string text = ProcessUrl(uri, _country);
			Dictionary<string, string> extraData = new Dictionary<string, string>
			{
				{ "uri", text },
				{ "country", _country }
			};
			if (!string.IsNullOrEmpty(text))
			{
				HostProxy.CurrentDispatcher?.Invoke(() => popupWin.IsOpen = true);
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				LoadResult loadResult = DotNetBrowserHelper.Instance.LoadUrl(browserView, text, delegate
				{
					HostProxy.CurrentDispatcher?.Invoke(() => popupWin.IsOpen = false);
				});
				stopwatch.Stop();
				HostProxy.BehaviorService.Collect(BusinessType.SUPPORT_MOLI, new BusinessData(BusinessType.SUPPORT_MOLI, null).Update(stopwatch.ElapsedMilliseconds, (loadResult == LoadResult.Failed) ? BusinessStatus.FALIED : BusinessStatus.SUCCESS, extraData));
			}
		});
	}

	private static void SetRegister()
	{
		string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
		if (Environment.Is64BitOperatingSystem)
		{
			Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", moduleName, 11001, RegistryValueKind.DWord);
		}
		else
		{
			Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", moduleName, 11001, RegistryValueKind.DWord);
		}
	}

	private void GoToMoli_Click(object sender, RoutedEventArgs e)
	{
	}

	private void GoToPrivacyStatement_Click(object sender, RoutedEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser("https://www3.lenovo.com/us/en/privacy");
	}

	protected string ProcessUrl(string uri, string _country)
	{
		JObject jObject = new JObject();
		string text = FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "LatestLoginUserInfo");
		if (string.IsNullOrEmpty(text))
		{
			LogHelper.LogInstance.Info("get user login info failed");
			return null;
		}
		JObject jObject2 = JsonHelper.DeserializeJson2Jobjcet(text);
		jObject.Add("country", _country);
		jObject.Add("appVersion", LMSAContext.MainProcessVersion);
		if (jObject2.SelectToken("$.LoginFormData.userSource").Value<string>() == "lmsa")
		{
			jObject.Add("phoneNumber", jObject2.Value<string>("phone"));
			jObject.Add("email", jObject2.Value<string>("email"));
			jObject.Add("userId", jObject2.SelectToken("$.UserName").Value<string>());
			jObject.Add("accountId", jObject2.SelectToken("$.UserID").Value<string>());
		}
		else
		{
			jObject.Add("email", null);
			jObject.Add("phoneNumber", null);
			JObject jObject3 = JsonHelper.DeserializeJson2Jobjcet(jObject2.SelectToken("$.LoginFormData.userData").Value<string>());
			string text2 = jObject3.Value<string>("lenovoId");
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Regex.Match(jObject2.Value<string>("UserName"), "(?<key>\\d+)&+(?<value>.+)", RegexOptions.IgnoreCase).Groups["key"].Value;
			}
			string text3 = jObject3.Value<string>("useName");
			if (!string.IsNullOrEmpty(text3) && Regex.IsMatch(text3, ".+@.+"))
			{
				jObject["email"] = text3;
			}
			else
			{
				jObject["phoneNumber"] = text3;
			}
			jObject.Add("userId", text2);
			jObject.Add("lenovoId", text2);
		}
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice == null)
		{
			if (uri.ToLower().Contains("lena"))
			{
				jObject.Add("product", "Tablet");
			}
			IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
			if (conntectedDevices != null && conntectedDevices.Count > 0)
			{
				masterDevice = conntectedDevices.First();
			}
		}
		else
		{
			jObject.Add("product", masterDevice.Property.ModelName);
			jObject.Add("imei", masterDevice.Property.IMEI1);
			jObject.Add("os", masterDevice.Property.AndroidVersion);
			jObject.Add("sn", masterDevice.Property.SN);
		}
		string text4 = JsonHelper.SerializeObject2Json(jObject);
		LogHelper.LogInstance.Debug("request moli url: " + uri + ", aparams: " + text4);
		string arg = GlobalFun.EncodeBase64(Encoding.UTF8.GetBytes(text4));
		return $"{uri}?data={arg}";
	}

	private void SwitchToLoading()
	{
		formshost.Visibility = Visibility.Collapsed;
		popupWin.IsOpen = true;
		homeViewPanel.Visibility = Visibility.Collapsed;
	}

	private void NavigatedHandler(object sender, WebBrowserNavigatedEventArgs e)
	{
		SwitchToHtmlView();
	}

	private void SwitchToHtmlView()
	{
		Task.Run(delegate
		{
			Thread.Sleep(5000);
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				popupWin.IsOpen = false;
				formshost.Visibility = Visibility.Visible;
				homeViewPanel.Visibility = Visibility.Collapsed;
			});
		});
	}

	private void SwitchToHome()
	{
		formshost.Visibility = Visibility.Collapsed;
		popupWin.IsOpen = false;
		homeViewPanel.Visibility = Visibility.Visible;
	}
}
