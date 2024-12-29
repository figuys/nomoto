using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.common.Form.View;

public partial class FormItemView : UserControl, IComponentConnector
{
	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(FormItemView), new PropertyMetadata(null));

	public static readonly DependencyProperty RequireProperty = DependencyProperty.Register("RequireTip", typeof(object), typeof(FormItemView), new PropertyMetadata(null));

	public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register("RightContent", typeof(object), typeof(FormItemView), new PropertyMetadata(null));

	public static readonly DependencyProperty ButtomContentProperty = DependencyProperty.Register("ButtomContent", typeof(object), typeof(FormItemView), new PropertyMetadata(null));

	public object Title
	{
		get
		{
			return GetValue(TitleProperty);
		}
		set
		{
			SetValue(TitleProperty, value);
		}
	}

	public object Require
	{
		get
		{
			return GetValue(RequireProperty);
		}
		set
		{
			SetValue(RequireProperty, value);
		}
	}

	public object RightContent
	{
		get
		{
			return GetValue(RightContentProperty);
		}
		set
		{
			SetValue(RightContentProperty, value);
		}
	}

	public object ButtomContent
	{
		get
		{
			return GetValue(ButtomContentProperty);
		}
		set
		{
			SetValue(ButtomContentProperty, value);
		}
	}

	public FormItemView()
	{
		InitializeComponent();
	}
}
