using System;
using System.Timers;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class RescuingViewModel : ViewModelBase
{
	private TimeSpan costTime;

	private double percentage;

	private string flashInfoText;

	public Timer TickTimer { get; }

	public TimeSpan CostTime
	{
		get
		{
			return costTime;
		}
		set
		{
			costTime = value;
			OnPropertyChanged("CostTime");
		}
	}

	public double Percentage
	{
		get
		{
			return percentage;
		}
		set
		{
			percentage = value;
			OnPropertyChanged("Percentage");
		}
	}

	public string FlashInfoText
	{
		get
		{
			return flashInfoText;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				flashInfoText = value;
				OnPropertyChanged("FlashInfoText");
			}
		}
	}

	public RescuingViewModel()
	{
		RescuingViewModel rescuingViewModel = this;
		DateTime start = DateTime.Now;
		Percentage = 0.0;
		TickTimer = new Timer(1000.0);
		TickTimer.Elapsed += delegate
		{
			rescuingViewModel.CostTime = DateTime.Now - start;
		};
		TickTimer.Start();
	}
}
