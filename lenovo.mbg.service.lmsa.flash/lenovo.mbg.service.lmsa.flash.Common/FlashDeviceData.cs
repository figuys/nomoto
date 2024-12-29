using System;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class FlashDeviceData
{
	public static double RESET_MINUTES = 2880.0;

	public DateTime failedfirst { get; set; }

	public DateTime failedlast { get; set; }

	public DateTime successfirst { get; set; }

	public DateTime successlast { get; set; }

	public int success { get; set; }

	public int failed { get; set; }

	public string modelname { get; set; }

	public double failedminutes
	{
		get
		{
			_ = failedfirst;
			_ = failedlast;
			return failedlast.Subtract(failedfirst).TotalMinutes;
		}
	}

	public double successminutes
	{
		get
		{
			_ = successfirst;
			_ = successlast;
			return successlast.Subtract(successfirst).TotalMinutes;
		}
	}
}
