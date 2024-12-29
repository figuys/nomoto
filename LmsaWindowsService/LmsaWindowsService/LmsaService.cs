using System;
using System.ComponentModel;
using System.IO;
using System.ServiceProcess;
using lenovo.mbg.service.common.log;
using LmsaWindowsService.Contexts;
using LmsaWindowsService.Tasks;

namespace LmsaWindowsService;

public class LmsaService : ServiceBase
{
	private IContainer components;

	public LmsaService()
	{
		InitializeComponent();
		AppDomain.CurrentDomain.UnhandledException += NtServiceUnhandExceptionHandler;
		LogHelper.ConfigFilePath = Path.Combine(ServiceContext.Appdirectory, "nt-log4net.config");
	}

	private void NtServiceUnhandExceptionHandler(object sender, UnhandledExceptionEventArgs e)
	{
		Exception exception = e.ExceptionObject as Exception;
		LogHelper.LogInstance.Error("Unhand exception: ", exception);
	}

	protected override void OnStart(string[] args)
	{
		TaskManager.RegisterTask(new LmsaLifeCycleMonitorTask());
		TaskManager.RegisterTask(new PipeTask());
	}

	protected override void OnStop()
	{
		TaskManager.Dispose();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new Container();
		base.ServiceName = "LmsaWindowsService";
	}
}
