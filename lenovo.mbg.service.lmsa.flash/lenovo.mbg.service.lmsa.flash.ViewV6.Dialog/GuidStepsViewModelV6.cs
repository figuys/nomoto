using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class GuidStepsViewModelV6 : BaseGuidStepsViewModelV6
{
	private bool ExistsStartPage;

	private bool ShowXt2;

	private bool LinkClick;

	private string Title;

	private List<ConnectStepsModel> CacheList;

	private Visibility notifyTextVisible = Visibility.Collapsed;

	public static Dictionary<string, List<ConnectStepsModel>> DefalutStepMapp = new Dictionary<string, List<ConnectStepsModel>>
	{
		{
			"FASTBOOT_DEFALUT",
			new List<ConnectStepsModel>
			{
				new ConnectStepsModel
				{
					Index = 1,
					Layout = "V",
					Image = BaseGuidStepsViewModelV6.ConvertImageUrl("fastboot-guide-01.gif"),
					Content = "K0913"
				},
				new ConnectStepsModel
				{
					Index = 2,
					Layout = "V",
					Image = BaseGuidStepsViewModelV6.ConvertImageUrl("fastboot-guide-02.gif"),
					Content = "K0914"
				},
				new ConnectStepsModel
				{
					Index = 3,
					Layout = "V",
					Image = BaseGuidStepsViewModelV6.ConvertImageUrl("fastboot-guide-03.gif"),
					Content = "K1008"
				}
			}
		},
		{
			"XT2",
			new List<ConnectStepsModel>
			{
				new ConnectStepsModel
				{
					Index = 1,
					Layout = "V",
					Image = BaseGuidStepsViewModelV6.ConvertImageUrl("fastboot-guide-01.gif"),
					Content = "K0913"
				},
				new ConnectStepsModel
				{
					Index = 2,
					Layout = "V",
					Image = BaseGuidStepsViewModelV6.ConvertImageUrl("fastboot-guide-05.gif"),
					Content = "K1764"
				}
			}
		}
	};

	public static ConnectStepsModel DefaultStartPage = new ConnectStepsModel
	{
		Index = 0,
		Layout = "S",
		Image = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/ConnFastboot.png",
		Content = "K1585"
	};

	public string LinkeModelName { get; set; }

	public ReplayCommand LinkCommand { get; }

	public Visibility NotifyTextVisible
	{
		get
		{
			return notifyTextVisible;
		}
		set
		{
			notifyTextVisible = value;
			OnPropertyChanged("NotifyTextVisible");
		}
	}

	public GuidStepsViewModelV6(GuidStepsViewV6 ui, bool autoPlay = false, bool popupWhenClose = false)
		: base(ui, autoPlay, 5000.0, showPlayControl: true, showClose: true, popupWhenClose)
	{
		LinkeModelName = HostProxy.LanguageService.Translate("K1453");
		LinkCommand = new ReplayCommand(delegate
		{
			FireLinkCommand();
		});
	}

	public BaseGuidStepsViewModelV6 Init(string modelname, bool showStartPage)
	{
		NotifyTextVisible = (MainFrameV6.Instance.IsChinaUs() ? Visibility.Collapsed : Visibility.Visible);
		if (showStartPage)
		{
			ExistsStartPage = true;
			base.StepModelList.Add(DefaultStartPage);
		}
		bool flag = false;
		if (string.IsNullOrEmpty(modelname) || modelname.Equals("UnKnown", StringComparison.CurrentCultureIgnoreCase))
		{
			ShowXt2 = true;
		}
		else if (Regex.Matches(LinkeModelName, "(?is)(?<=\\()[^\\)]+(?=\\))").Cast<Match>().Count((Match n) => modelname.Contains(n.Value)) > 0)
		{
			flag = true;
		}
		if (flag)
		{
			base.StepModelList.AddRange(DefalutStepMapp["XT2"]);
		}
		else
		{
			base.StepModelList.AddRange(DefalutStepMapp["FASTBOOT_DEFALUT"]);
		}
		Title = (ExistsStartPage ? "K1584" : "K1013");
		ShowXt2Link(ShowXt2);
		return Ready();
	}

	protected override void FirePrevCommand()
	{
		if (ExistsStartPage && base.CurrentStep.Index - 1 == base.MinIndex)
		{
			Title = "K1584";
		}
		base.FirePrevCommand();
		ShowXt2Link(ShowXt2 && base.CurrentStep.Index == base.MinIndex);
		if (LinkClick && base.CurrentStep.Index == base.MinIndex)
		{
			base.StepModelList.Clear();
			base.StepModelList = CacheList;
			LinkClick = false;
		}
	}

	protected override void FireNextCommand()
	{
		if (ExistsStartPage && base.CurrentStep.Index + 1 > base.MinIndex)
		{
			Title = "K1014";
		}
		base.FireNextCommand();
		ShowXt2Link(ShowXt2 && base.CurrentStep.Index == base.MinIndex);
	}

	protected virtual void FireLinkCommand()
	{
		LinkClick = true;
		CacheList = new List<ConnectStepsModel>(base.StepModelList);
		base.StepModelList.Clear();
		base.StepModelList.Add(base.CurrentStep);
		base.StepModelList.AddRange(DefalutStepMapp["XT2"]);
		FireNextCommand();
	}

	protected override void FirePlayAgainCommand()
	{
		if (LinkClick)
		{
			base.StepModelList.Clear();
			base.StepModelList = CacheList;
			LinkClick = false;
		}
		Title = (ExistsStartPage ? "K1584" : "K1013");
		base.FirePlayAgainCommand();
		ShowXt2Link(ShowXt2 && base.CurrentStep.Index == base.MinIndex);
	}

	protected override void ChangeTitle()
	{
		base.GuideTitle = Title;
	}

	protected void ShowXt2Link(bool show)
	{
		base.XT2Visibility = ((!show || !ExistsStartPage) ? Visibility.Collapsed : Visibility.Visible);
	}
}
