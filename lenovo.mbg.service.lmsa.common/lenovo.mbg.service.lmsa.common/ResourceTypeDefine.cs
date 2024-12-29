using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.common;

public class ResourceTypeDefine
{
	public const string STORAGE_DESCRIPTION = "{AF7750C4-A38C-400F-9A9C-5C3DAC0CA829}";

	public const string PIC = "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}";

	public const string MUSIC = "{242C8F16-6AC7-431B-BBF1-AE24373860F1}";

	public const string VIDEO = "{8BEBE14B-4E45-4D36-8726-8442E6242C01}";

	public const string CONTACT = "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}";

	public const string SMS = "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}";

	public const string CALL_LOG = "{89D4DB68-4258-4002-8557-E65959C558B3}";

	public const string APP = "{958781C8-0788-4F87-A4C3-CBD793AAB1A0}";

	public const string FILE = "{580C48C8-6CEF-4BBB-AF37-D880B349D142}";

	public const string FAIL = "_FAIL";

	public static Dictionary<string, string> ResourceTypeMap = new Dictionary<string, string>
	{
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "Video" },
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "Photos" },
		{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", "Contacts" },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "Songs" },
		{ "{89D4DB68-4258-4002-8557-E65959C558B3}", "Calllog" },
		{ "{958781C8-0788-4F87-A4C3-CBD793AAB1A0}", "App" },
		{ "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", "SMS" },
		{ "{580C48C8-6CEF-4BBB-AF37-D880B349D142}", "Files" }
	};

	public const string Success = "success";

	public const string Failed = "failed";

	public const string Undo = "undo";
}
