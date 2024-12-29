using System;
using System.Windows;

namespace lenovo.mbg.service.lmsa.phoneManager;

public class PopupWindowResources
{
	private static PopupWindowResources _singleInstance;

	public static PopupWindowResources SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new PopupWindowResources();
			}
			return _singleInstance;
		}
	}

	public ResourceDictionary ResourceDictionary { get; private set; }

	private PopupWindowResources()
	{
		ResourceDictionary = new ResourceDictionary();
		ResourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Themes/PopupWindow.xaml", UriKind.Relative);
	}

	public object GetResource(object key)
	{
		if (ResourceDictionary.Contains(key))
		{
			return ResourceDictionary[key];
		}
		return null;
	}
}
