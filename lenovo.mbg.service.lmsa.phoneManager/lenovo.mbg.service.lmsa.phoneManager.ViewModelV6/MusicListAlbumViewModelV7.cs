using System.Collections.ObjectModel;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class MusicListAlbumViewModelV7 : NotifyBase
{
	private ObservableCollection<MusicListAlbumViewModelV7> albums;

	private MusicListAlbumViewModelV7 _selecttedAlbum;

	public ObservableCollection<MusicListAlbumViewModelV7> Albums => albums;

	public MusicListAlbumViewModelV7 SelectedAlbum
	{
		get
		{
			return _selecttedAlbum;
		}
		set
		{
			if (_selecttedAlbum != value)
			{
				_selecttedAlbum = value;
			}
		}
	}
}
