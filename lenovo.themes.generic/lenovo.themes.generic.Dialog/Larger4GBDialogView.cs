using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.lang;

namespace lenovo.themes.generic.Dialog;

public partial class Larger4GBDialogView : UserControl, IComponentConnector
{
	public Larger4GBDialogView(int _type)
	{
		InitializeComponent();
		switch (_type)
		{
		case 0:
			txtSubContent.Text = LangTranslation.Translate("K1783");
			break;
		case 1:
			txtSubContent.Text = LangTranslation.Translate("K1889");
			break;
		case 2:
			txtSubContent.Text = LangTranslation.Translate("K1890");
			break;
		}
	}

	private void Run_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		Process process = new Process();
		process.StartInfo = new ProcessStartInfo(Configurations.TRANSFER_FILE_ERROR_TXT_PATH)
		{
			UseShellExecute = true
		};
		process.Start();
	}
}
