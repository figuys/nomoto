using System;
using System.Windows;

namespace lenovo.mbg.service.framework.services;

public interface IHostOperationService
{
	IntPtr ShowMaskLayer(string uid, WindowState state = WindowState.Normal);

	void CloseMaskLayer(string uid);

	void CloseAll();

	string GetAppVersion();

	void ShowGuideTips();

	void ShowFeedBack();

	void ShowBannerAsync(object data);
}
