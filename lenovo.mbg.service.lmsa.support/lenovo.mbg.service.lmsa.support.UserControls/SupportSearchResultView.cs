using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.support.UserControls;

public partial class SupportSearchResultView : UserControl, IComponentConnector
{
	public SupportSearchResultView()
	{
		InitializeComponent();
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		Button button = FindFirstVisualChild<Button>(GetParentObject<UserControl>(this, "us_support"), "btn_back");
		if (button != null)
		{
			button.Visibility = Visibility.Visible;
		}
	}

	public T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
	{
		for (DependencyObject parent = VisualTreeHelper.GetParent(obj); parent != null; parent = VisualTreeHelper.GetParent(parent))
		{
			if (parent is T && ((((T)parent).Name == name) | string.IsNullOrEmpty(name)))
			{
				return (T)parent;
			}
		}
		return null;
	}

	public T FindFirstVisualChild<T>(DependencyObject obj, string childName) where T : DependencyObject
	{
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(obj, i);
			if (child != null && child is T && child.GetValue(FrameworkElement.NameProperty).ToString() == childName)
			{
				return (T)child;
			}
			T val = FindFirstVisualChild<T>(child, childName);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}
}
