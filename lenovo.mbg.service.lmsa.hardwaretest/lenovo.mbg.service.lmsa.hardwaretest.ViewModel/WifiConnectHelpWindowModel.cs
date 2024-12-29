using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.hardwaretest.ViewModel;

public class WifiConnectHelpWindowModel : ViewModelBase
{
	private bool _IsPrevBtnEnable;

	private bool _IsNextBtnEnable = true;

	private ImageSource _qrImageSource;

	private string _EncryptCode = RandomAesKeyHelper.Instance.EncryptCode;

	public ReplayCommand CloseCommand { get; }

	public ReplayCommand NextCommand { get; }

	public ReplayCommand PreviousCommand { get; }

	public MainFrameViewModel MainFrame => Context.MainFrame;

	public ObservableCollection<StepViewModel> Steps { get; set; }

	public bool IsPrevBtnEnable
	{
		get
		{
			return _IsPrevBtnEnable;
		}
		set
		{
			_IsPrevBtnEnable = value;
			OnPropertyChanged("IsPrevBtnEnable");
		}
	}

	public bool IsNextBtnEnable
	{
		get
		{
			return _IsNextBtnEnable;
		}
		set
		{
			_IsNextBtnEnable = value;
			OnPropertyChanged("IsNextBtnEnable");
		}
	}

	public ImageSource QrImageSource
	{
		get
		{
			return _qrImageSource;
		}
		set
		{
			_qrImageSource = value;
			OnPropertyChanged("QrImageSource");
		}
	}

	public string EncryptCode
	{
		get
		{
			return _EncryptCode;
		}
		set
		{
			_EncryptCode = value;
			OnPropertyChanged("EncryptCode");
		}
	}

	public WifiConnectHelpWindowModel()
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		NextCommand = new ReplayCommand(NextCommandHandler);
		PreviousCommand = new ReplayCommand(PreviousCommandHandler);
		Steps = new ObservableCollection<StepViewModel>();
		Steps.Add(new StepViewModel
		{
			FirstTitle = "1.",
			Content = "K1477",
			TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/Step1.png")),
			IsSelected = true
		});
		Steps.Add(new StepViewModel
		{
			FirstTitle = "2.",
			Content = "K1479",
			TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/Step2.png"))
		});
		Steps.Add(new StepViewModel
		{
			FirstTitle = "3.",
			Content = "K1480",
			TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/Step3.png"))
		});
		Steps.Add(new StepViewModel
		{
			FirstTitle = "4.",
			Content = "K1481",
			TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/Step4.png"))
		});
		Steps.Add(new StepViewModel
		{
			FirstTitle = "5.",
			Content = "K1482",
			TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/Step5.png"))
		});
		GenerateQrCode();
	}

	private void CloseCommandHandler(object data)
	{
		(data as Window)?.Close();
	}

	private void NextCommandHandler(object data)
	{
		if (data != null)
		{
			StepViewModel stepViewModel = data as StepViewModel;
			int index = Steps.IndexOf(stepViewModel);
			StepViewModel nextStep = GetNextStep(index);
			if (nextStep != null)
			{
				int num = Steps.IndexOf(nextStep);
				IsNextBtnEnable = num < Steps.Count - 1;
				IsPrevBtnEnable = true;
				stepViewModel.IsSelected = false;
				nextStep.IsSelected = true;
			}
		}
	}

	private void PreviousCommandHandler(object data)
	{
		StepViewModel stepViewModel = data as StepViewModel;
		int index = Steps.IndexOf(stepViewModel);
		StepViewModel prevStep = GetPrevStep(index);
		int num = Steps.IndexOf(prevStep);
		IsPrevBtnEnable = num > 0;
		IsNextBtnEnable = true;
		stepViewModel.IsSelected = false;
		prevStep.IsSelected = true;
	}

	public StepViewModel GetNextStep(int index)
	{
		StepViewModel result = null;
		if (index < Steps.Count - 1)
		{
			result = Steps[++index];
		}
		return result;
	}

	public StepViewModel GetPrevStep(int index)
	{
		StepViewModel result = null;
		if (index > 0)
		{
			result = Steps.Take(index).LastOrDefault();
		}
		return result;
	}

	private void GenerateQrCode()
	{
		MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(HostProxy.LanguageService.IsChinaRegionAndLanguage() ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
		bitmapImage.EndInit();
		try
		{
			QrImageSource = bitmapImage;
		}
		catch (Exception)
		{
		}
	}
}
