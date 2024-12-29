using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.common.ImportExport;

public class AppDataTransferHelper
{
	public enum ErrorCode
	{
		OK,
		UnKnown,
		DiskSpaceNotEnoughs
	}

	public enum BackupRestoreResult
	{
		Success,
		Fail,
		Undo
	}

	public class AppDataExporter
	{
		private List<string> mIdList;

		private int mRequestServiceCode;

		public IAsyncTaskContext TaskContext { get; set; }

		public string GetIdListMethodName { get; set; }

		public string GetPathByIdMethodName { get; set; }

		public string ExportDataByIdMethodName { get; set; }

		public Action<string, string, BackupRestoreResult> OnNotifyUnExportRes { get; set; }

		public Func<string, Header, bool> ResourceItemReceivePrepare { get; set; }

		public Action<byte[], int, long, long> ReadStreamCallback { get; set; }

		public Action<string> ResourceItemStartExportCallback { get; set; }

		public Action<string, bool, ErrorCode> ResourceItemFinishExportCallback { get; set; }

		public AppDataExporter(int requestServiceCode)
		{
			GetIdListMethodName = "getIdList";
			GetPathByIdMethodName = "getPathById";
			ExportDataByIdMethodName = "exportDataById";
			mRequestServiceCode = requestServiceCode;
		}

		public AppDataExporter(int requestServiceCode, List<string> idList)
			: this(requestServiceCode)
		{
			mIdList = idList;
		}

		public int Export(TcpAndroidDevice currentDevice, Header requestHeader, bool isReadStreamString = false)
		{
			if (requestHeader == null)
			{
				requestHeader = new Header();
			}
			if (mIdList == null)
			{
				mIdList = GetIdList(currentDevice);
			}
			int num = 0;
			if (mIdList == null)
			{
				return 0;
			}
			num = mIdList.Count;
			int i = 0;
			string empty = string.Empty;
			bool flag = false;
			bool flag2 = true;
			ErrorCode arg = ErrorCode.OK;
			Header header = null;
			AppServiceResponse responseHandler = null;
			AppServiceRequest appServiceRequest = new AppServiceRequest(currentDevice.ExtendDataFileServiceEndPoint, currentDevice.RsaSocketEncryptHelper);
			for (; i < num && flag2 && currentDevice.PhysicalStatus == DevicePhysicalStateEx.Online && currentDevice.SoftStatus == DeviceSoftStateEx.Online && !TaskContext.IsCancelCommandRequested; i++)
			{
				flag = false;
				empty = mIdList[i];
				try
				{
					responseHandler = appServiceRequest.Request(mRequestServiceCode, ExportDataByIdMethodName, requestHeader, null);
					TaskContext.AddCancelSource(delegate
					{
						responseHandler.Dispose();
					});
				}
				catch (Exception exception)
				{
					LogHelper.LogInstance.Error("Export file id " + empty + " error", exception);
					if (OnNotifyUnExportRes == null)
					{
						ResourceItemStartExportCallback(empty);
						ResourceItemFinishExportCallback(empty, arg2: false, arg = ErrorCode.UnKnown);
					}
					else
					{
						OnNotifyUnExportRes?.Invoke(empty, "undo", BackupRestoreResult.Undo);
					}
					continue;
				}
				try
				{
					ResourceItemStartExportCallback(empty);
					requestHeader.AddOrReplace("Status", "-6");
					requestHeader.AddOrReplace("ResourceId", empty);
					responseHandler.WriteHeader(requestHeader);
					header = responseHandler.ReadHeader(null);
					if (!header.ContainsAndEqual("Status", "-6"))
					{
						flag = false;
						continue;
					}
					long @int = header.GetInt64("StreamLength", -1L);
					if (@int >= 0 && ResourceItemReceivePrepare(empty, header))
					{
						requestHeader.AddOrReplace("Status", "-6");
						responseHandler.WriteHeader(requestHeader);
						long num2 = 0L;
						num2 = ((!isReadStreamString) ? responseHandler.ReadStream(@int, currentDevice.RsaSocketEncryptHelper, ReadStreamCallback) : responseHandler.ReadStreamOld(@int, ReadStreamCallback));
						flag = num2 == @int && @int >= 0;
					}
					else
					{
						requestHeader.AddOrReplace("Status", "-9");
						responseHandler.WriteHeader(requestHeader);
						flag = false;
					}
				}
				catch (DiskSpaceNotEnoughExcpetion)
				{
					LogHelper.LogInstance.Error("Export file id " + empty + " error, not enough space");
					flag2 = false;
					flag = false;
					arg = ErrorCode.DiskSpaceNotEnoughs;
				}
				catch (Exception exception2)
				{
					LogHelper.LogInstance.Error("Export file id " + empty + " error", exception2);
					flag = false;
					arg = ErrorCode.UnKnown;
				}
				finally
				{
					if (responseHandler != null)
					{
						try
						{
							responseHandler.Dispose();
						}
						catch (Exception)
						{
						}
					}
					ResourceItemFinishExportCallback(empty, flag, arg);
				}
			}
			if (OnNotifyUnExportRes == null)
			{
				for (int j = i; j < num; j++)
				{
					ResourceItemStartExportCallback(mIdList[j]);
					ResourceItemFinishExportCallback(mIdList[j], arg2: false, arg);
				}
			}
			else
			{
				for (int k = i; k < num; k++)
				{
					OnNotifyUnExportRes?.Invoke(mIdList[k], "undo", BackupRestoreResult.Undo);
				}
			}
			return num;
		}

