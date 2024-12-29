using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace lenovo.themes.generic.Controls;

public class GifPanel : Canvas, IDisposable
{
	private class GifFrameInfo
	{
		private GifPanel m_outer;

		public BitmapSource FrameSource { get; set; }

		public uint DelayDuration { get; set; }

		public double Left { get; set; }

		public double Top { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public ushort Delay { get; set; }

		public int Disposal { get; set; }

		public GifFrameInfo(GifPanel gifPanel)
		{
			m_outer = gifPanel;
		}
	}

	private class PlayImageControl
	{
		public GifFrameInfo FrameInfo { get; private set; }

		public double Scaling { get; set; }

		public double Left { get; private set; }

		public double Top { get; private set; }

		public Image Image { get; private set; }

		public PlayImageControl Next { get; set; }

		public PlayImageControl(GifFrameInfo frameInfo, double scaling)
		{
			FrameInfo = frameInfo;
			Scaling = scaling;
			Image = new Image();
			Image.Source = frameInfo.FrameSource;
			Image.Stretch = Stretch.UniformToFill;
			Image.Width = frameInfo.Width * scaling;
			Image.Height = frameInfo.Height * scaling;
			Left = frameInfo.Left * scaling;
			Top = frameInfo.Top * scaling;
		}

		public void ReSetScaling(double scaling)
		{
			Image.Width = FrameInfo.Width * scaling;
			Image.Height = FrameInfo.Height * scaling;
			Left = FrameInfo.Left * scaling;
			Top = FrameInfo.Top * scaling;
		}
	}

	private class PlayImageControlContainer
	{
		public PlayImageControl Header { get; set; }

		public PlayImageControl Focused { get; set; }

		public PlayImageControl Footer { get; set; }

		public void Append(PlayImageControl bitmapInfo)
		{
			if (Header == null)
			{
				Header = bitmapInfo;
				Header.Next = bitmapInfo;
				Footer = bitmapInfo;
			}
			else
			{
				Footer.Next = bitmapInfo;
				bitmapInfo.Next = Header;
				Footer = bitmapInfo;
			}
		}

		public void SetNextToFocused()
		{
			if (Focused == null)
			{
				Focused = Header;
			}
			else
			{
				Focused = Focused.Next;
			}
		}

		public PlayImageControl GetFocusedNext()
		{
			if (Focused == null)
			{
				return Header;
			}
			return Focused.Next;
		}

		public void ReSetScaling(double scaling)
		{
			if (Header == null)
			{
				return;
			}
			PlayImageControl playImageControl = Header;
			while (true)
			{
				playImageControl.ReSetScaling(scaling);
				if (Footer.Next != Header)
				{
					playImageControl = playImageControl.Next;
					continue;
				}
				break;
			}
		}

		public void Clear()
		{
			Header = null;
			Focused = null;
		}
	}

	private class GifBitmapInfo
	{
		public IList<GifFrameInfo> FrameInfoList { get; set; }

		public ushort GifRawHeight { get; set; }

		public ushort GifRawWidth { get; set; }
	}

	private class Player : IDisposable
	{
		private GifPanel m_outer;

		private double m_scaling = 1.0;

		private Grid _BtnGrid;

		private GifBitmapInfo m_gifInfo;

		private PlayImageControlContainer m_imageControls;

		private volatile bool m_isDisposed;

		public double Scaling
		{
			get
			{
				return m_scaling;
			}
			set
			{
				if (value <= 0.0 || value > 1.0)
				{
					m_scaling = 1.0;
				}
				else
				{
					m_scaling = value;
				}
			}
		}

		public Button BtnNext { get; private set; }

		public Button BtnBack { get; private set; }

		public GifBitmapInfo GifInfo => m_gifInfo;

