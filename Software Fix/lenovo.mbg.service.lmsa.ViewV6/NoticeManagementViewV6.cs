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
using lenovo.mbg.service.lmsa.Login.Model;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class NoticeManagementViewV6 : UserControl, IComponentConnector, IStyleConnector
{
	private Timer mTimer;

	private volatile bool _IsLoading = false;

	private int _TickCount = 0;

	public ObservableCollection<NoticeInfo> NoticeLists { get; set; }

	public NoticeManagementViewV6()
	{
		InitializeComponent();
		NoticeLists = new ObservableCollection<NoticeInfo>();
		mTimer = new Timer(delegate
		{
			bool isExecuteWork = MainWindowControl.Instance.IsExecuteWork();
			HostProxy.CurrentDispatcher?.Invoke(() => btnNotification.IsEnabled = !isExecuteWork);
			_TickCount++;
			OnlineUserInfo currentLoggedInUser = UserService.Single.CurrentLoggedInUser;
			if (currentLoggedInUser != null && currentLoggedInUser.IsRtNotify && _TickCount >= 1800)
			{
				_TickCount = 0;
				LoadNotice();
			}
		}, null, 1000, 1000);
		base.Loaded += delegate
		{
			UserService.Single.OnlineUserChanged += Single_OnlineUserChanged;
		};
		base.Unloaded += delegate
		{
			UserService.Single.OnlineUserChanged -= Single_OnlineUserChanged;
		};
	}

	public void ChangeIsEnabled(bool isEnabled)
	{
		grid.IsEnabled = isEnabled;
		grid.Opacity = (isEnabled ? 1.0 : 0.3);
	}

	private void Single_OnlineUserChanged(object sender, OnlineUserChangedEventArgs e)
	{
		LoadNotice();
	}

	private void LoadNotice()
	{
		if (_IsLoading)
		{
			return;
		}
		_IsLoading = true;
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
				if (latestNotice?.type != "Feedback")
				{
					ShowNotice(latestNotice);
				}
				_IsLoading = false;
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
			ApplcationClass.ApplcationStartWindow.ShowMessage("K1296");
		}
		else if (!popupNotification.IsOpen)
		{
			popupNotification.IsOpen = true;
		}
	}

	private void OnBtnDelte(object sender, RoutedEventArgs e)
	{
		if (ApplcationClass.ApplcationStartWindow.ShowMessage("K0712", MessageBoxButton.OKCancel) == true)
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
			NoticeInfo noticeInfo = NoticeLists.First((NoticeInfo n) => n.id == notice.id && n.noticeType == notice.noticeType);
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
