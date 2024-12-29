using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.ControlsV6;

public partial class DeviceWifiWaitConnectView : UserControl, IComponentConnector
{
	public DeviceWifiWaitConnectView()
	{
		InitializeComponent();
		txtRandomCode.Text = RandomAesKeyHelper.Instance.EncryptCode;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
