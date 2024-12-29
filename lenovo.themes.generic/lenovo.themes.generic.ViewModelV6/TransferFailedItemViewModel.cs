using System.Linq;

namespace lenovo.themes.generic.ViewModelV6;

public class TransferFailedItemViewModel : ViewModelBase
{
	private string _Message;

	public string Id { get; set; }

	public string Message
	{
		get
		{
			string[] array = _Message.Split('/');
			if (array.Count() > 0)
			{
				return array[array.Count() - 1];
			}
			return _Message;
		}
		set
		{
			_Message = value;
			OnPropertyChanged("Message");
		}
	}
}
