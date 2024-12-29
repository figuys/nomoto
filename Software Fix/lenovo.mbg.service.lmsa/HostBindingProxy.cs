using System.Windows;

namespace lenovo.mbg.service.lmsa;

public class HostBindingProxy : Freezable
{
	public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(HostBindingProxy), new UIPropertyMetadata(null));

	public object Data
	{
		get
		{
			return GetValue(DataProperty);
		}
		set
		{
			SetValue(DataProperty, value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return new HostBindingProxy();
	}
}