		public Player(GifPanel outer, Stream playStream)
		{
			Player player = this;
			m_outer = outer;
			Button button = new Button();
			button.Width = 64.0;
			button.Height = 64.0;
			button.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
			button.HorizontalAlignment = HorizontalAlignment.Center;
			button.Style = Application.Current.FindResource("RedioBtnKey") as Style;
			button.Click += delegate
			{
				outer.FinishCommand?.Execute(false);
				player.Play(outer.ActualWidth, outer.ActualHeight);
			};
			BtnNext = new Button();
			BtnNext.Width = 64.0;
			BtnNext.Height = 64.0;
			BtnNext.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
			BtnNext.HorizontalAlignment = HorizontalAlignment.Right;
			BtnNext.Style = Application.Current.FindResource("NextCircleBtnKey") as Style;
			BtnNext.IsEnabled = outer.IsNextEnable;
			BtnNext.Click += delegate
			{
				player.m_outer.NextCommand.Execute(true);
			};
			BtnBack = new Button();
			BtnBack.Width = 64.0;
			BtnBack.Height = 64.0;
			BtnBack.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
			BtnBack.HorizontalAlignment = HorizontalAlignment.Left;
			BtnBack.Style = Application.Current.FindResource("BackCircleBtnKey") as Style;
			BtnBack.IsEnabled = outer.IsBackEnable;
			BtnBack.Click += delegate
			{
				player.m_outer.BackCommand.Execute(false);
			};
			_BtnGrid = new Grid
			{
				Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0))
			};
			_BtnGrid.Children.Add(BtnBack);
			_BtnGrid.Children.Add(button);
			_BtnGrid.Children.Add(BtnNext);
			m_gifInfo = LoadGifInfo(playStream);
		}

		private double CalculateScaling(double panelActualWidth, double panelActualHeight)
		{
			if (GifInfo == null)
			{
				return 1.0;
			}
			double num = 0.0;
			if (panelActualWidth == 0.0 || panelActualHeight == 0.0 || GifInfo.GifRawWidth == 0 || GifInfo.GifRawHeight == 0)
			{
				return 1.0;
			}
			double num2 = panelActualWidth / (double)(int)GifInfo.GifRawWidth;
			double num3 = panelActualHeight / (double)(int)GifInfo.GifRawHeight;
			num = ((num2 > num3) ? num3 : num2);
			if (num == 0.0)
			{
				num = 1.0;
			}
			return num;
		}

		public void ResetScaling(double panelActualWidth, double panelActualHeight)
		{
			Scaling = CalculateScaling(panelActualWidth, panelActualHeight);
			if (m_imageControls != null)
			{
				m_imageControls.ReSetScaling(Scaling);
			}
		}

		private PlayImageControlContainer CreatePlayImageControls(GifBitmapInfo GifInfo, double scaling)
		{
			if (GifInfo == null)
			{
				return null;
			}
			if (GifInfo.FrameInfoList == null || GifInfo.FrameInfoList.Count == 0)
			{
				return null;
			}
			PlayImageControlContainer playImageControlContainer = new PlayImageControlContainer();
			PlayImageControl playImageControl2 = (playImageControlContainer.Header = new PlayImageControl(GifInfo.FrameInfoList[0], scaling));
			PlayImageControl playImageControl3 = playImageControl2;
			for (int i = 1; i < GifInfo.FrameInfoList.Count; i++)
			{
				playImageControl3.Next = new PlayImageControl(GifInfo.FrameInfoList[i], scaling);
				playImageControl3 = playImageControl3.Next;
			}
			playImageControlContainer.Footer = playImageControl3;
			playImageControl3.Next = playImageControlContainer.Header;
			return playImageControlContainer;
		}

