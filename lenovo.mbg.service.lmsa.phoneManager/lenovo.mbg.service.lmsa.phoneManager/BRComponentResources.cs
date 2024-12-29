using System;
using System.Windows;

namespace lenovo.mbg.service.lmsa.phoneManager;

public class BRComponentResources
{
	private static BRComponentResources _singleInstance;

	public static BRComponentResources SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new BRComponentResources();
			}
			return _singleInstance;
		}
	}

	public ResourceDictionary ResourceDictionary { get; private set; }

	private BRComponentResources()
	{
		ResourceDictionary = new ResourceDictionary();
		ResourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Themes/Generic.xaml", UriKind.Relative);
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
