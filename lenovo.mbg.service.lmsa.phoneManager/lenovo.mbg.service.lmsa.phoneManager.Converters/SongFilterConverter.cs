using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class SongFilterConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null && values.Length < 2 && !(values[0] is ObservableCollection<MusicInfoViewModel>))
		{
			return null;
		}
		ObservableCollection<MusicInfoViewModel> observableCollection = values[0] as ObservableCollection<MusicInfoViewModel>;
		string key = values[1] as string;
		if (string.IsNullOrEmpty(key))
		{
			return observableCollection;
		}
		key = key.ToLower();
		return observableCollection.Where(delegate(MusicInfoViewModel p)
		{
			MusicInfo rawMusicInfo = p.RawMusicInfo;
			if (rawMusicInfo == null)
			{
				return false;
			}
			return (rawMusicInfo.DisplayName != null && rawMusicInfo.DisplayName.ToLower().Contains(key)) || (rawMusicInfo.Artist != null && rawMusicInfo.Artist.ToLower().Contains(key)) || (rawMusicInfo.RawMusicInfo != null && rawMusicInfo.RawMusicInfo.AlbumInfo != null && rawMusicInfo.RawMusicInfo.AlbumInfo.AlbumName != null && rawMusicInfo.RawMusicInfo.AlbumInfo.AlbumName.ToLower().Contains(key));
		});
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