		public void Play(double panelActualWidth, double panelActualHeight)
		{
			Scaling = CalculateScaling(panelActualWidth, panelActualHeight);
			m_imageControls = CreatePlayImageControls(GifInfo, Scaling);
			if (m_imageControls == null)
			{
				return;
			}
			m_isDisposed = false;
			if (m_outer.IsRepeatFoever)
			{
				Task.Factory.StartNew(delegate
				{
					try
					{
						while (!m_isDisposed)
						{
							PlayImageControl playImage = m_imageControls.GetFocusedNext();
							if (playImage == null)
							{
								break;
							}
							m_outer.Dispatcher.Invoke(delegate
							{
								if (playImage == m_imageControls.Header)
								{
									m_outer.Children.Clear();
								}
								if (playImage.FrameInfo.Disposal == 2)
								{
									m_outer.Children.Clear();
								}
								m_imageControls.SetNextToFocused();
								Canvas.SetLeft(playImage.Image, playImage.Left);
								Canvas.SetTop(playImage.Image, playImage.Top);
								m_outer.Children.Add(playImage.Image);
								if (m_isDisposed)
								{
									m_outer.Children.Clear();
								}
								if (!m_outer.IsVisible)
								{
									m_isDisposed = true;
								}
							});
							Thread.Sleep(TimeSpan.FromMilliseconds(playImage.FrameInfo.DelayDuration));
						}
					}
					catch
					{
					}
				});
				return;
			}
			m_outer.Children.Clear();
			Task.Run(delegate
			{
				while (!m_isDisposed)
				{
					PlayImageControl temp = m_imageControls.GetFocusedNext();
					if (temp == null)
					{
						break;
					}
					m_outer.Dispatcher.Invoke(delegate
					{
						if (temp.FrameInfo.Disposal == 2)
						{
							m_outer.Children.Clear();
						}
						m_imageControls.SetNextToFocused();
						Canvas.SetLeft(temp.Image, temp.Left);
						Canvas.SetTop(temp.Image, temp.Top);
						m_outer.Children.Add(temp.Image);
						if (temp == m_imageControls.Footer)
						{
							_BtnGrid.Width = m_imageControls.Header.Image.Width;
							_BtnGrid.Height = m_imageControls.Header.Image.Height;
							Canvas.SetLeft(_BtnGrid, m_imageControls.Header.Left);
							Canvas.SetTop(_BtnGrid, m_imageControls.Header.Top);
							if (m_outer.NextCommand == null && m_outer.BackCommand == null)
							{
								BtnNext.Visibility = Visibility.Collapsed;
								BtnBack.Visibility = Visibility.Collapsed;
							}
							else
							{
								BtnNext.Visibility = Visibility.Visible;
								BtnBack.Visibility = Visibility.Visible;
							}
							m_outer.Children.Add(_BtnGrid);
							m_outer.FinishCommand?.Execute(true);
							m_isDisposed = true;
						}
					});
					Thread.Sleep(TimeSpan.FromMilliseconds(temp.FrameInfo.DelayDuration));
				}
			});
		}

		private void fun(BitmapFrame frame, int index, int mode)
		{
			PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
			pngBitmapEncoder.Frames.Add(frame);
			using FileStream fileStream = new FileStream($"C:\\Users\\jiong.feng\\Desktop\\123\\{index}_{mode}.png", FileMode.Create);
			pngBitmapEncoder.Save(fileStream);
			fileStream.Flush();
			fileStream.Close();
		}

