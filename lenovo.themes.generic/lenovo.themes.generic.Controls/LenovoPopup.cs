using System.Windows;
using System.Windows.Controls.Primitives;

namespace lenovo.themes.generic.Controls;

public class LenovoPopup : Popup
{
	public static readonly DependencyProperty IsCanShowProperty = DependencyProperty.Register("IsCanShow", typeof(bool), typeof(LenovoPopup), new PropertyMetadata(null));

	public bool IsCanShow
	{
		get
		{
			return (bool)GetValue(IsCanShowProperty);
		}
		set
		{
			SetValue(IsCanShowProperty, value);
		}
	}
}
