using System;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.pipes;

public abstract class BasicPipe
{
	protected PipeStream pipeStream;

	protected Action<BasicPipe> asyncReaderStart;

	protected virtual string PipeName { get; set; }

	public event EventHandler<PipeEventArgs> DataReceived;

	public event EventHandler<EventArgs> PipeClosed;

	public BasicPipe()
	{
		PipeName = "LMSAPIPE";
	}

	public void StartStringReaderAsync()
	{
		StartByteReaderAsync(delegate(byte[] btyes)
		{
			string json = Encoding.UTF8.GetString(btyes).TrimEnd(default(char));
			PipeEventArgs e = DeserializeJson2Object<PipeEventArgs>(json);
			this.DataReceived?.Invoke(this, e);
		});
	}

	public Task WriteStringAsync(PipeMessage message, object data)
	{
		string s = SerializeObject2Json(new PipeEventArgs
		{
			message = message,
			data = data
		});
		return WriteBytesAsync(Encoding.UTF8.GetBytes(s));
	}

	public void Close()
	{
		if (pipeStream != null)
		{
			if (pipeStream.IsConnected)
			{
				pipeStream.WaitForPipeDrain();
			}
			pipeStream.Close();
			pipeStream.Dispose();
			pipeStream = null;
		}
	}

	protected void StartByteReaderAsync(Action<byte[]> packetReceived)
	{
		int num = 4;
		byte[] bDataLength = new byte[num];
		pipeStream.ReadAsync(bDataLength, 0, num).ContinueWith(delegate(Task<int> t)
		{
			int len = t.Result;
			if (len == 0)
			{
				this.PipeClosed?.Invoke(this, EventArgs.Empty);
			}
			else
			{
				int num2 = BitConverter.ToInt32(bDataLength, 0);
				byte[] data = new byte[num2];
				pipeStream.ReadAsync(data, 0, num2).ContinueWith(delegate(Task<int> t2)
				{
					len = t2.Result;
					if (len == 0)
					{
						this.PipeClosed?.Invoke(this, EventArgs.Empty);
					}
					else
					{
						packetReceived(data);
						StartByteReaderAsync(packetReceived);
					}
				});
			}
		});
	}

	protected Task WriteBytesAsync(byte[] bytes)
	{
		if (pipeStream.IsConnected)
		{
			byte[] array = BitConverter.GetBytes(bytes.Length).Concat(bytes).ToArray();
			return pipeStream.WriteAsync(array, 0, array.Length);
		}
		return null;
	}

	protected T DeserializeJson2Object<T>(string json) where T : class, new()
	{
		try
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
		catch
		{
			return null;
		}
	}

	protected string SerializeObject2Json(object obj)
	{
		try
		{
			return JsonConvert.SerializeObject(obj);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}
}