		private GifBitmapInfo LoadGifInfo(Stream stream)
		{
			if (stream == null)
			{
				return null;
			}
			GifBitmapInfo gifBitmapInfo = new GifBitmapInfo();
			List<GifFrameInfo> list2 = (List<GifFrameInfo>)(gifBitmapInfo.FrameInfoList = new List<GifFrameInfo>());
			try
			{
				GifBitmapDecoder gifBitmapDecoder = new GifBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
				gifBitmapInfo.GifRawHeight = Convert.ToUInt16(gifBitmapDecoder.Metadata.GetQuery("/logscrdesc/Height"));
				gifBitmapInfo.GifRawWidth = Convert.ToUInt16(gifBitmapDecoder.Metadata.GetQuery("/logscrdesc/Width"));
				foreach (BitmapFrame frame in gifBitmapDecoder.Frames)
				{
					BitmapMetadata obj = (BitmapMetadata)frame.Metadata;
					ushort num = Convert.ToUInt16(obj.GetQuery("/imgdesc/Left"));
					ushort num2 = Convert.ToUInt16(obj.GetQuery("/imgdesc/Top"));
					ushort num3 = Convert.ToUInt16(obj.GetQuery("/imgdesc/Width"));
					ushort num4 = Convert.ToUInt16(obj.GetQuery("/imgdesc/Height"));
					ushort num5 = Convert.ToUInt16(obj.GetQuery("/grctlext/Delay"));
					int disposal = Convert.ToInt32(obj.GetQuery("/grctlext/Disposal"));
					list2.Add(new GifFrameInfo(m_outer)
					{
						FrameSource = frame,
						Left = (int)num,
						Top = (int)num2,
						Width = (int)num3,
						Height = (int)num4,
						Delay = num5,
						Disposal = disposal,
						DelayDuration = (uint)(num5 * 10)
					});
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				stream.Dispose();
				stream.Close();
			}
			return gifBitmapInfo;
		}

		public void Dispose()
		{
			m_isDisposed = true;
			Thread.Sleep(500);
			m_gifInfo?.FrameInfoList.Clear();
			m_gifInfo = null;
			m_imageControls?.Clear();
		}
	}

	public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register("MyProperty", typeof(string), typeof(GifPanel), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty UriImageSourceProperty = DependencyProperty.Register("UriImageSource", typeof(Uri), typeof(GifPanel), new PropertyMetadata(null, OnUriImageSourceProperyValueChanged));

	public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(GifPanel), new PropertyMetadata(string.Empty, delegate(DependencyObject dependobj, DependencyPropertyChangedEventArgs dependProperty)
	{
		if (dependProperty.NewValue != dependProperty.OldValue)
		{
			(dependobj as GifPanel).OnImagePathChanged();
		}
	}));

	public static readonly DependencyProperty IsRepeatFoeverProperty = DependencyProperty.Register("IsRepeatFoever", typeof(bool), typeof(GifPanel), new PropertyMetadata(true));

