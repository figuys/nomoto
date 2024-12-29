using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.smartdevice.Steps;

namespace lenovo.mbg.service.framework.smartdevice;

public class ResultLogger
{
	protected UseCaseDevice UcDevice;

	private bool disposedValue;

	public string ClassGuid { get; }

	public long StartTimestamp { get; set; }

	public long EndTimestamp { get; set; }

	public SortedList<string, string> Info { get; set; }

	public Result OverallResult { get; set; }

	public Result? FailedResult { get; set; }

	public string FailedDescription { get; set; }

	public string FailedStepName { get; set; }

	public object Lock { get; }

	public UseCase UseCase { get; set; }

	public string SuccessPromptText { get; set; }

	public double CurrentProgress { get; set; }

	protected ConcurrentQueue<Tuple<string, bool>> LogQueue { get; }

	public string Logs => string.Join(Environment.NewLine, from n in LogQueue.ToList()
		select n.Item1);

	public string UploadLogs => string.Join(Environment.NewLine, from n in LogQueue.ToList()
		where n.Item2
		select n.Item1);

	public ResultLogger(UseCaseDevice device)
	{
		ClassGuid = Guid.NewGuid().ToString("N");
		StartTimestamp = GlobalFun.ToUtcTimeStamp(DateTime.Now);
		EndTimestamp = long.MaxValue;
		LogQueue = new ConcurrentQueue<Tuple<string, bool>>();
		Lock = new object();
		Info = new SortedList<string, string>();
		OverallResult = Result.PASSED;
		UcDevice = device;
		SuccessPromptText = string.Empty;
	}

	public void AddInfo(string name, string value)
	{
		lock (Lock)
		{
			Info[name] = value;
		}
	}

	public void AddResult(BaseStep step, Result result)
	{
		AddResult(step, result, null);
	}

	public void AddStart(BaseStep step)
	{
		AddLog($"{step.Index}.{step.Info.Name}({step.Info.Step}) start", upload: true);
		RecipeMessage recipeMessage = default(RecipeMessage);
		recipeMessage.UseCase = UseCase;
		recipeMessage.StepName = step.Info.Name;
		recipeMessage.Message = step.Info.Name;
		recipeMessage.Index = step.Index;
		recipeMessage.OverallResult = OverallResult;
		RecipeMessage recipeMessage2 = recipeMessage;
		NotifyAsync(RecipeMessageType.START, recipeMessage2);
	}

	public void AddResult(BaseStep step, Result result, string response)
	{
		lock (Lock)
		{
			string text = $"{step.Index}.{step.Info.Name}({step.Info.Step}) {result}";
			if (!string.IsNullOrEmpty(response))
			{
				text = text + ", response: " + response;
			}
			AddLog(text, upload: true);
			step.StepResult = ConvertResult(result);
			if (string.IsNullOrEmpty(step.RunResult))
			{
				step.RunResult = step.StepResult.ToString();
			}
			if (!step.IgnoreFinalResult)
			{
				if (!string.IsNullOrEmpty(response) && string.IsNullOrEmpty(FailedDescription))
				{
					FailedDescription = $"{step.Info.Name}({step.Info.Step}): {result}, {response}";
				}
				if (OverallResult != Result.QUIT)
				{
					if (step.StepResult == Result.PASSED)
					{
						OverallResult = Result.PASSED;
						FailedResult = null;
						FailedDescription = null;
					}
					else if (step.StepResult == Result.FAILED)
					{
						OverallResult = Result.FAILED;
						FailedResult = result;
						FailedStepName = step.Name;
					}
					else if (step.StepResult == Result.QUIT)
					{
						FailedStepName = step.Name;
						OverallResult = Result.QUIT;
						FailedResult = result;
					}
				}
			}
			RecipeMessage recipeMessage = default(RecipeMessage);
			recipeMessage.UseCase = UseCase;
			recipeMessage.StepName = step.Name;
			recipeMessage.Index = step.Index;
			recipeMessage.Progress = CurrentProgress;
			recipeMessage.OverallResult = OverallResult;
			RecipeMessage recipeMessage2 = recipeMessage;
			NotifyAsync(RecipeMessageType.STEP, recipeMessage2);
		}
	}

