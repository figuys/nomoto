using System;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class CategorySelectionChangedEventArgs : EventArgs
{
	public int SelectedCount { get; private set; }

	public CategorySelectionChangedEventArgs(int selectedCount)
	{
		SelectedCount = selectedCount;
	}
}
