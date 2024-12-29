using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Tool;

public class ToolCheckVersion : IVersionCheck
{
	private long m_CheckToolVersionLocker = 0L;

	private IVersionData m_versionData;

	public bool CheckToolVersionIsRunning => Interlocked.Read(ref m_CheckToolVersionLocker) != 0;

	public int CurrentVersionCode
	{
		get
		{
			int result = 0;
			try
			{
				string s = ConfigurationManager.AppSettings["VersionCode"];
				int.TryParse(s, out result);
			}
			catch (Exception)
			{
			}
			return result;
		}
	}

	public string CurrentVersion
	{
		get
		{
			try
			{
				return LMSAContext.MainProcessVersion;
			}
			catch (Exception)
			{
			}
			return "0.0.0.0";
		}
	}

	public IVersionData VersionData => m_versionData;

	public event EventHandler<CheckVersionEventArgs> OnCheckVersionStatusChanged;

	public ToolCheckVersion(IVersionData VerionData)
	{
		m_versionData = VerionData;
	}

	public void Check(bool isAutoMode)
	{
		if (CheckToolVersionIsRunning)
		{
			return;
		}
		Interlocked.Exchange(ref m_CheckToolVersionLocker, 1L);
		Task.Factory.StartNew(delegate
		{
			try
			{
				FireCheckVersionStatusChangedEvent(this, new CheckVersionEventArgs(isAutoMode, CheckVersionStatus.CHECK_VERSION_START));
				VersionModel versionModel = new VersionModel();
				object data = VersionData.GetData();
				if (data != null)
				{
					versionModel = (VersionModel)data;
					if (string.IsNullOrEmpty(versionModel.downloadUrl) || string.IsNullOrEmpty(versionModel.downloadMD5))
					{
						FireCheckVersionStatusChangedEvent(this, new CheckVersionEventArgs(isAutoMode, CheckVersionStatus.CHECK_VERSION_DATA_INVALID));
					}
					else
					{
						FireCheckVersionStatusChangedEvent(this, new CheckVersionEventArgs(isAutoMode, CheckVersionStatus.CHECK_VERSION_HAVE_NEW_VERSION, versionModel));
					}
				}
				else
				{
					FireCheckVersionStatusChangedEvent(this, new CheckVersionEventArgs(isAutoMode, CheckVersionStatus.CHECK_VERSION_NOT_NEW_VERSION));
				}
			}
			catch (Exception exception)
			{
				FireCheckVersionStatusChangedEvent(this, new CheckVersionEventArgs(isAutoMode, CheckVersionStatus.CHECK_VERSION_FAILED));
				LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.UpdateVersion.VersionCheck.CheckToolVersion.Check: Check Version Failed", exception);
			}
			finally
			{
				Interlocked.Exchange(ref m_CheckToolVersionLocker, 0L);
			}
		});
	}

	private void FireCheckVersionStatusChangedEvent(object sender, CheckVersionEventArgs e)
	{
		if (this.OnCheckVersionStatusChanged != null)
		{
			this.OnCheckVersionStatusChanged(sender, e);
		}
	}

	public bool CompareVersionCode(string serverVersionCode)
	{
		if (!string.IsNullOrEmpty(serverVersionCode))
		{
			string[] array = serverVersionCode.Split('.');
			string[] array2 = CurrentVersion.Split('.');
			int num = array.Length;
			int num2 = array2.Length;
			int[] array3 = Array.ConvertAll(array, delegate(string s)
			{
				int result = 0;
				int.TryParse(s, out result);
				return result;
			});
			int[] array4 = Array.ConvertAll(array2, delegate(string s)
			{
				int result2 = 0;
				int.TryParse(s, out result2);
				return result2;
			});
			int num3 = ((num > num2) ? num2 : num);
			for (int i = 0; i < num3; i++)
			{
				if (array3[i] > array4[i])
				{
					return true;
				}
				if (array3[i] < array4[i])
				{
					return false;
				}
			}
			if (num > num2)
			{
				int num4 = 0;
				for (int j = num2; j < num; j++)
				{
					num4 += array3[j];
				}
				if (num4 > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private int ConvertString2Int(string str)
	{
		int result = 0;
		int.TryParse(str, out result);
		return result;
	}
}
