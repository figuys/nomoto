using System.ComponentModel;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class ViewModelBase
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
