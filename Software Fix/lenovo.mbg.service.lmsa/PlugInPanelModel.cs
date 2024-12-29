using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa;

public partial class PlugInPanelModel : UserControl, IComponentConnector
{
	private BitmapImage imgSourceUnInstall = new BitmapImage(new Uri("Pack://application:,,,/Software Fix;Component/Resource/AssistantPictures/uninstall.png"));

	private PluginInfo targetPluginInfo = null;

	private bool isCancelUpdate = false;

	public static readonly DependencyProperty strTitleProperty = DependencyProperty.Register("strTitle", typeof(string), typeof(PlugInPanelModel), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty ucHeightProperty = DependencyProperty.Register("ucHeight", typeof(double), typeof(PlugInPanelModel), new PropertyMetadata(0.0));

	public static readonly DependencyProperty ucWidthProperty = DependencyProperty.Register("ucWidth", typeof(double), typeof(PlugInPanelModel), new PropertyMetadata(0.0));

	public static readonly DependencyProperty ucImageHeightProperty = DependencyProperty.Register("ucImageHeight", typeof(double), typeof(PlugInPanelModel), new PropertyMetadata(0.0));

	public static readonly DependencyProperty ucImageWidthProperty = DependencyProperty.Register("ucImageWidth", typeof(double), typeof(PlugInPanelModel), new PropertyMetadata(0.0));

	public static readonly DependencyProperty ucPictureProperty = DependencyProperty.Register("ucPicture", typeof(BitmapImage), typeof(PlugInPanelModel));

	public static readonly DependencyProperty ucPictureWhiteProperty = DependencyProperty.Register("ucPictureWhite", typeof(BitmapImage), typeof(PlugInPanelModel));

	public static readonly DependencyProperty TitleForeBrushProperty = DependencyProperty.Register("TitleForeBrush", typeof(SolidColorBrush), typeof(PlugInPanelModel));

	public static readonly DependencyProperty ucBackgroundProperty = DependencyProperty.Register("ucBackground", typeof(SolidColorBrush), typeof(PlugInPanelModel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

	public static readonly DependencyProperty SelectedItemBackgroundOpactiyProperty = DependencyProperty.Register("SelectedItemBackgroundOpactiy", typeof(double), typeof(PlugInPanelModel), new PropertyMetadata(0.0));

	public static readonly DependencyProperty IconImageSourceProperty = DependencyProperty.Register("IconImageSource", typeof(ImageSource), typeof(PlugInPanelModel), new PropertyMetadata(null));

	public static readonly DependencyProperty IconImageSourceSelectedProperty = DependencyProperty.Register("IconImageSourceSelected", typeof(ImageSource), typeof(PlugInPanelModel), new PropertyMetadata(null));

	public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(PlugInPanelModel), new PropertyMetadata(false, delegate
	{
	}));

	public static readonly DependencyProperty CheckedBackgroundProperty = DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(PlugInPanelModel), new PropertyMetadata(null));

	private bool isHasNewViserion = false;

	private bool isInstall = false;

	public static readonly DependencyProperty PropValueProperty = DependencyProperty.Register("PropValue", typeof(double), typeof(PlugInPanelModel), new PropertyMetadata(0.0));

	public bool _IsValid = true;

	public PluginInfo TargetPluginInfo
	{
		get
		{
			return targetPluginInfo;
		}
		set
		{
			targetPluginInfo = value;
			strTitle = targetPluginInfo.DisplayName;
			IsChecked = false;
		}
	}

	public bool IsCancelUpdate
	{
		get
		{
			return isCancelUpdate;
		}
		set
		{
			isCancelUpdate = value;
			if (isCancelUpdate)
			{
				ApplcationClass.UpdateTip.ShowUpdateTipWindow();
			}
			else
			{
				ApplcationClass.UpdateTip.HideUpdateTipWindow();
			}
		}
	}

	public string strTitle
	{
		get
		{
			return (string)GetValue(strTitleProperty);
		}
		set
		{
			SetValue(strTitleProperty, value);
		}
	}

	public double ucHeight
	{
		get
		{
			return (double)GetValue(ucHeightProperty);
		}
		set
		{
			SetValue(ucHeightProperty, value);
		}
	}

	public double ucWidth
	{
		get
		{
			return (double)GetValue(ucWidthProperty);
		}
		set
		{
			SetValue(ucWidthProperty, value);
		}
	}

	public double ucImageHeight
	{
		get
		{
			return (double)GetValue(ucImageHeightProperty);
		}
		set
		{
			SetValue(ucImageHeightProperty, value);
		}
	}

	public double ucImageWidth
	{
		get
		{
			return (double)GetValue(ucImageWidthProperty);
		}
		set
		{
			SetValue(ucImageWidthProperty, value);
		}
	}

	public BitmapImage ucPicture
	{
		get
		{
			return (BitmapImage)GetValue(ucPictureProperty);
		}
		set
		{
			SetValue(ucPictureProperty, value);
		}
	}

	public BitmapImage ucPictureWhite
	{
		get
		{
			return (BitmapImage)GetValue(ucPictureWhiteProperty);
		}
		set
		{
			SetValue(ucPictureWhiteProperty, value);
		}
	}

	public SolidColorBrush TitleForeBrush
	{
		get
		{
			return (SolidColorBrush)GetValue(TitleForeBrushProperty);
		}
		set
		{
			SetValue(TitleForeBrushProperty, value);
		}
	}

	public SolidColorBrush ucBackground
	{
		get
		{
			return (SolidColorBrush)GetValue(ucBackgroundProperty);
		}
		set
		{
			SetValue(ucBackgroundProperty, value);
		}
	}

	public double SelectedItemBackgroundOpactiy
	{
		get
		{
			return (double)GetValue(SelectedItemBackgroundOpactiyProperty);
		}
		set
		{
			SetValue(SelectedItemBackgroundOpactiyProperty, value);
		}
	}

	public int ShowIndex { get; set; }

	public ImageSource IconImageSource
	{
		get
		{
			return (ImageSource)GetValue(IconImageSourceProperty);
		}
		set
		{
			SetValue(IconImageSourceProperty, value);
		}
	}

	public ImageSource IconImageSourceSelected
	{
		get
		{
			return (ImageSource)GetValue(IconImageSourceSelectedProperty);
		}
		set
		{
			SetValue(IconImageSourceSelectedProperty, value);
		}
	}

	public bool IsChecked
	{
		get
		{
			return (bool)GetValue(IsCheckedProperty);
		}
		set
		{
			SetValue(IsCheckedProperty, value);
		}
	}

	public Brush CheckedBackground
	{
		get
		{
			return (Brush)GetValue(CheckedBackgroundProperty);
		}
		set
		{
			SetValue(CheckedBackgroundProperty, value);
		}
	}

	public bool IsHasNewViserion
	{
		get
		{
			return isHasNewViserion;
		}
		set
		{
			isHasNewViserion = value;
		}
	}

	public bool IsInstall
	{
		get
		{
			return isInstall;
		}
		set
		{
			isInstall = value;
			if (isInstall)
			{
				ExecuteInstallComplate();
			}
		}
	}

	public double PropValue
	{
		get
		{
			return (double)GetValue(PropValueProperty);
		}
		set
		{
			SetValue(PropValueProperty, value);
		}
	}

	public bool IsValid
	{
		get
		{
			return _IsValid;
		}
		set
		{
			_IsValid = value;
		}
	}

	public PlugInPanelModel()
	{
		InitializeComponent();
		base.Loaded += PlugInPanelModel_Loaded;
	}

	private void PlugInPanelModel_Loaded(object sender, RoutedEventArgs e)
	{
		PropValue = 10.0;
	}

	private double GetBackgroundOpactiy(int showIndex)
	{
		if (showIndex <= 0)
		{
			return 0.3;
		}
		if (showIndex >= 7)
		{
			return 1.0;
		}
		return (double)(3 + showIndex) * 0.1;
	}

	private void ExecuteInstallComplate()
	{
	}
}
