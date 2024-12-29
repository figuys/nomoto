using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public abstract class BaseStep : IDisposable
{
	private static readonly Random rand = new Random();

	protected bool audited;

	protected List<string> CacheComports = new List<string>();

	protected static object locker = new object();

	public string Name => Info.Name;

	public Recipe Recipe { get; private set; }

	public StepInfo Info { get; private set; }

	protected ResultLogger Log => Recipe.Log;

	protected SortedList<string, dynamic> Cache => Recipe.Cache;

	protected RecipeResources Resources => Recipe.Resources;

	protected SortedList<string, dynamic> CheckedLimits { get; private set; }

	protected AutoResetEvent WaitEvent { get; private set; }

	protected int TimeoutMilliseconds { get; set; }

	public string condition { get; private set; }

	public List<BaseStep> SubSteps { get; private set; }

	protected List<string> SkipCommands { get; private set; }

	protected List<JObject> ConditionSkipCommands { get; private set; }

	protected string OutCondition { get; private set; }

	protected List<string> IgnoreResultCommands { get; private set; }

	public virtual int Retry { get; set; }

	public int Index { get; set; }

	public string RunResult { get; set; }

	public Result StepResult { get; set; }

	public bool IgnoreCurrStepResult { get; private set; }

	public bool IgnoreFinalResult { get; private set; }

	public abstract void Run();

	public virtual bool RunSubSteps()
	{
		if (SubSteps != null && SubSteps.Count > 0)
		{
			foreach (BaseStep subStep in SubSteps)
			{
				Log.AddLog($"Middle::Running substep '{subStep.Info.Name}({subStep.Info.Step})'");
				subStep.Run();
				if (Log.OverallResult == Result.QUIT || Log.OverallResult == Result.FAILED)
				{
					return false;
				}
			}
		}
		return true;
	}

	protected string AnalysisFailedResponse(string response)
	{
		if (string.IsNullOrEmpty(response))
		{
			return null;
		}
		return response.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToList()
			.FirstOrDefault((string n) => Regex.IsMatch(n, "(fail)|(error)", RegexOptions.IgnoreCase));
	}

	public virtual DeviceEx GetDevice(ConnectType connectType, Predicate<object> predicate = null)
	{
		lock (locker)
		{
			IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
			DeviceEx deviceEx = null;
			Predicate<object> tp = predicate ?? ((Predicate<object>)((object s) => true));
			if (conntectedDevices != null && conntectedDevices.Count > 0)
			{
				deviceEx = ((Recipe.Device == null) ? conntectedDevices.FirstOrDefault((DeviceEx n) => n.WorkType == DeviceWorkType.None && n.ConnectType == connectType && tp(n)) : conntectedDevices.FirstOrDefault((DeviceEx n) => n.ConnectType == connectType && n.Identifer == Recipe.Device.Identifer && tp(n)));
			}
			Recipe.SetRecipeDevice(deviceEx);
			return deviceEx;
		}
	}

	public void Load(Recipe recipe, StepInfo info)
	{
		Recipe = recipe;
		Info = info;
		SkipCommands = new List<string>();
		IgnoreResultCommands = new List<string>();
		Retry = 0;
		if (info.Args != null)
		{
			IgnoreCurrStepResult = Info.Args.IgnoreCurrStepResult ?? ((object)false);
			IgnoreFinalResult = Info.Args.IgnoreFinalResult ?? ((object)false);
			if (info.Args.SkipCommands != null)
			{
				SkipCommands = info.Args.SkipCommands.ToObject<List<string>>();
			}
			if (info.Args.IgnoreResultCommands != null)
			{
				IgnoreResultCommands = info.Args.IgnoreResultCommands.ToObject<List<string>>();
			}
			if (info.Args.Retry != null)
			{
				Retry = info.Args.Retry;
			}
			if (info.Args.ConditionSkipCommands != null)
			{
				OutCondition = info.Args.ConditionSkipCommands.condition;
				if (info.Args.ConditionSkipCommands.Commands != null && info.Args.ConditionSkipCommands.Commands is JArray)
				{
					ConditionSkipCommands = info.Args.ConditionSkipCommands.Commands.ToObject<List<JObject>>();
				}
			}
		}
		if (info.SubSteps != null)
		{
			condition = info.SubSteps.Condition?.Value;
			if (info.SubSteps.Steps != null)
			{
				SubSteps = new List<BaseStep>();
				foreach (dynamic item in info.SubSteps.Steps)
				{
					StepInfo stepInfo = new StepInfo();
					stepInfo.Load(item);
					BaseStep baseStep = StepHelper.LoadNew<BaseStep>(stepInfo.Step);
					baseStep.Load(Recipe, stepInfo);
					SubSteps.Add(baseStep);
				}
			}
		}
		CheckedLimits = new SortedList<string, object>();
		if (Info.Args?.Timeout != null)
		{
			TimeoutMilliseconds = Info.Args?.Timeout;
			WaitEvent = new AutoResetEvent(initialState: false);
		}
	}

	public virtual bool Audit()
	{
		double? num = Info.Args.AuditPercent;
		if (!num.HasValue)
		{
			return true;
		}
		if (rand.NextDouble() >= num)
		{
			return true;
		}
		LogHelper.LogInstance.Debug($"Audit selected for {Name}({Info.Step})");
		bool flag = true;
		if (Info.Args.AuditSetup != null)
		{
			flag = Info.Args.AuditSetup;
		}
		try
		{
			if (flag)
			{
				Setup();
				if (Info.Args.AuditSettings != null)
				{
					Set(Info.Args.SettingsType.ToString(), Info.Args.AuditSettings);
				}
			}
			string type = Info.Args.PromptType;
			string text = Info.Args.PromptText;
			Result num2 = Prompt(type, text);
			audited = true;
			if (num2 == Result.PASSED)
			{
				LogHelper.LogInstance.Debug($"Audit failure for {Name}({Info.Step})");
				return false;
			}
			LogHelper.LogInstance.Debug($"Audit complete for {Name}({Info.Step})");
		}
		finally
		{
			if (flag)
			{
				TearDown();
			}
		}
		return true;
	}

	public virtual string LoadToolPath(string exe = null)
	{
		exe = exe ?? Info.Args.EXE;
		if (string.IsNullOrEmpty(exe))
		{
			exe = "fastboot.exe";
		}
		string text = Resources.GetLocalFilePath(exe);
		if (string.IsNullOrEmpty(text))
		{
			text = Configurations.FastbootPath;
		}
		Log.AddLog("fastboot tool path: " + text, upload: true);
		if (!File.Exists(text))
		{
			Log.AddLog(text + " not exists", upload: true);
		}
		return text;
	}

	public virtual void TearDown()
	{
		foreach (string key in CheckedLimits.Keys)
		{
			string text = $"{Name}-{key}";
			double num = CheckedLimits[key].Min;
			double num2 = CheckedLimits[key].Max;
			double num3 = CheckedLimits[key].Value;
			Result result = CheckedLimits[key].Result;
			LogHelper logInstance = LogHelper.LogInstance;
			object[] args = new string[5]
			{
				text,
				num.ToString(),
				num3.ToString(),
				num2.ToString(),
				result.ToString()
			};
			logInstance.Info(string.Format("{0} - {1} < {2} < {3}: {4}", args));
		}
	}

	public virtual void Setup()
	{
	}

	protected void ProgressUpdate(double progress)
	{
		double num = Info.Args["ProgressStart"];
		double num2 = (double)Info.Args["ProgressEnd"] - num;
		double progress2 = num + num2 * (progress / 100.0);
		RecipeMessage recipeMessage = default(RecipeMessage);
		recipeMessage.Progress = progress2;
		recipeMessage.UseCase = Log.UseCase;
		recipeMessage.OverallResult = Result.PROGRESS;
		RecipeMessage recipeMessage2 = recipeMessage;
		Log.NotifyAsync(RecipeMessageType.PROGRESS, recipeMessage2);
	}

	protected virtual void Set(string settingType, dynamic settings)
	{
	}

	protected Result Prompt(string type, string text)
	{
		if (audited)
		{
			text = "Please re-check: " + text;
		}
		List<string> list = new List<string>();
		foreach (object item2 in Info.Args.ButtonContent)
		{
			string item = (string)(dynamic)item2;
			list.Add(item);
		}
		string ok = null;
		string cancel = null;
		if (list.Count == 1)
		{
			ok = list[0];
		}
		else if (list.Count == 2)
		{
			ok = list[0];
			cancel = list[1];
		}
		if (Recipe.UcDevice.MessageBox.Show(Info.Name, text, ok, cancel).Result != true)
		{
			return Result.QUIT;
		}
		return Result.PASSED;
	}

	protected Task<List<string>> ComportMonitorTask(CancellationTokenSource tokenSource)
	{
		return Task.Run(delegate
		{
			List<string> result = new List<string>();
			List<string> list = new List<string>();
			new List<string>();
			CacheComports = new List<string>();
			do
			{
				List<string> comInfo = GlobalFun.GetComInfo();
				list = comInfo.Except(CacheComports).ToList();
				list.ForEach(delegate(string n)
				{
					result.Add($"device connected-{DateTime.Now:HH:mm:ss}: {n}");
				});
				CacheComports.AddRange(list);
				CacheComports.Except(comInfo).ToList().ForEach(delegate(string n)
				{
					result.Add($"device removed-{DateTime.Now:HH:mm:ss}: {n}");
					CacheComports.Remove(n);
				});
				Thread.Sleep(1000);
			}
			while (!tokenSource.IsCancellationRequested);
			if (result.Count == 0)
			{
				result.Add("no device connected");
			}
			return result;
		}, tokenSource.Token);
	}

	protected void PrintUnknownDevice()
	{
		List<string> list = new List<string>();
		using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_PnPEntity where service='WINUSB' or Status='Error'"))
		{
			foreach (ManagementBaseObject item in managementObjectSearcher.Get())
			{
				string text = item.GetPropertyValue("Status") as string;
				if (string.IsNullOrEmpty(text) || text.Equals("error", StringComparison.CurrentCultureIgnoreCase))
				{
					string text2 = item.GetPropertyValue("Name").ToString();
					string text3 = null;
					try
					{
						text3 = item.GetPropertyValue("DeviceID").ToString();
					}
					catch
					{
					}
					list.Add("Name: " + text2 + ", DeviceId: " + text3 + ", Status: " + text);
				}
			}
		}
		if (list.Count > 0)
		{
			Log.AddLog("unknown device list: " + Environment.NewLine + string.Join("\r\n", list), upload: true);
		}
	}

	public bool VerifyPreContionMet()
	{
		if (Info.Args.PreCondTest != null && Info.Args.PreCondValue != null)
		{
			List<string> list = new List<string>();
			if (Info.Args.PreCondTest is JArray { HasValues: not false } jArray)
			{
				list = jArray.Values<string>().ToList();
			}
			else
			{
				list.Add((string)Info.Args.PreCondTest);
			}
			string[] source = ((string)Info.Args.PreCondValue).Split(',', ';');
			bool flag = true;
			foreach (string condTest in list)
			{
				BaseStep baseStep = Recipe.Steps.FirstOrDefault((BaseStep n) => n.Name == condTest);
				if (baseStep != null && !string.IsNullOrEmpty(baseStep.RunResult))
				{
					flag = source.Contains(baseStep.RunResult, StringComparer.CurrentCultureIgnoreCase);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}
		return true;
	}

	protected string EncapsulationFastbootCommand(string command)
	{
		if (!string.IsNullOrEmpty(command))
		{
			string text = Recipe.Device?.Identifer;
			if (!string.IsNullOrEmpty(text))
			{
				command = "-s " + text + " " + command;
			}
		}
		return command;
	}

	public virtual void Dispose()
	{
		WaitEvent?.Dispose();
	}
}
