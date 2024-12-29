using System.ServiceProcess;

namespace LmsaWindowsService;

internal static class Program
{
	private static void Main()
	{
		ServiceBase.Run(new ServiceBase[1]
		{
			new LmsaService()
		});
	}
}
