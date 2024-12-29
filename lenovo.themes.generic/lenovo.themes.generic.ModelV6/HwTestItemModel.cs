using System;

namespace lenovo.themes.generic.ModelV6;

[Serializable]
public class HwTestItemModel
{
	public long log_id { get; set; }

	public string app_version_code { get; set; }

	public string app_version_name { get; set; }

	public string country { get; set; }

	public string language { get; set; }

	public string android_version { get; set; }

	public string model { get; set; }

	public string time_stamp { get; set; }

	public string imei { get; set; }

	public string imei2 { get; set; }

	public string test_type { get; set; }

	public string test_component { get; set; }

	public string result { get; set; }

	public int status { get; set; }
}
