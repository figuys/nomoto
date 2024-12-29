using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class HelpWindowViewModel : ViewModelBase
{
	public ObservableCollection<HelpWindowItemViewModel> Items { get; set; }

	public ButtonViewModel CloseButtonViewModel { get; set; }

	public Window Owner { get; set; }

	private void CloseButtonClickCommandHandler(object parameter)
	{
		if (parameter is HelpWindow helpWindow)
		{
			helpWindow.Close();
		}
	}

	public HelpWindowViewModel()
	{
		Items = new ObservableCollection<HelpWindowItemViewModel>();
		CloseButtonViewModel = new ButtonViewModel
		{
			Background = new ImageBrush(ComponentResources.SingleInstance.GetResource("drawingImage_close_normal") as ImageSource),
			MouseOverBackground = new ImageBrush(ComponentResources.SingleInstance.GetResource("drawingImage_close_mouseover") as ImageSource),
			ClickCommand = new ReplayCommand(CloseButtonClickCommandHandler)
		};
	}
}
