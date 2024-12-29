using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class ContactAddOrEditView : UserControl, IComponentConnector, IStyleConnector
{
	public ContactAddOrEditView()
	{
		InitializeComponent();
	}

	private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new Regex("[^\\d-\\+\\(\\),;#N/\\*\\.]+");
		e.Handled = regex.IsMatch(e.Text);
	}
}
