using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.backuprestore.Common;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Component.Progress;
using lenovo.themes.generic.Exceptions;

namespace lenovo.mbg.service.lmsa.backuprestore.Business;

public class ReleaseBackupFileWorker : IWorker, IDisposable
{
	private IBackupStorage _backupStorage;

	private string _releaseFolderPath;

	public string WorkerId => string.Empty;

	public int WorkerSequence => 0;

	public IAsyncTaskContext TaskContext { get; set; }

	protected Action<NotifyTypes, object> _exectingNotifyHandler { get; }

	protected Action<string, object> NotifyHandler { get; private set; }

	protected CancellationTokenSource CancelSource { get; set; }

	protected CancellationTokenSource AbortSource { get; set; }

	public ReleaseBackupFileWorker(IAsyncTaskContext context, IBackupStorage backupStorage, string releaseFolderPath)
	{
		_backupStorage = backupStorage;
		_releaseFolderPath = releaseFolderPath;
		TaskContext = context;
		_exectingNotifyHandler = (Action<NotifyTypes, object>)context.ObjectState;
	}

	public void Abort()
	{
		AbortSource?.Cancel();
	}

	public void Cancel()
	{
		CancelSource?.Cancel();
	}

	public void Dispose()
	{
		if (CancelSource != null)
		{
			CancelSource.Dispose();
			CancelSource = null;
		}
		if (AbortSource != null)
		{
			AbortSource.Dispose();
			AbortSource = null;
		}
	}

	public void DoProcess(object state)
	{
		string version = string.Empty;
		IBackupStorage storage = _backupStorage;
		try
		{
			TaskContext.AddCancelSource(delegate
			{
				storage?.Dispose();
			});
			IBackupResourceReader resourceReader = _backupStorage.OpenRead(out version);
			if (resourceReader == null)
			{
				return;
			}
			List<BackupResource> obj = resourceReader.GetChildResources(null) ?? throw new BackupRestoreException("Storage root resource is null");
			int bufferSize = 1024;
			byte[] buffer = new byte[bufferSize];
			int readedLength = 0;
			Dictionary<string, string> dirMapping = new Dictionary<string, string>
			{
				{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", "Contacts" },
				{ "{89D4DB68-4258-4002-8557-E65959C558B3}", "Call log" },
				{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "Pictures" },
				{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "Songs" },
				{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "Videos" },
				{ "{580C48C8-6CEF-4BBB-AF37-D880B349D142}", "Files" },
				{ "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", "SMS" }
			};
			int num = (from m in obj
				select m.Tag into n
				where dirMapping.Keys.Contains(n)
				select n).Count();
			_exectingNotifyHandler?.Invoke(NotifyTypes.PROGRESSINFO, new List<object>
			{
				ResourcesHelper.StringResources.SingleInstance.BACKUPRESTORE_RELEASE_CONTENT,
				num
			});
			foreach (BackupResource item in obj)
			{
				if (TaskContext.IsCancelCommandRequested)
				{
					throw new CacnelException("cancel release backup file");
				}
				if (!dirMapping.Keys.Contains(item.Tag))
				{
					continue;
				}
				string rootDir = Path.Combine(_releaseFolderPath, dirMapping[item.Tag]);
				string currentDir = rootDir;
				if (!Directory.Exists(rootDir))
				{
					Directory.CreateDirectory(rootDir);
				}
				HostProxy.ResourcesLoggingService.RegisterDir(currentDir);
				if ("{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}".Equals(item.Tag))
				{
					string text = "contacts.vcf";
					string path = Path.Combine(rootDir, text);
					if (File.Exists(path))
					{
						int num2 = text.LastIndexOf('.');
						string empty = string.Empty;
						string arg = string.Empty;
						if (num2 < 0)
						{
							empty = text;
						}
						else
						{
							empty = text.Remove(num2);
							arg = text.Substring(num2);
						}
						int num3 = 1;
						do
						{
							path = Path.Combine(currentDir, $"{empty}({num3}){arg}");
							num3++;
						}
						while (File.Exists(path));
					}
					FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
					try
					{
						resourceReader.Foreach(item, delegate(BackupResource rs)
						{
							if (TaskContext.IsCancelCommandRequested)
							{
								throw new CacnelException("cancel release backup file");
							}
							IBackupResourceStreamReader backupResourceStreamReader = resourceReader.Seek(rs);
							do
							{
								readedLength = backupResourceStreamReader.Read(buffer, 0, bufferSize);
								fs.Write(buffer, 0, readedLength);
							}
							while (readedLength != 0);
						});
					}
					finally
					{
						if (fs != null)
						{
							((IDisposable)fs).Dispose();
						}
					}
				}
				else
				{
					resourceReader.Foreach(item, delegate(BackupResource rs)
					{
						if (TaskContext.IsCancelCommandRequested)
						{
							throw new CacnelException("cancel release backup file");
						}
						if ("dir".Equals(rs.Tag))
						{
							string fileName = Path.GetFileName(rs.Value);
							currentDir = Path.Combine(rootDir, fileName);
							Directory.CreateDirectory(currentDir);
							return;
						}
						string fileName2 = Path.GetFileName(rs.Value);
						string text2 = Path.Combine(currentDir, Path.GetFileName(rs.Value));
						if (File.Exists(text2))
						{
							int num4 = fileName2.LastIndexOf('.');
							string empty2 = string.Empty;
							string arg2 = string.Empty;
							if (num4 < 0)
							{
								empty2 = fileName2;
							}
							else
							{
								empty2 = fileName2.Remove(num4);
								arg2 = fileName2.Substring(num4);
							}
							int num5 = 1;
							do
							{
								text2 = Path.Combine(currentDir, $"{empty2}({num5}){arg2}");
								num5++;
							}
							while (File.Exists(text2));
						}
						IBackupResourceStreamReader backupResourceStreamReader2 = resourceReader.Seek(rs);
						try
						{
							using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write))
							{
								fileStream.SetLength(rs.AssociatedStreamSize);
								do
								{
									readedLength = backupResourceStreamReader2.Read(buffer, 0, bufferSize);
									fileStream.Write(buffer, 0, readedLength);
								}
								while (readedLength != 0);
							}
							if (rs.Attributes != null)
							{
								FileInfo fileInfo = new FileInfo(text2);
								string value = null;
								DateTime result = DateTime.Now;
								rs.Attributes.TryGetValue("CreateDateTime", out value);
								if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out result))
								{
									fileInfo.CreationTime = result;
								}
								rs.Attributes.TryGetValue("LastModifyDateTime", out value);
								if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out result))
								{
									fileInfo.LastWriteTime = result;
								}
							}
						}
						catch (Exception)
						{
							try
							{
								File.Delete(text2);
							}
							catch (Exception)
							{
							}
							throw;
						}
					});
				}
				_exectingNotifyHandler?.Invoke(NotifyTypes.PERCENT, 1);
			}
		}
		finally
		{
			if (storage != null)
			{
				storage.Dispose();
			}
		}
	}

	public void SetNotifyHandler(Action<string, object> notifyHandler)
	{
		NotifyHandler = notifyHandler;
	}
}