	public void AddLog(string log, bool upload = false, Exception ex = null)
	{
		string text = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} [{Thread.CurrentThread.ManagedThreadId,2}] - {log}";
		if (ex != null)
		{
			text = $"{text}{Environment.NewLine}{ex}";
		}
		LogQueue.Enqueue(new Tuple<string, bool>(text, upload));
	}

	public void LogClear()
	{
		while (!LogQueue.IsEmpty)
		{
			LogQueue.TryDequeue(out var _);
		}
	}

	public Task<object> NotifyAsync(RecipeMessageType tag, object data)
	{
		if (tag == RecipeMessageType.UNDO)
		{
			AddLog($"***********device: {UcDevice.Id}, usecase: {UseCase} finished, overResult: UnDo***********", upload: true);
			EndTimestamp = GlobalFun.ToUtcTimeStamp(DateTime.Now);
			LogHelper.LogInstance.Info(Environment.NewLine + Logs);
		}
		return UcDevice.NotifyAsync(tag, data);
	}

	private Result ConvertResult(Result result)
	{
		switch (result)
		{
		case Result.FAILED:
		case Result.ABORTED:
		case Result.ADB_CONNECT_FAILED:
		case Result.CLEAR_FACTORYMODE_FAILED:
		case Result.FASTBOOT_FLASH_SINGLEPARTITION_FAILED:
		case Result.FASTBOOT_SHELL_FAILED:
		case Result.FASTBOOT_FLASH_FAILED:
		case Result.FASTBOOT_FLASH_ERASEDATE_FAILED:
		case Result.FASTBOOT_FLASH_FILE_MATCH_FAILED:
		case Result.FASTBOOT_SLOT_SET_FAILED:
		case Result.FASTBOOT_CONNECT_FAILED:
		case Result.FIND_COMPORT_FAILED:
		case Result.SHELL_CONNECTED_FAILED:
		case Result.SHELL_RESCUE_FAILED:
		case Result.ROM_UNMATCH_FAILED:
		case Result.PROCESS_FORCED_TEREMINATION:
		case Result.FIND_PNPDEVICE_FAILED:
		case Result.DEVICE_CONNECT_FAILED:
		case Result.AUTRORIZED_FAILED:
		case Result.INSTALL_VC_RUNNINGTIME_FAILED:
		case Result.COPYFILES_FAILED:
		case Result.COPYLOGS_FAILED:
		case Result.DRIVER_INSTALL_FAILED:
			return Result.FAILED;
		case Result.PASSED:
			return Result.PASSED;
		case Result.QUIT:
		case Result.MANUAL_QUIT:
		case Result.INTERCEPTOR_QUIT:
		case Result.FASTBOOT_DEGRADE_QUIT:
		case Result.FASTBOOT_CID_CHECKE_QUIT:
		case Result.FASTBOOT_ERROR_RULES_QUIT:
		case Result.LOAD_RESOURCE_FAILED:
		case Result.LOAD_RESOURCE_FAILED_REPLACE:
		case Result.LOAD_RESOURCE_FAILED_COUNTRYCODE:
		case Result.SHELL_EXE_TERMINATED_EXIT:
		case Result.SHELL_EXE_START_FAILED:
		case Result.MODELNAME_CHECK_FAILED_QUIT:
		case Result.CHECK_ROM_FILE_FAILED:
		case Result.ROM_DIRECTORY_NOT_EXISTS:
		case Result.CLIENT_VERSION_LOWER_QUIT:
			return Result.QUIT;
		default:
			return result;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			lock (Lock)
			{
				RecipeMessage recipeMessage = default(RecipeMessage);
				recipeMessage.OverallResult = OverallResult;
				recipeMessage.UseCase = UseCase;
				recipeMessage.Message = SuccessPromptText;
				recipeMessage.Info = Info;
				recipeMessage.failedDescription = FailedDescription;
				recipeMessage.FailedResult = FailedResult;
				recipeMessage.FailedStepName = FailedStepName;
				RecipeMessage recipeMessage2 = recipeMessage;
				if (!string.IsNullOrEmpty(UcDevice.Device?.Identifer))
				{
					Info.Add("trackId", UcDevice.Device?.Identifer);
				}
				NotifyAsync(RecipeMessageType.FINISH, recipeMessage2);
				if (FailedResult.HasValue)
				{
					AddLog($"***********device: {UcDevice.Id}, usecase: {UseCase} finished, failedResult: {FailedResult}, overResult: {OverallResult}***********", upload: true);
				}
				else
				{
					AddLog($"***********device: {UcDevice.Id}, usecase: {UseCase} finished, overResult: {OverallResult}***********", upload: true);
				}
				EndTimestamp = GlobalFun.ToUtcTimeStamp(DateTime.Now);
				LogHelper.LogInstance.Info(Environment.NewLine + Logs);
			}
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
