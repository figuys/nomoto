using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.UserControls;

public partial class NoticeManagementView : UserControl, IComponentConnector, IStyleConnector
{
	private Timer mTimer;

	public ObservableCollection<NoticeInfo> NoticeLists { get; set; }

	public NoticeManagementView()
	{
		InitializeComponent();
		NoticeLists = new ObservableCollection<NoticeInfo>();
		LoadNotice();
		mTimer = new Timer(delegate
		{
			if (MainWindowControl.Instance.IsExecuteWork())
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					btnNotification.IsEnabled = false;
				});
			}
			else
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					btnNotification.IsEnabled = true;
				});
			}
		}, null, 1000, 1000);
		UserService.Single.OnlineUserChanged += Single_OnlineUserChanged;
	}

	private void Single_OnlineUserChanged(object sender, OnlineUserChangedEventArgs e)
	{
		LoadNotice();
	}

	private void LoadNotice()
	{
		Task.Factory.StartNew((Func<Task>)async delegate
		{
			List<NoticeInfo> notices = await Notices.Single.SyncAsync();
			NoticeInfo latestNotice = Notices.Single.LatestNotice;
			if (notices != null)
			{
				notices = notices.OrderByDescending((NoticeInfo m) => m.modifyDate).ToList();
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				redDot10.Visibility = ((latestNotice == null) ? Visibility.Collapsed : Visibility.Visible);
				NoticeLists.Clear();
				if (notices != null)
				{
					foreach (NoticeInfo item in notices)
					{
						NoticeLists.Add(item);
					}
				}
				NotifyListBox.ItemsSource = NoticeLists;
				ShowNotice(latestNotice);
			});
		});
	}

	private void btnDeleteClick(object sender, RoutedEventArgs e)
	{
		NoticeInfo notice = (sender as Button).CommandParameter as NoticeInfo;
		RemoveNotice(notice);
	}

	private void OnNotifySelected(object sender, SelectionChangedEventArgs e)
	{
		if (NotifyListBox.SelectedIndex != -1)
		{
			popupNotification.IsOpen = false;
			NoticeInfo notice = NotifyListBox.SelectedValue as NoticeInfo;
			ShowNotice(notice);
			NotifyListBox.SelectedIndex = -1;
		}
	}

	private void OnBtnNotification(object sender, RoutedEventArgs e)
	{
		if (NoticeLists.Count == 0)
		{
			LenovoPopupWindow win = new OkWindowModel().CreateCustomizeWindow(HostProxy.LanguageService.Translate("K0071"), HostProxy.LanguageService.Translate("K1296"), "K0327");
			HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				win.ShowDialog();
			});
		}
		else if (!popupNotification.IsOpen)
		{
			popupNotification.IsOpen = true;
		}
	}

	private void OnBtnDelte(object sender, RoutedEventArgs e)
	{
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0711", "K0712", "K0208", "K0583", null);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		if (win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
		{
			NoticeInfo notice = (e.OriginalSource as Button).Tag as NoticeInfo;
			RemoveNotice(notice);
		}
	}

	private void ShowNotice(NoticeInfo notice)
	{
		if (notice != null)
		{
			Notices.Single.Show(notice);
			NoticeInfo noticeInfo = NoticeLists.Where((NoticeInfo n) => n.id == notice.id).First();
			noticeInfo.isChecked = true;
			UpdateNotifyRedDot();
		}
	}

	private void RemoveNotice(NoticeInfo notice)
	{
		if (notice != null)
		{
			Notices.Single.RemoveLocalNotice(notice);
			NoticeLists.Remove(notice);
			UpdateNotifyRedDot();
		}
	}

	private void UpdateNotifyRedDot()
	{
		if (NoticeLists.FirstOrDefault((NoticeInfo p) => !p.isChecked) == null)
		{
			redDot10.Visibility = Visibility.Collapsed;
		}
		else
		{
			redDot10.Visibility = Visibility.Visible;
		}
	}
}
