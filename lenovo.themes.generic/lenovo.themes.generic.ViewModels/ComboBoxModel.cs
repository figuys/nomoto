using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class ComboBoxModel : ViewModelBase
{
	private string m_key;

	private string m_value;

	private bool m_isChecked;

	private int m_countNum;

	private ObservableCollection<ComboBoxModel> m_Childs = new ObservableCollection<ComboBoxModel>();

	private Visibility m_toggleButtonVisibility;

	private Visibility m_checkboxVisibility;

	private bool m_checkboxDisabled;

	private bool m_hasValue;

	private int m_checkFlag;

	public ReplayCommand SelectedAllCommand { get; set; }

	public ComboBoxModel Parent { get; set; }

	public string Key
	{
		get
		{
			return m_key;
		}
		set
		{
			m_key = value;
			OnPropertyChanged("Key");
		}
	}

	public string Value
	{
		get
		{
			return m_value;
		}
		set
		{
			m_value = value;
			OnPropertyChanged("Value");
		}
	}

	public bool IsChecked
	{
		get
		{
			return m_isChecked;
		}
		set
		{
			m_isChecked = value;
			if (Childs != null && Childs.Count > 0)
			{
				foreach (ComboBoxModel child in Childs)
				{
					child.IsChecked = m_isChecked;
				}
			}
			if (Parent != null)
			{
				int num = Parent.Childs.Count((ComboBoxModel n) => n.IsChecked);
				if (num == Parent.Childs.Count)
				{
					Parent.CheckFlag = 2;
				}
				else if (num > 0)
				{
					Parent.CheckFlag = 1;
				}
				else
				{
					Parent.CheckFlag = 0;
				}
			}
			else
			{
				CheckFlag = (m_isChecked ? 2 : 0);
			}
			OnPropertyChanged("IsChecked");
		}
	}

	public int CheckFlag
	{
		get
		{
			return m_checkFlag;
		}
		set
		{
			m_checkFlag = value;
			OnPropertyChanged("CheckFlag");
		}
	}

	public bool HasValue
	{
		get
		{
			return m_hasValue;
		}
		set
		{
			m_hasValue = value;
			OnPropertyChanged("HasValue");
		}
	}

	public bool CheckboxDisabled
	{
		get
		{
			return m_checkboxDisabled;
		}
		set
		{
			m_checkboxDisabled = value;
			OnPropertyChanged("CheckboxDisabled");
		}
	}

	public int CountNum
	{
		get
		{
			return m_countNum;
		}
		set
		{
			m_countNum = value;
			if (m_countNum > 0)
			{
				CheckboxDisabled = true;
			}
			else
			{
				CheckboxDisabled = false;
			}
			Value = $"{Key}({m_countNum})";
			OnPropertyChanged("CountNum");
		}
	}

	public object ExtraData { get; set; }

	public ObservableCollection<ComboBoxModel> Childs
	{
		get
		{
			return m_Childs;
		}
		set
		{
			m_Childs = value;
			OnPropertyChanged("Childs");
		}
	}

	public Visibility ToggleButtonVisibility
	{
		get
		{
			return m_toggleButtonVisibility;
		}
		set
		{
			m_toggleButtonVisibility = value;
			OnPropertyChanged("ToggleButtonVisibility");
		}
	}

	public Visibility CheckboxVisibility
	{
		get
		{
			return m_checkboxVisibility;
		}
		set
		{
			m_checkboxVisibility = value;
			OnPropertyChanged("CheckboxVisibility");
		}
	}
}
