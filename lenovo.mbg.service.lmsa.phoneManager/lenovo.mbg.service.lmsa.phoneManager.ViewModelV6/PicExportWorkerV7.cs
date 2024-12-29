using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.phoneManager.Business;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicExportWorkerV7 : IDisposable
{
	private class ExportTask : IDisposable
	{
		private PicInfoViewModelV7 m_pic;

		private PicExportWorkerV7 m_outer;

		public ExportTask(PicExportWorkerV7 outer, PicInfoViewModelV7 pic)
		{
			m_outer = outer;
			m_pic = pic;
		}

		public void BeginStart(Action<string, bool, string> callback)
		{
			Task.Factory.StartNew(delegate
			{
				try
				{
					if (string.IsNullOrEmpty(m_pic.RawPicInfo.RawFilePath))
					{
						List<ServerPicInfo> pics = new List<ServerPicInfo> { m_pic.RawPicInfo };
						m_outer.m_dpm.FillPicPath(ref pics);
					}
					if (string.IsNullOrEmpty(m_pic.RawPicInfo.RawFilePath))
					{
						throw new Exception("Raw file path is empty");
					}
					string rawFilePath = m_pic.RawPicInfo.RawFilePath;
					rawFilePath = rawFilePath.Replace("/", "\\").Trim('\\');
					string directoryName = Path.GetDirectoryName(rawFilePath);
					new List<ServerPicInfo>().Add(m_pic.RawPicInfo);
					string saveDir = Path.Combine(Configurations.PicOriginalCacheDir, directoryName);
					Path.Combine(Configurations.PicOriginalCacheDir, rawFilePath);
					new ImportAndExportWrapper().ExportFileWithNoProgress(17, new List<string> { m_pic.RawPicInfo.Id }, saveDir, null, delegate(string id, bool isSuccess, string path)
					{
						callback?.Invoke(m_pic.RawPicInfo.Id, isSuccess, path);
					});
				}
				catch (Exception)
				{
					callback?.Invoke(m_pic.RawPicInfo.Id, arg2: false, string.Empty);
				}
			});
		}

		private void Img_DecodeFailed(object sender, ExceptionEventArgs e)
		{
			LogHelper.LogInstance.Error("Create bitmapimge throw exception:" + e.ErrorException.ToString());
		}

		public void Dispose()
		{
		}
	}

	private Action<string, TransferResult, string> m_resultHandler;

	private readonly object m_sync = new object();

	private List<string> m_handledPathList = new List<string>();

	private Dictionary<string, ExportTask> m_exportingPathList = new Dictionary<string, ExportTask>();

	private Dictionary<string, string> m_exportedPathList = new Dictionary<string, string>();

	private DevicePicManagement m_dpm = new DevicePicManagement();

	public PicExportWorkerV7(Action<string, TransferResult, string> resultHandler)
	{
		m_resultHandler = resultHandler;
	}

	public string BeginExport(PicInfoViewModelV7 pic)
	{
		string id = pic.RawPicInfo.Id;
		lock (m_sync)
		{
			if (!m_handledPathList.Contains(id))
			{
				ExportTask exportTask = new ExportTask(this, pic);
				m_exportingPathList.Add(id, exportTask);
				exportTask.BeginStart(delegate(string fid, bool isOk, string filePath)
				{
					lock (m_sync)
					{
						m_exportingPathList.Remove(fid);
						if (!isOk)
						{
							m_handledPathList.Remove(fid);
						}
					}
					m_resultHandler?.Invoke(fid, (!isOk) ? TransferResult.FAILD : TransferResult.SUCCESS, filePath);
				});
				return null;
			}
			return m_exportedPathList.ContainsKey(id) ? m_exportedPathList[id] : null;
		}
	}

	public void Dispose()
	{
		lock (m_sync)
		{
			foreach (KeyValuePair<string, ExportTask> exportingPath in m_exportingPathList)
			{
				exportingPath.Value.Dispose();
			}
		}
	}
}
