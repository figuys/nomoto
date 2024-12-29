using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.OrderView;
using lenovo.themes.generic.Controls.Windows;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class B2BPurchaseOverviewV6 : UserControl, IComponentConnector, IStyleConnector
{
	public ObservableCollection<OrderModel> ActiveOrderArr { get; set; }

	public ObservableCollection<OrderModel> OrderArr { get; set; }

	public B2BPurchaseOverviewV6()
	{
		InitializeComponent();
		OrderArr = new ObservableCollection<OrderModel>();
		ActiveOrderArr = new ObservableCollection<OrderModel>();
		base.Loaded += delegate
		{
			List<PurchaseBtnModel> itemsSource = new List<PurchaseBtnModel>
			{
				new PurchaseBtnModel
				{
					IsSelected = true,
					NameLangKey = "Current Purchases",
					NormalIcon = (FindResource("v6_CurrentPurchasesIcon") as ImageSource),
					SelectedIcon = (FindResource("v6_CurrentPurchasesIconSelected") as ImageSource)
				},
				new PurchaseBtnModel
				{
					NameLangKey = "Purchase History",
					NormalIcon = (FindResource("v6_PurchaseHistoryIcon") as ImageSource),
					SelectedIcon = (FindResource("v6_PurchaseHistoryIconSelected") as ImageSource)
				},
				new PurchaseBtnModel
				{
					NameLangKey = "Support",
					NormalIcon = (FindResource("v6_SupportIcon") as ImageSource),
					SelectedIcon = (FindResource("v6_SupportIconSelected") as ImageSource)
				}
			};
			PurchaseBtnList.ItemsSource = itemsSource;
			txtUser.Text = UserService.Single.CurrentLoggedInUser.FullName;
			Task.Run(delegate
			{
				WaitTips tip = null;
				Task.Run(delegate
				{
					Application.Current.Dispatcher.Invoke(delegate
					{
						tip = new WaitTips("K1714");
						HostProxy.HostMaskLayerWrapper.New(tip, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => tip.ShowDialog());
					});
				});
				RespOrders data = AppContext.WebApi.RequestContent<RespOrders>(WebApiUrl.CALL_B2B_ACTIVE_ORDERS_URL, new
				{
					macAddressRsa = RsaHelper.RSAEncrypt(WebApiContext.RSA_PUBLIC_KEY, GlobalFun.GetMacAddr())
				});
				base.Dispatcher.Invoke(delegate
				{
					ActiveOrderArr.Clear();
					tip.Close();
					if (data?.enableOrderDtos != null)
					{
						foreach (OrderItem enableOrderDto in data.enableOrderDtos)
						{
							if (!(enableOrderDto.type == "FREE") && enableOrderDto.display)
							{
								DateTime dateTime = GlobalFun.ConvertDateTime(enableOrderDto.expiredDate);
								OrderModel orderModel = new OrderModel
								{
									Id = enableOrderDto.orderId.ToString(),
									Package = enableOrderDto.orderLevelDesc,
									EnableRefund = enableOrderDto.refund
								};
								if (enableOrderDto.effectiveDate.HasValue)
								{
									orderModel.Purchase = GlobalFun.ConvertDateTime(enableOrderDto.effectiveDate);
								}
								else
								{
									orderModel.Purchase = new DateTime(1970, 1, 1);
								}
								orderModel.Expired = dateTime;
								orderModel.MacAddr = RsaHelper.RSADecryptByPublicKey(WebApiContext.RSA_PUBLIC_KEY, enableOrderDto.macAddressRsa);
								orderModel.Status = enableOrderDto.orderStatus;
								orderModel.ServerStatus = enableOrderDto.serverOrderStatus;
								orderModel.ImeiUsedCount = enableOrderDto.imeiUsedCount;
								orderModel.IsMonthly = !enableOrderDto.imeiCount.HasValue;
								orderModel.RemainImei = enableOrderDto.imeiCount.GetValueOrDefault() - enableOrderDto.imeiUsedCount;
								orderModel.RemainDays = (dateTime - DateTime.Now).Days;
								orderModel.FlashedDevArr = enableOrderDto.imeiDtos;
								orderModel.DevInUse = enableOrderDto.imeiUsedCount.ToString();
								if (enableOrderDto.imeiCount > 0)
								{
									orderModel.DevInUse += $" / {enableOrderDto.imeiCount}";
								}
								if (enableOrderDto.usingDate.HasValue)
								{
									orderModel.StartUseDate = GlobalFun.ConvertDateTime(enableOrderDto.usingDate);
								}
								ActiveOrderArr.Add(orderModel);
							}
						}
						if (ActiveOrderArr.Count > 0)
						{
							ActiveOrderArr[ActiveOrderArr.Count - 1].SpliterVisible = Visibility.Collapsed;
						}
					}
				});
			}).ContinueWith(delegate
			{
				RespOrders data2 = AppContext.WebApi.RequestContent<RespOrders>(WebApiUrl.CALL_B2B_ORDERS_URL, new
				{
					macAddressRsa = RsaHelper.RSAEncrypt(WebApiContext.RSA_PUBLIC_KEY, GlobalFun.GetMacAddr())
				});
				base.Dispatcher.Invoke(delegate
				{
					OrderArr.Clear();
					RespOrders respOrders = data2;
					if (respOrders != null && respOrders.enableOrderDtos?.Count > 0)
					{
						foreach (OrderItem enableOrderDto2 in data2.enableOrderDtos)
						{
							if (!(enableOrderDto2.type == "FREE") && enableOrderDto2.display)
							{
								DateTime dateTime2 = GlobalFun.ConvertDateTime(enableOrderDto2.expiredDate);
								OrderModel orderModel2 = new OrderModel
								{
									Id = enableOrderDto2.orderId.ToString(),
									Package = enableOrderDto2.orderLevelDesc,
									EnableRefund = enableOrderDto2.refund,
									Purchase = GlobalFun.ConvertDateTime(enableOrderDto2.effectiveDate),
									Expired = dateTime2,
									MacAddr = RsaHelper.RSADecryptByPublicKey(WebApiContext.RSA_PUBLIC_KEY, enableOrderDto2.macAddressRsa),
									Status = enableOrderDto2.orderStatus,
									ServerStatus = enableOrderDto2.serverOrderStatus,
									ImeiUsedCount = enableOrderDto2.imeiUsedCount,
									IsMonthly = !enableOrderDto2.imeiCount.HasValue,
									RemainImei = enableOrderDto2.imeiCount.GetValueOrDefault() - enableOrderDto2.imeiUsedCount,
									RemainDays = (dateTime2 - DateTime.Now).Days,
									FlashedDevArr = enableOrderDto2.imeiDtos
								};
								orderModel2.DevInUse = enableOrderDto2.imeiUsedCount.ToString();
								if (enableOrderDto2.imeiCount > 0)
								{
									orderModel2.DevInUse += $" / {enableOrderDto2.imeiCount}";
								}
								if (enableOrderDto2.usingDate.HasValue)
								{
									orderModel2.StartUseDate = GlobalFun.ConvertDateTime(enableOrderDto2.usingDate);
								}
								OrderArr.Add(orderModel2);
							}
						}
					}
					if (data2?.unableOrderDtos == null)
					{
						return;
					}
					foreach (OrderItem unableOrderDto in data2.unableOrderDtos)
					{
						if (!(unableOrderDto.type == "FREE") && unableOrderDto.display)
						{
							OrderModel orderModel3 = new OrderModel
							{
								Id = unableOrderDto.orderId.ToString(),
								Package = unableOrderDto.orderLevelDesc,
								EnableRefund = unableOrderDto.refund,
								Purchase = GlobalFun.ConvertDateTime(unableOrderDto.effectiveDate),
								Expired = GlobalFun.ConvertDateTime(unableOrderDto.expiredDate),
								MacAddr = RsaHelper.RSADecryptByPublicKey(WebApiContext.RSA_PUBLIC_KEY, unableOrderDto.macAddressRsa),
								Status = unableOrderDto.orderStatus,
								IsMonthly = !unableOrderDto.imeiCount.HasValue,
								ServerStatus = unableOrderDto.serverOrderStatus,
								ImeiUsedCount = unableOrderDto.imeiUsedCount,
								RemainImei = unableOrderDto.imeiCount.GetValueOrDefault() - unableOrderDto.imeiUsedCount,
								RemainDays = 0,
								FlashedDevArr = unableOrderDto.imeiDtos
							};
							orderModel3.DevInUse = unableOrderDto.imeiUsedCount.ToString();
							if (unableOrderDto.imeiCount > 0)
							{
								orderModel3.DevInUse += $" / {unableOrderDto.imeiCount}";
							}
							if (unableOrderDto.usingDate.HasValue)
							{
								orderModel3.StartUseDate = GlobalFun.ConvertDateTime(unableOrderDto.usingDate);
							}
							OrderArr.Add(orderModel3);
						}
					}
				});
			});
		};
		base.DataContext = this;
		BuyPanel.OnBuyAction = null;
		if (UserService.Single.CurrentLoggedInUser.B2BBuyNowDisplay)
		{
			txtBuyPanelTitle.Visibility = Visibility.Visible;
			BuyPanel.Visibility = Visibility.Visible;
		}
		else
		{
			txtBuyPanelTitle.Visibility = Visibility.Hidden;
			BuyPanel.Visibility = Visibility.Hidden;
		}
	}

	private void PurchaseBtnList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ListBox listBox = sender as ListBox;
		if (listBox.SelectedIndex != -1)
		{
			if (listBox.SelectedIndex == 0)
			{
				panel1.Visibility = Visibility.Visible;
				panel2.Visibility = Visibility.Collapsed;
				panel3.Visibility = Visibility.Collapsed;
			}
			else if (listBox.SelectedIndex == 1)
			{
				panel1.Visibility = Visibility.Collapsed;
				panel2.Visibility = Visibility.Visible;
				panel3.Visibility = Visibility.Collapsed;
			}
			else
			{
				panel1.Visibility = Visibility.Collapsed;
				panel2.Visibility = Visibility.Collapsed;
				panel3.Visibility = Visibility.Visible;
			}
		}
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
	}

	private void OnLBtnDetail(object sender, MouseButtonEventArgs e)
	{
		OrderModel order = (sender as TextBlock).Tag as OrderModel;
		OrderDetailView orderDetailView = new OrderDetailView(order);
		orderDetailView.ShowDialog();
	}

	private void OnLBtnViewOrderId(object sender, MouseButtonEventArgs e)
	{
		OrderModel orderModel = (sender as TextBlock).Tag as OrderModel;
		string id = AppContext.WebApi.RequestBase(WebApiUrl.CALL_B2B_GET_ORDERID_URL, new
		{
			macAddressRsa = RsaHelper.RSAEncrypt(WebApiContext.RSA_PUBLIC_KEY, GlobalFun.GetMacAddr()),
			orderId = orderModel.Id
		}).content?.ToString();
		OrderRefundIDView orderRefundIDView = new OrderRefundIDView(id);
		orderRefundIDView.ShowDialog();
	}

	private void OnLBtnRefund(object sender, MouseButtonEventArgs e)
	{
		OrderModel orderModel = (sender as TextBlock).Tag as OrderModel;
		OrderRefundView orderRefundView = new OrderRefundView(orderModel.Id);
		orderRefundView.ShowDialog();
	}

	private void OnLBtnCancel(object sender, MouseButtonEventArgs e)
	{
		OrderCancelView orderCancelView = new OrderCancelView();
		orderCancelView.ShowDialog();
	}

	private void OnLBtnUrl(object sender, MouseButtonEventArgs e)
	{
		string text = (sender as TextBlock).Text;
		GlobalFun.OpenUrlByBrowser(text);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
