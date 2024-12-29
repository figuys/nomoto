using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class SmartMMatchViewModel : ManualMatchViewModel
{
	protected bool _IsModelFirst;

	protected UserControl _View;

	protected RedoIfFailedTimer _Timer;

	private Visibility _ProductMoreBtnVisible;

	private Dictionary<string, List<ManualComboboxViewModel>> _CategoryArr;

	private ImageSource _SmartImage;

	private SmartCategoryModel _SelSmartCategory;

	public LComboBoxViewModelV6 CbxMarketNameVM { get; protected set; }

	public ObservableCollection<SmartCategoryModel> SmartArr { get; set; }

	public ImageSource SmartImage
	{
		get
		{
			return _SmartImage;
		}
		set
		{
			_SmartImage = value;
			OnPropertyChanged("SmartImage");
		}
	}

	public SmartCategoryModel SelSmartCategory
	{
		get
		{
			return _SelSmartCategory;
		}
		set
		{
			_SelSmartCategory = value;
			OnPropertyChanged("SelSmartCategory");
			if (value == null)
			{
				return;
			}
			_View.Dispatcher.Invoke(delegate
			{
				CbxMarketNameVM.StepComboBoxItemSource.Clear();
				_CategoryArr[value.name].ForEach(delegate(ManualComboboxViewModel p)
				{
					CbxMarketNameVM.StepComboBoxItemSource.Add(p);
				});
				base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
				base.CbxModelNameVM.ComboBoxFilter = ModelBySmartCategoryFilter;
			});
		}
	}

	public SmartMMatchViewModel(UserControl ui, DevCategory category)
		: base(ui, category)
	{
		_View = ui;
		_Category = category;
		_Timer = new RedoIfFailedTimer(delegate
		{
			try
			{
				LoadProductName();
			}
			finally
			{
				CbxMarketNameVM.IsDataLoading = false;
			}
		});
		base.CbxModelNameVM.DropDownOpenedChanged = delegate(bool isOpen)
		{
			if (!isOpen)
			{
				if (CbxMarketNameVM.ComboBoxSelectedValue == null)
				{
					base.CbxModelNameVM.ComboBoxFilter = ModelNameInitFilter;
					base.CbxModelNameVM.ComboBoxMoreButtonVisibility = _ModelMoreBtnVisible;
				}
				else
				{
					base.CbxModelNameVM.ComboBoxFilter = ModelByProductFilter;
					base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
				}
			}
		};
		CbxMarketNameVM = new LComboBoxViewModelV6();
		CbxMarketNameVM.IsEditable = true;
		CbxMarketNameVM.ComboBoxSelectedIndex = -1;
		CbxMarketNameVM.ComboBoxWatermark = "K0846";
		CbxMarketNameVM.ItemSelChangedActon = OnMarketNameSelected;
		CbxMarketNameVM.DropDownOpenedChanged = delegate(bool isOpen)
		{
			if (!(CbxMarketNameVM.StepComboBoxItemSource == null || isOpen) && (CbxMarketNameVM.ComboBoxSelectedValue == null || !(CbxMarketNameVM.ComboBoxSelectedValue as ManualComboboxViewModel).IsMore))
			{
				CbxMarketNameVM.ComboBoxFilter = ProductInitFilter;
			}
		};
		CbxMarketNameVM.SetTopClickCommand = new RoutedCommand();
		CbxMarketNameVM.ComboBoxMoreButtonCommand = new RoutedCommand();
		CbxMarketNameVM.ComboBoxTextChangedCommand = new RoutedCommand();
		ui.CommandBindings.Add(new CommandBinding(CbxMarketNameVM.ComboBoxMoreButtonCommand, OnMarketNameMoreBtnClicked));
		ui.CommandBindings.Add(new CommandBinding(CbxMarketNameVM.ComboBoxTextChangedCommand, OnMarketNameTextChanged));
		ui.CommandBindings.Add(new CommandBinding(CbxMarketNameVM.SetTopClickCommand, OnSetTopClicked));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.ComboBoxTextChangedCommand, OnModelNameTextChanged));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.ComboBoxMoreButtonCommand, base.OnModelNameMoreBtnClicked));
		Task.Run(delegate
		{
			LoadProductName();
			LoadModelName();
		}).ContinueWith(delegate
		{
			if (Application.Current.Dispatcher.Invoke(() => _View.IsLoaded))
			{
				if (_CoditionMap == null)
				{
					if (base.CbxModelNameVM.StepComboBoxItemSource != null && base.CbxModelNameVM.StepComboBoxItemSource.Count != 0)
					{
						MainFrameV6.Instance.IMsgManager.SelRegistedDevIfExist($"{_Category}", delegate(string modelName)
						{
							if (!string.IsNullOrEmpty(modelName))
							{
								ManualComboboxViewModel manualComboboxViewModel = base.CbxModelNameVM.StepComboBoxItemSource?.FirstOrDefault((ManualComboboxViewModel p) => (p.Tag as DeviceModelInfoModel).ModelName == modelName);
								if (manualComboboxViewModel == null)
								{
									MainFrameV6.Instance.IMsgManager.ShowMessage("K0098");
								}
								else
								{
									base.CbxModelNameVM.ComboBoxFilter = DefaultFilter;
									base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
									base.CbxModelNameVM.IsDropDownEnabled = false;
									base.CbxModelNameVM.ComboBoxSelectedValue = manualComboboxViewModel;
								}
							}
						});
					}
				}
				else
				{
					AutoMatchByCoditonMap();
				}
			}
		});
		SmartArr = new ObservableCollection<SmartCategoryModel>();
		base.CbxModelNameVM.DropDownOpenedChanged = null;
		CbxMarketNameVM.DropDownOpenedChanged = null;
		CbxMarketNameVM.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
		{
			if (!(e.PropertyName != "ComboBoxSelectedValue"))
			{
				if (sender is LComboBoxViewModelV6 { ComboBoxSelectedValue: not null } lComboBoxViewModelV)
				{
					ManualComboboxViewModel manualComboboxViewModel2 = lComboBoxViewModelV.ComboBoxSelectedValue as ManualComboboxViewModel;
					SmartImage = (manualComboboxViewModel2.Tag as SmartMarketNameModel).Image;
				}
				else
				{
					SmartImage = null;
				}
			}
		};
		_CategoryArr = new Dictionary<string, List<ManualComboboxViewModel>>();
	}

	protected virtual void LoadProductName()
	{
		ResponseModel<List<SmartCategoryModel>> responseModel = FlashContext.SingleInstance.service.Request<List<SmartCategoryModel>>(WebApiUrl.LOAD_SMART_DEVICE, null);
		if (!responseModel.success)
		{
			_Timer.RegisterLoadDataTimer();
			_Timer.IsLoadFailed = true;
			return;
		}
		_Timer.DestoryLoadDataTimer();
		_Timer.IsLoadFailed = false;
		List<SmartCategoryModel> categoryArr = responseModel.content;
		if (categoryArr == null || categoryArr.Count <= 0)
		{
			return;
		}
		List<ManualComboboxViewModel> list = new List<ManualComboboxViewModel>();
		foreach (SmartCategoryModel item2 in categoryArr)
		{
			if (item2.marketNames.Count == 0)
			{
				continue;
			}
			_CategoryArr.Add(item2.name, new List<ManualComboboxViewModel>());
			foreach (SmartMarketNameModel marketName in item2.marketNames)
			{
				ManualComboboxViewModel item = new ManualComboboxViewModel
				{
					ItemText = marketName.MarketName,
					Tag = marketName
				};
				_CategoryArr[item2.name].Add(item);
				list.Add(item);
			}
		}
		list = list.OrderBy((ManualComboboxViewModel p) => p.ItemText).ToList();
		SetProductCbxItems(list, isMore: false);
		_View.Dispatcher.Invoke(delegate
		{
			categoryArr.ForEach(delegate(SmartCategoryModel p)
			{
				if (p.marketNames.Count > 0)
				{
					SmartArr.Add(p);
				}
			});
		});
	}

	public void MatchFromDownloadCenter(Dictionary<string, string> data)
	{
		_CoditionMap = data;
		if (CbxMarketNameVM.StepComboBoxItemSource != null)
		{
			AutoMatchByCoditonMap();
		}
	}

	private void AutoMatchByCoditonMap()
	{
		if (_CoditionMap.ContainsKey("marketName"))
		{
			ManualComboboxViewModel manualComboboxViewModel = CbxMarketNameVM.StepComboBoxItemSource?.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText == _CoditionMap["marketName"]);
			if (manualComboboxViewModel == null)
			{
				_CoditionMap = null;
				MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K0098", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
				ResetCoditionSelUi();
			}
			else
			{
				ResetCoditionSelUi();
				CbxMarketNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
				CbxMarketNameVM.ComboBoxFilter = base.DefaultFilter;
				CbxMarketNameVM.IsDropDownEnabled = false;
				CbxMarketNameVM.ComboBoxSelectedValue = manualComboboxViewModel;
			}
		}
	}

	private void OnSetTopClicked(object sender, ExecutedRoutedEventArgs e)
	{
		ManualComboboxViewModel unStop = e.Parameter as ManualComboboxViewModel;
		unStop.IsUsed = false;
		List<ManualComboboxViewModel> list = LoadRescuedDevice($"$.product{_Category}");
		int num = list.FindIndex((ManualComboboxViewModel n) => n.ItemText == unStop.ItemText);
		if (num != -1)
		{
			list.RemoveAt(num);
			FileHelper.WriteJsonWithAesEncrypt(Configurations.RescueManualMatchFile, $"product{_Category}", list, async: true);
		}
	}

	protected void SetProductCbxItems(List<ManualComboboxViewModel> remote, bool isMore)
	{
		foreach (ManualComboboxViewModel item in LoadRescuedDevice($"$.product{_Category}"))
		{
			ManualComboboxViewModel manualComboboxViewModel = remote.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText == item.ItemText);
			if (manualComboboxViewModel != null)
			{
				manualComboboxViewModel.IsUsed = true;
			}
		}
		if (isMore)
		{
			_ProductMoreBtnVisible = Visibility.Visible;
		}
		else if (remote.FirstOrDefault((ManualComboboxViewModel p) => p.IsMore) == null)
		{
			_ProductMoreBtnVisible = Visibility.Collapsed;
		}
		else
		{
			_ProductMoreBtnVisible = Visibility.Visible;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			CbxMarketNameVM.ComboBoxFilter = ProductInitFilter;
			CbxMarketNameVM.StepComboBoxItemSource = new ObservableCollection<ManualComboboxViewModel>(remote);
			CbxMarketNameVM.ComboBoxMoreButtonVisibility = _ProductMoreBtnVisible;
		});
		CbxMarketNameVM.IsDataLoading = false;
	}

	protected virtual void OnModelNameTextChanged(object sender, ExecutedRoutedEventArgs e)
	{
		if (CbxMarketNameVM.ComboBoxSelectedValue == null)
		{
			base.CbxModelNameVM.ComboBoxFilter = SearchFilter;
			base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		}
		else
		{
			base.CbxModelNameVM.ComboBoxFilter = ModelByProductFilter;
			base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		}
	}

	protected virtual void OnMarketNameTextChanged(object sender, ExecutedRoutedEventArgs e)
	{
		if (e.Parameter != null && e.Parameter as string == (CbxMarketNameVM.ComboBoxSelectedValue as ManualComboboxViewModel)?.ItemText)
		{
			CbxMarketNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
			CbxMarketNameVM.ComboBoxFilter = base.DefaultFilter;
		}
		else
		{
			CbxMarketNameVM.ComboBoxFilter = ProductSearchFilter;
		}
	}

	protected virtual void OnMarketNameSelected(object ob)
	{
		_ReqParams.Clear();
		if (!(ob is ManualComboboxViewModel manualComboboxViewModel))
		{
			_IsModelFirst = false;
			base.CbxModelNameVM.ComboBoxMoreButtonVisibility = ((_Category != DevCategory.Tablet) ? Visibility.Collapsed : Visibility.Visible);
			base.CbxModelNameVM.ComboBoxFilter = ModelNameInitFilter;
			base.CbxModelNameVM.ComboBoxSelectedValue = null;
			return;
		}
		base.ResetForModelName();
		if (!_IsModelFirst)
		{
			base.CbxModelNameVM.ComboBoxSelectedValue = null;
		}
		dynamic tag = manualComboboxViewModel.Tag;
		if (tag.ReadSupport)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("marketName", tag.MarketName);
			LoadFastbootData(dictionary);
		}
		else
		{
			_ReqParams.AddParameter("marketName", tag.MarketName);
			base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
			base.CbxModelNameVM.ComboBoxFilter = ModelByProductFilter;
			Dictionary<string, string> coditionMap = _CoditionMap;
			if (coditionMap != null && coditionMap.ContainsKey("modelName"))
			{
				ManualComboboxViewModel manualComboboxViewModel2 = base.CbxModelNameVM.StepComboBoxItemSource.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText == _CoditionMap["modelName"]);
				if (manualComboboxViewModel2 == null)
				{
					_CoditionMap = null;
					MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K0098", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
					ResetCoditionSelUi();
				}
				else
				{
					base.CbxModelNameVM.IsDropDownEnabled = false;
					base.CbxModelNameVM.ComboBoxSelectedValue = manualComboboxViewModel2;
				}
			}
		}
		_IsModelFirst = false;
	}

	protected override void OnModelNameSelected(object obj)
	{
		if (obj != null && CbxMarketNameVM.ComboBoxSelectedValue == null)
		{
			SetSelectedProductByModelName(obj as ManualComboboxViewModel);
		}
		base.OnModelNameSelected(obj);
	}

	private void OnMarketNameMoreBtnClicked(object sender, ExecutedRoutedEventArgs e)
	{
		CbxMarketNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		CbxMarketNameVM.ComboBoxFilter = base.DefaultFilter;
	}

	public void SetSelectedProductByModelName(ManualComboboxViewModel model)
	{
		_IsModelFirst = true;
		DeviceModelInfoModel info = model.Tag as DeviceModelInfoModel;
		ManualComboboxViewModel comboBoxSelectedValue = CbxMarketNameVM.StepComboBoxItemSource.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText == info.MarketName);
		CbxMarketNameVM.IsDropDownEnabled = false;
		CbxMarketNameVM.ComboBoxFilter = base.DefaultFilter;
		CbxMarketNameVM.ComboBoxSelectedValue = comboBoxSelectedValue;
		CbxMarketNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		base.CbxModelNameVM.ComboBoxFilter = ModelByProductFilter;
		base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
	}

	protected override void ResetForModelName()
	{
		string parameter = _ReqParams.GetParameter("marketName");
		_ReqParams.Clear();
		_ReqParams.AddParameter("marketName", parameter);
		_ReqParams.AddParameter("countryCode", GlobalFun.GetRegionInfo().TwoLetterISORegionName);
		base.ResetForModelName();
	}

	protected override void ResetCoditionSelUi()
	{
		CbxMarketNameVM.ComboBoxSelectedValue = null;
	}

	public override void RegisterDeviceAsync()
	{
		try
		{
			List<ManualComboboxViewModel> list = LoadRescuedDevice($"$.product{_Category}");
			ManualComboboxViewModel cur = CbxMarketNameVM.ComboBoxSelectedValue as ManualComboboxViewModel;
			cur.IsUsed = true;
			int num = list.FindIndex((ManualComboboxViewModel n) => n.ItemText == cur.ItemText);
			if (num != -1)
			{
				list.RemoveAt(num);
			}
			list.Insert(0, cur);
			FileHelper.WriteJsonWithAesEncrypt(Configurations.RescueManualMatchFile, $"product{_Category}", list, async: true);
		}
		catch (Exception)
		{
		}
	}

	public string GetMarktNameByModelName(string modelname)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("modelName", modelname);
		return FlashContext.SingleInstance.service.RequestContent<DeviceModelInfoModel>(WebServicesContext.GET_FASTBOOTDATA_RECIPE, dictionary)?.MarketName;
	}

	protected int ModelByProductFilter(object source, string product)
	{
		ManualComboboxViewModel obj = source as ManualComboboxViewModel;
		ManualComboboxViewModel manualComboboxViewModel = CbxMarketNameVM.ComboBoxSelectedValue as ManualComboboxViewModel;
		if (!((obj.Tag as DeviceModelInfoModel).MarketName == manualComboboxViewModel.ItemText))
		{
			return 1;
		}
		return 0;
	}

	protected int ProductInitFilter(object source, string keywords)
	{
		return 0;
	}

	protected int ProductSearchFilter(object source, string keywords)
	{
		ManualComboboxViewModel manualComboboxViewModel = source as ManualComboboxViewModel;
		if (string.IsNullOrEmpty(keywords))
		{
			return 0;
		}
		if (!manualComboboxViewModel.ItemText.ToLowerInvariant().Contains(keywords.ToLowerInvariant()))
		{
			return 1;
		}
		return 0;
	}

	protected override int ModelNameInitFilter(object source, string keywords)
	{
		return ModelBySmartCategoryFilter(source, keywords);
	}

	protected override int SearchFilter(object source, string keywords)
	{
		ManualComboboxViewModel manualComboboxViewModel = source as ManualComboboxViewModel;
		if (string.IsNullOrEmpty(keywords))
		{
			return ModelBySmartCategoryFilter(source, keywords);
		}
		if (manualComboboxViewModel.ItemText.ToLowerInvariant().Contains(keywords.ToLowerInvariant()))
		{
			return ModelBySmartCategoryFilter(manualComboboxViewModel, keywords);
		}
		return 1;
	}

	protected override int DefaultFilter(object source, string keywords)
	{
		return ModelBySmartCategoryFilter(source, keywords);
	}

	protected int ModelBySmartCategoryFilter(object source, string product)
	{
		ManualComboboxViewModel manualComboboxViewModel = source as ManualComboboxViewModel;
		string marketName = (manualComboboxViewModel.Tag as DeviceModelInfoModel).MarketName;
		if (CbxMarketNameVM.StepComboBoxItemSource.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText == marketName) != null)
		{
			return 0;
		}
		return 1;
	}
}