	public static readonly DependencyProperty NextCommandProperty = DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(GifPanel), new PropertyMetadata((object)null));

	public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(GifPanel), new PropertyMetadata((object)null));

	public static readonly DependencyProperty FinishCommandProperty = DependencyProperty.Register("FinishCommand", typeof(ICommand), typeof(GifPanel), new PropertyMetadata((object)null));

	public static readonly DependencyProperty IsNextEnableProperty = DependencyProperty.Register("IsNextEnable", typeof(bool), typeof(GifPanel), new PropertyMetadata(false, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue != e.OldValue && sender is GifPanel { m_currentPlayer: not null } gifPanel && gifPanel.m_currentPlayer.BtnNext != null)
		{
			gifPanel.m_currentPlayer.BtnNext.IsEnabled = (bool)e.NewValue;
		}
	}));

	public static readonly DependencyProperty IsBackEnableroperty = DependencyProperty.Register("IsBackEnable", typeof(bool), typeof(GifPanel), new PropertyMetadata(false, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue != e.OldValue && sender is GifPanel { m_currentPlayer: not null } gifPanel2 && gifPanel2.m_currentPlayer.BtnBack != null)
		{
			gifPanel2.m_currentPlayer.BtnBack.IsEnabled = (bool)e.NewValue;
		}
	}));

	private Player m_currentPlayer;

	private readonly object m_currentPlayerLock = new object();

	public string MyProperty
	{
		get
		{
			return (string)GetValue(MyPropertyProperty);
		}
		set
		{
			SetValue(MyPropertyProperty, value);
		}
	}

	public Uri UriImageSource
	{
		get
		{
			return (Uri)GetValue(UriImageSourceProperty);
		}
		set
		{
			SetValue(UriImageSourceProperty, value);
		}
	}

	public string ImagePath
	{
		get
		{
			return (string)GetValue(ImagePathProperty);
		}
		set
		{
			SetValue(ImagePathProperty, value);
		}
	}

	public bool IsRepeatFoever
	{
		get
		{
			return (bool)GetValue(IsRepeatFoeverProperty);
		}
		set
		{
			SetValue(IsRepeatFoeverProperty, value);
		}
	}

	public ICommand NextCommand
	{
		get
		{
			return (ICommand)GetValue(NextCommandProperty);
		}
		set
		{
			SetValue(NextCommandProperty, value);
		}
	}

	public ICommand BackCommand
	{
		get
		{
			return (ICommand)GetValue(BackCommandProperty);
		}
		set
		{
			SetValue(BackCommandProperty, value);
		}
	}

	public ICommand FinishCommand
	{
		get
		{
			return (ICommand)GetValue(FinishCommandProperty);
		}
		set
		{
			SetValue(FinishCommandProperty, value);
		}
	}

	public bool IsNextEnable
	{
		get
		{
			return (bool)GetValue(IsNextEnableProperty);
		}
		set
		{
			SetValue(IsNextEnableProperty, value);
		}
	}

	public bool IsBackEnable
	{
		get
		{
			return (bool)GetValue(IsBackEnableroperty);
		}
		set
		{
			SetValue(IsBackEnableroperty, value);
		}
	}

	public bool IsDisposed { get; private set; }

	public GifPanel()
	{
		base.SizeChanged += GifPanel_SizeChanged;
		base.Loaded += GifPanel_Loaded;
	}

	private void GifPanel_Loaded(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(ImagePath) || UriImageSource != null)
		{
			ImagePathChangedPlayHandler();
		}
	}

	private void GifPanel_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (!e.NewSize.IsEmpty && e.NewSize.Width > 0.0 && e.NewSize.Height > 0.0)
		{
			PanelSizeChangedPlayHandler();
		}
	}

	private Stream GetStream(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return null;
		}
		if (!File.Exists(fileName))
		{
			return null;
		}
		return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	private Stream GetStream(Uri uri)
	{
		try
		{
			return Application.GetResourceStream(uri)?.Stream;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void OnImagePathChanged()
	{
		ImagePathChangedPlayHandler();
	}

	private static void OnUriImageSourceProperyValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue != e.OldValue)
		{
			(obj as GifPanel).OnImagePathChanged();
		}
	}

	private void ImagePathChangedPlayHandler()
	{
		if (base.ActualWidth <= 0.0 || base.ActualHeight <= 0.0)
		{
			return;
		}
		lock (m_currentPlayerLock)
		{
			if (m_currentPlayer != null)
			{
				m_currentPlayer.Dispose();
				base.Children.Clear();
			}
			Stream playStream = null;
			if (UriImageSource != null)
			{
				playStream = GetStream(UriImageSource);
			}
			else if (!string.IsNullOrWhiteSpace(ImagePath))
			{
				playStream = GetStream(ImagePath);
			}
			m_currentPlayer = new Player(this, playStream);
			m_currentPlayer.Play(base.ActualWidth, base.ActualHeight);
		}
	}

	private void PanelSizeChangedPlayHandler()
	{
		if (base.ActualWidth <= 0.0 || base.ActualHeight <= 0.0)
		{
			return;
		}
		lock (m_currentPlayerLock)
		{
			if (m_currentPlayer == null)
			{
				m_currentPlayer = new Player(this, GetStream(ImagePath));
				m_currentPlayer.Play(base.ActualWidth, base.ActualHeight);
			}
			else
			{
				m_currentPlayer.ResetScaling(base.ActualWidth, base.ActualHeight);
			}
		}
	}

	public void Dispose()
	{
		IsDisposed = true;
		if (m_currentPlayer != null)
		{
			m_currentPlayer.Dispose();
		}
	}
}
