using System;
using System.ComponentModel;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class MusicInfoViewModel : ViewModelBase, ISupportInitialize, IWeakEventListener
{
	private string songName;

	private string filePath;

	private string totalTime;

	private string hasPlayTime;

	private bool _isSelected;

	public int ID { get; set; }

	public string Artist { get; set; }

	public string AlbumID { get; set; }

	public string Album { get; set; }

	public string Size { get; set; }

	public int IntDuration { get; set; }

	public string Duration { get; set; }

	public string ModifiedDate { get; set; }

	public double Frequency { get; set; }

	public MusicInfo RawMusicInfo { get; set; }

	public string SongName
	{
		get
		{
			return songName;
		}
		set
		{
			songName = value;
			OnPropertyChanged("SongName");
		}
	}

	public string FilePath
	{
		get
		{
			return filePath;
		}
		set
		{
			filePath = value;
			OnPropertyChanged("FilePath");
		}
	}

	public string TotalTime
	{
		get
		{
			return totalTime;
		}
		set
		{
			totalTime = value;
		}
	}

	public string HasPlay
	{
		get
		{
			return hasPlayTime;
		}
		set
		{
			hasPlayTime = value;
			OnPropertyChanged("HasPlay");
		}
	}

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public long LongSize { get; set; }

	public MusicInfoViewModel(MusicInfo Music)
	{
		ID = Music.ID;
		SongName = Music.DisplayName;
		IntDuration = Music.Duration;
		Duration = new TimeSpan(0, 0, Music.Duration / 1000).ToString("hh\\:mm\\:ss");
		AlbumID = Music.AlbumID;
		Album = Music.Album;
		Artist = Music.Artist;
		Frequency = Music.Frequency;
		Size = GlobalFun.ConvertLong2String(Music.Size, "F2");
		LongSize = Music.Size;
		ModifiedDate = Music.ModifiedDate;
		FilePath = Music.FilePath;
		RawMusicInfo = Music;
	}

	public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return false;
	}

	public void BeginInit()
	{
		throw new NotImplementedException();
	}

	public void EndInit()
	{
		throw new NotImplementedException();
	}
}
