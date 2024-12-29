using System;
using System.Windows;
using System.Windows.Media;

namespace lenovo.mbg.service.framework.services;

public interface IMessageBox
{
	void FrozenWindow(bool isShowLoading = false);

	void UnFrozenWindow();

	void SetUiWorkEnable(bool isWorkEnable);

	void ShowDownloadCenter(bool isShow = true);

	void SelRegistedDevIfExist(string category, Action<string> callBack = null);

	void CallMotoCare(string imei, object wModel);

	bool? ShowMessage(IUserMsgControl userUi);

	bool? ShowMessage(string msg, out IUserMsgControl win, MessageBoxImage icon = MessageBoxImage.Asterisk, MessageBoxButton btn = MessageBoxButton.OK);

	bool? ShowMessage(string title, string msg, string okBtnText, string cancelBtnText, bool isCloseBtn, ImageSource image, out Window wnd);

	bool? ShowMessage(string title, string msg, string okBtnText, string cancelBtnText, MessageBoxImage icon, string notifyText);

	bool? ShowMessage(string msg, MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Asterisk, bool isCloseBtn = false, bool isPrivacy = false, Action<bool?> closeAction = null);

	bool? ShowMessage(string title, string msg, string okBtnText = "K0327", string cancelBtnText = null, bool isCloseBtn = false, ImageSource image = null, MessageBoxImage icon = MessageBoxImage.Asterisk, Action<bool?> closeAction = null, Window ownerWindow = null);

	bool? ShowOutterCloseMessage(string title, string msg, MessageBoxImage icon = MessageBoxImage.Asterisk, Action<IUserMsgControl> outterAction = null);

	bool? ShowConfrimCloseMessage(out Action<bool?> callBack, string title, string msg, string okBtnText = "K0327", string cancelBtnText = null, string tips = null, ImageSource image = null, Action<bool?> closeAction = null);

	bool? ShowMessageWithPrivacy(string title, string msg, string okBtnText = "K0327", string cancelBtnText = null, bool isCloseBtn = false, MessageBoxImage icon = MessageBoxImage.Asterisk, Action<bool?> closeAction = null);

	bool? ShowRescueBackupWnd(string title, string msg, string imagePath, string okBtnText = "K0327", string cancelBtnText = null, bool isCloseBtn = false, bool isNotifyText = false);

	bool? ContentMssage(FrameworkElement content, string title, string okBtn = "K0327", string cancelBtn = null, bool isCloseBtn = false, MessageBoxImage icon = MessageBoxImage.Asterisk);

	void CloseLowLeveWnd();

	void ShowQuitSurvey();

	void ShowMutilIcon(bool showIcon, bool showList);

	void ChangeIsEnabled(bool isEnabled);

	void ShowB2BExpired(int _modeType);

	void ShowB2BRemind(int used, int total);

	void ShowMutilTutorials(bool show);

	void ChangePinStoryboard(bool start);

	void SetDriverButtonStatus(string _code);

	void SetDeviceConnectIconStatus(int _status);

	void LogOut();
}
