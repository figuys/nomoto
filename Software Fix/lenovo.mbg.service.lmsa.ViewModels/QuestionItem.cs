using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class QuestionItem : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	public Visibility _Visibile = Visibility.Collapsed;

	public string _Data;

	public string Id { get; private set; }

	public string QuestionType { get; set; }

	public string GroupName { get; set; }

	public string ItemText { get; set; }

	public bool IsSelected { get; set; }

	public double Width { get; set; }

	public Visibility Visibile
	{
		get
		{
			return _Visibile;
		}
		set
		{
			_Visibile = value;
			OnPropertyChanged("Visibile");
		}
	}

	public string Data
	{
		get
		{
			return _Data;
		}
		set
		{
			_Data = value;
			OnPropertyChanged("Data");
		}
	}

	public QuestionItem(QuestionType _type, string _text)
	{
		QuestionType = Enum.GetName(typeof(QuestionType), _type);
		Id = _text;
		ItemText = _text;
		IsSelected = false;
		Width = 330.0;
	}

	public QuestionItem(QuestionType _type, string _id, string _text)
	{
		QuestionType = Enum.GetName(typeof(QuestionType), _type);
		Id = _id;
		ItemText = _text;
		IsSelected = false;
	}
}