		public List<string> GetIdList(TcpAndroidDevice currentDevice)
		{
			Header header = new Header();
			AppServiceResponse responseHandler = null;
			try
			{
				AppServiceRequest appServiceRequest = new AppServiceRequest(currentDevice.ExtendDataFileServiceEndPoint, currentDevice.RsaSocketEncryptHelper);
				Header header2 = null;
				responseHandler = appServiceRequest.Request(mRequestServiceCode, GetIdListMethodName, header, null);
				if (responseHandler != null)
				{
					TaskContext.AddCancelSource(delegate
					{
						responseHandler.Dispose();
					});
				}
				header2 = responseHandler.ReadHeader(null);
				if (!header2.ContainsAndEqual("Status", "-6"))
				{
					return null;
				}
				long @int = header2.GetInt64("StreamLength", -1L);
				if (@int == -1)
				{
					return null;
				}
				byte[] array = new byte[@int];
				if (responseHandler.ReadStream(array, currentDevice.RsaSocketEncryptHelper, @int, null) == @int)
				{
					return JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(array));
				}
				return null;
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("Get id list error", exception);
				return null;
			}
			finally
			{
				if (responseHandler != null)
				{
					try
					{
						responseHandler.Dispose();
					}
					catch (Exception)
					{
					}
				}
			}
		}

