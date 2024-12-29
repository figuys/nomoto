using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ModelV6;

namespace lenovo.themes.generic.ViewModelV6;

public class WarrantyInfoViewModelV6
{
	protected WarrantyInfoBaseModel WarrantyInfo;

	public Brush StatusForeground
	{
		get
		{
			if (WarrantyInfo != null)
			{
				WarrantyInfoBaseModel warrantyInfo = WarrantyInfo;
				if (warrantyInfo == null || !warrantyInfo.InWarranty)
				{
					return Application.Current.Resources["v6_comm_warranty_label_red"] as SolidColorBrush;
				}
				return Application.Current.Resources["v6_comm_warranty_label_df"] as SolidColorBrush;
			}
			return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9FAEBF"));
		}
	}

	public string WarrantyContent
	{
		get
		{
			if (WarrantyInfo != null)
			{
				if (!WarrantyInfo.InWarranty)
				{
					return "K1365";
				}
				return "K1364";
			}
			return "K1409";
		}
	}

	public string WarrantyStartDate => WarrantyInfo?.WarrantyStartDate ?? "-";

	public string WarrantyEndDate => WarrantyInfo?.WarrantyEndDate ?? "-";

	public string ExpriedDays
	{
		get
		{
			if (WarrantyInfo == null)
			{
				return "-";
			}
			if (WarrantyInfo.ExpriedDays >= 0)
			{
				return WarrantyInfo.ExpriedDays.ToString();
			}
			return "0";
		}
	}

	public WarrantyInfoViewModelV6(WarrantyInfoBaseModel warranty)
	{
		WarrantyInfo = warranty;
	}
}
