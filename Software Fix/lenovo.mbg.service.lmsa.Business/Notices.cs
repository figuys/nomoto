using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Business.Notice;

namespace lenovo.mbg.service.lmsa.Business;

public class Notices
{
	private static Notices single = new Notices();

	private List<INoticeSource> noticeSources = new List<INoticeSource>();

	public static Notices Single => single;

	public NoticeInfo LatestNotice
	{
		get
		{
			lock (this)
			{
				if (DisplayNotices == null)
				{
					return null;
				}
				return DisplayNotices.FirstOrDefault((NoticeInfo m) => !m.isDeleted && !m.isChecked);
			}
		}
	}

	protected LanguageNotices local_notices { get; set; }

	protected List<NoticeInfo> DisplayNotices { get; set; }

	public Notices()
	{
		noticeSources.Add(new NormalNotice());
		noticeSources.Add(new FeedbackNotice());
	}

	public async Task<LanguageNotices> LoadLocalDataAsync()
	{
		Task<LanguageNotices> task = new Task<LanguageNotices>(delegate
		{
			if (local_notices == null)
			{
				lock (this)
				{
					if (local_notices == null)
					{
						local_notices = JsonHelper.DeserializeJson2Object<LanguageNotices>(FileHelper.ReadWithAesDecrypt(Configurations.NoticesPath));
					}
				}
			}
			return local_notices;
		});
		task.Start();
		return await task;
	}

	public async Task<List<NoticeInfo>> SyncAsync()
	{
		LanguageNotices local = await LoadLocalDataAsync();
		lock (this)
		{
			List<NoticeInfo> server = Request();
			if (server != null && server.Count > 0)
			{
				int num;
				if (local != null)
				{
					List<NoticeInfo> noticeArr = local.NoticeArr;
					num = ((noticeArr != null && noticeArr.Count == 0) ? 1 : 0);
				}
				else
				{
					num = 1;
				}
				if (num != 0)
				{
					Notices notices = this;
					LanguageNotices obj = new LanguageNotices
					{
						Language = LMSAContext.CurrentLanguage,
						NoticeArr = server
					};
					LanguageNotices languageNotices = obj;
					local = obj;
					notices.local_notices = languageNotices;
				}
				else
				{
					List<NoticeInfo> _newNotice = new List<NoticeInfo>();
					foreach (NoticeInfo iter in server)
					{
						NoticeInfo _temp = local.NoticeArr.FirstOrDefault((NoticeInfo p) => p.id == iter.id && p.noticeType == iter.noticeType);
						if (_temp == null)
						{
							_newNotice.Add(iter);
							continue;
						}
						_temp.noticeTitle = iter.noticeTitle;
						_temp.noticeContent = iter.noticeContent;
						_temp.noticeType = iter.noticeType;
						_temp.isServerReplay = iter.isServerReplay;
						if (_temp.modifyDate < iter.modifyDate)
						{
							_temp.modifyDate = iter.modifyDate;
							_temp.isChecked = false;
							_temp.isDeleted = false;
						}
						_newNotice.Add(_temp);
					}
					local = (local_notices = new LanguageNotices
					{
						Language = LMSAContext.CurrentLanguage,
						NoticeArr = _newNotice
					});
				}
			}
			else if (local?.Language != LMSAContext.CurrentLanguage)
			{
				if (local != null)
				{
					local.Language = LMSAContext.CurrentLanguage;
				}
				return DisplayNotices = null;
			}
			SerializeDataFile();
			if (local != null && local.NoticeArr.Count > 0)
			{
				List<NoticeInfo> filterList = Filter(local.NoticeArr.Where((NoticeInfo m) => !m.isDeleted).ToList());
				return DisplayNotices = filterList;
			}
			return DisplayNotices = null;
		}
	}

	private List<NoticeInfo> Filter(List<NoticeInfo> source)
	{
		List<NoticeInfo> list = new List<NoticeInfo>();
		foreach (INoticeSource noticeSource in noticeSources)
		{
			list.AddRange(noticeSource.Filter(source));
		}
		return list;
	}

	protected List<NoticeInfo> Request()
	{
		Task<List<NoticeInfo>>[] array = new Task<List<NoticeInfo>>[noticeSources.Count];
		Task<object> task = Task.FromResult<object>(null);
		for (int i = 0; i < noticeSources.Count; i++)
		{
			array[i] = Task.Factory.StartNew((object index) => noticeSources[(int)index].GetNoticesAsync(), i);
		}
		List<NoticeInfo> list = new List<NoticeInfo>();
		Task[] tasks = array;
		Task.WaitAll(tasks);
		Task<List<NoticeInfo>>[] array2 = array;
		foreach (Task<List<NoticeInfo>> task2 in array2)
		{
			if (task2.Result != null)
			{
				list.AddRange(task2.Result);
			}
		}
		return list;
	}

	public void Show(NoticeInfo checkNotice)
	{
		if (checkNotice == null)
		{
			return;
		}
		string latestNoticeType = checkNotice.type;
		INoticeSource noticeSource = noticeSources.FirstOrDefault((INoticeSource m) => latestNoticeType == m.NoticeType);
		if (noticeSource != null)
		{
			noticeSource.Show(checkNotice);
			Task.Run(delegate
			{
				UpdateIsCheck(checkNotice);
			});
		}
	}

	private void UpdateIsCheck(NoticeInfo notice)
	{
		lock (this)
		{
			NoticeInfo noticeInfo = local_notices?.NoticeArr?.Where((NoticeInfo n) => n.id == notice.id && n.noticeType == notice.noticeType).FirstOrDefault();
			if (noticeInfo != null)
			{
				noticeInfo.isChecked = true;
				SerializeDataFile();
			}
		}
	}

	public void RemoveLocalNotice(NoticeInfo model)
	{
		Task.Run(delegate
		{
			lock (this)
			{
				NoticeInfo noticeInfo = local_notices?.NoticeArr?.Where((NoticeInfo n) => n.id == model.id && n.noticeType == model.noticeType).First();
				if (noticeInfo != null)
				{
					noticeInfo.isChecked = true;
					noticeInfo.isDeleted = true;
					SerializeDataFile();
				}
			}
		});
	}

	protected void SerializeDataFile()
	{
		Task.Factory.StartNew(delegate
		{
			lock (this)
			{
				FileHelper.WriteFileWithAesEncrypt(Configurations.NoticesPath, JsonHelper.SerializeObject2Json(local_notices));
			}
		});
	}
}
