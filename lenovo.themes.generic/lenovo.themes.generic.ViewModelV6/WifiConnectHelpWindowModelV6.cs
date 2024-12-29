using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.ViewModelV6;

public class WifiConnectHelpWindowModelV6 : ViewModelBase
{
	private bool _IsPrevBtnEnable;

	private bool _IsNextBtnEnable = true;

	private ImageSource _qrImageSource;

	private ImageSource _qrWifiImageSource;

	public ReplayCommand CloseCommand { get; }

	public ReplayCommand NextCommand { get; }

	public ReplayCommand PreviousCommand { get; }

	public WifiTutorialsType TutorialsType { get; set; }

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

	public ImageSource QrWifiImageSource
	{
		get
		{
			return _qrWifiImageSource;
		}
		set
		{
			_qrWifiImageSource = value;
			OnPropertyChanged("QrWifiImageSource");
		}
	}

	public WifiConnectHelpWindowModelV6(WifiTutorialsType tutorialsType = WifiTutorialsType.RESCUE_PHONE)
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		NextCommand = new ReplayCommand(NextCommandHandler);
		PreviousCommand = new ReplayCommand(PreviousCommandHandler);
		Steps = new ObservableCollection<StepViewModel>();
		TutorialsType = tutorialsType;
		switch (tutorialsType)
		{
		case WifiTutorialsType.HWTEST:
			Steps.Add(new StepViewModel
			{
				FirstTitle = "1.",
				Content = "K1477",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step1.png")),
				IsSelected = true
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "2.",
				Content = "K1571",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step4.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "3.",
				Content = "K1573",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step5.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "4.",
				Content = "K1572",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Wifi_Step4.png"))
			});
			break;
		case WifiTutorialsType.RESCUE_TABLET_DEBUG:
			Steps.Add(new StepViewModel
			{
				FirstTitle = "1.",
				Content = "K1751",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Step1.png")),
				IsSelected = true
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "2.",
				Content = "K1752",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Step2.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "3.",
				Content = "K1753",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Step3.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "4.",
				Content = "K1754",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Step4.png"))
			});
			break;
		case WifiTutorialsType.RESCUE_TABLET_WIFI:
			Steps.Add(new StepViewModel
			{
				FirstTitle = "1.",
				Content = "K1779",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step1.png")),
				IsSelected = true
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "2.",
				Content = "K1775",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Wifi_Step2.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "3.",
				Content = "K1776",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Wifi_Step3.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "4.",
				Content = "K1777",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Tablet_Wifi_Step4.png"))
			});
			break;
		default:
			Steps.Add(new StepViewModel
			{
				FirstTitle = "1.",
				Content = "K1477",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step1.png")),
				IsSelected = true
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "2.",
				Content = "K1479",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step2.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "3.",
				Content = "K1480",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step3.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "4.",
				Content = "K1481",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step4.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "5.",
				Content = "K1482",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step5.png"))
			});
			Steps.Add(new StepViewModel
			{
				FirstTitle = "6.",
				Content = "K1773",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/wifi_connect/Step6.png"))
			});
			break;
		}
		GenerateQrCode();
		global::Smart.DeviceManagerEx.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
		OnWifiMonitoringEndPointChanged(global::Smart.DeviceManagerEx.WirelessWaitForConnectEndPoints);
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
				nextStep.IsSelected = true;
				stepViewModel.IsSelected = false;
				int num = Steps.IndexOf(nextStep);
				IsNextBtnEnable = num < Steps.Count - 1;
				IsPrevBtnEnable = true;
			}
		}
	}

	private void PreviousCommandHandler(object data)
	{
		if (data != null)
		{
			StepViewModel stepViewModel = data as StepViewModel;
			int index = Steps.IndexOf(stepViewModel);
			StepViewModel prevStep = GetPrevStep(index);
			if (prevStep != null)
			{
				prevStep.IsSelected = true;
				stepViewModel.IsSelected = false;
				int num = Steps.IndexOf(prevStep);
				IsPrevBtnEnable = num > 0;
				IsNextBtnEnable = true;
			}
		}
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
		MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(global::Smart.LanguageService.IsChinaRegionAndLanguage() ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
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

	private void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> underWatchEndPoints)
	{
		if (underWatchEndPoints == null)
		{
			return;
		}
		StringBuilder sb = new StringBuilder();
		sb.Append($"V:{Configurations.AppVersionCode}").Append(Environment.NewLine);
		sb.Append($"DV:{Configurations.AppMinVersionCodeOfMoto}").Append(Environment.NewLine);
		foreach (Tuple<string, string> underWatchEndPoint in underWatchEndPoints)
		{
			sb.Append("IP:").Append(underWatchEndPoint.Item1).Append(Environment.NewLine);
		}
		try
		{
			Application.Current?.Dispatcher.BeginInvoke((Action)delegate
			{
				MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(sb.ToString());
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
				bitmapImage.EndInit();
				try
				{
					QrWifiImageSource = bitmapImage;
				}
				catch (Exception)
				{
				}
			});
		}
		catch (Exception)
		{
		}
	}
}
