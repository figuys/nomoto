using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.hardwaretest.View;

public partial class StartView : UserControl, IComponentConnector
{
	public StartView()
	{
		InitializeComponent();
		txtRandomCode.Text = RandomAesKeyHelper.Instance.EncryptCode;
	}
}
