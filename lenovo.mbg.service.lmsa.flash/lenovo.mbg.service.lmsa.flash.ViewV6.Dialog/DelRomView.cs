using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class DelRomView : UserControl, IMessageViewV6, IComponentConnector
{
	protected string Rom;

	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public DelRomView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(string rom)
	{
		Rom = rom;
	}

	public bool IsNotNotShowDeleteRomMore()
	{
		return cbx.IsChecked == true;
	}

	public void DeleteRomFile(string fileName)
	{
		GlobalCmdHelper.Instance.Execute(new
		{
			type = GlobalCmdType.DELETE_ROM_AFTER_RESCUE,
			data = fileName
		});
	}

	public void SetNotShowDeleteRom()
	{
		Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		if (configuration.AppSettings.Settings.AllKeys.Contains("NotShowDeleteRom"))
		{
			configuration.AppSettings.Settings.Remove("NotShowDeleteRom");
		}
		configuration.AppSettings.Settings.Add("NotShowDeleteRom", "true");
		configuration.Save(ConfigurationSaveMode.Modified);
		ConfigurationManager.RefreshSection("appSettings");
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		FireClose(true);
		DeleteRomFile(Rom);
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		FireClose(false);
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
		if (IsNotNotShowDeleteRomMore())
		{
			SetNotShowDeleteRom();
		}
	}
}
