using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace lenovo.themes.generic.AttachedProperty;

public class AdornedPlaceholder : FrameworkElement
{
	public Adorner Adorner
	{
		get
		{
			Visual visual = this;
			while (visual != null && !(visual is Adorner))
			{
				visual = (Visual)VisualTreeHelper.GetParent(visual);
			}
			return (Adorner)visual;
		}
	}

	public FrameworkElement AdornedElement
	{
		get
		{
			if (Adorner != null)
			{
				return Adorner.AdornedElement as FrameworkElement;
			}
			return null;
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (Adorner is ControlAdorner controlAdorner)
		{
			controlAdorner.Placeholder = this;
		}
		FrameworkElement adornedElement = AdornedElement;
		return new Size(adornedElement.ActualWidth, adornedElement.ActualHeight);
	}
}
