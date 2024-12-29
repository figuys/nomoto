using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

internal class MessageBoxHelper
{
	public static bool DeleteConfirmMessagebox(string title, string content)
	{
		return DeleteConfirmMessagebox(title, content, null);
	}

	public static bool DeleteConfirmMessagebox(string title, string content, ImageSource image)
	{
		if (image == null)
		{
			image = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Assets/Images/PicPopup/delete.png"));
		}
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, content, "K0208", "K0583", image);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		return win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult;
	}

	public static bool DeleteConfirmMessagebox(string title, string content, string button1, string button2, ImageSource image)
	{
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, content, button1, button2, image);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		return win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult;
	}
}
