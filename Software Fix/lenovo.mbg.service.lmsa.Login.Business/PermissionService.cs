using System.Collections.Generic;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.Login.Protocol;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class PermissionService
{
	private static PermissionService single;

	private readonly Dictionary<string, List<UserPermissionResponseData>> mPermissionList;

	private static readonly object mPermissionListLock = new object();

	public static PermissionService Single
	{
		get
		{
			if (single == null)
			{
				lock (mPermissionListLock)
				{
					if (single == null)
					{
						single = new PermissionService();
					}
				}
			}
			return single;
		}
	}

	private PermissionService()
	{
		mPermissionList = new Dictionary<string, List<UserPermissionResponseData>>();
	}

	public List<UserPermissionResponseData> GetPermission(string userId, string parentId)
	{
		lock (mPermissionListLock)
		{
			string key = userId + "#" + parentId;
			if (!mPermissionList.ContainsKey(key))
			{
				ResponseData<List<UserPermissionResponseData>> permission = WebServiceProxy.SingleInstance.GetPermission(new Dictionary<string, string> { { "privId", parentId } });
				if (string.Compare("0000", permission.Code) == 0)
				{
					mPermissionList[key] = permission.Data;
				}
				else
				{
					mPermissionList[key] = new List<UserPermissionResponseData>();
				}
				return mPermissionList[key];
			}
			return mPermissionList[key];
		}
	}

	public bool CheckPermission(string userId, string permissionId, string parentId)
	{
		List<UserPermissionResponseData> permission = GetPermission(userId, parentId);
		return permission.Exists((UserPermissionResponseData m) => string.Compare(permissionId, m.PrivId) == 0);
	}

	public void Start()
	{
		lock (mPermissionListLock)
		{
			mPermissionList.Clear();
		}
	}

	public void Stop()
	{
		lock (mPermissionListLock)
		{
			mPermissionList.Clear();
		}
	}
}
