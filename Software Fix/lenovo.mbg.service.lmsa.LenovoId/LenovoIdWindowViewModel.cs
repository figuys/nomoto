using System;
using System.Text.RegularExpressions;
using System.Windows;
using GoogleAnalytics;
using lenovo.mbg.service.lmsa.Feedback.View;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.Login.Protocol;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.LenovoId;

public class LenovoIdWindowViewModel : ViewModelBase
{
	private Visibility m_doNetBrowserVisibility = Visibility.Collapsed;

	private Visibility m_winformBrowserVisibility = Visibility.Collapsed;

	private Visibility m_refreshVisibility = Visibility.Visible;

	public ReplayCommand CloseCommand { get; private set; }

	public ReplayCommand RefreshCommand { get; private set; }

	public ReplayCommand FeedBackCommand { get; }

	public string Token { get; private set; }

	public Visibility DoNetBrowserVisibility
	{
		get
		{
			return m_doNetBrowserVisibility;
		}
		set
		{
			if (m_doNetBrowserVisibility != value)
			{
				m_doNetBrowserVisibility = value;
				OnPropertyChanged("DoNetBrowserVisibility");
			}
		}
	}

	public Visibility WinformBrowserVisibility
	{
		get
		{
			return m_winformBrowserVisibility;
		}
		set
		{
			if (m_winformBrowserVisibility != value)
			{
				m_winformBrowserVisibility = value;
				OnPropertyChanged("WinformBrowserVisibility");
			}
		}
	}

	public Visibility RefreshVisibility
	{
		get
		{
			return m_refreshVisibility;
		}
		set
		{
			m_refreshVisibility = value;
			OnPropertyChanged("RefreshVisibility");
		}
	}

	public LenovoIdWindowViewModel()
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		FeedBackCommand = new ReplayCommand(FeedBackCommandHandler);
	}

	public void Init(bool isUseWebBrowser)
	{
		if (isUseWebBrowser)
		{
			WinformBrowserVisibility = Visibility.Visible;
			DoNetBrowserVisibility = Visibility.Collapsed;
		}
		else
		{
			DoNetBrowserVisibility = Visibility.Visible;
			WinformBrowserVisibility = Visibility.Collapsed;
		}
	}

	public void TokenAnalysis(string url, Action action)
	{
		Match match = Regex.Match(url, "lenovoid.wust=(?<token>[^&]+)");
		Token = match.Groups["token"]?.Value;
		UserLoginFormData userLoginFormData = new UserLoginFormData();
		userLoginFormData.UserSource = "lenovoId";
		LenovoIdUserLoginFormData lenovoIdUserLoginFormData = new LenovoIdUserLoginFormData();
		lenovoIdUserLoginFormData.UserName = null;
		lenovoIdUserLoginFormData.WUST = Token;
		userLoginFormData.UserData = JsonConvert.SerializeObject(lenovoIdUserLoginFormData);
		UserService.Single.Login(userLoginFormData);
		action?.Invoke();
	}

	private void CloseCommandHandler(object parameter)
	{
		(parameter as LenovoIdWindow)?.Close();
	}

	private void RefreshCommandHandler(object parameter)
	{
		(parameter as LenovoIdWindow)?.Reload();
	}

	private void FeedBackCommandHandler(object data)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuFeedbackButtonClick", "menu-feedback button click", 0L).Build());
		Window window = new FeedbackMainView(isMainWindowLoad: false);
		window.ShowDialog();
	}
}
