using System;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.support.ViewContext;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.support;

[PluginExport(typeof(IPlugin), "a6099126929a4e74ac86f1c2405dcb32")]
public class Plugin : PluginBase
{
	public override void Init()
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		resourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.support;component/Themes/Generic.xaml", UriKind.Relative);
		Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
	}

	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		Context.mainViewModel = new MainFrameViewModel();
		Context.mainViewModel.LoadData();
		return new MainFrame
		{
			DataContext = Context.mainViewModel
		};
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void OnSelected(string val)
	{
		base.OnSelected(val);
	}

	public override void OnInit(object data)
	{
		base.OnInit(data);
		if (!(data is JObject jObject))
		{
			return;
		}
		string text = jObject.Value<string>("action");
		string category = jObject.Value<string>("category");
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (!(text == "SelectForum"))
		{
			if (text == "SelectMoliLena")
			{
				Context.mainViewModel.SelectTarget(ViewType.MOLI, category);
			}
		}
		else
		{
			Context.mainViewModel.SelectTarget(ViewType.FORUM, category);
		}
	}
}
