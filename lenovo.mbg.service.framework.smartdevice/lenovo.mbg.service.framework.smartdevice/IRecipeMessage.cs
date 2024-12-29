using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.smartdevice.Steps;
using lenovo.themes.generic;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice;

public interface IRecipeMessage
{
	Task<bool?> Show(string title, string message, string ok = "K0327", string cancel = null, bool showClose = false, MessageBoxImage icon = MessageBoxImage.Asterisk, string notifyText = null, bool isPrivacy = false);

	Task<bool?> ShowCustom(IMessageViewV6 view);

	Task<bool?> WaitByAdb(string title, string message, string image);

	Task<bool?> WaitByFastboot(UseCase _usecase, string modelname);

	Task<bool?> Linker(string title, string message, object data);

	Task<bool?> RightPic(string title, string message, string image, string ok = "K0327", string cancel = null, string tips = null, bool showClose = false, bool popupWhenClose = false);

	bool? AutoClose(string title, string message, string image, List<string> buttons = null, int milliseconds = -1, string link = null, bool showClose = false, bool popupWhenClose = false, bool format = true, bool? autoCloseResult = true);

	bool? AutoCloseMoreStep(string title, ConnectStepInfo stepArr, int milliseconds = -1, bool popupWhenClose = false);

	bool? AutoCloseConnectTutorials(string title, JArray steps, int milliseconds = -1, bool autoPlay = false, double interval = 5000.0, bool showPlayControl = true, bool showClose = true, bool popupWhenClose = true);

	Task<bool?> TabletTurnoff(string title, string message, string image, string note);

	Task<bool?> BackConfirm(string title, string message, string ok = "K0327", string cancel = null, bool showClose = false, bool isNotifyText = false);

	Task<bool?> EraseData();

	void ShowDownloadCenter(bool show);

	void Close(bool? result, Action<bool?> closeCallback = null);

	void SetMainWindowDriverBtnStatus(string _code);
}
