using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class MusicPlayerViewModel : ViewModelBase
{
	private bool isMediaEnd;

	private static MusicPlayerViewModel _SingleInstance;

	private static object locker = new object();

	protected MusicPlayerControlV7 _View;

	private bool _ShowMusicPlayer;

	private string _MusicCacheDir = Configurations.MusicCacheDir;

	private Timer _Timer;

	public static MusicPlayerViewModel SingleInstance
	{
		get
		{
			if (_SingleInstance == null)
			{
				lock (locker)
				{
					if (_SingleInstance == null)
					{
						_SingleInstance = new MusicPlayerViewModel();
					}
				}
			}
			return _SingleInstance;
		}
	}

	private ObservableCollection<MusicInfoViewModel> _SongList => MusicViewModel.SingleInstance.SongList;

	public int CurrentPlayId { get; set; }

	public bool ShowMusicPlayer
	{
		get
		{
			return _ShowMusicPlayer;
		}
		set
		{
			if (_ShowMusicPlayer != value)
			{
				_ShowMusicPlayer = value;
				OnPropertyChanged("ShowMusicPlayer");
			}
		}
	}

	public ReplayCommand SoundMediaOpenedCommand { get; private set; }

	public ReplayCommand PlayPreCommand { get; private set; }

	public ReplayCommand PlayCommand { get; private set; }

	public ReplayCommand PlayPauseCommand { get; private set; }

	public ReplayCommand PlayNextCommand { get; private set; }

	public ReplayCommand SliderPositionPreviewMouseUpCommand { get; private set; }

	private MusicPlayerViewModel()
	{
		SoundMediaOpenedCommand = new ReplayCommand(SoundMediaOpenedCommandHandler);
		PlayPreCommand = new ReplayCommand(PlayPreCommandHandler);
		PlayCommand = new ReplayCommand(PlayCommandHandler);
		PlayPauseCommand = new ReplayCommand(PlayPauseCommandHandler);
		PlayNextCommand = new ReplayCommand(PlayNextCommandHandler);
		SliderPositionPreviewMouseUpCommand = new ReplayCommand(SliderPositionPreviewMouseUpCommandHandler);
		CurrentPlayId = 0;
		_Timer = new Timer(1000.0);
		_Timer.Elapsed += PlayTimerHandler;
		_Timer.Enabled = false;
		isMediaEnd = true;
	}

	public void InitializeView(UserControl view)
	{
		_View = view as MusicPlayerControlV7;
		_View.Sound.MediaEnded += delegate
		{
			isMediaEnd = true;
			_Timer.Enabled = false;
			_View.Dispatcher.Invoke(delegate
			{
				_View.BtnPause.Visibility = Visibility.Hidden;
				_View.BtnPlay.Visibility = Visibility.Visible;
			});
		};
	}

	public override void Dispose()
	{
		base.Dispose();
		_ShowMusicPlayer = false;
		_SingleInstance = null;
	}

	public void SongDBClick(MusicInfoViewModel music)
	{
		MusicInfoViewModel musicInfoViewModel = FoundMusic(0);
		if (musicInfoViewModel != null)
		{
			if (!_Timer.Enabled)
			{
				_Timer.Enabled = true;
			}
			BeforePlayHandle(musicInfoViewModel);
			HostProxy.BehaviorService.Collect(BusinessType.SONG_PLAY, null);
		}
	}

	public void Stop()
	{
		_View?.Sound?.Stop();
	}

	public void ResetSongText()
	{
		CurrentPlayId = 0;
		_View.totaltime.Text = "/ 00:00";
		_View.playingUri.Text = string.Empty;
		_View.Sound.Position = new TimeSpan(0, 0, 0);
		_View.BtnPause.Visibility = Visibility.Hidden;
		_View.BtnPlay.Visibility = Visibility.Visible;
	}

	private void SoundMediaOpenedCommandHandler(object aParams)
	{
		try
		{
			_View.slider_play.Minimum = 0.0;
			TimeSpan timeSpan = _View.Sound.NaturalDuration.TimeSpan;
			_View.slider_play.Maximum = timeSpan.TotalSeconds;
			_View.totaltime.Text = "/  " + timeSpan.ToString("mm\\:ss");
		}
		catch
		{
			MessageBox.Show("Unsupported file type.");
		}
		_View.BtnPause.Visibility = Visibility.Visible;
		_View.BtnPlay.Visibility = Visibility.Hidden;
	}

	private void PlayPreCommandHandler(object aParams)
	{
		if (CurrentPlayId != 0)
		{
			MusicInfoViewModel musicInfoViewModel = FoundMusic(-1);
			MusicViewModel.SingleInstance.SelectedItem = musicInfoViewModel;
			BeforePlayHandle(musicInfoViewModel);
		}
	}

	private void PlayCommandHandler(object aParams)
	{
		if (CurrentPlayId != 0 || MusicViewModel.SingleInstance.SelectedItem != null)
		{
			_Timer.Enabled = true;
			if (MusicViewModel.SingleInstance.SelectedItem != null && (MusicViewModel.SingleInstance.SelectedItem.ID != CurrentPlayId || isMediaEnd))
			{
				isMediaEnd = true;
				CurrentPlayId = MusicViewModel.SingleInstance.SelectedItem.ID;
				MusicInfoViewModel music = FoundMusic(0);
				HostProxy.BehaviorService.Collect(BusinessType.SONG_PLAY, null);
				BeforePlayHandle(music);
			}
			else
			{
				Play();
			}
		}
	}

	private void PlayPauseCommandHandler(object aParams)
	{
		_View.Sound.Pause();
		_Timer.Enabled = false;
		_View.BtnPause.Visibility = Visibility.Hidden;
		_View.BtnPlay.Visibility = Visibility.Visible;
	}

	private void PlayNextCommandHandler(object aParams)
	{
		if (CurrentPlayId != 0)
		{
			MusicInfoViewModel musicInfoViewModel = FoundMusic(1);
			MusicViewModel.SingleInstance.SelectedItem = musicInfoViewModel;
			BeforePlayHandle(musicInfoViewModel);
		}
	}

	private void SliderPositionPreviewMouseUpCommandHandler(object aParams)
	{
		_View.Sound.Position = TimeSpan.FromSeconds(_View.slider_play.Value);
	}

	private MusicInfoViewModel FoundMusic(int moveType)
	{
		if (_SongList == null || _SongList.Count == 0)
		{
			return null;
		}
		ICollectionView defaultView = CollectionViewSource.GetDefaultView(_SongList);
		MusicInfoViewModel musicInfoViewModel = null;
		if (defaultView.CurrentItem == null || moveType == 0)
		{
			if (MusicViewModel.SingleInstance.SelectedItem != null)
			{
				if (defaultView.MoveCurrentTo(MusicViewModel.SingleInstance.SelectedItem))
				{
					musicInfoViewModel = defaultView.CurrentItem as MusicInfoViewModel;
				}
			}
			else
			{
				defaultView.MoveCurrentToFirst();
				musicInfoViewModel = defaultView.CurrentItem as MusicInfoViewModel;
			}
		}
		else if (moveType < 0)
		{
			defaultView.MoveCurrentToPrevious();
			musicInfoViewModel = defaultView.CurrentItem as MusicInfoViewModel;
		}
		else
		{
			defaultView.MoveCurrentToNext();
			musicInfoViewModel = defaultView.CurrentItem as MusicInfoViewModel;
		}
		return MusicViewModel.SingleInstance.SelectedItem = musicInfoViewModel;
	}

	private void BeforePlayHandle(MusicInfoViewModel music)
	{
		if (music != null)
		{
			string text = Path.Combine(_MusicCacheDir, music.SongName);
			if (File.Exists(text))
			{
				SetMusicSoundAndPlay(music, text);
			}
			else
			{
				CacheAndPlayMusic(music);
			}
		}
	}

	private void SetMusicSoundAndPlay(MusicInfoViewModel musice, string uri)
	{
		isMediaEnd = false;
		CurrentPlayId = musice.ID;
		_View.playingUri.Text = musice.SongName;
		_View.Sound.Source = new Uri(uri);
		Play();
	}

	private void ReSetMusicSoundInfo()
	{
		CurrentPlayId = 0;
		_View.playingUri.Text = "";
		_View.Sound.Source = null;
		Stop();
	}

	private void Play()
	{
		_View.Sound.Play();
		_View.BtnPause.Visibility = Visibility.Visible;
		_View.BtnPlay.Visibility = Visibility.Hidden;
	}

	private void CacheAndPlayMusic(MusicInfoViewModel m)
	{
		AsyncDataLoader.BeginLoading(delegate(object data)
		{
			string text = m.RawMusicInfo?.RawMusicInfo?.DisplayName;
			if (string.IsNullOrEmpty(text))
			{
				text = string.Empty;
			}
			string text2 = Path.GetExtension(text).ToLower();
			if (!text2.Equals(".mp3") && !text2.Equals(".wav"))
			{
				((dynamic)data).FilePath = null;
				return new Tuple<bool, string, string>(item1: true, "K0071", "K0557");
			}
			NavigationManagementBLL navigationManagementBLL = new NavigationManagementBLL();
			string text3 = null;
			try
			{
				text3 = navigationManagementBLL.PrepareToPlayMusic(m, _MusicCacheDir);
			}
			catch (Exception)
			{
			}
			if (string.IsNullOrEmpty(text3))
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					LenovoPopupWindow win = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0071", ResourcesHelper.StringResources.SingleInstance.MUSIC_PLAY_WARN, "K0327", null);
					HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
					{
						win.ShowDialog();
					});
				});
			}
			((dynamic)data).FilePath = text3;
			return (Tuple<bool, string, string>)null;
		}, delegate(object data)
		{
			try
			{
				if (!(string.IsNullOrEmpty(((dynamic)data).FilePath) ? true : false))
				{
					string uri = ((dynamic)data).FilePath;
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						SetMusicSoundAndPlay(m, uri);
					});
				}
			}
			catch
			{
			}
		}, ViewContext.SingleInstance.MainViewModel);
	}

	private void PlayTimerHandler(object sender, ElapsedEventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (_View.Sound.NaturalDuration.HasTimeSpan)
			{
				TimeSpan position = _View.Sound.Position;
				if (Mouse.LeftButton != MouseButtonState.Pressed)
				{
					_View.slider_play.Value = position.TotalSeconds;
				}
				_View.hasPlaytime.Text = position.ToString("mm\\:ss");
			}
		});
	}
}
