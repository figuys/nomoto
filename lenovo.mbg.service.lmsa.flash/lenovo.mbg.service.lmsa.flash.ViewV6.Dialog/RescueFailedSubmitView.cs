using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueFailedSubmitView : UserControl, IMessageViewV6, IComponentConnector
{
	private string _Category;

	private bool _IsSupportedMoliLena;

	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public RescueFailedSubmitView(string category)
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
		_Category = category;
		if (HostProxy.GlobalCache.Get("countrySupportedMoliLenaList") is JObject jObject)
		{
			string empty = string.Empty;
			try
			{
				empty = jObject.SelectToken("$.*[?(@.deviceType == '" + category + "')].url", errorWhenNoMatch: false)?.Value<string>();
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("support cache data" + jObject.ToString() + " ,error info:" + ex);
				empty = string.Empty;
			}
			_IsSupportedMoliLena = !string.IsNullOrEmpty(empty);
		}
		else
		{
			_IsSupportedMoliLena = false;
		}
		if (_IsSupportedMoliLena || "motoPhone".Equals(category))
		{
			VirtualAgent.Visibility = Visibility.Visible;
		}
		else
		{
			VirtualAgent.Visibility = Visibility.Collapsed;
		}
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		FireClose(true);
	}

	private void OnVirtualAgent(object sender, MouseButtonEventArgs e)
	{
		if (_IsSupportedMoliLena)
		{
			JObject jobj = new JObject
			{
				{ "action", "SelectMoliLena" },
				{ "category", _Category }
			};
			FireClose(true, jobj);
		}
		else if ("motoPhone".Equals(_Category))
		{
			Process.Start("https://moli.lenovo.com/callcenter/moli");
		}
	}

	private void OnforumClicked(object sender, MouseButtonEventArgs e)
	{
		JObject jobj = new JObject
		{
			{ "action", "SelectForum" },
			{ "category", _Category }
		};
		FireClose(true, jobj);
	}

	private void FireClose(bool? result, JObject jobj = null)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
		if (jobj != null)
		{
			HostProxy.HostNavigation.SwitchTo("a6099126929a4e74ac86f1c2405dcb32", jobj);
		}
	}
}
