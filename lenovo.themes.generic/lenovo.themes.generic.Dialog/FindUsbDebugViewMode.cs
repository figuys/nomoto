using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Dialog;

public class FindUsbDebugViewMode : NotifyBase
{
	private Uri _ImagePath;

	private string _Note;

	public string Title { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public Uri ImagePath
	{
		get
		{
			return _ImagePath;
		}
		set
		{
			_ImagePath = value;
		}
	}

	public string Note
	{
		get
		{
			return _Note;
		}
		set
		{
			_Note = value;
		}
	}

	public FindUsbDebugViewMode(string image, string info)
	{
		Note = info;
		ImagePath = new Uri(image);
		Title = "K1082";
		CloseCommand = new ReplayCommand(delegate(object param)
		{
			(param as Window).Close();
		});
	}
}
