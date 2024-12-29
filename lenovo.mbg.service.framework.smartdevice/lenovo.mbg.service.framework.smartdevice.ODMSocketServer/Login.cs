using System.Runtime.InteropServices;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Login
{
	public static string UserName { get; set; }

	public static string Password { get; set; }

	public static Login Default => new Login(string.Empty, string.Empty);

	public Login(string userName, string password)
	{
		UserName = userName;
		Password = password;
	}
}
