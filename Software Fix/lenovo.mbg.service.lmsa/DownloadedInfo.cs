using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa;

public class DownloadedInfo : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private const string initData = "-";

	private string m_marketName = "-";

	private string m_modelName = "-";

	private string m_hwCode = "-";

	private string m_country = "-";

	private string m_simCount = "-";

	private string m_Memory = "-";

	private DownloadInfo m_downloadInfo;

	public string marketName
	{
		get
		{
			return m_marketName;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_marketName = value;
			}
		}
	}

	public string modelName
	{
		get
		{
			return m_modelName;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_modelName = value;
			}
		}
	}

	public string hwCode
	{
		get
		{
			return m_hwCode;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_hwCode = value;
			}
		}
	}

	public string country
	{
		get
		{
			return m_country;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_country = value;
			}
		}
	}

	public string simCount
	{
		get
		{
			return m_simCount;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_simCount = value;
			}
		}
	}

	public string Memory
	{
		get
		{
			return m_Memory;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				m_Memory = value;
			}
		}
	}

	public string FileUrl { get; set; }

	public string DevDetailInfo
	{
		get
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(marketName) && marketName != "-")
			{
				text = text + HostProxy.LanguageService.Translate("K0723") + " " + marketName + "\n";
			}
			if (!string.IsNullOrEmpty(modelName) && modelName != "-")
			{
				text = text + HostProxy.LanguageService.Translate("K0455") + " " + modelName + "\n";
			}
			if (!string.IsNullOrEmpty(hwCode) && hwCode != "-")
			{
				text = text + HostProxy.LanguageService.Translate("K1125") + " " + hwCode + "\n";
			}
			if (!string.IsNullOrEmpty(simCount) && simCount != "-")
			{
				text = text + HostProxy.LanguageService.Translate("K1127") + " " + simCount + "\n";
			}
			if (!string.IsNullOrEmpty(country) && country != "-")
			{
				text = text + HostProxy.LanguageService.Translate("K1126") + " " + country + "\n";
			}
			if (!string.IsNullOrEmpty(Memory) && Memory != "-")
			{
				text = text + HostProxy.LanguageService.Translate("K1128") + " " + Memory + "\n";
			}
			return text.Trim('\n');
		}
	}

	[JsonIgnore]
	public DownloadInfo downloadInfo
	{
		get
		{
			return m_downloadInfo;
		}
		set
		{
			m_downloadInfo = value;
			OnPropertyChanged("downloadInfo");
		}
	}

	public DownloadedInfo(DownloadInfo downloadInfo)
	{
		this.downloadInfo = downloadInfo;
	}
}
