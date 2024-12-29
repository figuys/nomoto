using System;
using System.Windows;

namespace lenovo.mbg.service.lmsa.phoneManager;

public class LeftNavResources
{
	private static LeftNavResources _singleInstance;

	public static LeftNavResources SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new LeftNavResources();
			}
			return _singleInstance;
		}
	}

	public ResourceDictionary ResourceDictionary { get; private set; }

	private LeftNavResources()
	{
		ResourceDictionary = new ResourceDictionary();
		ResourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Themes/MenuNavigationIcon.xaml", UriKind.Relative);
		ResourceDictionary.MergedDictionaries.Add(new ResourceDictionary
		{
			Source = new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/ThemesV6/IconSvgDrawingIamge.xaml", UriKind.Relative)
		});
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
