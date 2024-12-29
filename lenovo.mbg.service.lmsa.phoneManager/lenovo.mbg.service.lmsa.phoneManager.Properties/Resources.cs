using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace lenovo.mbg.service.lmsa.phoneManager.Properties;

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
				resourceMan = new ResourceManager("lenovo.mbg.service.lmsa.phoneManager.Properties.Resources", typeof(Resources).Assembly);
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

	internal static string appDrawingImage => ResourceManager.GetString("appDrawingImage", resourceCulture);

	internal static string bottom_charbot_bkDrawingImage => ResourceManager.GetString("bottom_charbot_bkDrawingImage", resourceCulture);

	internal static string bottom_charbotDrawingImage => ResourceManager.GetString("bottom_charbotDrawingImage", resourceCulture);

	internal static string bottom_forumDrawingImage => ResourceManager.GetString("bottom_forumDrawingImage", resourceCulture);

	internal static string bottom_messenger_bigBackgroundDrawingImage => ResourceManager.GetString("bottom_messenger_bigBackgroundDrawingImage", resourceCulture);

	internal static string bottom_messengerDrawingImage => ResourceManager.GetString("bottom_messengerDrawingImage", resourceCulture);

	internal static string bottom_tips_bigBackgroundDrawImage => ResourceManager.GetString("bottom_tips_bigBackgroundDrawImage", resourceCulture);

	internal static string bottom_tipsDrawingImage => ResourceManager.GetString("bottom_tipsDrawingImage", resourceCulture);

	internal static string bottomNavBackground => ResourceManager.GetString("bottomNavBackground", resourceCulture);

	internal static string contactDrawingImage => ResourceManager.GetString("contactDrawingImage", resourceCulture);

	internal static string musicDrawingImage => ResourceManager.GetString("musicDrawingImage", resourceCulture);

	internal static string picDrawingImage => ResourceManager.GetString("picDrawingImage", resourceCulture);

	internal static string smsDrawingImage => ResourceManager.GetString("smsDrawingImage", resourceCulture);

	internal static string videoDrawingImage => ResourceManager.GetString("videoDrawingImage", resourceCulture);

	internal Resources()
	{
	}
}
