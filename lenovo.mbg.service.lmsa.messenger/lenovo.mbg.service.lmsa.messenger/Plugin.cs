using System.Windows;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.messenger;

[PluginExport(typeof(IPlugin), "d7deab64b8cb4e31b600ad0d839b6d73")]
public class Plugin : PluginBase
{
	private const string screenName = "lmsa-plugin-Messenger";

	private MessengerFrame _MessengerFrame { get; set; }

	public override void Init()
	{
	}

	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		_MessengerFrame = new MessengerFrame();
		return _MessengerFrame;
	}
}
