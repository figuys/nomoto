using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace lenovo.themes.generic.AttachedProperty;

public class ControlAdorner : Adorner
{
	private Control _child;

	public AdornedPlaceholder Placeholder { get; set; }

	protected override int VisualChildrenCount => 1;

	public Control Child
	{
		get
		{
			return _child;
		}
		set
		{
			if (_child != null)
			{
				RemoveVisualChild(_child);
			}
			_child = value;
			if (_child != null)
			{
				AddVisualChild(_child);
			}
		}
	}

	public ControlAdorner(UIElement adornedElement)
		: base(adornedElement)
	{
	}

	protected override Visual GetVisualChild(int index)
	{
		if (index != 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		return _child;
	}

	protected override Size MeasureOverride(Size constraint)
	{
		_child.Measure(constraint);
		return _child.DesiredSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		finalSize.Height = 103.0;
		_child.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
		UpdateLocation();
		return new Size(_child.ActualWidth, _child.ActualHeight);
	}

	private void UpdateLocation()
	{
		if (Placeholder == null)
		{
			return;
		}
		Transform transform = (Transform)TransformToDescendant(Placeholder);
		if (transform != Transform.Identity)
		{
			Transform renderTransform = base.RenderTransform;
			if (renderTransform == null || renderTransform == Transform.Identity)
			{
				base.RenderTransform = transform;
				return;
			}
			TransformGroup transformGroup = new TransformGroup();
			transformGroup.Children.Add(renderTransform);
			transformGroup.Children.Add(transform);
			base.RenderTransform = new MatrixTransform(transformGroup.Value);
		}
	}
}
