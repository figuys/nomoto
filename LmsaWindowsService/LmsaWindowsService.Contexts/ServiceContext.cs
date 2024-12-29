using System;

namespace LmsaWindowsService.Contexts;

public class ServiceContext
{
	public static bool LmsaNormalColsed { get; set; }

	public static string Appdirectory => AppDomain.CurrentDomain.BaseDirectory;
}
