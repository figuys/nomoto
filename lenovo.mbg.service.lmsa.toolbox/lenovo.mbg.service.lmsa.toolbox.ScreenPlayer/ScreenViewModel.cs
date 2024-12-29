using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class ScreenViewModel : NotifyBase
{
	private string _AuthorizeImagePath;

	private Visibility _IsAuthorize1Visible;

	private Visibility _IsUsbModelVisible;

	private ScreenView _Win;

	private byte[] buff;

	private Int32Rect rectUpdate;

	private WriteableBitmap _Bitmap;

	private AVDecoderHelper decoderHelper;

	private WebSocketSession socketSession;

	private WebSocketServer webSocket;

	private Timer tipTimer;

	private Timer tickTimer;

	private int tickCount;

	private bool isStartMirror;

	private bool isVerticalMode;

	private bool isNormalState;

	private volatile bool IsDecoderUnEable;

	private object async_lock = new object();

	private Stopwatch watch = new Stopwatch();

	public ReplayCommand MinCommand { get; set; }

	public ReplayCommand MaxCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public string AuthorizeImagePath
	{
		get
		{
			return _AuthorizeImagePath;
		}
		set
		{
			_AuthorizeImagePath = value;
			OnPropertyChanged("AuthorizeImagePath");
		}
	}

	public Visibility IsAuthorize1Visible
	{
		get
		{
			return _IsAuthorize1Visible;
		}
		set
		{
			_IsAuthorize1Visible = value;
			OnPropertyChanged("IsAuthorize1Visible");
		}
	}

	public Visibility IsUsbModelVisible
	{
		get
		{
			return _IsUsbModelVisible;
		}
		set
		{
			_IsUsbModelVisible = value;
			OnPropertyChanged("IsUsbModelVisible");
		}
	}

	public WriteableBitmap Bitmap
	{
		get
		{
			return _Bitmap;
		}
		set
		{
			_Bitmap = value;
			OnPropertyChanged("Bitmap");
		}
	}

	public Action<Size> OnClientStartEvent { get; set; }

	public bool IsUsbModel { get; set; }

	public ScreenViewModel(ScreenView win)
	{
		_Win = win;
		isStartMirror = false;
		isNormalState = true;
		IsDecoderUnEable = true;
		IsAuthorize1Visible = Visibility.Collapsed;
		_IsUsbModelVisible = Visibility.Collapsed;
		AuthorizeImagePath = "pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/authorize.png";
		MinCommand = new ReplayCommand(delegate
		{
			_Win.WindowState = WindowState.Minimized;
		});
		MaxCommand = new ReplayCommand(delegate
		{
			isNormalState = !isNormalState;
			SetWindowSize(isNormalState);
		});
		CloseCommand = new ReplayCommand(delegate
		{
			WebSocketSession webSocketSession = socketSession;
			if (webSocketSession != null && webSocketSession.Connected)
			{
				socketSession.Send("SERVER_STOP");
			}
			Stop();
			NotifyStopCmd();
			lock (async_lock)
			{
				decoderHelper?.Release();
				IsDecoderUnEable = true;
			}
			_Win.Close();
		});
		buff = new byte[1048576];
		tickTimer = new Timer();
		tickTimer.Interval = 2000.0;
		tickTimer.Elapsed += delegate
		{
			tickCount++;
			WebSocketSession webSocketSession2 = socketSession;
			if (webSocketSession2 == null || !webSocketSession2.Connected)
			{
				tickTimer.Stop();
				_Win.Dispatcher.Invoke(delegate
				{
					CloseCommand.Execute(_Win);
					MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, "K1120", MessageBoxButton.OK);
				});
			}
			else if (isStartMirror && tickCount > 30)
			{
				tickTimer.Stop();
				_Win.Dispatcher.Invoke(delegate
				{
					if (MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, "K1114") == true)
					{
						CloseCommand.Execute(_Win);
					}
				});
			}
		};
		tipTimer = new Timer();
		tipTimer.Interval = 5000.0;
		tipTimer.Elapsed += delegate
		{
			tipTimer.Stop();
			_Win.Dispatcher.Invoke(delegate
			{
				IsUsbModelVisible = Visibility.Collapsed;
			});
		};
	}

	public void InitFFmpeg(int srcWidth, int srcHeight)
	{
		IsDecoderUnEable = true;
		decoderHelper = new AVDecoderHelper();
		InitBitmapAndDecorder(srcWidth, srcHeight);
		SetWindowSize(isNormalState);
		IsDecoderUnEable = false;
	}

	public void SocketProc(Window wnd, int time)
	{
		if (time != 0)
		{
			return;
		}
		webSocket = new WebSocketServer();
		webSocket.NewSessionConnected += delegate(WebSocketSession session)
		{
			WebSocketSession webSocketSession = socketSession;
			if (webSocketSession == null || !webSocketSession.Connected)
			{
				socketSession = session;
				tickTimer.Start();
			}
			else
			{
				session.Send("CMD_ANOTHER_CONNECTION_EXISTED");
				session.Close(CloseReason.Unknown);
			}
		};
		webSocket.SessionClosed += delegate(WebSocketSession session, CloseReason value)
		{
			if (socketSession?.SessionID == session.SessionID)
			{
				socketSession = null;
			}
		};
		webSocket.NewMessageReceived += delegate(WebSocketSession session, string msg)
		{
			if (!(socketSession?.SessionID != session.SessionID))
			{
				OnNetMsgCommand(msg, session);
			}
		};
		webSocket.NewDataReceived += delegate(WebSocketSession session, byte[] data)
		{
			if (!IsDecoderUnEable && session.Connected && !(socketSession?.SessionID != session.SessionID))
			{
				tickCount = 0;
				lock (async_lock)
				{
					if (!decoderHelper.DecodeFrame(data, data.Length - 4))
					{
						return;
					}
				}
				OnUpdateScreenAfterDataRecv();
			}
		};
		webSocket.Setup(new ServerConfig
		{
			Ip = "Any",
			Port = 10086,
			MaxConnectionNumber = 4,
			MaxRequestLength = 5242880,
			ReceiveBufferSize = 5242880
		});
		webSocket.Start();
	}

	public void Stop()
	{
		tickTimer?.Stop();
		IsDecoderUnEable = true;
		socketSession?.Close();
		webSocket?.Stop();
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern void RtlMoveMemory(IntPtr dest, IntPtr source, int size);

	private void OnUpdateScreenAfterDataRecv()
	{
		Application.Current.Dispatcher.InvokeAsync(delegate
		{
			try
			{
				Bitmap.Lock();
				Bitmap.AddDirtyRect(rectUpdate);
			}
			finally
			{
				Bitmap.Unlock();
			}
		});
	}

	private void OnNetMsgCommand(string message, WebSocketSession session)
	{
		_Win.Dispatcher.BeginInvoke((Action)delegate
		{
			string[] array = message.Split('&', '#');
			switch (array[0])
			{
			case "CMD_STOP_WEBSOCKET":
			case "CMD_CLIENT_CANCEL":
				isStartMirror = false;
				CloseCommand.Execute(_Win);
				break;
			case "CMD_START_NOW":
				isStartMirror = true;
				IsUsbModelVisible = ((!IsUsbModel) ? Visibility.Collapsed : Visibility.Visible);
				IsAuthorize1Visible = Visibility.Collapsed;
				tipTimer.Start();
				break;
			case "CMD_SCREEN_SPIN":
				IsDecoderUnEable = true;
				InitBitmapAndDecorder(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
				SetWindowSize(isNormalState);
				IsDecoderUnEable = false;
				break;
			case "CMD_CLIENT_START":
				OnClientStartEvent?.Invoke(new Size(Convert.ToInt32(array[1]), Convert.ToInt32(array[2])));
				break;
			case "CMD_CAPTURE_AUTHORIZE":
				IsAuthorize1Visible = Visibility.Visible;
				if (Convert.ToInt32(array[1]) >= 10)
				{
					AuthorizeImagePath = "pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/authorize.png";
				}
				else
				{
					AuthorizeImagePath = "pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/authorize3.png";
				}
				break;
			}
		});
	}

	public bool NotifyStopCmd()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager?.CreateMessageReaderAndWriter(2000);
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		try
		{
			if (messageReaderAndWriter.SendAndReceiveSync("stopScreenCapture", "stopScreenCaptureResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void SaveXXX(WriteableBitmap wtbBmp)
	{
		if (wtbBmp == null)
		{
			return;
		}
		try
		{
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(wtbBmp.PixelWidth, wtbBmp.PixelHeight, wtbBmp.DpiX, wtbBmp.DpiY, PixelFormats.Default);
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawImage(wtbBmp, new Rect(0.0, 0.0, wtbBmp.Width, wtbBmp.Height));
			}
			renderTargetBitmap.Render(drawingVisual);
			JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
			jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			string text = "D:\\XXX\\";
			string path = text + DateTime.Now.ToString("yyyyMMddfff") + ".jpg";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!File.Exists(path))
			{
				jpegBitmapEncoder.Save(File.OpenWrite(path));
			}
		}
		catch (Exception)
		{
		}
	}

	private unsafe void UpdateUI()
	{
		Random random = new Random();
		byte* ptr = (byte*)(void*)Bitmap.BackBuffer;
		for (int i = 0; i < 1280; i++)
		{
			for (int j = 0; j < 720; j++)
			{
				*ptr = (byte)random.Next(255);
				ptr++;
				*ptr = (byte)random.Next(255);
				ptr++;
				*ptr = (byte)random.Next(255);
				ptr++;
				*ptr = byte.MaxValue;
				ptr++;
			}
		}
		Bitmap.Lock();
		Bitmap.AddDirtyRect(rectUpdate);
		Bitmap.Unlock();
	}

	private unsafe void InitBitmapAndDecorder(int wScene, int hScene)
	{
		int num = 0;
		int num2 = 0;
		isVerticalMode = wScene < hScene;
		wScene = 16 * (wScene / 16);
		hScene = 2 * (hScene / 2);
		if (isVerticalMode)
		{
			num = ((wScene > 720) ? 720 : wScene);
			num2 = ((wScene > 720) ? (720 * hScene / wScene) : hScene);
		}
		else
		{
			num = ((hScene > 720) ? (720 * wScene / hScene) : wScene);
			num2 = ((hScene > 720) ? 720 : hScene);
		}
		lock (async_lock)
		{
			rectUpdate = new Int32Rect(0, 0, num, num2);
			Bitmap = new WriteableBitmap(num, num2, 96.0, 96.0, PixelFormats.Bgra32, null);
			decoderHelper.SizeChanged(num, num2, (byte*)(void*)Bitmap.BackBuffer, wScene, hScene);
		}
	}

	private void SetWindowSize(bool isNormalSize)
	{
		int num = 0;
		int num2 = 0;
		if (isVerticalMode)
		{
			num = 700 * (int)Bitmap.Width / (int)Bitmap.Height;
			num2 = 700;
		}
		else
		{
			num = 700;
			num2 = 700 * (int)Bitmap.Height / (int)Bitmap.Width;
		}
		if (isNormalSize)
		{
			_Win.Width = num;
			_Win.Height = num2;
			_Win.img.Width = num;
			_Win.img.Height = num2;
			_Win.Left = (SystemParameters.WorkArea.Width - (double)num) / 2.0;
			_Win.Top = (SystemParameters.WorkArea.Height - (double)num2) / 2.0;
			_Win.WindowState = WindowState.Normal;
			return;
		}
		_Win.WindowState = WindowState.Maximized;
		if (isVerticalMode)
		{
			_Win.img.Width = (double)num * _Win.Height / (double)num2;
			_Win.img.Height = _Win.Height;
			return;
		}
		double num3 = _Win.Width * (double)num2 / (double)num;
		if (num3 < _Win.Height)
		{
			_Win.img.Width = _Win.Width;
			_Win.img.Height = num3;
		}
		else
		{
			_Win.img.Width = _Win.Height * (double)num / (double)num2;
			_Win.img.Height = _Win.Height;
		}
	}
}
