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
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class MusicPlayerViewModelV7 : ViewModelBase
{
	private bool isMediaEnd;

	private MusicInfoViewModelV7 m_CurrentPlayMusic;

	private static MusicPlayerViewModelV7 _SingleInstance;

	private static object locker = new object();

	protected MusicPlayerControlV7 _View;

	private bool _ShowMusicPlayer;

	private string _MusicCacheDir = Configurations.MusicCacheDir;

	private Timer _Timer;

	private MusicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<MusicMgtViewModelV7>(typeof(MusicMgtViewV7));

	public static MusicPlayerViewModelV7 SingleInstance
	{
		get
		{
			if (_SingleInstance == null)
			{
				lock (locker)
				{
					if (_SingleInstance == null)
					{
						_SingleInstance = new MusicPlayerViewModelV7();
					}
				}
			}
			return _SingleInstance;
		}
	}

	private ObservableCollection<MusicInfoViewModelV7> _SongList
	{
		get
		{
			ObservableCollection<MusicInfoViewModelV7> observableCollection = new ObservableCollection<MusicInfoViewModelV7>();
			foreach (MusicInfo cachedAllMusic in GetCurrentViewModel.FocusedAlbum.CachedAllMusics)
			{
				observableCollection.Add(new MusicInfoViewModelV7(cachedAllMusic));
			}
			return observableCollection;
		}
	}

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

	private MusicPlayerViewModelV7()
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
				_View.hasPlaytime.Text = _View.Sound.NaturalDuration.TimeSpan.ToString("mm\\:ss");
				_View.BtnPause.Visibility = Visibility.Hidden;
				_View.BtnPlay.Visibility = Visibility.Visible;
			});
		};
	}

	public override void Dispose()
	{
		base.Dispose();
		_ShowMusicPlayer = false;
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			_View.Sound.Stop();
			ResetSongText();
		});
	}

	public void SongDBClick(MusicInfoViewModelV7 _music)
	{
		if (_music != null)
		{
			if (!_Timer.Enabled)
			{
				_Timer.Enabled = true;
			}
			m_CurrentPlayMusic = _music;
			BeforePlayHandle(_music);
		}
	}

	public void Stop()
	{
		_View?.Sound?.Stop();
	}

	public void ResetSongText()
	{
		CurrentPlayId = 0;
		m_CurrentPlayMusic = null;
		_View.hasPlaytime.Text = "00:00";
		_View.totaltime.Text = "00:00";
		_View.slider_play.Value = 0.0;
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
			_View.totaltime.Text = timeSpan.ToString("mm\\:ss");
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
			MusicInfoViewModelV7 musicInfoViewModelV = FoundMusic(-1);
			GetCurrentViewModel.SelectedItem = musicInfoViewModelV;
			BeforePlayHandle(musicInfoViewModelV);
		}
	}

	private void PlayCommandHandler(object aParams)
	{
		if (CurrentPlayId != 0 || m_CurrentPlayMusic != null)
		{
			_Timer.Enabled = true;
			if (m_CurrentPlayMusic != null && (m_CurrentPlayMusic.ID != CurrentPlayId || isMediaEnd))
			{
				isMediaEnd = true;
				CurrentPlayId = m_CurrentPlayMusic.ID;
				BeforePlayHandle(m_CurrentPlayMusic);
			}
			else
			{
				Play(TimeSpan.FromSeconds(_View.slider_play.Value));
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
			MusicInfoViewModelV7 musicInfoViewModelV = FoundMusic(1);
			GetCurrentViewModel.SelectedItem = musicInfoViewModelV;
			BeforePlayHandle(musicInfoViewModelV);
		}
	}

	private void SliderPositionPreviewMouseUpCommandHandler(object aParams)
	{
		TimeSpan position = TimeSpan.FromSeconds(_View.slider_play.Value);
		_View.hasPlaytime.Text = position.ToString("mm\\:ss");
		_View.Sound.Position = position;
		if (isMediaEnd && _View.Sound.HasAudio)
		{
			isMediaEnd = position.TotalSeconds >= _View.Sound.NaturalDuration.TimeSpan.TotalSeconds;
			_View.Sound.Pause();
		}
	}

	private MusicInfoViewModelV7 FoundMusic(int moveType)
	{
		if (_SongList == null || _SongList.Count == 0)
		{
			return null;
		}
		ICollectionView defaultView = CollectionViewSource.GetDefaultView(_SongList);
		MusicInfoViewModelV7 musicInfoViewModelV = null;
		if (defaultView.CurrentItem == null || moveType == 0)
		{
			if (GetCurrentViewModel.SelectedItem != null)
			{
				if (defaultView.MoveCurrentTo(GetCurrentViewModel.SelectedItem))
				{
					musicInfoViewModelV = defaultView.CurrentItem as MusicInfoViewModelV7;
				}
			}
			else
			{
				defaultView.MoveCurrentToFirst();
				musicInfoViewModelV = defaultView.CurrentItem as MusicInfoViewModelV7;
			}
		}
		else if (moveType < 0)
		{
			defaultView.MoveCurrentToPrevious();
			musicInfoViewModelV = defaultView.CurrentItem as MusicInfoViewModelV7;
		}
		else
		{
			defaultView.MoveCurrentToNext();
			musicInfoViewModelV = defaultView.CurrentItem as MusicInfoViewModelV7;
		}
		return GetCurrentViewModel.SelectedItem = musicInfoViewModelV;
	}

	private void BeforePlayHandle(MusicInfoViewModelV7 music)
	{
		if (music != null)
		{
			_Timer.Enabled = true;
			string text = Path.Combine(_MusicCacheDir, music.SongName);
			if (File.Exists(text))
			{
				HostProxy.BehaviorService.Collect(BusinessType.SONG_PLAY, new BusinessData(BusinessType.SONG_PLAY, Context.CurrentDevice).Update(0L, BusinessStatus.SUCCESS, null));
				SetMusicSoundAndPlay(music, text);
			}
			else
			{
				CacheAndPlayMusic(music);
			}
		}
	}

	private void SetMusicSoundAndPlay(MusicInfoViewModelV7 musice, string uri)
	{
		isMediaEnd = false;
		CurrentPlayId = musice.ID;
		_View.slider_play.Value = 0.0;
		_View.Sound.Source = new Uri(uri);
		_View.totaltime.Text = musice.Duration;
		_View.playingUri.Text = musice.SongName;
		Play();
	}

	private void Play(TimeSpan? _timeSpan = null)
	{
		_View.Sound.Play();
		_View.Sound.Position = _timeSpan ?? TimeSpan.Zero;
		_View.BtnPause.Visibility = Visibility.Visible;
		_View.BtnPlay.Visibility = Visibility.Hidden;
	}

	private void CacheAndPlayMusic(MusicInfoViewModelV7 m)
	{
		AsyncDataLoader.Loading(delegate
		{
			string text = m.RawMusicInfo?.RawMusicInfo?.DisplayName;
			if (string.IsNullOrEmpty(text))
			{
				text = string.Empty;
			}
			string text2 = Path.GetExtension(text).ToLower();
			if (!text2.Equals(".mp3") && !text2.Equals(".wav"))
			{
				HostProxy.BehaviorService.Collect(BusinessType.SONG_PLAY, new BusinessData(BusinessType.SONG_PLAY, Context.CurrentDevice).Update(0L, BusinessStatus.FALIED, null));
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					Context.MessageBox.ShowMessage("K0071", "K0557");
				});
				return;
			}
			NavigationManagementBLLV6 navigationManagementBLLV = new NavigationManagementBLLV6();
			string filePath = string.Empty;
			try
			{
				filePath = navigationManagementBLLV.PrepareToPlayMusic(m, _MusicCacheDir);
				if (string.IsNullOrEmpty(filePath))
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						Context.MessageBox.ShowMessage("K0071", ResourcesHelper.StringResources.SingleInstance.MUSIC_PLAY_WARN);
					});
				}
				else
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						SetMusicSoundAndPlay(m, filePath);
					});
				}
			}
			catch
			{
			}
		});
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
