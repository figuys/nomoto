using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class QTreeNode : NotifyBase
{
	private string _Question;

	private bool _IsSelected;

	private Visibility _Visible = Visibility.Visible;

	[JsonProperty("id")]
	public long Id { get; set; }

	[JsonProperty("content")]
	public string Context { get; set; }

	[JsonProperty("parentId")]
	public int Parent { get; set; }

	[JsonProperty("question")]
	public string Question
	{
		get
		{
			return _Question;
		}
		set
		{
			_Question = value;
			OnPropertyChanged("Question");
		}
	}

	[JsonProperty("url")]
	public string Url { get; set; }

	[JsonProperty("hyperlink")]
	public string Title { get; set; }

	[JsonProperty("selected")]
	public bool IsSelected
	{
		get
		{
			return _IsSelected;
		}
		set
		{
			_IsSelected = value;
			OnPropertyChanged("IsSelected");
			if (!_IsSelected)
			{
				Children?.ForEach(delegate(QTreeNode p)
				{
					p.IsSelected = false;
				});
			}
		}
	}

	[JsonIgnore]
	public ICommand LinkCommand { get; set; }

	[JsonIgnore]
	public Visibility Visible
	{
		get
		{
			return _Visible;
		}
		set
		{
			_Visible = value;
			OnPropertyChanged("Visible");
		}
	}

	[JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
	public List<QTreeNode> Children { get; set; }

	public void ProcBeforeCommit()
	{
		Context = null;
		Question = null;
		Title = null;
		Url = null;
		Children?.ForEach(delegate(QTreeNode p)
		{
			p.ProcBeforeCommit();
		});
	}

	public long GetLastSelectedId()
	{
		if (Children == null || Children.Count == 0)
		{
			return Id;
		}
		return Children.FirstOrDefault((QTreeNode p) => p.IsSelected)?.GetLastSelectedId() ?? Id;
	}
}
