using System;
using System.ComponentModel;

namespace lenovo.mbg.service.lmsa.Business;

[Serializable]
public class NoticeInfo : INotifyPropertyChanged
{
	private string _type = "Normal";

	private bool _isChecked;

	public long id { get; set; }

	public string noticeTitle { get; set; }

	public string noticeContent { get; set; }

	public DateTime? modifyDate { get; set; }

	public bool isDeleted { get; set; }

	public bool isServerReplay { get; set; }

	public string tag { get; set; }

	public int noticeType { get; set; }

	public string type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
			if (string.IsNullOrEmpty(_type))
			{
				_type = "Normal";
			}
		}
	}

	public bool isChecked
	{
		get
		{
			if (type == "Feedback" && !isServerReplay)
			{
				return true;
			}
			return _isChecked;
		}
		set
		{
			_isChecked = value;
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;
}
