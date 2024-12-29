using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace lenovo.mbg.service.common.webservices.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
				resourceMan = new ResourceManager("lenovo.mbg.service.common.webservices.Properties.Resources", typeof(Resources).Assembly);
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

	internal static string Code => ResourceManager.GetString("Code", resourceCulture);

	internal static string IbaseParam => ResourceManager.GetString("IbaseParam", resourceCulture);

	internal static string Name => ResourceManager.GetString("Name", resourceCulture);

	internal static string SdeParam => ResourceManager.GetString("SdeParam", resourceCulture);

	internal Resources()
	{
	}
}
