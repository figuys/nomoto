using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace lenovo.mbg.service.lmsa.tips.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("lenovo.mbg.service.lmsa.tips.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string CarchTip_Assembleing => ResourceManager.GetString("CarchTip_Assembleing", resourceCulture);

	internal static string CarchTip_Assembleing_Failed => ResourceManager.GetString("CarchTip_Assembleing_Failed", resourceCulture);

	internal static string CarchTip_Loading => ResourceManager.GetString("CarchTip_Loading", resourceCulture);

	internal static string ChooseDeviceButtonTitle => ResourceManager.GetString("ChooseDeviceButtonTitle", resourceCulture);

	internal static string ChooseDeviceTip => ResourceManager.GetString("ChooseDeviceTip", resourceCulture);

	internal static string ComboxSeriesDefultTitle => ResourceManager.GetString("ComboxSeriesDefultTitle", resourceCulture);

	internal static string ComboxSubSeriesDefultTitle => ResourceManager.GetString("ComboxSubSeriesDefultTitle", resourceCulture);

	internal static string Converter_WebHeadTitle_Smartphone => ResourceManager.GetString("Converter_WebHeadTitle_Smartphone", resourceCulture);

	internal static string Converter_WebHeadTitle_Tablet => ResourceManager.GetString("Converter_WebHeadTitle_Tablet", resourceCulture);

	internal static string DeviceHeadTitle_SmartphoneTitle => ResourceManager.GetString("DeviceHeadTitle_SmartphoneTitle", resourceCulture);

	internal static string DeviceHeadTitle_TabletTitle => ResourceManager.GetString("DeviceHeadTitle_TabletTitle", resourceCulture);

	internal static string TipsOnline_ConnectError => ResourceManager.GetString("TipsOnline_ConnectError", resourceCulture);

	internal static string WebHeadTitle_Content1 => ResourceManager.GetString("WebHeadTitle_Content1", resourceCulture);

	internal static string WebHeadTitle_Content2 => ResourceManager.GetString("WebHeadTitle_Content2", resourceCulture);

	internal Resources()
	{
	}
}
