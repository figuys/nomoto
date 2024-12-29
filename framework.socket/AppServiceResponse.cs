using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.socket;

public class AppServiceResponse : IDisposable
{
	public static int BytesBufferSize = 1048576;

	private readonly byte[] mByteBuffer = new byte[BytesBufferSize];

	private readonly byte[] splitBuffer = Encoding.UTF8.GetBytes("<EOF>");

	private AppServiceRequest mAppServiceRequest;

	private Socket mAppSocketClient;

	public RsaSocketDataSecurityFactory _RsaSocketEncryptHelper;

	private volatile bool mIsDisposed;

	public byte[] BytesBuffer => mByteBuffer;

	public bool IsDisposed
	{
		get
		{
			return mIsDisposed;
		}
		protected set
		{
			mIsDisposed = value;
		}
	}

	internal AppServiceResponse(AppServiceRequest request, RsaSocketDataSecurityFactory encryptHelper)
	{
		mAppServiceRequest = request;
		mAppSocketClient = request.GetRawAppSocketClient;
		_RsaSocketEncryptHelper = encryptHelper;
	}

	public int Read(byte[] bytes, int offset, int length)
	{
		if (mAppSocketClient == null)
		{
			return 0;
		}
		return mAppSocketClient.Receive(bytes, offset, length, SocketFlags.None);
	}

	public long ReadStream(byte[] output, RsaSocketDataSecurityFactory encryptHelper, long targetLength, Action<int, long, long> progress)
	{
		long readTotal = 0L;
		ReadStream(targetLength, encryptHelper, delegate(byte[] bytes, int rl, long rtl, long tl)
		{
			Array.Copy(bytes, 0L, output, readTotal, rl);
			readTotal += rl;
			progress?.Invoke(rl, rtl, tl);
		});
		return readTotal;
	}

	public long ReadStream(long targetLength, RsaSocketDataSecurityFactory encryptHelper, Action<byte[], int, long, long> progress)
	{
		if (!_RsaSocketEncryptHelper.IsSecurity)
		{
			return ReadStreamOld(targetLength, progress);
		}
		int num = 0;
		long num2 = 0L;
		byte[] bytesBuffer = BytesBuffer;
		MemoryStream memoryStream = new MemoryStream();
		string text = CustomConvert.Instance.BytesToHex(Encoding.UTF8.GetBytes("<EOF>"));
		string empty = string.Empty;
		while (!IsDisposed && mAppSocketClient.Connected && num2 < targetLength)
		{
			if ((num = Read(bytesBuffer, 0, BytesBufferSize)) > 0)
			{
				memoryStream.Write(bytesBuffer, 0, num);
				empty = CustomConvert.Instance.BytesToHex(memoryStream.ToArray());
				if (!empty.Contains(text))
				{
					continue;
				}
				string[] array = empty.Split(new string[1] { text }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array2 = CustomConvert.Instance.HexToBytes(array[i]);
					if (i == array.Length - 1 && !empty.EndsWith(text) && memoryStream.Length == 0L)
					{
						memoryStream.Write(array2, 0, array2.Length);
						continue;
					}
					byte[] array3 = _RsaSocketEncryptHelper.Decrypt(array2);
					num2 += array3.Length;
					progress?.Invoke(array3, array3.Length, num2, targetLength);
					memoryStream = new MemoryStream();
				}
			}
			else
			{
				if (num == 0 && mAppSocketClient.Available == 0)
				{
					break;
				}
				if (num == 0)
				{
					_ = mAppSocketClient.Available;
					_ = 0;
				}
			}
		}
		return num2;
	}

