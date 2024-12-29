using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

internal class WPFTreeHelper
{
	private static string GetTypeDescription(object obj)
	{
		return obj.GetType().FullName;
	}

	public static TreeViewItem GetLogicTree(DependencyObject dobj)
	{
		if (dobj == null)
		{
			return null;
		}
		TreeViewItem treeViewItem = new TreeViewItem
		{
			Header = GetTypeDescription(dobj),
			IsExpanded = true
		};
		foreach (object child in LogicalTreeHelper.GetChildren(dobj))
		{
			TreeViewItem logicTree = GetLogicTree(child as DependencyObject);
			if (logicTree != null)
			{
				treeViewItem.Items.Add(logicTree);
			}
		}
		return treeViewItem;
	}

	public static TreeViewItem GetVisualTree(DependencyObject dobj)
	{
		if (dobj == null)
		{
			return null;
		}
		TreeViewItem treeViewItem = new TreeViewItem
		{
			Header = GetTypeDescription(dobj),
			IsExpanded = true
		};
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dobj); i++)
		{
			TreeViewItem visualTree = GetVisualTree(VisualTreeHelper.GetChild(dobj, i));
			if (visualTree != null)
			{
				treeViewItem.Items.Add(visualTree);
			}
		}
		return treeViewItem;
	}
}
