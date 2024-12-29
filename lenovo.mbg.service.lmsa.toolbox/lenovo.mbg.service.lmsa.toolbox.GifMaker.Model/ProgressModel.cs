using System.Windows.Input;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;

public class ProgressModel : ViewModelBase
{
	private double _Percentage;

	private string _Information;

	public double Percentage
	{
		get
		{
			return _Percentage;
		}
		set
		{
			_Percentage = value;
			OnPropertyChanged("Percentage");
		}
	}

	public string Information
	{
		get
		{
			return _Information;
		}
		set
		{
			_Information = value;
			OnPropertyChanged("Information");
		}
	}

	public ICommand CloseCmd { get; set; }
}
