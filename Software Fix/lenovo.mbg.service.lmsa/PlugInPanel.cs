using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using GoogleAnalytics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.hostcontroller;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa;

public partial class PlugInPanel : UserControl, IComponentConnector
{
	private int? lastSelectedIndex = null;

	private PlugInPanelModel selectedItem;

	private object m_pluginData;

	public PlugInPanelModel upgradePlugIn { get; set; }

	public PlugInPanelModel TargetPlugInPanelModel { get; set; }

	public PlugInPanel()
	{
		InitializeComponent();
		InitializeSource();
		ApplcationClass.PlugInPanel = this;
	}

	private void InitializeSource()
	{
		listBoxMain.SelectionChanged += listBoxMain_SelectionChanged;
	}

	public void LoadPlugins(IntPtr handle)
	{
		List<PluginInfo> list = MainWindowControl.Instance.LoadPlguinInfo();
		int num = 0;
		PluginContainer.Instance.Init((from n in list
			where !string.IsNullOrEmpty(n.PluginID)
			select n.PluginDir).ToList());
		foreach (PluginInfo item in list)
		{
			item.DisplayName = item.PluginName;
			item.PluginName = item.PluginName;
			PlugInPanelModel plugInPanelModel = new PlugInPanelModel();
			plugInPanelModel.IsInstall = item.Install;
			bool isHasNewViserion = item.haveNewVersion && item.Install;
			plugInPanelModel.IsHasNewViserion = isHasNewViserion;
			plugInPanelModel.TargetPluginInfo = item;
			plugInPanelModel.ShowIndex = num++;
			if (!string.IsNullOrEmpty(item.PluginIconPath))
			{
				Uri uriSource = new Uri("pack://application:,,,/Software Fix;component/Resource/PluginIcons/" + item.PluginIconPath + "_white.png");
				plugInPanelModel.IconImageSource = new BitmapImage(uriSource);
				Uri uriSource2 = new Uri("pack://application:,,,/Software Fix;component/Resource/PluginIcons/" + item.PluginIconPath + ".png");
				plugInPanelModel.IconImageSourceSelected = new BitmapImage(uriSource2);
			}
			listBoxMain.Items.Add(plugInPanelModel);
		}
		listBoxMain.SelectedIndex = 0;
	}

	private void SelecteFirstPlugin()
	{
		if (listBoxMain.Items.Count > 0)
		{
			listBoxMain.SelectedIndex = 0;
		}
	}

	private void listBoxMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (listBoxMain.SelectedItem == null)
		{
			return;
		}
		if (MainWindowControl.Instance.IsExecuteWork())
		{
			listBoxMain.SelectedItem = selectedItem;
			return;
		}
		listBoxMain.IsEnabled = false;
		if (ApplcationClass.isAddPlugIning)
		{
			e.Handled = false;
			LogHelper.LogInstance.Info("lenovo.mbg.service.lmsa.PlugInPanel.listBoxMain_SelectionChanged: Plugin is adding....  TargetPluginName:" + ApplcationClass.PlugInPanelModelCurrentSelected.TargetPluginInfo.PluginName);
			if (lastSelectedIndex.HasValue && listBoxMain.SelectedIndex != lastSelectedIndex.Value)
			{
				listBoxMain.SelectedIndex = lastSelectedIndex.Value;
				MessageBox_Common msgAddBusy = new MessageBox_Common(ApplcationClass.ApplcationStartWindow, TypeItems.MessageBoxType.OK, "K0346", "K0327", "");
				HostProxy.HostMaskLayerWrapper.New(msgAddBusy, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
				{
					msgAddBusy.ShowDialog();
				});
			}
			listBoxMain.IsEnabled = true;
			return;
		}
		foreach (PlugInPanelModel item in (IEnumerable)listBoxMain.Items)
		{
			if (item.IsChecked)
			{
				item.IsChecked = false;
				break;
			}
		}
		selectedItem = listBoxMain.SelectedItem as PlugInPanelModel;
		if (string.IsNullOrEmpty(selectedItem.TargetPluginInfo.PluginID))
		{
			foreach (PlugInPanelModel item2 in (IEnumerable)listBoxMain.Items)
			{
				if (item2.TargetPluginInfo.PluginID == "02928af025384c75ae055aa2d4f256c8")
				{
					item2.IsChecked = true;
					item2.IsValid = true;
					selectedItem = item2;
					m_pluginData = "lmsa-plugin-Device-backuprestore";
					listBoxMain.SelectedItem = item2;
					DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
					if (masterDevice != null && masterDevice.ConnectType == ConnectType.Wifi)
					{
						MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, "K1331", MessageBoxButton.OK);
					}
					return;
				}
			}
		}
		else
		{
			selectedItem.IsChecked = true;
			selectedItem.IsValid = true;
		}
		StartTargetPlugin();
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, selectedItem.TargetPluginInfo.PluginName + "Click", "plugin " + selectedItem.TargetPluginInfo.PluginName + " click", 0L).Build());
	}

	private void StartTargetPlugin()
	{
		ApplcationClass.PlugInPanelModelCurrentSelected = (PlugInPanelModel)listBoxMain.SelectedItem;
		if (!ApplcationClass.PlugInPanelModelCurrentSelected.IsValid)
		{
			listBoxMain.Items.Remove(ApplcationClass.PlugInPanelModelCurrentSelected);
			SelecteFirstPlugin();
		}
		object pluginData = m_pluginData;
		ApplcationClass.AddFreamElementToUI(pluginData);
		m_pluginData = null;
	}

	private void Completed(object sender, EventArgs e)
	{
		PlugInPanelModel plugInPanelModel = (PlugInPanelModel)listBoxMain.SelectedItem;
		plugInPanelModel.IsChecked = true;
	}

	public void SwitchTo(string pluginID)
	{
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			PlugInPanelModel plugInPanelModel = null;
			foreach (PlugInPanelModel item in (IEnumerable)listBoxMain.Items)
			{
				if (item.TargetPluginInfo.PluginID.Equals(pluginID))
				{
					plugInPanelModel = item;
					break;
				}
			}
			if (plugInPanelModel != null)
			{
				listBoxMain.SelectedItem = plugInPanelModel;
			}
		});
	}

	public void SwitchTo(string pluginID, object data)
	{
		m_pluginData = data;
		SwitchTo(pluginID);
	}
}
