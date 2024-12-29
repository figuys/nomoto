using System.Windows;

namespace lenovo.mbg.service.framework.lang;

public class LangToolTip
{
	public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached("ToolTip", typeof(string), typeof(LangToolTip), new PropertyMetadata(string.Empty, OnToolTipChanged));

	public static void SetToolTipKey(DependencyObject dp, object value)
	{
		dp.SetValue(ToolTipProperty, value);
	}

	public static string GetToolTipKey(DependencyObject dp)
	{
		return dp.GetValue(ToolTipProperty) as string;
	}

	private static void OnToolTipChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue != null && args.NewValue != args.OldValue && obj is FrameworkElement)
		{
			(obj as FrameworkElement).ToolTip = LangTranslation.Translate(args.NewValue.ToString());
		}
	}
}
