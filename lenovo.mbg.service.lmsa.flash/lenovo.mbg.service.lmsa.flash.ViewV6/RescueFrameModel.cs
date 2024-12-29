using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class RescueFrameModel : ViewModelBase
{
	private string viewName = "RESCUING";

	public string ViewName
	{
		get
		{
			return viewName;
		}
		set
		{
			viewName = value;
			OnPropertyChanged("ViewName");
		}
	}

	public void ChangeView(string viewName)
	{
		Application.Current.Dispatcher.Invoke(() => ViewName = viewName);
	}
}
