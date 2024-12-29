using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.support.Business;
using lenovo.mbg.service.lmsa.support.Commons;
using lenovo.mbg.service.lmsa.support.Contract;
using lenovo.mbg.service.lmsa.support.UserControls;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class SearchViewExModel : ViewModelBase
{
	protected static WarrantyService warrantyService = new WarrantyService();

	private bool isDataLoading;

	private string HelperLinkUrl = "https://lenovomobilesupport.lenovo.com/{0}/{1}/solutions/find-product-name";

	private string currentLanguage = string.Empty;

	private ModelnameSn selectedItem;

	private bool m_Checked;

	private Visibility warnVisibility = Visibility.Collapsed;

	private Visibility m_toggleButtonVisibility = Visibility.Collapsed;

	private Func<object, string, int> filterCompareDelegate;

	private ObservableCollection<ModelnameSn> _RedisteredDevices = new ObservableCollection<ModelnameSn>();

	private object m_comboBoxWatermark = "K0264";

	public bool IsDataLoading
	{
		get
		{
			return isDataLoading;
		}
		set
		{
			if (isDataLoading != value)
			{
				isDataLoading = value;
				OnPropertyChanged("IsDataLoading");
			}
		}
	}

	public ModelnameSn SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			if (selectedItem != value)
			{
				selectedItem = value;
				LogHelper.LogInstance.Info("WarnVisibilityChanged");
				OnPropertyChanged("SelectedItem");
			}
		}
	}

	public bool Checked
	{
		get
		{
			return m_Checked;
		}
		set
		{
			m_Checked = value;
			OnPropertyChanged("Checked");
		}
	}

	public Visibility WarnVisibility
	{
		get
		{
			return warnVisibility;
		}
		set
		{
			if (warnVisibility != value)
			{
				warnVisibility = value;
				OnPropertyChanged("WarnVisibility");
			}
		}
	}

	public Visibility ToggleButtonVisibility
	{
		get
		{
			return m_toggleButtonVisibility;
		}
		set
		{
			if (m_toggleButtonVisibility != value)
			{
				m_toggleButtonVisibility = value;
				OnPropertyChanged("ToggleButtonVisibility");
			}
		}
	}

	public Func<object, string, int> FilterCompareDelegate
	{
		get
		{
			return filterCompareDelegate;
		}
		set
		{
			if (filterCompareDelegate != value)
			{
				filterCompareDelegate = value;
				OnPropertyChanged("FilterCompareDelegate");
			}
		}
	}

	public ObservableCollection<ModelnameSn> RedisteredDevices
	{
		get
		{
			return _RedisteredDevices;
		}
		set
		{
			_RedisteredDevices = value;
			OnPropertyChanged("RedisteredDevices");
		}
	}

	public ReplayCommand SearchCommand { get; private set; }

	public ReplayCommand HelperCommand { get; private set; }

	public object ComboBoxWatermark
	{
		get
		{
			if (HostProxy.LanguageService.IsNeedTranslate())
			{
				return HostProxy.LanguageService.Translate(m_comboBoxWatermark.ToString());
			}
			return m_comboBoxWatermark;
		}
	}

	public SearchViewExModel()
	{
		RedisteredDevices = new ObservableCollection<ModelnameSn>();
		SearchCommand = new ReplayCommand(SearchCommandHandler);
		HelperCommand = new ReplayCommand(HelperCommandHandler);
		FilterCompareDelegate = FilterCompareDelegateHandler;
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		LoadRegisterDevices();
	}

	public override void LoadData()
	{
		base.LoadData();
		LoadRegisterDevices();
	}

	private void LoadRegisterDevices()
	{
		lock (this)
		{
			base.LoadData();
			SyncViewDeviceList();
			HandleMasterDevice();
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				ToggleButtonVisibility = ((RedisteredDevices.Count == 0) ? Visibility.Hidden : Visibility.Visible);
			});
		}
	}

	private List<ModelnameSn> GetRegisterDevice()
	{
		if (HostProxy.User.user != null)
		{
			JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.DefaultOptionsFileName, "$.devices[?(@.userId =='" + HostProxy.User.user.UserId + "')].registeredModels");
			if (jArray != null && jArray.HasValues)
			{
				ToggleButtonVisibility = Visibility.Visible;
				List<JToken> list = jArray.OrderBy((JToken n) => n.Value<string>("modelName")).ToList();
				List<ModelnameSn> list2 = new List<ModelnameSn>();
				{
					foreach (JToken item in list)
					{
						string text = item.Value<string>("modelName");
						string text2 = item.Value<string>("category");
						string text3 = item.Value<string>("sn");
						if (!string.IsNullOrEmpty(text2) && text2.Equals("phone", StringComparison.CurrentCultureIgnoreCase))
						{
							text3 = item.Value<string>("imei");
							if (string.IsNullOrEmpty(text3))
							{
								text3 = item.Value<string>("imei2");
							}
						}
						list2.Add(new ModelnameSn
						{
							modelName = text,
							RegisteredModelName = text,
							sn = text3
						});
					}
					return list2;
				}
			}
		}
		return null;
	}

	private void SyncViewDeviceList()
	{
		List<ModelnameSn> registerDevice = GetRegisterDevice();
		if (registerDevice != null && registerDevice.Count > 0)
		{
			List<ModelnameSn> viewDevices = RedisteredDevices.ToList();
			if (viewDevices == null)
			{
				viewDevices = new List<ModelnameSn>();
			}
			ModelnameSn target = null;
			int i;
			for (i = viewDevices.Count - 1; i >= 0; i--)
			{
				target = viewDevices[i];
				target.modelName = target.RegisteredModelName;
				if (string.IsNullOrEmpty(target.sn) || !registerDevice.Exists((ModelnameSn m) => target.sn.Equals(m.sn)))
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						viewDevices.RemoveAt(i);
						RedisteredDevices.Remove(target);
					});
				}
			}
			{
				foreach (ModelnameSn d in registerDevice)
				{
					if (!string.IsNullOrEmpty(d.sn) && !viewDevices.Exists((ModelnameSn m) => d.sn.Equals(m.sn)))
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							viewDevices.Add(d);
							RedisteredDevices.Add(d);
						});
					}
				}
				return;
			}
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			RedisteredDevices.Clear();
		});
	}

	private void HandleMasterDevice()
	{
		if (HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { SoftStatus: DeviceSoftStateEx.Online, Property: not null } tcpAndroidDevice)
		{
			string text = tcpAndroidDevice.Property.SN;
			string category = tcpAndroidDevice.Property.Category;
			if (!string.IsNullOrEmpty(category) && category.Equals("phone", StringComparison.CurrentCultureIgnoreCase))
			{
				text = tcpAndroidDevice.Property.IMEI1;
				if (string.IsNullOrEmpty(text))
				{
					text = tcpAndroidDevice.Property.IMEI2;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			List<ModelnameSn> devices = RedisteredDevices.ToList();
			ModelnameSn target = null;
			foreach (ModelnameSn item in devices)
			{
				if (!string.IsNullOrEmpty(item.sn) && item.sn.Equals(text))
				{
					target = item;
					item.modelName = "current device";
				}
			}
			if (target == null)
			{
				target = new ModelnameSn
				{
					RegisteredModelName = tcpAndroidDevice.Property.ModelName,
					modelName = "current device",
					sn = text
				};
				devices.Add(target);
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				SelectedItem = null;
				RedisteredDevices = new ObservableCollection<ModelnameSn>(devices);
				SelectedItem = target;
			});
		}
		else
		{
			SelectedItem = null;
		}
	}

	private void SearchCommandHandler(object data)
	{
		Checked = false;
		WarnVisibility = Visibility.Collapsed;
		string keyWords = data?.ToString();
		IsDataLoading = true;
		Task.Factory.StartNew(delegate
		{
			if (!string.IsNullOrEmpty(keyWords))
			{
				Dictionary<string, string> extraData = new Dictionary<string, string> { { "data", keyWords } };
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				bool flag = ViaSupportApi(keyWords) || ViaSdepoiApi(keyWords);
				if (!flag)
				{
					HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
					{
						WarnVisibility = Visibility.Visible;
					});
				}
				stopwatch.Stop();
				HostProxy.BehaviorService.Collect(BusinessType.SUPPORT_WARRRANTY, new BusinessData(BusinessType.SUPPORT_WARRRANTY, null).Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, extraData));
			}
			HostProxy.CurrentDispatcher?.Invoke(() => IsDataLoading = false);
		});
	}

	private void HelperCommandHandler(object args)
	{
		Tuple<string, string> currentLanguageInfo = GetCurrentLanguageInfo();
		if (currentLanguageInfo != null)
		{
			HelperLinkUrl = string.Format(HelperLinkUrl, currentLanguageInfo.Item1, currentLanguageInfo.Item2);
		}
		else
		{
			HelperLinkUrl = string.Format(HelperLinkUrl, "us", "en");
		}
		LogHelper.LogInstance.Info("helper link url:[" + HelperLinkUrl + "]");
		GlobalFun.OpenUrlByBrowser(HelperLinkUrl);
	}

	private Tuple<string, string> GetCurrentLanguageInfo()
	{
		string text = HostProxy.LanguageService.GetCurrentLanguage();
		if (!string.IsNullOrEmpty(text) && !text.Equals(currentLanguage))
		{
			string[] array = text.Split('-');
			if (array.Length == 2)
			{
				return new Tuple<string, string>(array[1].ToLower(), array[0].ToLower());
			}
		}
		return null;
	}

	private bool ViaSupportApi(string keyWords)
	{
		string supportWarranty = warrantyService.GetSupportWarranty(keyWords);
		if (string.IsNullOrWhiteSpace(supportWarranty))
		{
			return false;
		}
		SupportWarrantyInfo warrantyInfo = null;
		try
		{
			warrantyInfo = JsonConvert.DeserializeObject<SupportWarrantyInfo>(supportWarranty);
			if (warrantyInfo != null && warrantyInfo.Warranties != null)
			{
				foreach (SupportWarrantyItemInfo warranty in warrantyInfo.Warranties)
				{
					if (EnumMapping.DeliveryTypeMapping.ContainsKey(warranty.DeliveryType))
					{
						warranty.DeliveryType = EnumMapping.DeliveryTypeMapping[warranty.DeliveryType];
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Convert warranty info to obj error:" + ex);
			warrantyInfo = null;
		}
		if (string.IsNullOrEmpty(warrantyInfo?.ID))
		{
			return false;
		}
		HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
		{
			((ViewModelBase)(lenovo.mbg.service.lmsa.support.Commons.ViewContext.SwitchView<SupportSearchResultView>().DataContext = new SupportSearchResultViewModel()))?.LoadData((object)warrantyInfo);
		});
		return true;
	}

	private bool ViaIBaseApi(string keyWords)
	{
		string ibaseWarranty = warrantyService.GetIbaseWarranty(keyWords);
		if (string.IsNullOrWhiteSpace(ibaseWarranty))
		{
			return false;
		}
		IBaseWarrantyInfo info = IBaseWarrantyConverter.Convert(ibaseWarranty);
		if (info == null)
		{
			return false;
		}
		HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
		{
			((ViewModelBase)(lenovo.mbg.service.lmsa.support.Commons.ViewContext.SwitchView<IBaseSearchResultView>().DataContext = new IBaseSearchResultViewModel()))?.LoadData((object)info);
		});
		return true;
	}

	private bool ViaSdepoiApi(string keyWords)
	{
		string sdeWarranty = warrantyService.GetSdeWarranty(keyWords);
		if (string.IsNullOrWhiteSpace(sdeWarranty))
		{
			return false;
		}
		IBaseWarrantyInfo info = IBaseWarrantyConverter.ConvertEx(sdeWarranty);
		if (info == null)
		{
			return false;
		}
		HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
		{
			((ViewModelBase)(lenovo.mbg.service.lmsa.support.Commons.ViewContext.SwitchView<IBaseSearchResultView>().DataContext = new IBaseSearchResultViewModel()))?.LoadData((object)info);
		});
		return true;
	}

	private int FilterCompareDelegateHandler(object value, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		if (value is ModelnameSn modelnameSn)
		{
			string value2 = text.ToUpper();
			string modelName = modelnameSn.modelName;
			string sn = modelnameSn.sn;
			if (!string.IsNullOrEmpty(modelName) && modelName.ToUpper().Contains(value2))
			{
				return 0;
			}
			if (!string.IsNullOrEmpty(sn) && sn.ToUpper().Contains(value2))
			{
				return 0;
			}
		}
		return -1;
	}
}
