using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace lenovo.themes.generic.AttachedProperty;

public class ListViewLayoutManager
{
	public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(ListViewLayoutManager), new FrameworkPropertyMetadata(OnLayoutManagerEnabledChanged));

	private readonly ListView listView;

	private ScrollViewer scrollViewer;

	private bool loaded;

	private Cursor resizeCursor;

	private ScrollBarVisibility verticalScrollBarVisibility = ScrollBarVisibility.Auto;

	public ListView ListView => listView;

	public ScrollBarVisibility VerticalScrollBarVisibility
	{
		get
		{
			return verticalScrollBarVisibility;
		}
		set
		{
			verticalScrollBarVisibility = value;
		}
	}

	public ListViewLayoutManager(ListView listView)
	{
		if (listView == null)
		{
			throw new ArgumentNullException("listView");
		}
		this.listView = listView;
		this.listView.Loaded += ListViewLoaded;
	}

	public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
	{
		dependencyObject.SetValue(EnabledProperty, enabled);
	}

	private void RegisterEvents(DependencyObject start)
	{
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
		{
			Visual visual = VisualTreeHelper.GetChild(start, i) as Visual;
			if (visual is Thumb)
			{
				GridViewColumn gridViewColumn = FindColumn(visual);
				if (gridViewColumn == null)
				{
					continue;
				}
				Thumb obj = visual as Thumb;
				obj.PreviewMouseMove += ThumbPreviewMouseMove;
				obj.PreviewMouseLeftButtonDown += ThumbPreviewMouseLeftButtonDown;
				DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn)).AddValueChanged(gridViewColumn, GridColumnWidthChanged);
			}
			else if (scrollViewer == null && visual is ScrollViewer)
			{
				scrollViewer = visual as ScrollViewer;
			}
			RegisterEvents(visual);
		}
	}

	private GridViewColumn FindColumn(DependencyObject element)
	{
		if (element == null)
		{
			return null;
		}
		while (element != null)
		{
			if (element is GridViewColumnHeader)
			{
				return ((GridViewColumnHeader)element).Column;
			}
			element = VisualTreeHelper.GetParent(element);
		}
		return null;
	}

	protected virtual void ResizeColumns()
	{
		if (!(listView.View is GridView gridView))
		{
			return;
		}
		double num = ((scrollViewer != null) ? scrollViewer.ViewportWidth : listView.ActualWidth);
		if (num <= 0.0)
		{
			return;
		}
		double num2 = 0.0;
		double num3 = 0.0;
		foreach (GridViewColumn column in gridView.Columns)
		{
			num3 += column.ActualWidth;
		}
		if (!(num2 <= 0.0))
		{
			_ = num - num3;
			_ = 0.0;
		}
	}

	private double SetRangeColumnToBounds(GridViewColumn gridViewColumn)
	{
		double width = gridViewColumn.Width;
		double? rangeMinWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
		double? rangeMaxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);
		if (rangeMinWidth.HasValue && rangeMaxWidth.HasValue && rangeMinWidth > rangeMaxWidth)
		{
			return 0.0;
		}
		if (rangeMinWidth.HasValue && gridViewColumn.Width < rangeMinWidth.Value)
		{
			gridViewColumn.Width = rangeMinWidth.Value;
		}
		else if (rangeMaxWidth.HasValue && gridViewColumn.Width > rangeMaxWidth.Value)
		{
			gridViewColumn.Width = rangeMaxWidth.Value;
		}
		return gridViewColumn.Width - width;
	}

	private void ListViewLoaded(object sender, RoutedEventArgs e)
	{
		RegisterEvents(listView);
		ResizeColumns();
		loaded = true;
	}

	private void ThumbPreviewMouseMove(object sender, MouseEventArgs e)
	{
		Thumb thumb = sender as Thumb;
		GridViewColumn gridViewColumn = FindColumn(thumb);
		if (gridViewColumn == null || !thumb.IsMouseCaptured || !RangeColumn.IsRangeColumn(gridViewColumn))
		{
			return;
		}
		double? rangeMinWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
		double? rangeMaxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);
		if (!rangeMinWidth.HasValue || !rangeMaxWidth.HasValue || !(rangeMinWidth > rangeMaxWidth))
		{
			if (resizeCursor == null)
			{
				resizeCursor = thumb.Cursor;
			}
			if (rangeMinWidth.HasValue && gridViewColumn.Width <= rangeMinWidth.Value)
			{
				thumb.Cursor = Cursors.No;
			}
			else if (rangeMaxWidth.HasValue && gridViewColumn.Width >= rangeMaxWidth.Value)
			{
				thumb.Cursor = Cursors.No;
			}
			else
			{
				thumb.Cursor = resizeCursor;
			}
		}
	}

	private void ThumbPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		Thumb element = sender as Thumb;
		FindColumn(element);
	}

	private void GridColumnWidthChanged(object sender, EventArgs e)
	{
		if (loaded)
		{
			GridViewColumn gridViewColumn = sender as GridViewColumn;
			if (!RangeColumn.IsRangeColumn(gridViewColumn) || SetRangeColumnToBounds(gridViewColumn) == 0.0)
			{
				ResizeColumns();
			}
		}
	}

	private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		if (loaded && e.ViewportWidthChange != 0.0)
		{
			ResizeColumns();
		}
	}

	private static void OnLayoutManagerEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		if (dependencyObject is ListView listView && (bool)e.NewValue)
		{
			new ListViewLayoutManager(listView);
		}
	}
}
