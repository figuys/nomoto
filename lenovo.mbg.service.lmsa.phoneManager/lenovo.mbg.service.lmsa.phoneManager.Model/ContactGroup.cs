using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class ContactGroup : BaseNotify
{
	private bool _isSelected;

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonIgnore]
	public bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			OnPropertyChanged("isSelected");
		}
	}
}