	public long ReadStreamOld(long targetLength, Action<byte[], int, long, long> progress)
	{
		int num = 0;
		long num2 = 0L;
		byte[] bytesBuffer = BytesBuffer;
		int bytesBufferSize = BytesBufferSize;
		MemoryStream memoryStream = new MemoryStream();
		string value = CustomConvert.Instance.BytesToHex(Encoding.UTF8.GetBytes("\\r\\n"));
		_ = string.Empty;
		while (!IsDisposed && mAppSocketClient.Connected && num2 < targetLength)
		{
			if ((num = Read(bytesBuffer, 0, bytesBufferSize)) > 0)
			{
				memoryStream.Write(bytesBuffer, 0, num);
				if (_RsaSocketEncryptHelper.IsSecurity)
				{
					if (CustomConvert.Instance.BytesToHex(memoryStream.ToArray()).Contains(value))
					{
						byte[] array = _RsaSocketEncryptHelper.DecryptBase64(memoryStream.ToArray());
						num2 += array.Length;
						progress?.Invoke(array, array.Length, num2, targetLength);
						memoryStream = new MemoryStream();
					}
				}
				else
				{
					byte[] array2 = memoryStream.ToArray();
					num2 += array2.Length;
					progress?.Invoke(array2, array2.Length, num2, targetLength);
					memoryStream = new MemoryStream();
				}
			}
			else
			{
				if (num == 0 && mAppSocketClient.Available == 0)
				{
					break;
				}
				if (num == 0)
				{
					_ = mAppSocketClient.Available;
					_ = 0;
				}
			}
		}
		return num2;
	}

	public Header ReadHeader(Action<int, long, long> progress)
	{
		int num = 0;
		byte[] array = new byte[BytesBufferSize];
		_ = BytesBufferSize;
		int length = (_RsaSocketEncryptHelper.IsSecurity ? 28 : 4);
		while (!mIsDisposed && (num = Read(array, 0, length)) == 0)
		{
			if (num == 0 && mAppSocketClient.Available == 0)
			{
				return new Header();
			}
			if (num == 0)
			{
				_ = mAppSocketClient.Available;
				_ = 0;
			}
		}
		int num2 = BitConverter.ToInt32(_RsaSocketEncryptHelper.DecryptBase64(array), 0);
		byte[] bytes = array;
		if (num2 > BytesBufferSize)
		{
			bytes = new byte[num2];
		}
		int num3 = 0;
		do
		{
			if ((num = Read(bytes, num3, num2)) > 0)
			{
				num3 += num;
				progress?.Invoke(num, num3, num2);
				continue;
			}
			if (num == 0 && mAppSocketClient.Available == 0)
			{
				return new Header();
			}
			if (num == 0)
			{
				_ = mAppSocketClient.Available;
				_ = 0;
			}
		}
		while (!mIsDisposed && mAppSocketClient.Connected && num3 < num2);
		string @string = Encoding.UTF8.GetString(bytes, 0, num3);
		@string = _RsaSocketEncryptHelper.DecryptFromBase64(@string);
		return new Header(JsonConvert.DeserializeObject<Dictionary<string, string>>(@string) ?? new Dictionary<string, string>());
	}

	public int Write(byte[] bytes)
	{
		return Write(bytes, 0, bytes.Length);
	}

	public int Write(byte[] bytes, int offset, int length)
	{
		return mAppSocketClient.Send(bytes, offset, length, SocketFlags.None);
	}

