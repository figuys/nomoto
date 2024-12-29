using System.Windows;

namespace lenovo.themes.generic;

public class Items : DependencyObject
{
	public static readonly DependencyProperty Item0Property = DependencyProperty.Register("Item0", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item1Property = DependencyProperty.Register("Item1", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item2Property = DependencyProperty.Register("Item2", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item3Property = DependencyProperty.Register("Item3", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item4Property = DependencyProperty.Register("Item4", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item5Property = DependencyProperty.Register("Item5", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item6Property = DependencyProperty.Register("Item6", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item7Property = DependencyProperty.Register("Item7", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item8Property = DependencyProperty.Register("Item8", typeof(object), typeof(Items), new PropertyMetadata(null));

	public static readonly DependencyProperty Item9Property = DependencyProperty.Register("Item9", typeof(object), typeof(Items), new PropertyMetadata(null));

	public object Item0
	{
		get
		{
			return GetValue(Item0Property);
		}
		set
		{
			SetValue(Item0Property, value);
		}
	}

	public object Item1
	{
		get
		{
			return GetValue(Item1Property);
		}
		set
		{
			SetValue(Item1Property, value);
		}
	}

	public object Item2
	{
		get
		{
			return GetValue(Item2Property);
		}
		set
		{
			SetValue(Item2Property, value);
		}
	}

	public object Item3
	{
		get
		{
			return GetValue(Item3Property);
		}
		set
		{
			SetValue(Item3Property, value);
		}
	}

	public object Item4
	{
		get
		{
			return GetValue(Item4Property);
		}
		set
		{
			SetValue(Item4Property, value);
		}
	}

	public object Item5
	{
		get
		{
			return GetValue(Item5Property);
		}
		set
		{
			SetValue(Item5Property, value);
		}
	}

	public object Item6
	{
		get
		{
			return GetValue(Item6Property);
		}
		set
		{
			SetValue(Item6Property, value);
		}
	}

	public object Item7
	{
		get
		{
			return GetValue(Item7Property);
		}
		set
		{
			SetValue(Item7Property, value);
		}
	}

	public object Item8
	{
		get
		{
			return GetValue(Item8Property);
		}
		set
		{
			SetValue(Item8Property, value);
		}
	}

	public object Item9
	{
		get
		{
			return GetValue(Item9Property);
		}
		set
		{
			SetValue(Item9Property, value);
		}
	}
}
