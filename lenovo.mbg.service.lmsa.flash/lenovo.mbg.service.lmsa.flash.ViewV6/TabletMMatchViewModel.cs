using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class TabletMMatchViewModel : ManualMatchViewModel
{
	protected bool _IsModelFirst;

	protected FrameworkElement _View;

	public LComboBoxViewModelV6 CbxMarketNameVM { get; protected set; }

	public TabletMMatchViewModel(FrameworkElement ui, DevCategory category)
		: base(ui, category)
	{
		_View = ui;
		_Category = category;
		base.CbxModelNameVM.DropDownOpenedChanged = delegate(bool isOpen)
		{
			if (!(base.CbxModelNameVM.StepComboBoxItemSource == null || isOpen) && (base.CbxModelNameVM.ComboBoxSelectedValue == null || !(base.CbxModelNameVM.ComboBoxSelectedValue as ManualComboboxViewModel).IsMore))
			{
				base.CbxModelNameVM.ComboBoxFilter = ModelNameInitFilter;
				base.CbxModelNameVM.ComboBoxMoreButtonVisibility = _ModelMoreBtnVisible;
			}
		};
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.SetTopClickCommand, OnSetTopClicked));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.ComboBoxTextChangedCommand, OnModelNameTextChanged));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.ComboBoxMoreButtonCommand, base.OnModelNameMoreBtnClicked));
		ShowTutorial(isModelName: true);
		Task.Run(delegate
		{
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
	}

	public void MatchFromDownloadCenter(Dictionary<string, string> data)
	{
		_CoditionMap = data;
		if (base.CbxModelNameVM.StepComboBoxItemSource != null)
		{
			AutoMatchByCoditonMap();
		}
	}

	private void AutoMatchByCoditonMap()
	{
		if (_CoditionMap.ContainsKey("modelName"))
		{
			ManualComboboxViewModel manualComboboxViewModel = base.CbxModelNameVM.StepComboBoxItemSource?.FirstOrDefault((ManualComboboxViewModel p) => (p.Tag as DeviceModelInfoModel).ModelName == _CoditionMap["modelName"]);
			if (manualComboboxViewModel == null)
			{
				_CoditionMap = null;
				MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K0098", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
				ResetCoditionSelUi();
			}
			else
			{
				ResetCoditionSelUi();
				base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
				base.CbxModelNameVM.ComboBoxFilter = DefaultFilter;
				base.CbxModelNameVM.IsDropDownEnabled = false;
				base.CbxModelNameVM.ComboBoxSelectedValue = manualComboboxViewModel;
			}
		}
	}

	public override void ChangeConfirmVisibile(Visibility visibile)
	{
		if (_View is TabletMMatchViewV6 tabletMMatchViewV)
		{
			tabletMMatchViewV.ChangeConfirmVisibile(visibile);
		}
		else
		{
			(_View as SmartMMatchViewV6).ChangeConfirmVisibile(visibile);
		}
	}

	public void ShowTutorial(bool isModelName)
	{
		if (isModelName)
		{
			base.HelpImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/tModelName.png";
			base.AndroidSettingText = "K1086";
		}
		else
		{
			base.HelpImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/tPowerOff.png";
			base.AndroidSettingText = "K1259";
		}
	}

	protected virtual void OnModelNameTextChanged(object sender, ExecutedRoutedEventArgs e)
	{
		base.CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		if (base.CbxModelNameVM.ComboBoxSelectedValue == null && base.CbxModelNameVM.ComboBoxSelectedIndex == -1)
		{
			base.CbxModelNameVM.ComboBoxFilter = SearchFilter;
		}
		else
		{
			base.CbxModelNameVM.ComboBoxFilter = DefaultFilter;
		}
	}

	private void OnSetTopClicked(object sender, ExecutedRoutedEventArgs e)
	{
		ManualComboboxViewModel unStop = e.Parameter as ManualComboboxViewModel;
		unStop.IsUsed = false;
		List<ManualComboboxViewModel> list = LoadRescuedDevice($"$.modelname{_Category}");
		int num = list.FindIndex((ManualComboboxViewModel n) => n.ItemText == unStop.ItemText);
		if (num != -1)
		{
			list.RemoveAt(num);
			FileHelper.WriteJsonWithAesEncrypt(Configurations.RescueManualMatchFile, $"modelname{_Category}", list, async: true);
		}
	}

	protected override void ResetForModelName()
	{
		_ReqParams.Clear();
		_ReqParams.AddParameter("countryCode", GlobalFun.GetRegionInfo().TwoLetterISORegionName);
		base.ResetForModelName();
	}
}
