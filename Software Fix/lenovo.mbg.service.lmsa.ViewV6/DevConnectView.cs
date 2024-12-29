using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class DevConnectView : Window, IComponentConnector
{
	public List<string> DevNotifyArr { get; private set; }

	public DevConnectView()
	{
		InitializeComponent();
		DevNotifyArr = new List<string>();
		txtRandomCode.Text = RandomAesKeyHelper.Instance.EncryptCode;
	}

	public void ShowWnd(string id)
	{
		if (!DevNotifyArr.Contains(id))
		{
			DevNotifyArr.Add(id);
		}
		Display();
	}

	public void HideWnd(string id)
	{
		DevNotifyArr.Remove(id);
		if (DevNotifyArr.Count == 0)
		{
			DisVisible();
		}
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		DisVisible();
	}

	public void Display()
	{
		base.Owner.LocationChanged -= Owner_LocationChanged;
		base.Owner.LocationChanged += Owner_LocationChanged;
		MainWindow mainWindow = base.Owner as MainWindow;
		base.Top = mainWindow.Top + (mainWindow.Height - base.Height) / 2.0;
		base.Left = mainWindow.Left + (mainWindow.Width - base.Width) / 2.0;
		mainWindow.FrozenWindow();
		Show();
	}

	private void DisVisible()
	{
		(base.Owner as MainWindow).UnFrozenWindow();
		base.Owner.Loaded -= Owner_LocationChanged;
		Hide();
	}

	private void Owner_LocationChanged(object sender, EventArgs e)
	{
		if (base.IsVisible)
		{
			Window owner = base.Owner;
			base.Top = owner.Top + (owner.Height - base.Height) / 2.0;
			base.Left = owner.Left + (owner.Width - base.Width) / 2.0;
			Show();
		}
	}
}
