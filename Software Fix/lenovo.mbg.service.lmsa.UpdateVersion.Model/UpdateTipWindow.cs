using System.Windows;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Model;

public class UpdateTipWindow
{
	private WindowUpdateTip updateTipWindow = null;

	public WindowUpdateTip UpdateTipWindowShow
	{
		get
		{
			return updateTipWindow;
		}
		set
		{
			updateTipWindow = value;
		}
	}

	public void ShowUpdateTipWindow()
	{
		if (updateTipWindow != null)
		{
			if (ApplcationClass.ApplcationStartWindow.WindowState == WindowState.Maximized)
			{
				UpdateTipWindowShow.Left = SystemParameters.PrimaryScreenWidth - 60.0;
				UpdateTipWindowShow.Top = 90.0;
			}
			else if (ApplcationClass.ApplcationStartWindow.WindowState == WindowState.Normal)
			{
				updateTipWindow.Left = ApplcationClass.ApplcationStartWindow.Left + 860.0;
				updateTipWindow.Top = ApplcationClass.ApplcationStartWindow.Top + 90.0;
			}
			HostProxy.HostMaskLayerWrapper.New(updateTipWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				updateTipWindow.Show();
			});
		}
	}

	public void HideUpdateTipWindow()
	{
		if (updateTipWindow != null)
		{
			updateTipWindow.Hide();
		}
	}

	public void ResetLocation()
	{
		if (updateTipWindow.IsVisible)
		{
			ShowUpdateTipWindow();
		}
	}
}
