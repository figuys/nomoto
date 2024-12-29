using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.backuprestore.Common;

public class MessageBoxHelper
{
	public static bool DeleteConfirmMessagebox(string title, string content)
	{
		return DeleteConfirmMessagebox(title, content, null);
	}

	public static bool DeleteConfirmMessagebox(string title, string content, ImageSource image)
	{
		if (image == null)
		{
			image = (ImageSource)Application.Current.FindResource("msgbox_delete_icon");
		}
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, content, "K0208", "K0583", image);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		return win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult;
	}
}
