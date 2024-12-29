using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.lmsa.common.ImportExport;

namespace lenovo.mbg.service.lmsa.common;

public class ResourceExecuteResult
{
	public Dictionary<string, Dictionary<string, int>> ResultMap;

	public Dictionary<string, int> ResultMapEx;

	public bool HasFailed => ResultMap.Values.Where((Dictionary<string, int> n) => n.ContainsKey("failed")).Sum((Dictionary<string, int> n) => n["failed"]) > 0;

	public int Status { get; set; }

	public ResourceExecuteResult()
	{
		ResultMap = new Dictionary<string, Dictionary<string, int>>();
		ResultMapEx = new Dictionary<string, int>
		{
			{ "success", 0 },
			{ "failed", 0 },
			{ "undo", 0 }
		};
		Status = 0;
	}

	public void Update(bool success)
	{
		UpdateStatus(success);
		if (success)
		{
			ResultMapEx["success"]++;
		}
		else
		{
			ResultMapEx["failed"]++;
		}
	}

	public void Update(AppDataTransferHelper.BackupRestoreResult _result)
	{
		UpdateStatus(_result == AppDataTransferHelper.BackupRestoreResult.Success);
		switch (_result)
		{
		case AppDataTransferHelper.BackupRestoreResult.Success:
			ResultMapEx["success"]++;
			break;
		case AppDataTransferHelper.BackupRestoreResult.Fail:
			ResultMapEx["failed"]++;
			break;
		default:
			ResultMapEx["undo"]++;
			break;
		}
	}

	public void SetIsInternal(bool _isInternal)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("IsInternal", _isInternal ? 1 : 0);
		ResultMap.Add("Setting", dictionary);
	}

	public void Update(string resourceType, int count, AppDataTransferHelper.BackupRestoreResult _result, bool retry)
	{
		if (!ResourceTypeDefine.ResourceTypeMap.ContainsKey(resourceType))
		{
			return;
		}
		string key = ResourceTypeDefine.ResourceTypeMap[resourceType];
		if (!ResultMap.ContainsKey(key))
		{
			ResultMap.Add(key, new Dictionary<string, int>
			{
				{ "success", 0 },
				{ "failed", 0 },
				{ "undo", 0 }
			});
		}
		UpdateStatus(_result == AppDataTransferHelper.BackupRestoreResult.Success);
		if (!retry)
		{
			switch (_result)
			{
			case AppDataTransferHelper.BackupRestoreResult.Success:
				ResultMap[key]["success"] += count;
				break;
			case AppDataTransferHelper.BackupRestoreResult.Fail:
				ResultMap[key]["failed"] += count;
				break;
			default:
				ResultMap[key]["undo"] += count;
				break;
			}
			return;
		}
		switch (_result)
		{
		case AppDataTransferHelper.BackupRestoreResult.Success:
			ResultMap[key]["success"] += count;
			if (ResultMap[key]["failed"] > 0)
			{
				ResultMap[key]["failed"] -= count;
			}
			else
			{
				ResultMap[key]["undo"] -= count;
			}
			break;
		case AppDataTransferHelper.BackupRestoreResult.Fail:
			ResultMap[key]["failed"] += count;
			ResultMap[key]["undo"] -= count;
			break;
		}
		if (ResultMap[key]["undo"] < 0)
		{
			ResultMap[key]["undo"] = 0;
		}
	}

	public void UpdateClone(string resourceType, int success, int failed)
	{
		if (ResourceTypeDefine.ResourceTypeMap.ContainsKey(resourceType))
		{
			string key = ResourceTypeDefine.ResourceTypeMap[resourceType];
			UpdateStatus(success > 0);
			if (!ResultMap.ContainsKey(key))
			{
				ResultMap.Add(key, new Dictionary<string, int>
				{
					{ "success", success },
					{ "failed", failed }
				});
			}
		}
	}

	private void UpdateStatus(bool success)
	{
		if (Status == 0 && success)
		{
			Status = 1;
		}
	}
}
