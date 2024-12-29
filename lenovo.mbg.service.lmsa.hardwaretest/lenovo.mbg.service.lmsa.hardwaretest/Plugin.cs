using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hardwaretest.View;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.hardwaretest;

[PluginExport(typeof(IPlugin), "985c66acdde2483ed96844a6b5ea4337")]
public class Plugin : PluginBase
{
	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		Context.MessageBox = iMessage;
		MainFrame result = new MainFrame
		{
			DataContext = Context.MainFrame
		};
		Context.MainFrame.LoadData();
		return result;
	}

	public override void Init()
	{
	}

	public override void OnInit(object data)
	{
		base.OnInit(data);
		DeviceEx device = HostProxy.deviceManager.MasterDevice;
		if (device != null && device.SoftStatus == DeviceSoftStateEx.Online && !(Context.MainFrame.CurrentDevice?.Identifer == device.Identifer))
		{
			Task.Run(delegate
			{
				Context.MainFrame.SetHWTestDevice(device);
			});
		}
	}
}
