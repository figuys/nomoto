using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class PhoneMMatchViewModelV6 : ManualMatchViewModel
{
	private int _Current;

	private readonly PhoneMMatchViewV6 _View;

	private bool _IsBtnPreEnable;

	private bool _IsBtnNextEnable;

	private string _StepTitle;

	public bool IsBtnPreEnable
	{
		get
		{
			return _IsBtnPreEnable;
		}
		set
		{
			_IsBtnPreEnable = value;
			OnPropertyChanged("IsBtnPreEnable");
		}
	}

	public bool IsBtnNextEnable
	{
		get
		{
			return _IsBtnNextEnable;
		}
		set
		{
			_IsBtnNextEnable = value;
			OnPropertyChanged("IsBtnNextEnable");
		}
	}

	public string StepTitle
	{
		get
		{
			return _StepTitle;
		}
		set
		{
			_StepTitle = value;
			OnPropertyChanged("StepTitle");
		}
	}

	public PhoneMMatchViewModelV6(PhoneMMatchViewV6 ui)
		: base(ui, DevCategory.Phone)
	{
		_View = ui;
		base.CbxModelNameVM.DropDownOpenedChanged = delegate(bool isOpen)
		{
			if (!(base.CbxModelNameVM.StepComboBoxItemSource == null || isOpen) && (base.CbxModelNameVM.ComboBoxSelectedValue == null || !(base.CbxModelNameVM.ComboBoxSelectedValue as ManualComboboxViewModel).IsMore))
			{
				base.CbxModelNameVM.ComboBoxFilter = ModelNameInitFilter;
				base.CbxModelNameVM.ComboBoxMoreButtonVisibility = _ModelMoreBtnVisible;
			}
		};
		_ModelNameTipsArr = new List<Tuple<string, int, string>>
		{
			new Tuple<string, int, string>("K1084", 1, "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/ModelName1.png"),
			new Tuple<string, int, string>("K1004", 2, "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/ModelName2.png"),
			new Tuple<string, int, string>("K1005", 3, "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/ModelName3.png"),
			new Tuple<string, int, string>("K1259", 0, "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/phonepoweroff.png")
		};
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.MouseEnterCommand, OnMouseEnter));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.SetTopClickCommand, OnSetTopClicked));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.ComboBoxTextChangedCommand, OnModelNameTextChanged));
		ui.CommandBindings.Add(new CommandBinding(base.CbxModelNameVM.ComboBoxMoreButtonCommand, base.OnModelNameMoreBtnClicked));
		ShowTutorial();
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
								ManualComboboxViewModel manualComboboxViewModel = base.CbxModelNameVM.StepComboBoxItemSource?.FirstOrDefault((ManualComboboxViewModel n) => (n.Tag as DeviceModelInfoModel).ModelName.Equals(modelName));
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

	public override void ChangeConfirmVisibile(Visibility visibile)
	{
		_View.ChangeConfirmVisibile(visibile);
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

	public void Previous()
	{
		if (--_Current <= 0)
		{
			IsBtnPreEnable = false;
		}
		IsBtnNextEnable = true;
		SetTutorial(_Current);
	}

	public void Next()
	{
		if (++_Current >= 2)
		{
			IsBtnNextEnable = false;
		}
		IsBtnPreEnable = true;
		SetTutorial(_Current);
	}

	public void ShowTutorial(bool isModelName = true)
	{
		_Current = 0;
		IsBtnPreEnable = false;
		IsBtnNextEnable = true;
		SetTutorial((!isModelName) ? 3 : 0);
	}

	private void SetTutorial(int index)
	{
		base.HelpImage = _ModelNameTipsArr[index].Item3;
		if (_ModelNameTipsArr[index].Item2 > 0)
		{
			StepTitle = $"Step{_ModelNameTipsArr[index].Item2}:";
		}
		else
		{
			StepTitle = null;
		}
		base.AndroidSettingText = _ModelNameTipsArr[index].Item1;
	}

	protected void OnModelNameTextChanged(object sender, ExecutedRoutedEventArgs e)
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

	protected override void ResetForModelName()
	{
		_ReqParams.Clear();
		base.ResetForModelName();
	}

	private void OnMouseEnter(object sender, ExecutedRoutedEventArgs e)
	{
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
}
