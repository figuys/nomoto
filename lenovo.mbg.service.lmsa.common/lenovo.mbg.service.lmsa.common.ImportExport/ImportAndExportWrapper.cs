using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Component.ProgressEx;

namespace lenovo.mbg.service.lmsa.common.ImportExport;

public class ImportAndExportWrapper
{
	public void ImportFile(BusinessType businessType, int requestServiceCode, string progressTitle, string resourceType, string resourceTypeName, Func<List<string>> createDataHandler, Func<string, string> dataPathConverter)
	{
		AsyncTaskContext taskContext = new AsyncTaskContext(null);
		ProgressWindowWrapper progress = new ProgressWindowWrapper(new ProgressTipsItemViewModel
		{
			Message = progressTitle
		}, canRetry: false, canShowDeatil: true);
		Stopwatch sw = new Stopwatch();
		ResourceExecuteResult executeResult = new ResourceExecuteResult();
		progress.CompletedCallback = delegate
		{
			progress.AboveTips.Clear();
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0543"
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = ", "
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = progress.SuccessCount + " ",
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005D7F"))
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0602"
			});
			if (progress.FailCount == 0)
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
			else
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = ", "
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = progress.FailCount + " ",
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA0E0E"))
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "K0637"
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
		};
		progress.BeginProcess(delegate(ProgressWindowWrapper self)
		{
			TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			BusinessData businessData = new BusinessData(businessType, tcpAndroidDevice);
			List<string> list = createDataHandler();
			int count = list.Count;
			self.ProgressMaxValue = count;
			self.CloseWindowCallback = delegate(int code)
			{
				Task.Run(delegate
				{
					sw.Stop();
					HostProxy.BehaviorService.Collect(businessType, businessData.Update(sw.ElapsedMilliseconds, (code == -1) ? BusinessStatus.QUIT : ((executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED), executeResult.ResultMapEx));
				});
				taskContext.Cancel();
			};
			self.SetSubProgressInfo(resourceTypeName, count);
			AppDataTransferHelper.AppDataInporter<string> obj = new AppDataTransferHelper.AppDataInporter<string>(requestServiceCode, list)
			{
				TaskContext = taskContext,
				CloseStreamAfterSend = true
			};
			string currentResourceName = string.Empty;
			Header requestHeader = new Header();
			obj.ResourceItemStartImportCallback = delegate(string rs)
			{
				if (!string.IsNullOrEmpty(rs))
				{
					currentResourceName = Path.GetFileName(rs);
				}
			};
			obj.ResourceItemFinishImportCallback = delegate(string rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
			{
				executeResult.Update(isSuccess);
				if (isSuccess == AppDataTransferHelper.BackupRestoreResult.Success)
				{
					self.AddSuccessCount(rs, 1);
				}
				else
				{
					self.AddFailCount(resourceType, rs, rs, 1);
				}
			};
			obj.ItemProgressCallback = delegate(string rs, int rl, long rt, long tl)
			{
				self.UpdateRate(currentResourceName, rt, tl);
			};
			obj.DataPathConverter = dataPathConverter;
			obj.CreateDataReadStream = (string localPath, string remotePath) => new FileStream(localPath, FileMode.Open, FileAccess.Read);
			obj.HeaderOperate = delegate(string res, Header header)
			{
				FileInfo fileInfo = new FileInfo(res);
				header.AddOrReplace("CreateDateTime", fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
				header.AddOrReplace("LastModifyDateTime", fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
			};
			sw.Start();
			obj.Import(tcpAndroidDevice, requestHeader);
		});
	}

	public void ImportFileWithNoProgress(int requestServiceCode, List<string> dataList, Func<string, string> dataPathConverter, Action<string, bool> resultCallback, Action<long, long> progressNotify = null)
	{
		AsyncTaskContext taskContext = new AsyncTaskContext(null);
		_ = dataList.Count;
		new Dictionary<string, string>();
		AppDataTransferHelper.AppDataInporter<string> obj = new AppDataTransferHelper.AppDataInporter<string>(requestServiceCode, dataList)
		{
			TaskContext = taskContext,
			CloseStreamAfterSend = true
		};
		string currentResourceName = string.Empty;
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		obj.ResourceItemStartImportCallback = delegate(string rs)
		{
			if (!string.IsNullOrEmpty(rs))
			{
				currentResourceName = Path.GetFileName(rs);
			}
		};
		obj.ResourceItemFinishImportCallback = delegate(string rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
		{
			resultCallback?.Invoke(rs, isSuccess == AppDataTransferHelper.BackupRestoreResult.Success);
		};
		obj.DataPathConverter = dataPathConverter;
		obj.CreateDataReadStream = (string localPath, string remotePath) => new FileStream(localPath, FileMode.Open, FileAccess.Read);
		obj.ItemProgressCallback = delegate(string id, int rl, long rt, long tl)
		{
			progressNotify?.Invoke(rt, tl);
		};
		obj.Import(currentDevice, new Header());
	}

	public void ImportString(BusinessType businessType, int requestServiceCode, string progressTitle, string resourceType, string resourceTypeName, Func<List<string>> createDataHandler)
	{
		AsyncTaskContext taskContext = new AsyncTaskContext(null);
		ProgressWindowWrapper progress = new ProgressWindowWrapper(new ProgressTipsItemViewModel
		{
			Message = progressTitle
		}, canRetry: false, canShowDeatil: false);
		Stopwatch sw = new Stopwatch();
		ResourceExecuteResult executeResult = new ResourceExecuteResult();
		progress.CompletedCallback = delegate
		{
			progress.AboveTips.Clear();
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0543"
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = ", "
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = progress.SuccessCount + " ",
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005D7F"))
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0602"
			});
			if (progress.FailCount == 0)
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
			else
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = ", "
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = progress.FailCount + " ",
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA0E0E"))
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "K0637"
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
		};
		progress.BeginProcess(delegate(ProgressWindowWrapper self)
		{
			TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			BusinessData businessData = new BusinessData(businessType, tcpAndroidDevice);
			List<string> list = createDataHandler();
			int count = list.Count;
			self.ProgressMaxValue = count;
			self.CloseWindowCallback = delegate(int code)
			{
				Task.Run(delegate
				{
					sw.Stop();
					HostProxy.BehaviorService.Collect(businessType, businessData.Update(sw.ElapsedMilliseconds, (code == -1) ? BusinessStatus.QUIT : ((executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED), executeResult.ResultMapEx));
				});
				taskContext.Cancel();
			};
			self.SetSubProgressInfo(resourceTypeName, count);
			new Dictionary<string, string>();
			AppDataTransferHelper.AppDataInporter<string> obj = new AppDataTransferHelper.AppDataInporter<string>(requestServiceCode, list)
			{
				TaskContext = taskContext,
				CloseStreamAfterSend = false,
				ResourceItemStartImportCallback = delegate
				{
				},
				ResourceItemFinishImportCallback = delegate(string rs, AppDataTransferHelper.BackupRestoreResult isSuccess)
				{
					executeResult.Update(isSuccess);
					if (isSuccess == AppDataTransferHelper.BackupRestoreResult.Success)
					{
						self.AddSuccessCount(string.Empty, 1);
					}
					else
					{
						self.AddFailCount(resourceType, string.Empty, string.Empty, 1);
					}
				},
				ItemProgressCallback = delegate(string rs, int rl, long rt, long tl)
				{
					self.UpdateRate(rt, tl);
				},
				DataPathConverter = (string str1) => string.Empty
			};
			MemoryStream memoryStream = new MemoryStream();
			obj.CreateDataReadStream = delegate(string str1, string str2)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(str1);
				int num = bytes.Length;
				memoryStream.Seek(0L, SeekOrigin.Begin);
				memoryStream.Write(bytes, 0, num);
				memoryStream.SetLength(num);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream;
			};
			sw.Start();
			obj.Import(tcpAndroidDevice, new Header(), isReadStreamString: true);
		});
	}

	public void ExportFile(BusinessType businessType, int requestServiceCode, List<string> idList, string progressTitle, string resourceType, string resourceTypeName, string saveDir, Func<string, Header, string> SpecifyLocalFileName)
	{
		int fileCount = idList.Count;
		Stopwatch sw = new Stopwatch();
		ResourceExecuteResult executeResult = new ResourceExecuteResult();
		ProgressWindowWrapper progress = new ProgressWindowWrapper(new ProgressTipsItemViewModel
		{
			Message = progressTitle
		}, canRetry: false, canShowDeatil: true);
		AppDataTransferHelper.ErrorCode resultErrorCode = AppDataTransferHelper.ErrorCode.OK;
		progress.CompletedCallback = delegate
		{
			progress.AboveTips.Clear();
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0542"
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = ", "
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = progress.SuccessCount + " ",
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005D7F"))
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0602"
			});
			if (progress.FailCount == 0)
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
			else
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = ", "
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = progress.FailCount + " ",
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA0E0E"))
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "K0637"
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
			if (AppDataTransferHelper.ErrorCode.DiskSpaceNotEnoughs == resultErrorCode)
			{
				progress.viewModel.ErrorTips = "K0784";
			}
		};
		progress.BeginProcess(delegate(ProgressWindowWrapper self)
		{
			FileInfo file = null;
			string remoteFullName = string.Empty;
			string remoteResourceName = string.Empty;
			string fileFullName = string.Empty;
			DateTime dateAdded = DateTime.Now;
			DateTime dateModified = DateTime.Now;
			AsyncTaskContext context = new AsyncTaskContext(null);
			new Dictionary<string, string>();
			TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			BusinessData businessData = new BusinessData(businessType, tcpAndroidDevice);
			self.ProgressMaxValue = fileCount;
			self.CloseWindowCallback = delegate(int code)
			{
				Task.Run(delegate
				{
					sw.Stop();
					HostProxy.BehaviorService.Collect(businessType, businessData.Update(sw.ElapsedMilliseconds, (code == -1) ? BusinessStatus.QUIT : ((executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED), executeResult.ResultMapEx));
				});
				context.Cancel();
			};
			self.SetSubProgressInfo(resourceTypeName, fileCount);
			AppDataTransferHelper.AppDataExporter exporter = new AppDataTransferHelper.AppDataExporter(requestServiceCode, idList);
			exporter.TaskContext = context;
			exporter.ResourceItemStartExportCallback = delegate
			{
				file = null;
			};
			exporter.ResourceItemReceivePrepare = delegate(string id, Header header)
			{
				remoteFullName = header.GetString("FileFullName");
				fileFullName = ((SpecifyLocalFileName == null) ? Path.Combine(saveDir, Path.GetFileName(remoteFullName)) : SpecifyLocalFileName(id, header));
				header.GetInt64("StreamLength", 0L);
				dateAdded = header.GetDateTime("CreateDateTime", DateTime.Now);
				dateModified = header.GetDateTime("LastModifyDateTime", DateTime.Now);
				string directoryName = Path.GetDirectoryName(fileFullName);
				if (File.Exists(fileFullName))
				{
					int num = 0;
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileFullName);
					string extension = Path.GetExtension(fileFullName);
					do
					{
						fileFullName = $"{directoryName}//{fileNameWithoutExtension} ({++num}){extension}";
					}
					while (File.Exists(fileFullName));
				}
				else if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				file = new FileInfo(fileFullName);
				remoteResourceName = file.Name;
				return true;
			};
			exporter.ReadStreamCallback = delegate(byte[] bytes, int rl, long rt, long tl)
			{
				if (rl > 0)
				{
					WriteFile(fileFullName, bytes, rl);
					self.UpdateRate(remoteResourceName, rt, tl);
				}
			};
			exporter.ResourceItemFinishExportCallback = delegate(string id, bool isSuccess, AppDataTransferHelper.ErrorCode errorCode)
			{
				executeResult.Update(isSuccess);
				resultErrorCode = errorCode;
				if (isSuccess)
				{
					self.AddSuccessCount(id, 1);
				}
				else
				{
					try
					{
						if (true == file?.Exists)
						{
							file.Delete();
						}
					}
					catch (Exception)
					{
					}
					if (string.IsNullOrEmpty(remoteFullName))
					{
						remoteFullName = exporter.GetPathById(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, id);
					}
					self.AddFailCount(resourceType, id, remoteFullName, 1);
					remoteFullName = string.Empty;
				}
			};
			sw.Start();
			exporter.Export(tcpAndroidDevice, null);
		});
	}

	public void ExportFileWithNoProgress(int requestServiceCode, List<string> idList, string saveDir, Func<string, Header, string> SpecifyLocalFileName, Action<string, bool, string> resultCallback, Action<long, long> progressNotify = null)
	{
		_ = idList.Count;
		FileInfo file = null;
		string remoteFullName = string.Empty;
		new Dictionary<string, string>();
		string fileFullName = string.Empty;
		AppDataTransferHelper.AppDataExporter appDataExporter = new AppDataTransferHelper.AppDataExporter(requestServiceCode, idList);
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		appDataExporter.TaskContext = new AsyncTaskContext(null);
		appDataExporter.ResourceItemStartExportCallback = delegate
		{
			file = null;
		};
		appDataExporter.ResourceItemReceivePrepare = delegate(string id, Header header)
		{
			remoteFullName = header.GetString("FileFullName");
			fileFullName = ((SpecifyLocalFileName == null) ? Path.Combine(saveDir, Path.GetFileName(remoteFullName.Replace(":", ""))) : SpecifyLocalFileName(id, header));
			file = new FileInfo(fileFullName);
			try
			{
				if (!file.Directory.Exists)
				{
					file.Directory.Create();
				}
				else if (file.Exists)
				{
					file.Delete();
				}
				return true;
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Resource receive prepare throw ex:" + ex);
				return false;
			}
		};
		appDataExporter.ReadStreamCallback = delegate(byte[] bytes, int rl, long rt, long tl)
		{
			if (rl > 0)
			{
				WriteFile(fileFullName, bytes, rl);
			}
			progressNotify?.Invoke(rt, tl);
		};
		appDataExporter.ResourceItemFinishExportCallback = delegate(string id, bool isSuccess, AppDataTransferHelper.ErrorCode errorCode)
		{
			try
			{
				if (!isSuccess && true == file?.Exists)
				{
					file.Delete();
				}
			}
			catch (Exception)
			{
			}
			resultCallback?.Invoke(id, isSuccess, file?.FullName);
		};
		appDataExporter.Export(currentDevice, null);
	}

	public void ExportString(BusinessType businessType, int requestServiceCode, string progressTitle, string resourceType, string resourceTypeName, Func<IAsyncTaskContext, List<string>> createIdListHandler, Action<Action<Action<string>>> trigger)
	{
		AsyncTaskContext taskContext = new AsyncTaskContext(null);
		ProgressWindowWrapper progress = new ProgressWindowWrapper(new ProgressTipsItemViewModel
		{
			Message = progressTitle
		}, canRetry: false, canShowDeatil: false);
		Stopwatch sw = new Stopwatch();
		ResourceExecuteResult executeResult = new ResourceExecuteResult();
		progress.CompletedCallback = delegate
		{
			progress.AboveTips.Clear();
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0542"
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = ", "
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = progress.SuccessCount + " ",
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005D7F"))
			});
			progress.AboveTips.Add(new ProgressTipsItemViewModel
			{
				Message = "K0602"
			});
			if (progress.FailCount == 0)
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
			else
			{
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = ", "
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = progress.FailCount + " ",
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA0E0E"))
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "K0637"
				});
				progress.AboveTips.Add(new ProgressTipsItemViewModel
				{
					Message = "."
				});
			}
		};
		TcpAndroidDevice device = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		BusinessData businessData = new BusinessData(businessType, device);
		Action<ProgressWindowWrapper, Action<string>> backgroundAction = delegate(ProgressWindowWrapper self, Action<string> receiveCallback)
		{
			List<string> list = createIdListHandler(taskContext);
			int count = list.Count;
			self.ProgressMaxValue = count;
			self.CloseWindowCallback = delegate(int code)
			{
				taskContext.Cancel();
				Task.Run(delegate
				{
					sw.Stop();
					HostProxy.BehaviorService.Collect(businessType, businessData.Update(sw.ElapsedMilliseconds, (code == -1) ? BusinessStatus.QUIT : ((executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED), executeResult.ResultMapEx));
				});
			};
			self.SetSubProgressInfo(resourceTypeName, count);
			new Dictionary<string, string>();
			AppDataTransferHelper.AppDataExporter obj = new AppDataTransferHelper.AppDataExporter(requestServiceCode, list)
			{
				TaskContext = taskContext
			};
			long bufferSize = 1048576L;
			byte[] buffer = new byte[bufferSize];
			int readTotal = 0;
			obj.ResourceItemStartExportCallback = delegate
			{
				readTotal = 0;
			};
			obj.ResourceItemReceivePrepare = (string id, Header header) => true;
			obj.ReadStreamCallback = delegate(byte[] bytes, int rl, long rt, long tl)
			{
				if (tl > bufferSize)
				{
					bufferSize = tl;
					buffer = new byte[bufferSize];
				}
				Array.Copy(bytes, 0, buffer, readTotal, rl);
				readTotal += rl;
				self.UpdateRate(rt, tl);
			};
			obj.ResourceItemFinishExportCallback = delegate(string id, bool isSuccess, AppDataTransferHelper.ErrorCode errorCode)
			{
				executeResult.Update(isSuccess);
				if (isSuccess)
				{
					self.AddSuccessCount(id, 1);
				}
				else
				{
					self.AddFailCount(resourceType, string.Empty, string.Empty, 1);
				}
				if (isSuccess && receiveCallback != null)
				{
					string @string = Encoding.UTF8.GetString(buffer, 0, readTotal);
					receiveCallback(@string);
				}
				readTotal = 0;
			};
			obj.Export(device, null, isReadStreamString: true);
		};
		progress.BeginProcess(delegate(ProgressWindowWrapper self)
		{
			try
			{
				trigger(delegate(Action<string> s)
				{
					backgroundAction(self, s);
				});
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Export string throw ex:" + ex);
			}
		});
	}

	private void WriteFile(string filePath, byte[] mBuffer, int readLength)
	{
		try
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
			fileStream.Write(mBuffer, 0, readLength);
		}
		catch (Exception ex)
		{
			if (ex.HResult == -2147024784 || ex.HResult == -2147024857)
			{
				throw new DiskSpaceNotEnoughExcpetion("There is not enough disk space", ex);
			}
			LogHelper.LogInstance.Error("Resource receive prepare throw ex:" + ex);
		}
	}
}
