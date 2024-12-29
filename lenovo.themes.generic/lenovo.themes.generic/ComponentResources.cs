using System;
using System.Windows;

namespace lenovo.themes.generic;

public class ComponentResources
{
	private static ComponentResources _singleInstance;

	public static ComponentResources SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new ComponentResources();
			}
			return _singleInstance;
		}
	}

	public ResourceDictionary ResourceDictionary { get; private set; }

	private ComponentResources()
	{
		ResourceDictionary = new ResourceDictionary();
		ResourceDictionary.Source = new Uri("/lenovo.themes.generic;component/Themes/Generic.xaml", UriKind.Relative);
	}

	public object GetResource(object key)
	{
		ComponentResourceKey key2 = new ComponentResourceKey(typeof(ComponentResources), key);
		if (ResourceDictionary.Contains(key2))
		{
			return ResourceDictionary[key2];
		}
		return null;
	}
}
