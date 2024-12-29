using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.ViewV6;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderDetailView : Window, IComponentConnector
{
	public string RowItem1 { get; set; }

	public string RowItem2 { get; set; }

	public string RowItem3 { get; set; }

	public double TotalDays { get; set; }

	public double RemainDays { get; set; }

	public ObservableCollection<FlashedDevModel> FlashDevArr { get; set; }

	public OrderDetailView(OrderModel order)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		if (order.IsMonthly)
		{
			if (!string.IsNullOrEmpty(order.Package) && order.Package.Contains("B2B"))
			{
				base.Title = "K1738";
			}
			else
			{
				base.Title = "K1723";
			}
			RowItem1 = string.Format("{0}:   {1}", LangTranslation.Translate("K1725"), order?.ImeiUsedCount);
			RowItem2 = string.Format("{0}:  {1:MM-dd-yyyy}", LangTranslation.Translate("K1706"), order?.Expired);
			RowItem3 = string.Format("{0}: {1}", LangTranslation.Translate("K1716"), order?.RemainDays);
		}
		else
		{
			base.Title = "K1724";
			RowItem1 = string.Format("{0}:   {1}", LangTranslation.Translate("K1725"), order?.ImeiUsedCount);
			RowItem2 = string.Format("{0}:  {1}", LangTranslation.Translate("K1726"), order?.RemainImei);
			if (order != null && order.StartUseDate.HasValue)
			{
				RowItem3 = string.Format("IMEI {0}:   {1:MM-dd-yyyy}", LangTranslation.Translate("K0276"), order.StartUseDate);
			}
			else
			{
				RowItem3 = "IMEI " + LangTranslation.Translate("K0276") + ":   " + LangTranslation.Translate("K0307");
			}
		}
		TotalDays = 30.0;
		if (order != null)
		{
			RemainDays = order.RemainDays;
		}
		if (order.FlashedDevArr != null)
		{
			FlashDevArr = new ObservableCollection<FlashedDevModel>(order.FlashedDevArr);
		}
		base.DataContext = this;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
