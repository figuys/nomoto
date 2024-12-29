using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion.download;

public class HttpDownload
{
	private Action<VersionV1EventArgs> eventHandler;

	private VersionModel Model;

	private long totalSizeOfSec;

	private volatile bool KeepRunning = true;

	public HttpDownload(VersionModel model, Action<VersionV1EventArgs> handler)
	{
		Model = model;
		eventHandler = handler;
	}

	public void Start()
	{
		Task.Factory.StartNew(delegate
		{
			KeepRunning = true;
			eventHandler(new VersionV1EventArgs(VersionV1Status.VERSION_DOWNLOADING, Model));
			int num = 3;
			if (CheckPre())
			{
				while (!Download())
				{
					LogHelper.LogInstance.Info("download failed, retry again");
					if (!KeepRunning || --num <= 0)
					{
						break;
					}
				}
				CheckAft();
			}
		});
	}

	public void Stop()
	{
		KeepRunning = false;
	}

	private bool Download()
	{
		System.Timers.Timer timer = RegisterTimer();
		try
		{
			HttpWebRequest httpWebRequest = CreateRequest(Model.url, 30000);
			httpWebRequest.AddRange(Model.downloadedSize);
			using (FileStream fileStream = new FileStream(Model.localPath + ".tmp", FileMode.Append, FileAccess.Write))
			{
				using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream stream = httpWebResponse.GetResponseStream();
				try
				{
					stream.ReadTimeout = 60000;
					int num = 1024;
					byte[][] array = new byte[2][]
					{
						new byte[num],
						new byte[num]
					};
					int num2 = 0;
					int readCount = 0;
					ManualResetEvent manualReset = new ManualResetEvent(initialState: true);
					timer.Enabled = true;
					do
					{
						readCount = stream.Read(array[num2], 0, num);
						manualReset.WaitOne();
						if (readCount > 0 && Model.size > Model.downloadedSize)
						{
							manualReset.Reset();
							fileStream.BeginWrite(array[num2], 0, readCount, delegate(IAsyncResult ar)
							{
								stream.EndWrite(ar);
								Model.downloadedSize += readCount;
								totalSizeOfSec += readCount;
								manualReset.Set();
							}, null);
							num2 ^= 1;
						}
					}
					while (KeepRunning && readCount > 0);
					stream.Flush();
				}
				finally
				{
					if (stream != null)
					{
						((IDisposable)stream).Dispose();
					}
				}
			}
			return Model.size == Model.downloadedSize;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
			return false;
		}
		finally
		{
			timer.Enabled = false;
			timer.Dispose();
		}
	}

	private bool CheckPre()
	{
		if (File.Exists(Model.localPath + ".tmp"))
		{
			File.Delete(Model.localPath + ".tmp");
		}
		if (File.Exists(Model.localPath))
		{
			string md5Hash = GlobalFun.GetMd5Hash(Model.localPath);
			if (!string.IsNullOrEmpty(md5Hash) && !string.IsNullOrEmpty(Model.md5) && Model.md5.Equals(md5Hash))
			{
				eventHandler(new VersionV1EventArgs(VersionV1Status.VERSION_DOWNLOADING_SUCCESS, Model));
				return false;
			}
			File.Delete(Model.localPath);
		}
		if (Model.size <= 0)
		{
			if (!GetSize(Model.url, out var filesize))
			{
				LogHelper.LogInstance.Info("get size failed, url " + Model.url);
				eventHandler(new VersionV1EventArgs(VersionV1Status.VERSION_DOWNLOADING_FAILED, Model));
				return false;
			}
			Model.size = filesize;
		}
		return true;
	}

	private bool CheckAft()
	{
		string text = Model.localPath + ".tmp";
		if (File.Exists(text))
		{
			string md5Hash = GlobalFun.GetMd5Hash(text);
			if (!string.IsNullOrEmpty(md5Hash) && !string.IsNullOrEmpty(Model.md5) && Model.md5.Equals(md5Hash))
			{
				GlobalFun.FileRename(text, Model.localPath);
				eventHandler(new VersionV1EventArgs(VersionV1Status.VERSION_DOWNLOADING_SUCCESS, Model));
				return true;
			}
		}
		eventHandler(new VersionV1EventArgs(VersionV1Status.VERSION_DOWNLOADING_FAILED, Model));
		return false;
	}

	private bool GetSize(string url, out long filesize)
	{
		filesize = 0L;
		try
		{
			if (!url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
			{
				url = "http://" + url;
			}
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
			obj.Method = "HEAD";
			using WebResponse webResponse = obj.GetResponse();
			filesize = webResponse.ContentLength;
		}
		catch (WebException)
		{
			try
			{
				long result = 0L;
				HttpWebRequest obj2 = WebRequest.Create(url) as HttpWebRequest;
				obj2.Method = "GET";
				using HttpWebResponse httpWebResponse = (HttpWebResponse)obj2.GetResponse();
				if (long.TryParse(httpWebResponse.Headers["Content-Length"], out result))
				{
					filesize = result;
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	private System.Timers.Timer RegisterTimer()
	{
		System.Timers.Timer timer = new System.Timers.Timer(1500.0);
		timer.Elapsed += SpeedSacnHandler;
		timer.Enabled = false;
		return timer;
	}

	private void SpeedSacnHandler(object sender, ElapsedEventArgs e)
	{
		lock (this)
		{
			long bytes = totalSizeOfSec;
			totalSizeOfSec = 0L;
			Model.speed = GlobalFun.ConvertLong2String(bytes);
			eventHandler(new VersionV1EventArgs(VersionV1Status.VERSION_DOWNLOADING_PERCENT, Model.speed));
		}
	}

	private HttpWebRequest CreateRequest(string url, int timeout)
	{
		HttpWebRequest obj = WebRequest.Create(url) as HttpWebRequest;
		obj.Method = "GET";
		obj.Timeout = timeout;
		return obj;
	}
}
