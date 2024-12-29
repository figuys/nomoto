using System;
using System.Collections.ObjectModel;
using System.Windows;
using GoogleAnalytics;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.UserControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox;

public class ToolBoxListViewModelV6 : ViewModelBase
{
	private FrameworkElement _CurrentView;

	private Visibility _mainViewVisbility;

	private Visibility _toolViewVisbility = Visibility.Collapsed;

	public ObservableCollection<ToolBoxItemViewModel> ToolboxList { get; set; }

	public ReplayCommand ClickCommand { get; private set; }

	public ReplayCommand BackCommand { get; private set; }

	public FrameworkElement CurrentView
	{
		get
		{
			return _CurrentView;
		}
		set
		{
			_CurrentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public Visibility MainViewVisbility
	{
		get
		{
			return _mainViewVisbility;
		}
		set
		{
			_mainViewVisbility = value;
			OnPropertyChanged("MainViewVisbility");
		}
	}

	public Visibility ToolViewVisbility
	{
		get
		{
			return _toolViewVisbility;
		}
		set
		{
			_toolViewVisbility = value;
			OnPropertyChanged("ToolViewVisbility");
		}
	}

	public ToolBoxListViewModelV6()
	{
		ClickCommand = new ReplayCommand(ClickCommandCommandHandler);
		BackCommand = new ReplayCommand(BackCommandCommandHandler);
		ToolboxList = new ObservableCollection<ToolBoxItemViewModel>();
		ToolboxList.Add(new ToolBoxItemViewModel
		{
			ToolboxIcon = "pack://application:,,,/lenovo.mbg.service.lmsa.toolBox;component/ResourcesV6/icon_clipboard.png",
			ToolboxTitle = "Clipboard",
			ToolboxDescrption = "Copy text between device and PC",
			ToolBoxType = ToolBoxType.Clipboard,
			ItemClickCommand = ClickCommand
		});
		ToolboxList.Add(new ToolBoxItemViewModel
		{
			ToolboxIcon = "pack://application:,,,/lenovo.mbg.service.lmsa.toolBox;component/ResourcesV6/icon_ringtone.png",
			ToolboxTitle = "Ringtone Maker",
			ToolboxDescrption = "Make ringtone from your music",
			ToolBoxType = ToolBoxType.RingtoneMaker,
			ItemClickCommand = ClickCommand
		});
		ToolboxList.Add(new ToolBoxItemViewModel
		{
			ToolboxIcon = "pack://application:,,,/lenovo.mbg.service.lmsa.toolBox;component/ResourcesV6/icon_gif_maker.png",
			ToolboxTitle = "GIF Maker",
			ToolboxDescrption = "Make animated GIFs from pictures on PC",
			ToolBoxType = ToolBoxType.GIFMaker,
			ItemClickCommand = ClickCommand
		});
		ToolboxList.Add(new ToolBoxItemViewModel
		{
			ToolboxIcon = "pack://application:,,,/lenovo.mbg.service.lmsa.toolBox;component/ResourcesV6/icon_screen_recorder.png",
			ToolboxTitle = "Screen Recorder",
			ToolboxDescrption = "Record device screen",
			ToolBoxType = ToolBoxType.ScreenRecorder,
			ItemClickCommand = ClickCommand
		});
	}

	private void ClickCommandCommandHandler(object prameter)
	{
		MainViewVisbility = Visibility.Collapsed;
		ToolViewVisbility = Visibility.Visible;
		switch ((ToolBoxType)prameter)
		{
		case ToolBoxType.Clipboard:
			OnClipBoard();
			break;
		case ToolBoxType.GIFMaker:
			OnGifMaker();
			break;
		case ToolBoxType.RingtoneMaker:
			OnRingtoneMaker();
			break;
		case ToolBoxType.ScreenRecorder:
			OnScreenCapture();
			break;
		}
	}

	private void BackCommandCommandHandler(object prameter)
	{
		MainViewVisbility = Visibility.Visible;
		ToolViewVisbility = Visibility.Collapsed;
		((IDisposable)CurrentView).Dispose();
		CurrentView = null;
	}

	private void OnClipBoard()
	{
		HostProxy.BehaviorService.Collect(BusinessType.CLIP_BOARD, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "clipboardClick", "Click clipboard tool button", 0L).Build());
		Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-clipboard").Build());
		CurrentView = new ClipboardUserControlV6();
	}

	private void OnGifMaker()
	{
		HostProxy.BehaviorService.Collect(BusinessType.GIF_MAKER, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "gifMakerClick", "Click GIF Maker tool button", 0L).Build());
		Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-gifmaker").Build());
		CurrentView = new GifMakerViewV6();
	}

	private void OnRingtoneMaker()
	{
		HostProxy.BehaviorService.Collect(BusinessType.RINGTONE_MAKER, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "ringtoneMakerClick", "Click Ringtone Maker tool button", 0L).Build());
		Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-ringtonemaker").Build());
		CurrentView = new RingtoneMakerViewV6();
	}

	private void OnScreenCapture()
	{
		HostProxy.BehaviorService.Collect(BusinessType.SCREEN_RECORDER, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "screenCaptureClick", "Click Screen Capture tool button", 0L).Build());
		Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-screencapture").Build());
		CurrentView = new ScreenCaptureFrameV6();
	}
}
