using System;
using System.Collections.Generic;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class PicOriginalHolderManager
{
	private static PicOriginalHolderManager _single;

	private readonly object _holderLock;

	public static PicOriginalHolderManager Single
	{
		get
		{
			if (_single == null)
			{
				_single = new PicOriginalHolderManager();
			}
			return _single;
		}
	}

	public PicOriginalHolder Holder { get; private set; }

	public PicOriginalHolderViewModel ViewModel { get; private set; }

	private PicOriginalHolderManager()
	{
		_holderLock = new object();
	}

	public void Prepar(List<PicInfoViewModel> pics)
	{
	}

	public void DisplayNext()
	{
	}

	public void DisplayPrev()
	{
	}

	private void ViewModel_Closing(object sender, EventArgs e)
	{
		CloseHolder();
	}

	public void CloseHolder()
	{
		lock (_holderLock)
		{
			if (Holder != null)
			{
				Holder.Close();
			}
			if (ViewModel != null)
			{
				ViewModel.Dispose();
			}
		}
	}
}
