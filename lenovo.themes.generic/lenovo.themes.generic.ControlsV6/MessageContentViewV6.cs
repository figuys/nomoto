using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.ControlsV6;

public partial class MessageContentViewV6 : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; private set; }

	public bool? Result
	{
		get
		{
			return Vw.Result;
		}
		set
		{
			Vw.Result = value;
		}
	}

	protected IMessageViewV6 Vw { get; set; }

	public MessageContentViewV6()
	{
		InitializeComponent();
	}

	public void Init(IMessageViewV6 view)
	{
		Vw = view;
		WaitHandler = view.WaitHandler;
		CloseAction = view.CloseAction;
		content.Content = view;
	}
}
