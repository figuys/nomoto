using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.ControlsV6;

public partial class TransferProgressWiew : Window, ILoading, IComponentConnector
{
	private string titleFormat;

	private double mbSize = 1048576.0;

	public TransferProgressWiew()
	{
		InitializeComponent();
	}

	public void Abort()
	{
	}

	public void Hiden(object handler)
	{
		base.Dispatcher.Invoke(delegate
		{
			Close();
		});
	}

	public void Show(object handler)
	{
		base.Dispatcher.Invoke(delegate
		{
			AsyncDataLoader.MaskLayer.New(this, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				ShowDialog();
			});
		});
	}

	public void ProgressUpdate(long current, long filesize)
	{
		base.Dispatcher.Invoke(delegate
		{
			bdValue.Width = 460.0 * (double)current / (double)filesize;
			txtInfo.Text = string.Format(titleFormat, GlobalFun.ConvertLong2String(current, "F2"), GlobalFun.ConvertLong2String(filesize, "F2"));
		});
	}

	public void SetProgressTitle(string format, string defaultTitle)
	{
		base.Dispatcher.Invoke(delegate
		{
			txtInfo.Text = defaultTitle;
			titleFormat = format;
		});
	}

	private string ConvertBytesWithUnit(long size)
	{
		if ((double)size >= mbSize)
		{
			return Math.Round((double)size / mbSize, 2) + " MB";
		}
		if (size >= 1024)
		{
			return Math.Round((double)size / 1024.0, 2) + " KB";
		}
		return size + " Bytes";
	}
}
