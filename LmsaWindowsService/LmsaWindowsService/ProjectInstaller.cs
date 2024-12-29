using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace LmsaWindowsService;

[RunInstaller(true)]
public class ProjectInstaller : Installer
{
	private readonly string SERVICE_NAME = "LmsaWindowsService";

	private IContainer components;

	private ServiceProcessInstaller serviceProcessInstaller1;

	private ServiceInstaller serviceInstaller1;

	public ProjectInstaller()
	{
		InitializeComponent();
		base.AfterInstall += ProjectInstaller_AfterInstall;
	}

	public override void Install(IDictionary stateSaver)
	{
		ServiceController[] services = ServiceController.GetServices();
		foreach (ServiceController serviceController in services)
		{
			if (serviceController.ServiceName == SERVICE_NAME)
			{
				if (serviceController.Status == ServiceControllerStatus.Running)
				{
					serviceController.Stop();
				}
				serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
				Process process = new Process();
				process.StartInfo.FileName = "cmd.exe";
				process.StartInfo.Arguments = "/c sc delete " + SERVICE_NAME;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = true;
				process.Start();
				process.WaitForExit();
				Thread.Sleep(5000);
				break;
			}
		}
		base.Install(stateSaver);
	}

	public override void Uninstall(IDictionary savedState)
	{
		ServiceController[] services = ServiceController.GetServices();
		for (int i = 0; i < services.Length; i++)
		{
			if (services[i].ServiceName == SERVICE_NAME)
			{
				base.Uninstall(savedState);
				break;
			}
		}
	}

	protected override void OnBeforeUninstall(IDictionary savedState)
	{
		ServiceController[] services = ServiceController.GetServices();
		foreach (ServiceController serviceController in services)
		{
			if (serviceController.ServiceName == SERVICE_NAME)
			{
				if (serviceController.Status == ServiceControllerStatus.Running)
				{
					serviceController.Stop();
				}
				serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
				break;
			}
		}
		base.OnBeforeUninstall(savedState);
	}

	private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
	{
		ServiceController serviceController = new ServiceController();
		serviceController.ServiceName = SERVICE_NAME;
		serviceController.Start();
	}

	public static void UnInstallService(string filepath, string serviceName, string[] options)
	{
		try
		{
			if (IsServiceExisted(serviceName))
			{
				AssemblyInstaller assemblyInstaller = new AssemblyInstaller();
				assemblyInstaller.UseNewContext = true;
				assemblyInstaller.Path = filepath;
				assemblyInstaller.CommandLine = options;
				assemblyInstaller.Uninstall(null);
				assemblyInstaller.Dispose();
			}
		}
		catch (Exception ex)
		{
			throw new Exception("UnInstall Service Error\n" + ex.Message);
		}
	}

	public static bool IsServiceExisted(string serviceName)
	{
		ServiceController[] services = ServiceController.GetServices();
		for (int i = 0; i < services.Length; i++)
		{
			if (services[i].ServiceName == serviceName)
			{
				return true;
			}
		}
		return false;
	}

	private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
	{
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
		serviceProcessInstaller1 = new ServiceProcessInstaller();
		serviceInstaller1 = new ServiceInstaller();
		serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
		serviceProcessInstaller1.Password = null;
		serviceProcessInstaller1.Username = null;
		serviceInstaller1.Description = "Lenovo Smart Windows Service";
		serviceInstaller1.DisplayName = "Lenovo Smart Windows Service";
		serviceInstaller1.ServiceName = "LmsaWindowsService";
		serviceInstaller1.StartType = ServiceStartMode.Automatic;
		base.Installers.AddRange(new Installer[2] { serviceProcessInstaller1, serviceInstaller1 });
	}
}
