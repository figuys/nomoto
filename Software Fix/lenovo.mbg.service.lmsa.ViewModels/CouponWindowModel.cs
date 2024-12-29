using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.mbg.service.lmsa.ViewV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class CouponWindowModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	public string subText1 = "AS a thank you, please use this coupon code for an additional {0} off any of our new phones at checkout.";

	protected const string UrlFormat = "https://www.motorola.com/us/vtex-rnt-esp/?pid=5PS8C04083&deviceSerialNumber={0}&SN={1}&warrantyStartDate={2}&warrantyEndDate={3}&qty=1&orderSource=RSA&promoCode={4}";

	private bool _IsCopied;

	public ReplayCommand CloseCommand { get; }

	public ReplayCommand CopyCommand { get; }

	public ReplayCommand BuyCommand { get; }

	public string Discount { get; set; }

	public string Code { get; set; }

	public string Url { get; set; }

	public string SubText { get; set; }

	public int PositionStart { get; set; }

	public int PositionCount { get; set; }

	public Visibility TextVisibility { get; set; }

	public bool IsCopied
	{
		get
		{
			return _IsCopied;
		}
		set
		{
			_IsCopied = value;
			OnPropertyChanged("IsCopied");
		}
	}

	public CouponWindowModel(MotoCareInfo data, CouponInfo coupon)
	{
		Discount = coupon.discountInfo;
		Code = coupon.discountCode;
		Url = (data.InWarranty ? $"https://www.motorola.com/us/vtex-rnt-esp/?pid=5PS8C04083&deviceSerialNumber={data.imei}&SN={data.sn}&warrantyStartDate={data.WarrantyStartDate}&warrantyEndDate={data.WarrantyEndDate}&qty=1&orderSource=RSA&promoCode={coupon.discountCode}" : data.url);
		TextVisibility = ((data.type != 2) ? Visibility.Hidden : Visibility.Visible);
		string text = LangTranslation.Translate("K1391");
		text = text + " " + Discount;
		SubText = string.Format(LangTranslation.Translate("K1390"), text);
		PositionStart = SubText.IndexOf(text);
		PositionCount = text.Length;
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		CopyCommand = new ReplayCommand(CopyCommandHandler);
		BuyCommand = new ReplayCommand(BuyCommandHandler);
	}

	private void CloseCommandHandler(object data)
	{
		CouponWindow couponWindow = data as CouponWindow;
		couponWindow.Close();
		couponWindow.CloseAction?.Invoke(true);
		MainWindowViewModel.SingleInstance.MiddleBannerVisibility = Visibility.Visible;
	}

	private void CopyCommandHandler(object data)
	{
		Clipboard.SetDataObject(Code);
		IsCopied = true;
		Task.Run(delegate
		{
			Thread.Sleep(800);
		}).ContinueWith((Task s) => HostProxy.CurrentDispatcher.Invoke(() => IsCopied = false));
	}

	private void BuyCommandHandler(object data)
	{
		CouponWindow couponWindow = data as CouponWindow;
		couponWindow.Close();
		couponWindow.CloseAction?.Invoke(true);
		GlobalFun.OpenUrlByBrowser(Url);
	}
}
