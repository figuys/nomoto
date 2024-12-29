using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.ControlsV6;

public partial class DeviceTutorialsDialogViewV6 : Window, IUserMsgControl, IComponentConnector
{
	private class ItemDataModel
	{
		public int ID { get; set; }

		public string TextContent { get; set; }

		public Uri ImageUri { get; set; }

		public ItemDataType ItemType { get; set; }
	}

	private enum ItemDataType
	{
		Phone,
		LegionPhone,
		Tablet
	}

	private List<ItemDataModel> ListData;

	private ItemDataType _currentType;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public DeviceTutorialsDialogViewV6(bool isTabletOnly = false)
	{
		InitializeComponent();
		InitListData();
		SetListData();
		if (isTabletOnly)
		{
			radioBtnPhone.Visibility = Visibility.Collapsed;
			radioBtnTablet.IsChecked = true;
			borderTabType.Background = new SolidColorBrush(Colors.Transparent);
			DeviceTypeSelectClick(null, null);
		}
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void DeviceTypeSelectClick(object sender, RoutedEventArgs e)
	{
		if (radioBtnPhone.IsChecked == true)
		{
			phoneFrame.Visibility = Visibility.Visible;
			tabletFrame.Visibility = Visibility.Collapsed;
			spPhoneSubType.Visibility = Visibility.Visible;
			radioBtnSubPhone.IsChecked = true;
			_currentType = ItemDataType.Phone;
		}
		else if (radioBtnTablet.IsChecked == true)
		{
			phoneFrame.Visibility = Visibility.Collapsed;
			tabletFrame.Visibility = Visibility.Visible;
			spPhoneSubType.Visibility = Visibility.Collapsed;
			_currentType = ItemDataType.Tablet;
		}
		SetListData();
	}

	private void DeviceSubTypeSelectClick(object sender, RoutedEventArgs e)
	{
		if (radioBtnSubPhone.IsChecked == true)
		{
			_currentType = ItemDataType.Phone;
		}
		else if (radioBtnSubLegionPhone.IsChecked == true)
		{
			_currentType = ItemDataType.LegionPhone;
		}
		SetListData();
	}

	private void SetListData()
	{
		List<ItemDataModel> itemsSource = ListData.Where((ItemDataModel m) => m.ItemType == _currentType).ToList();
		listTutorialContent.ItemsSource = itemsSource;
		SetListCurrentData(0);
	}

	private void SetListCurrentData(int _index)
	{
		listTutorialContent.SelectedIndex = _index;
		Uri imageUri = ListData.Where((ItemDataModel m) => m.ItemType == _currentType).ToList()[_index].ImageUri;
		if (_currentType == ItemDataType.Tablet)
		{
			gifPhone.UriImageSource = null;
			gifTablet.UriImageSource = imageUri;
		}
		else
		{
			gifPhone.UriImageSource = imageUri;
			gifTablet.UriImageSource = null;
		}
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Result = false;
		Close();
		CloseAction?.Invoke(false);
	}

	private void InitListData()
	{
		_currentType = ItemDataType.Phone;
		btnPrev.IsEnabled = false;
		ListData = new List<ItemDataModel>();
		ListData.Add(new ItemDataModel
		{
			ID = 0,
			TextContent = "K1097",
			ItemType = ItemDataType.Phone,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone11.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 1,
			TextContent = "K1098",
			ItemType = ItemDataType.Phone,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone12.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 2,
			TextContent = "K1099",
			ItemType = ItemDataType.Phone,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone13.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 0,
			TextContent = "K1100",
			ItemType = ItemDataType.LegionPhone,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone21.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 1,
			TextContent = "K1101",
			ItemType = ItemDataType.LegionPhone,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone22.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 2,
			TextContent = "K1108",
			ItemType = ItemDataType.LegionPhone,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Phone/phone23.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 0,
			TextContent = "K1102",
			ItemType = ItemDataType.Tablet,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet31.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 1,
			TextContent = "K1103",
			ItemType = ItemDataType.Tablet,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet32.gif")
		});
		ListData.Add(new ItemDataModel
		{
			ID = 2,
			TextContent = "K1107",
			ItemType = ItemDataType.Tablet,
			ImageUri = new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/UsbDebug/Tablet/tablet33.gif")
		});
	}

	private void NextBtnClick(object sender, RoutedEventArgs e)
	{
		btnPrev.IsEnabled = true;
		if (listTutorialContent.SelectedIndex == 0)
		{
			SetListCurrentData(1);
		}
		else if (listTutorialContent.SelectedIndex == 1)
		{
			SetListCurrentData(2);
			btnNext.IsEnabled = false;
		}
	}

	private void PrevBtnClick(object sender, RoutedEventArgs e)
	{
		btnNext.IsEnabled = true;
		if (listTutorialContent.SelectedIndex == 2)
		{
			SetListCurrentData(1);
		}
		else if (listTutorialContent.SelectedIndex == 1)
		{
			SetListCurrentData(0);
			btnPrev.IsEnabled = false;
		}
	}

	private void listTutorialContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		int num = listTutorialContent.SelectedIndex;
		if (num < 0)
		{
			num = 0;
		}
		btnPrev.IsEnabled = true;
		btnNext.IsEnabled = true;
		switch (num)
		{
		case 0:
			btnPrev.IsEnabled = false;
			break;
		case 2:
			btnNext.IsEnabled = false;
			break;
		}
		SetListCurrentData(num);
	}
}
