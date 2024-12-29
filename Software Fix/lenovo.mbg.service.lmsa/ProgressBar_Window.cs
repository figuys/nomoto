using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.UpdateVersion;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;

namespace lenovo.mbg.service.lmsa;

public partial class ProgressBar_Window : Window, IComponentConnector
{
	public static readonly DependencyProperty StrProgressTitleProperty = DependencyProperty.Register("StrProgressTitle", typeof(string), typeof(ProgressBar_Window), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty PercentNumProperty = DependencyProperty.Register("PercentNum", typeof(double), typeof(ProgressBar_Window), new PropertyMetadata(0.0));

	public static readonly DependencyProperty StrPercentProperty = DependencyProperty.Register("StrPercent", typeof(string), typeof(ProgressBar_Window), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty StrDownloadSpeedProperty = DependencyProperty.Register("StrDownloadSpeed", typeof(string), typeof(ProgressBar_Window), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty StrButtonTitle_CancelProperty = DependencyProperty.Register("StrButtonTitle_Cancel", typeof(string), typeof(ProgressBar_Window), new PropertyMetadata(string.Empty));

	private bool IsShowPercent
	{
		set
		{
			TxbPercent.Visibility = ((!value) ? Visibility.Collapsed : Visibility.Visible);
		}
	}

	public string StrProgressTitle
	{
		get
		{
			return (string)GetValue(StrProgressTitleProperty);
		}
		set
		{
			SetValue(StrProgressTitleProperty, value);
		}
	}

	public double PercentNum
	{
		get
		{
			return (double)GetValue(PercentNumProperty);
		}
		set
		{
			SetValue(PercentNumProperty, value);
			StrPercent = Math.Round(value, 0) + "%";
		}
	}

	public string StrPercent
	{
		get
		{
			return (string)GetValue(StrPercentProperty);
		}
		set
		{
			SetValue(StrPercentProperty, value);
		}
	}

	public string StrDownloadSpeed
	{
		get
		{
			return (string)GetValue(StrDownloadSpeedProperty);
		}
		set
		{
			SetValue(StrDownloadSpeedProperty, value);
		}
	}

	public string StrButtonTitle_Cancel
	{
		get
		{
			return (string)GetValue(StrButtonTitle_CancelProperty);
		}
		set
		{
			SetValue(StrButtonTitle_CancelProperty, value);
		}
	}

	public object ContentTag { get; set; }

	public ProgressBar_Window(Window windowOwner, bool isShowPercent, string strProgressTitle, bool isShowCancel = false)
	{
		InitializeComponent();
		base.Owner = windowOwner;
		base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		IsShowPercent = isShowPercent;
		StrProgressTitle = strProgressTitle;
		StrButtonTitle_Cancel = "K0208";
		TxbCancel.Visibility = ((!isShowCancel) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void TxbCancel_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (ContentTag == null)
		{
			return;
		}
		try
		{
			VersionModel NewVersionModel = ContentTag as VersionModel;
			ThreadPool.QueueUserWorkItem(delegate
			{
				UpdateManager.Instance.ToolUpdateWorker.CancelDownloadVersion(NewVersionModel);
			});
			ApplcationClass.IsUpdatingPlug = false;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.ProgressBar_Window.TxbCancel_MouseDown event error. ex:" + ex.ToString());
		}
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
