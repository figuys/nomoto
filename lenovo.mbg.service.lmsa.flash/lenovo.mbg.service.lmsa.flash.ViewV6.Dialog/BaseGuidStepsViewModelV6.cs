using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using GoogleAnalytics;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class BaseGuidStepsViewModelV6 : ViewModelBase
{
	private readonly GuidStepsViewV6 _View;

	private static readonly string GifBasePath = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/Conn/{0}";

	protected Timer AutoPlayTimer;

	private ConnectStepsModel currentStep;

	private bool isAutoPlay;

	private bool showPlayControl;

	private string guideTitle;

	private Visibility checkingTxtVisibility = Visibility.Collapsed;

	private Visibility replayBtnVisibility = Visibility.Hidden;

	private Visibility xt2Visibility = Visibility.Collapsed;

	private bool preBtnEnabled;

	private bool nextBtnEnabled = true;

	public ReplayCommand CloseWinCommand { get; }

	public ReplayCommand PreviousCommand { get; }

	public ReplayCommand NextCommand { get; }

	public ReplayCommand PlayAgainCommand { get; }

	public ReplayCommand PopCancelClick { get; }

	public ReplayCommand PopOkClick { get; }

	public ReplayCommand OkCommand { get; }

	public bool ClosePopup { get; set; }

	public ConnectStepsModel Steps { get; set; }

	private static IGoogleAnalyticsTracker GoogleAnalytics => HostProxy.GoogleAnalyticsTracker;

	private double TimerInterval { get; }

	protected List<ConnectStepsModel> StepModelList { get; set; }

	protected int MinIndex => StepModelList.Min((ConnectStepsModel n) => n.Index);

	protected int MaxIndex => StepModelList.Max((ConnectStepsModel n) => n.Index);

	public ConnectStepsModel CurrentStep
	{
		get
		{
			return currentStep;
		}
		set
		{
			currentStep = value;
			OnPropertyChanged("CurrentStep");
		}
	}

	public bool IsAutoPlay
	{
		get
		{
			return isAutoPlay;
		}
		set
		{
			isAutoPlay = value;
			OnPropertyChanged("IsAutoPlay");
		}
	}

	public bool ShowPlayControl
	{
		get
		{
			return showPlayControl;
		}
		set
		{
			showPlayControl = value;
			OnPropertyChanged("ShowPlayControl");
		}
	}

	public bool ShowCloseBtn { get; set; }

	public string GuideTitle
	{
		get
		{
			return guideTitle;
		}
		set
		{
			guideTitle = value;
			OnPropertyChanged("GuideTitle");
		}
	}

	public Visibility CheckingTxtVisibility
	{
		get
		{
			return checkingTxtVisibility;
		}
		set
		{
			checkingTxtVisibility = value;
			OnPropertyChanged("CheckingTxtVisibility");
		}
	}

	public Visibility ReplayBtnVisibility
	{
		get
		{
			return replayBtnVisibility;
		}
		set
		{
			replayBtnVisibility = value;
			OnPropertyChanged("ReplayBtnVisibility");
		}
	}

	public Visibility XT2Visibility
	{
		get
		{
			return xt2Visibility;
		}
		set
		{
			xt2Visibility = value;
			OnPropertyChanged("XT2Visibility");
		}
	}

	public bool PreBtnEnabled
	{
		get
		{
			return preBtnEnabled;
		}
		set
		{
			preBtnEnabled = value;
			OnPropertyChanged("PreBtnEnabled");
		}
	}

	public bool NextBtnEnabled
	{
		get
		{
			return nextBtnEnabled;
		}
		set
		{
			nextBtnEnabled = value;
			OnPropertyChanged("NextBtnEnabled");
		}
	}

	protected static string ConvertImageUrl(string image)
	{
		return string.Format(GifBasePath, image, UriKind.Absolute);
	}

	public BaseGuidStepsViewModelV6(GuidStepsViewV6 ui, bool autoPlay = false, double interval = 5000.0, bool showPlayControl = true, bool showClose = true, bool popupWhenClose = false)
	{
		_View = ui;
		IsAutoPlay = autoPlay;
		if (interval == 0.0)
		{
			interval = 5000.0;
		}
		TimerInterval = interval;
		ShowPlayControl = showPlayControl;
		ShowCloseBtn = showClose;
		ClosePopup = popupWhenClose;
		StepModelList = new List<ConnectStepsModel>();
		CloseWinCommand = new ReplayCommand(CloseWinCommandHandler);
		PreviousCommand = new ReplayCommand(delegate
		{
			FirePrevCommand();
		});
		NextCommand = new ReplayCommand(delegate
		{
			FireNextCommand();
		});
		PlayAgainCommand = new ReplayCommand(delegate
		{
			FirePlayAgainCommand();
		});
		PopCancelClick = new ReplayCommand(PopCancelClickHandler);
		PopOkClick = new ReplayCommand(PopOkClickHandler);
		OkCommand = new ReplayCommand(OkCommandHandler);
	}

	public virtual BaseGuidStepsViewModelV6 InitSteps(string title, JArray steps)
	{
		List<ConnectStepsModel> list = new List<ConnectStepsModel>();
		for (int i = 0; i < steps.Count; i++)
		{
			list.Add(new ConnectStepsModel
			{
				Index = i,
				Layout = "V",
				Title = (title ?? steps[i].Value<string>("Title")),
				Image = ConvertImageUrl(steps[i].Value<string>("Image")),
				Content = steps[i].Value<string>("Content")
			});
		}
		StepModelList = list;
		return this;
	}

	public BaseGuidStepsViewModelV6 Ready()
	{
		CurrentStep = StepModelList.First();
		ChangeTitle();
		InitAutoPlay();
		return this;
	}

	protected virtual void FirePrevCommand()
	{
		int idx = CurrentStep.Index;
		CurrentStep = StepModelList.First((ConnectStepsModel n) => n.Index == idx - 1);
		ChangeTitle();
		PreBtnEnabled = CurrentStep.Index > MinIndex;
		NextBtnEnabled = CurrentStep.Index < MaxIndex;
		GoogleAnalytics.Send(HitBuilder.CreateCustomEvent("lmsa-plugin-flash", "ConnectDevGifPrev", "Connect device tutorial gif previous button clicked!", 0L).Build());
	}

	protected virtual void FireNextCommand()
	{
		int idx = CurrentStep.Index;
		CurrentStep = StepModelList.First((ConnectStepsModel n) => n.Index == idx + 1);
		ChangeTitle();
		PreBtnEnabled = CurrentStep.Index > MinIndex;
		NextBtnEnabled = CurrentStep.Index < MaxIndex;
		if (IsAutoPlay && !NextBtnEnabled)
		{
			AutoPlayStop();
		}
		GoogleAnalytics.Send(HitBuilder.CreateCustomEvent("lmsa-plugin-flash", "ConnectDevGifNext", "Connect device tutorial gif next button clicked!", 0L).Build());
	}

	protected virtual void FirePlayAgainCommand()
	{
		CurrentStep = StepModelList.First();
		ChangeTitle();
		AutoPlayStart();
	}

	protected virtual void ChangeTitle()
	{
		GuideTitle = CurrentStep.Title;
	}

	protected virtual void InitAutoPlay()
	{
		if (IsAutoPlay)
		{
			AutoPlayTimer = new Timer
			{
				Interval = TimerInterval
			};
			AutoPlayTimer.Elapsed += delegate
			{
				FireNextCommand();
			};
			AutoPlayStart();
		}
	}

	protected virtual void AutoPlayStart()
	{
		AutoPlayTimer.Start();
		ReplayBtnVisibility = Visibility.Hidden;
	}

	private void AutoPlayStop()
	{
		AutoPlayTimer.Stop();
		ReplayBtnVisibility = Visibility.Visible;
	}

	private void PopCancelClickHandler(object args)
	{
		_View.pop.IsOpen = false;
		_View.Close();
	}

	private void PopOkClickHandler(object args)
	{
		_View.pop.IsOpen = false;
	}

	private void CloseWinCommandHandler(object args)
	{
		if (ClosePopup)
		{
			_View.pop.IsOpen = true;
		}
		else
		{
			_View.Close();
		}
	}

	private void OkCommandHandler(object args)
	{
		if (CurrentStep.Index == MaxIndex)
		{
			_View.Close();
			return;
		}
		int idx = CurrentStep.Index;
		CurrentStep = StepModelList.First((ConnectStepsModel n) => n.Index == idx + 1);
		ChangeTitle();
	}

	public override void Dispose()
	{
		if (AutoPlayTimer != null)
		{
			AutoPlayTimer.Stop();
			AutoPlayTimer.Dispose();
			AutoPlayTimer = null;
		}
	}
}
