using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lenovo.mbg.service.lmsa.phoneManager;

public class BaseNotify : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