		public string GetPathById(TcpAndroidDevice currentDevice, string id)
		{
			string text = "Missing";
			Header header = new Header();
			AppServiceResponse responseHandler = null;
			try
			{
				if (currentDevice == null || currentDevice.SoftStatus != DeviceSoftStateEx.Online)
				{
					return text;
				}
				AppServiceRequest appServiceRequest = new AppServiceRequest(currentDevice.ExtendDataFileServiceEndPoint, currentDevice.RsaSocketEncryptHelper);
				Header header2 = null;
				responseHandler = appServiceRequest.Request(mRequestServiceCode, GetPathByIdMethodName, header, null);
				if (responseHandler != null)
				{
					TaskContext.AddCancelSource(delegate
					{
						responseHandler.Dispose();
					});
				}
				header.AddOrReplace("Status", "-6");
				header.AddOrReplace("ResourceId", id);
				responseHandler.WriteHeader(header);
				header2 = responseHandler.ReadHeader(null);
				if (!header2.ContainsAndEqual("Status", "-6"))
				{
					return text;
				}
				long @int = header2.GetInt64("StreamLength", -1L);
				if (@int == -1)
				{
					return string.Empty;
				}
				byte[] array = new byte[@int];
				if (responseHandler.ReadStream(array, currentDevice.RsaSocketEncryptHelper, @int, null) == @int)
				{
					string text2 = currentDevice.RsaSocketEncryptHelper.DecryptFromBase64(Encoding.UTF8.GetString(array));
					return string.IsNullOrEmpty(text2) ? text : text2;
				}
				return text;
			}
			catch (Exception)
			{
				LogHelper.LogInstance.Info("File id " + id + " does not exist or access is denied");
				return text;
			}
			finally
			{
				responseHandler?.Dispose();
			}
		}
	}

	public class AppDataInporter<T>
	{
		private List<T> mDataPathList;

		private int mRequestServiceCode;

		public bool CloseStreamAfterSend;

		public IAsyncTaskContext TaskContext { get; set; }

		public string ImportMethodName { get; set; }

		public Action<byte[], int, long, long> SendStreamCallback { get; set; }

		public Action<T> ResourceItemStartImportCallback { get; set; }

		public Action<T, BackupRestoreResult> ResourceItemFinishImportCallback { get; set; }

		public Func<T, string> DataPathConverter { get; set; }

		public Func<T, string, Stream> CreateDataReadStream { get; set; }

		public Action<T, int, long, long> ItemProgressCallback { get; set; }

		public Action<T, Header> HeaderOperate { get; set; }

		public AppDataInporter(int requestServiceCode, List<T> dataPathList)
		{
			ImportMethodName = "importData";
			mRequestServiceCode = requestServiceCode;
			mDataPathList = dataPathList;
		}

		public int Import(TcpAndroidDevice currentDevice, Header requestHeader, bool isReadStreamString = false)
		{
			AppServiceRequest appServiceRequest = new AppServiceRequest(currentDevice.ExtendDataFileServiceEndPoint, currentDevice.RsaSocketEncryptHelper);
			AppServiceResponse responseHandler = null;
			T targetData = default(T);
			int i = 0;
			int num = 0;
			if (mDataPathList != null)
			{
				num = mDataPathList.Count;
			}
			bool flag = false;
			while (i < num && currentDevice.PhysicalStatus == DevicePhysicalStateEx.Online && currentDevice.SoftStatus == DeviceSoftStateEx.Online && !TaskContext.IsCancelCommandRequested)
			{
				targetData = mDataPathList[i];
				string text = ((DataPathConverter != null) ? DataPathConverter(targetData) : targetData.ToString());
				try
				{
					responseHandler = appServiceRequest.Request(mRequestServiceCode, ImportMethodName, requestHeader, null);
				}
				catch (Exception exception)
				{
					i++;
					LogHelper.LogInstance.Error("create request error during import: " + text, exception);
					ResourceItemStartImportCallback?.Invoke(targetData);
					ResourceItemFinishImportCallback(targetData, BackupRestoreResult.Fail);
					continue;
				}
				try
				{
					TaskContext.AddCancelSource(delegate
					{
						responseHandler.Dispose();
					});
					for (; i < mDataPathList.Count; i++)
					{
						if (currentDevice.PhysicalStatus != DevicePhysicalStateEx.Online)
						{
							break;
						}
						if (currentDevice.SoftStatus != DeviceSoftStateEx.Online)
						{
							break;
						}
						if (TaskContext.IsCancelCommandRequested)
						{
							break;
						}
						targetData = mDataPathList[i];
						flag = false;
						ResourceItemStartImportCallback?.Invoke(targetData);
						Stream stream = null;
						try
						{
							HeaderOperate?.Invoke(targetData, requestHeader);
							text = ((DataPathConverter != null) ? DataPathConverter(targetData) : targetData.ToString());
							stream = CreateDataReadStream?.Invoke(targetData, text);
							long length = stream.Length;
							requestHeader.AddOrReplace("Status", "-6");
							requestHeader.AddOrReplace("StreamLength", length.ToString());
							requestHeader.AddOrReplace("FileFullName", text);
							responseHandler.WriteHeader(requestHeader);
							if (responseHandler.ReadHeader(null).ContainsAndEqual("Status", "-6"))
							{
								flag = ((!isReadStreamString) ? (responseHandler.WriteStream(stream, delegate(int rl, long rt, long len)
								{
									ItemProgressCallback?.Invoke(targetData, rl, rt, len);
								}, currentDevice.RsaSocketEncryptHelper) == length) : (responseHandler.WriteStreamOld(stream, delegate(int rl, long rt, long len)
								{
									ItemProgressCallback?.Invoke(targetData, rl, rt, len);
								}) == length));
							}
						}
						catch (Exception exception2)
						{
							LogHelper.LogInstance.Error("import: " + text + " error", exception2);
							flag = false;
						}
						finally
						{
							if (flag)
							{
								ResourceItemFinishImportCallback(targetData, BackupRestoreResult.Success);
							}
							else
							{
								ResourceItemFinishImportCallback(targetData, BackupRestoreResult.Fail);
							}
							if (CloseStreamAfterSend && stream != null)
							{
								try
								{
									stream.Close();
									stream = null;
								}
								catch (Exception)
								{
								}
							}
						}
						if (!flag)
						{
							i++;
							break;
						}
					}
				}
				finally
				{
					try
					{
						responseHandler?.Dispose();
					}
					catch (Exception)
					{
					}
				}
			}
			for (int j = i; j < num; j++)
			{
				ResourceItemStartImportCallback?.Invoke(mDataPathList[j]);
				ResourceItemFinishImportCallback?.Invoke(mDataPathList[j], BackupRestoreResult.Undo);
			}
			return num;
		}
	}
}
