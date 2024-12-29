using System.Windows;
using System.Windows.Media;

namespace lenovo.themes.generic.ModelV6;

public class ListViewItemModel
{
	private ImageSource _PrevImage;

	private bool _IsSelected;

	public int Id { get; }

	public string Text { get; set; }

	public ImageSource PrevImage
	{
		get
		{
			return _PrevImage;
		}
		set
		{
			_PrevImage = value;
			if (_PrevImage != null)
			{
				HasPrevImage = true;
			}
		}
	}

	public ImageSource PrevImageSelected { get; set; }

	public bool HasPrevImage { get; set; }

	public bool IsSelected
	{
		get
		{
			return _IsSelected;
		}
		set
		{
			_IsSelected = value;
		}
	}

	public ListViewItemModel(int id, string text)
	{
		Id = id;
		Text = text;
	}

	public ListViewItemModel(int id, string text, string prevImage, string prevImageSelected)
		: this(id, text)
	{
		PrevImage = Application.Current.Resources[prevImage] as ImageSource;
		PrevImageSelected = Application.Current.Resources[prevImageSelected] as ImageSource;
	}
}
