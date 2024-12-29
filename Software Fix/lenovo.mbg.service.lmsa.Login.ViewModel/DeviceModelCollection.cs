using System.Collections.ObjectModel;
using System.Collections.Specialized;
using lenovo.mbg.service.lmsa.Business;

namespace lenovo.mbg.service.lmsa.Login.ViewModel;

public class DeviceModelCollection : ObservableCollection<DeviceModel>
{
	protected override void RemoveItem(int index)
	{
		base.RemoveItem(index);
		Refresh();
	}

	public void Refresh()
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}
}
