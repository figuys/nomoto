using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lenovo.themes.generic.ViewModelV6;

public class NotifyBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string name = "")
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
