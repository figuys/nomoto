using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class FileViewV7 : System.Windows.Controls.UserControl, IComponentConnector, IStyleConnector
{
	public ICommand LoadedCommand { get; }

	public ICommand OnCheckedCommand { get; }

	private FileViewModelV7 GetCurrentContext
	{
		get
		{
			FileViewModelV7 _dataContext = null;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				_dataContext = base.DataContext as FileViewModelV7;
			});
			return _dataContext;
		}
	}

	public FileViewV7()
	{
		InitializeComponent();
		OnCheckedCommand = new RelayCommand<string>(OnChecked);
		base.Loaded += delegate
		{
			sdview.Cbx.SelectionChanged -= Cbx_SelectionChanged;
			sdview.Cbx.SelectionChanged += Cbx_SelectionChanged;
			if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto" && GetCurrentContext != null)
			{
				GetCurrentContext.StorageSelectPanelVisibility = Visibility.Collapsed;
			}
			else
			{
				GetCurrentContext.StorageSelectPanelVisibility = Visibility.Visible;
			}
		};
	}

	private void Cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sdview.Cbx.SelectedIndex == 1)
		{
			GetCurrentContext.ImportToolBtnEnable = false;
		}
		else
		{
			GetCurrentContext.ImportToolBtnEnable = true;
		}
		GetCurrentContext.RefreshCommandHandler("1");
	}

	private void pcDirecotries_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
	}

	private void pcDirecotries_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && Context.FindViewModel<FileViewModelV7>(typeof(FileViewV7)).PCDataGridSelected is FileDataViewModel { IsFolder: false } fileDataViewModel)
		{
			new System.Windows.DataObject(typeof(DrapDropFileParameter), new DrapDropFileParameter(0, fileDataViewModel));
		}
	}

	private void pcDirecotries_Drop(object sender, System.Windows.DragEventArgs e)
	{
		if (e.Data.GetData(typeof(DrapDropFileParameter)) is DrapDropFileParameter { Owner: not 0 } drapDropFileParameter)
		{
			Context.FindViewModel<FileViewModelV7>(typeof(FileViewV7)).Export(drapDropFileParameter.FolderFile as FileDataViewModel);
		}
	}

	private void phoneDirecotries_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && Context.FindViewModel<FileViewModelV7>(typeof(FileViewV7)).PhoneDataGridSelected is FileDataViewModel { IsFolder: false } fileDataViewModel)
		{
			System.Windows.DataObject data = new System.Windows.DataObject(typeof(DrapDropFileParameter), new DrapDropFileParameter(1, fileDataViewModel));
			DragDrop.DoDragDrop(phoneDirecotries, data, System.Windows.DragDropEffects.Move);
		}
	}

	public List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement
	{
		DependencyObject dependencyObject = null;
		List<T> list = new List<T>();
		for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
		{
			dependencyObject = VisualTreeHelper.GetChild(obj, i);
			if (dependencyObject is T)
			{
				list.Add((T)dependencyObject);
			}
			list.AddRange(GetChildObjects<T>(dependencyObject));
		}
		return list;
	}

	private void OnCheckBoxSelectAll(object sender, RoutedEventArgs e)
	{
		bool? isChecked = (sender as System.Windows.Controls.CheckBox).IsChecked;
		for (int i = 0; i < GetCurrentContext.DeviceDataGridSources.Count; i++)
		{
			if (i == GetCurrentContext.DeviceDataGridSources.Count - 1)
			{
				System.Windows.Controls.CheckBox checkBox = new System.Windows.Controls.CheckBox();
				checkBox.DataContext = GetCurrentContext.DeviceDataGridSources[i];
				checkBox.IsChecked = isChecked;
				GetCurrentContext.OnCheckedCommand.Execute(checkBox);
			}
			else
			{
				GetCurrentContext.DeviceDataGridSources[i].IsChecked = isChecked;
			}
		}
		if (phoneDirecotries.Items.Count == 0)
		{
			FileDataViewModel fileDataViewModel = GetCurrentContext.allDeviceChildrens.FirstOrDefault((FileDataViewModel x) => x.Data == GetCurrentContext.CurDevicePath && x.IsFolder);
			if (fileDataViewModel != null)
			{
				fileDataViewModel.IsChecked = isChecked;
			}
			GetCurrentContext.OnCheckedCommand.Execute(fileDataViewModel);
		}
	}

	private void phoneDirecotries_Drop(object sender, System.Windows.DragEventArgs e)
	{
		if (e.Data.GetData(typeof(DrapDropFileParameter)) is DrapDropFileParameter { Owner: not 1 } drapDropFileParameter)
		{
			Context.FindViewModel<FileViewModelV7>(typeof(FileViewV7)).Import(drapDropFileParameter.FolderFile as FileDataViewModel);
		}
	}

	private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
	{
	}

	private void OnChecked(string a)
	{
	}

	private void pcDirecotries_Drop(object sender, RoutedEventArgs e)
	{
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		List<FileDataViewModel> allDeviceChildrens = GetCurrentContext.allDeviceChildrens;
		List<FileDataViewModel> list = new List<FileDataViewModel>();
		foreach (FileDataViewModel item in allDeviceChildrens)
		{
			if (!item.IsFolder && item.IsChecked == true)
			{
				if (item.FileSize > 4294967296L)
				{
					dictionary.Add(item.Data, item.FileSize);
				}
				else
				{
					list.Add(item);
				}
			}
		}
		DeviceCommonManagementV6.CheckExportFiles(dictionary);
		if (list == null || list.Count() == 0)
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel)
		{
			string saveDir = folderBrowserDialog.SelectedPath.Trim();
			ImportAndExportWrapper importAndExportWrapper = new ImportAndExportWrapper();
			List<string> idList = list.Select((FileDataViewModel m) => m.Data.ToString()).ToList();
			importAndExportWrapper.ExportFile(BusinessType.FILE_EXPORT, 24, idList, "K1786", "{580C48C8-6CEF-4BBB-AF37-D880B349D142}", "K1800", saveDir, null);
			GetCurrentContext.RefreshCommandHandler("1");
		}
	}

	private void LangButton_Click(object sender, RoutedEventArgs e)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Multiselect = true;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() == true)
		{
			List<string> fileNames = openFileDialog.FileNames.ToList();
			fileNames = DeviceCommonManagementV6.CheckImportFiles(fileNames);
			if (fileNames.Count() == 0)
			{
				return;
			}
			TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			if (tcpAndroidDevice == null && tcpAndroidDevice.Property == null)
			{
				return;
			}
			string appSaveDir = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, GetCurrentContext.CurDevicePath) + "/";
			new ImportAndExportWrapper().ImportFile(BusinessType.FILE_IMPORT, 24, "K1785", "{580C48C8-6CEF-4BBB-AF37-D880B349D142}", "K1801", () => fileNames, (string sourcePath) => appSaveDir + Path.GetFileName(sourcePath));
		}
		GetCurrentContext.RefreshCommandHandler("2");
	}

	private void TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (GetCurrentContext.backStack.Peek() != (sender as System.Windows.Controls.TreeView).SelectedItem as FileDataViewModel)
		{
			GetCurrentContext.backStack.Push((sender as System.Windows.Controls.TreeView).SelectedItem as FileDataViewModel);
		}
	}

	private void LangButton_Click_1(object sender, RoutedEventArgs e)
	{
		if (btn_all.LangKey == "K0764")
		{
			GetCurrentContext.DeviceTreeSources[0].IsChecked = true;
			for (int i = 0; i < GetCurrentContext.allDeviceChildrens.Count; i++)
			{
				GetCurrentContext.allDeviceChildrens[i].IsChecked = true;
			}
			GetCurrentContext.IsAllCheckedForCheckBox = true;
			GetCurrentContext.DeviceSelectFileCount = GetCurrentContext.allDeviceChildrens.Where((FileDataViewModel x) => !x.IsFolder && x.IsChecked == true).Count();
		}
		else
		{
			GetCurrentContext.DeviceTreeSources[0].IsChecked = false;
			for (int j = 0; j < GetCurrentContext.allDeviceChildrens.Count; j++)
			{
				GetCurrentContext.allDeviceChildrens[j].IsChecked = false;
			}
			GetCurrentContext.IsAllCheckedForCheckBox = false;
			GetCurrentContext.DeviceSelectFileCount = 0;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
