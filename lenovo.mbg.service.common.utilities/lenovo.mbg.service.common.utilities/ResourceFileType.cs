using System.Runtime.InteropServices;

namespace lenovo.mbg.service.common.utilities;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ResourceFileType
{
	public const string ROM = "ROM";

	public const string APK = "APK";

	public const string TOOL = "TOOL";

	public const string ICON = "ICON";

	public const string COUNTRYCODE = "COUNTRYCODE";

	public const string JSON = "JSON";

	public const string BANNER = "BANNER";

	public const string XAML = "XAML";

	public const string UNKNOWN = "UNKNOWN";
}
