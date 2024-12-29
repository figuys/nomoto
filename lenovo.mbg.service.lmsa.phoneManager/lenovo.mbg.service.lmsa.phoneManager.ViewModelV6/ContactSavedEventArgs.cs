using System;
using lenovo.mbg.service.lmsa.phoneManager.Business;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ContactSavedEventArgs : EventArgs
{
	public bool SavedSuccess { get; private set; }

	public EditMode EditMode { get; private set; }

	public ContactSavedEventArgs()
	{
	}

	public ContactSavedEventArgs(EditMode editModel, bool savedSuccess)
	{
		EditMode = editModel;
		SavedSuccess = savedSuccess;
	}
}
