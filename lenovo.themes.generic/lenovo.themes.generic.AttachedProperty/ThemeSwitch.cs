using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace lenovo.themes.generic.AttachedProperty;

public class ThemeSwitch
{
	public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(Uri), typeof(ThemeSwitch), new PropertyMetadata(null, PropertyChangedCallback));

	public static Uri GetSource(DependencyObject obj)
	{
		return (Uri)obj.GetValue(SourceProperty);
	}

	public static void SetSource(DependencyObject obj, Uri value)
	{
		obj.SetValue(SourceProperty, value);
	}

	private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (!(d is FrameworkElement frameworkElement))
		{
			return;
		}
		Uri uri = e.OldValue as Uri;
		Uri uri2 = e.NewValue as Uri;
		Collection<ResourceDictionary> mergedDictionaries = frameworkElement.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			if (uri != null)
			{
				ResourceDictionary resourceDictionary = new ResourceDictionary();
				resourceDictionary.Source = uri;
				mergedDictionaries.Remove(resourceDictionary);
			}
			if (uri2 != null)
			{
				ResourceDictionary resourceDictionary2 = new ResourceDictionary();
				resourceDictionary2.Source = uri2;
				mergedDictionaries.Add(resourceDictionary2);
			}
		}
	}
}
