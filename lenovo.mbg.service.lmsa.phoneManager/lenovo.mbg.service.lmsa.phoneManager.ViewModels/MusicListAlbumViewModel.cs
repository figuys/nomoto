using System.Collections.ObjectModel;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class MusicListAlbumViewModel : NotifyBase
{
	private ObservableCollection<MusicListAlbumViewModel> albums;

	private MusicListAlbumViewModel _selecttedAlbum;

	public ObservableCollection<MusicListAlbumViewModel> Albums => albums;

	public MusicListAlbumViewModel SelectedAlbum
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
