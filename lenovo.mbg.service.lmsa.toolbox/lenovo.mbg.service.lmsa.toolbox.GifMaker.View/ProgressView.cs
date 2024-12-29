using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.View;

public partial class ProgressView : Window, IComponentConnector
{
	public ProgressModel Model { get; set; }

	public ProgressView()
	{
		InitializeComponent();
		base.ShowInTaskbar = false;
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		Model = new ProgressModel
		{
			Percentage = 0.0,
			CloseCmd = new RoutedCommand(),
			Information = "K0226"
		};
		base.CommandBindings.Add(new CommandBinding(Model.CloseCmd, delegate
		{
			Close();
		}));
		base.DataContext = Model;
	}
}