	public long WriteStream(Stream inputStream, Action<int, long, long> progress, RsaSocketDataSecurityFactory encryptHelper)
	{
		if (!_RsaSocketEncryptHelper.IsSecurity)
		{
			return WriteStreamOld(inputStream, progress);
		}
		long fileLength = inputStream.Length;
		Task<long> task = new Task<long>(delegate
		{
			long num = 0L;
			try
			{
				int num2 = (_RsaSocketEncryptHelper.IsSecurity ? 28 : 4);
				byte[] array = new byte[num2];
				int num3 = 0;
				int num4 = 0;
				while (!IsDisposed && mAppSocketClient.Connected && num < fileLength)
				{
					num4 = Read(array, 0, num2);
					if (num4 == num2)
					{
						num += (num3 = BitConverter.ToInt32(_RsaSocketEncryptHelper.DecryptBase64(array), 0));
						progress?.Invoke(num3, num, fileLength);
					}
					else
					{
						if (num4 == 0 && mAppSocketClient.Available == 0)
						{
							break;
						}
						if (num4 == 0)
						{
							_ = mAppSocketClient.Available;
							_ = 0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Write stream throw ex:" + ex);
			}
			return num;
		});
		task.Start();
		long num5 = 0L;
		int num6 = 0;
		while (!IsDisposed && mAppSocketClient.Connected && num5 < fileLength)
		{
			MemoryStream memoryStream2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num7 = inputStream.Read(BytesBuffer, 0, BytesBufferSize);
				if (num7 > 0)
				{
					memoryStream.Write(BytesBuffer, 0, num7);
				}
				else if (num5 < fileLength && inputStream.GetType().Name == "BackupResourceStreamAESReader")
				{
					int num8 = Convert.ToInt32(fileLength - num5);
					num7 = num8;
					byte[] buffer = new byte[num8];
					memoryStream.Write(buffer, 0, num8);
				}
				num5 += num7;
				byte[] array2 = _RsaSocketEncryptHelper.Encrypt(memoryStream.ToArray());
				memoryStream2 = new MemoryStream();
				memoryStream2.Write(array2, 0, array2.Length);
				memoryStream2.Write(splitBuffer, 0, splitBuffer.Length);
				memoryStream2.Position = 0L;
			}
			byte[] array3 = new byte[Convert.ToInt32(memoryStream2.Length)];
			if ((num6 = memoryStream2.Read(array3, 0, array3.Length)) > 0)
			{
				Write(array3, 0, num6);
			}
		}
		task.Wait();
		return task.Result;
	}

	public long WriteStreamOld(Stream inputStream, Action<int, long, long> progress)
	{
		Stream stream;
		if (_RsaSocketEncryptHelper.IsSecurity)
		{
			byte[] array = new byte[1024];
			using MemoryStream memoryStream = new MemoryStream();
			int count;
			while ((count = inputStream.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, count);
			}
			byte[] array2 = _RsaSocketEncryptHelper.EncryptBase64(memoryStream.ToArray());
			int count2 = array2.Length;
			stream = new MemoryStream();
			stream.Write(array2, 0, count2);
			stream.Write(splitBuffer, 0, splitBuffer.Length);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		else
		{
			stream = inputStream;
		}
		long sourceLength = inputStream.Length;
		Task<long> task = new Task<long>(delegate
		{
			long num = 0L;
			try
			{
				int num2 = (_RsaSocketEncryptHelper.IsSecurity ? 28 : 4);
				byte[] array3 = new byte[num2];
				int num3 = 0;
				int num4 = 0;
				while (!IsDisposed && mAppSocketClient.Connected && num < sourceLength)
				{
					num4 = Read(array3, 0, num2);
					if (num4 == num2)
					{
						num3 = BitConverter.ToInt32(_RsaSocketEncryptHelper.DecryptBase64(array3), 0);
						num += num3;
						progress?.Invoke(num3, num, sourceLength);
					}
					else
					{
						if (num4 == 0 && mAppSocketClient.Available == 0)
						{
							break;
						}
						if (num4 == 0)
						{
							_ = mAppSocketClient.Available;
							_ = 0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Write stream string throw ex:" + ex);
			}
			return num;
		});
		task.Start();
		long num5 = 0L;
		int num6 = 0;
		byte[] bytesBuffer = BytesBuffer;
		int bytesBufferSize = BytesBufferSize;
		long length = stream.Length;
		while (!IsDisposed && mAppSocketClient.Connected && num5 < length)
		{
			if ((num6 = stream.Read(bytesBuffer, 0, bytesBufferSize)) > 0)
			{
				num5 += num6;
				Write(bytesBuffer, 0, num6);
			}
		}
		task.Wait();
		return task.Result;
	}

	public void WriteHeader(Header header)
	{
		string text = JsonConvert.SerializeObject(header.Headers);
		string s = _RsaSocketEncryptHelper.EncryptToBase64(text);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = _RsaSocketEncryptHelper.EncryptBase64(BitConverter.GetBytes(bytes.Length));
		Write(bytes2);
		Write(bytes);
	}

	public void Dispose()
	{
		if (mIsDisposed)
		{
			return;
		}
		mIsDisposed = true;
		if (mAppSocketClient == null)
		{
			return;
		}
		try
		{
			if (mAppSocketClient.Connected)
			{
				Header header = new Header();
				header.AddOrReplace("Status", "-11");
				WriteHeader(header);
			}
			mAppSocketClient.Close();
			mAppSocketClient = null;
		}
		catch (Exception)
		{
		}
	}
}
