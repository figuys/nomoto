using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FindComPorts : BaseStep
{
	protected int timeout = 180000;

	protected bool quit;

	protected static bool running = false;

	protected static bool comporttask = false;

	protected static List<string> comports;

	protected static List<string> usedcomports = new List<string>();

	public override void Run()
	{
		running = true;
		ComPortResidentTask();
		ShowConnectMessagebox();
		Result result = Result.FIND_COMPORT_FAILED;
		if (Search())
		{
			result = Result.PASSED;
		}
		else if (!quit && ShowMessageWhenReConnect())
		{
			result = (Search() ? Result.PASSED : Result.FIND_COMPORT_FAILED);
		}
		if (result == Result.PASSED)
		{
			ShowMessageWhenConnectedSuccess();
		}
		if (quit)
		{
			result = Result.MANUAL_QUIT;
		}
		running = false;
		if (result != Result.PASSED)
		{
			base.Recipe.PrintComInfo();
		}
		base.Log.AddResult(this, result, (result == Result.PASSED) ? null : "find comport failed");
	}

	protected static void ComPortResidentTask()
	{
		Thread.Sleep(new Random().Next(100));
		if (comporttask)
		{
			return;
		}
		comporttask = true;
		Task.Run(delegate
		{
			do
			{
				comports = GlobalFun.GetComInfo();
				usedcomports = usedcomports.Intersect(comports).ToList();
				if (usedcomports.Count == 0 && !running)
				{
					comports = null;
					LogHelper.LogInstance.Debug("device removed, comport resident task stop");
					comporttask = false;
				}
				Thread.Sleep(1000);
			}
			while (comporttask);
		});
	}

	protected void ShowConnectMessagebox()
	{
		object obj = base.Info.Args.ConnectTutorials;
		JObject jobj = obj as JObject;
		if (jobj != null && jobj.Value<JArray>("Steps") != null)
		{
			Task.Run(delegate
			{
				Show(jobj, "Steps");
			});
		}
		else if (base.Info.Args.ConnectSteps != null)
		{
			ConnectStepInfo _connectStep = new ConnectStepInfo
			{
				NoteText = base.Info.Args.ConnectSteps.NoteText?.ToString(),
				RetryText = base.Info.Args.ConnectSteps.RetryText?.ToString(),
				WidthRatio = (base.Info.Args.ConnectSteps.WidthRatio?.ToString() ?? "1:2:2")
			};
			Task.Run(delegate
			{
				List<ConnectSteps> list = new List<ConnectSteps>();
				foreach (dynamic item in base.Info.Args.ConnectSteps.Steps)
				{
					list.Add(new ConnectSteps
					{
						StepImage = item.Image.ToString(),
						StepContent = item.Content.ToString()
					});
				}
				_connectStep.Steps = list;
				if (!base.Recipe.UcDevice.MessageBox.AutoCloseMoreStep(base.Name, _connectStep, -1, popupWhenClose: true).HasValue)
				{
					quit = true;
				}
			});
		}
		else
		{
			if (!((base.Info.Args.PromptText != null) ? true : false))
			{
				return;
			}
			Task.Run(delegate
			{
				dynamic val = base.Info.Args.Image?.ToString();
				dynamic val2 = base.Info.Args.PromptText.ToString();
				if (base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, val2, val, showClose: true, popupWhenClose: true) == null)
				{
					quit = true;
				}
			});
		}
	}

	protected bool ShowMessageWhenReConnect()
	{
		bool result = true;
		object obj = base.Info.Args.ConnectTutorials;
		JObject jobj = obj as JObject;
		if (jobj != null && jobj.Value<JArray>("ReSteps") != null)
		{
			Task.Run(delegate
			{
				Show(jobj, "ReSteps");
			});
		}
		else if (base.Info.Args.ReconnectPromptText != null)
		{
			Task.Run(delegate
			{
				string image = base.Info.Args.Image?.ToString();
				string message = base.Info.Args.ReconnectPromptText.ToString();
				if (!base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, message, image, null, -1, null, showClose: true, popupWhenClose: true, format: true, true).HasValue)
				{
					quit = true;
				}
			});
		}
		else
		{
			result = false;
		}
		return result;
	}

	protected void ShowMessageWhenConnectedSuccess()
	{
		if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("ReleaseSteps") != null)
		{
			JArray steps = jObject.Value<JArray>("ReleaseSteps");
			LogHelper.LogInstance.Debug("Args.ConnectTutorials.ReleaseSteps will show");
			base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(null, steps, 5000, autoPlay: false, 5000.0, showPlayControl: false, showClose: false);
		}
		else if (base.Info.Args.ExPromptText != null)
		{
			string image = base.Info.Args.ExImage?.ToString();
			string message = base.Info.Args.ExPromptText.ToString();
			base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, message, image, null, 5000, null, showClose: false, popupWhenClose: false, format: true, true);
		}
	}

	private void Show(JObject jobj, string stepKey)
	{
		string title = jobj.Value<string>("Title");
		JArray steps = jobj.Value<JArray>(stepKey);
		bool autoPlay = jobj.Value<bool>("AutoPlay");
		double interval = jobj.Value<double>("Interval");
		LogHelper.LogInstance.Debug("Args.ConnectTutorials.Steps will show");
		if (!base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(title, steps, -1, autoPlay, interval).HasValue)
		{
			quit = true;
		}
	}

	protected bool Search()
	{
		DateTime now = DateTime.Now;
		int num = base.Info.Args.Timeout ?? ((object)180000);
		List<string> list = base.Info.Args.ComPorts.ToObject<List<string>>();
		bool flag = false;
		bool flag2 = false;
		Task<bool?> task = null;
		while (!quit && !flag && DateTime.Now.Subtract(now).TotalMilliseconds < (double)num && (task == null || !task.IsCompleted))
		{
			if (!comporttask)
			{
				ComPortResidentTask();
			}
			if (comports == null || comports.Count <= 0)
			{
				continue;
			}
			foreach (string item in list)
			{
				string text = comports.FirstOrDefault((string n) => Regex.IsMatch(n, item, RegexOptions.IgnoreCase));
				if (text == null)
				{
					continue;
				}
				string value = Regex.Match(text, "(?<key>COM)(?<com>\\d+)").Groups["com"].Value;
				if (!string.IsNullOrEmpty(value))
				{
					int num2 = Convert.ToInt32(value);
					if (!usedcomports.Contains(text))
					{
						base.Log.AddLog($"device: {base.Recipe.UcDevice.Id}, comport: {num2}");
						usedcomports.Add(text);
						base.Cache[item] = num2;
						base.Cache["comport"] = num2;
						flag = true;
						break;
					}
				}
			}
			if (!flag && !flag2 && base.Info.Args.Error900ePromptText != null && comports.Exists((string n) => Regex.IsMatch(n, "Qualcomm.*900e", RegexOptions.IgnoreCase)))
			{
				flag2 = true;
				base.Recipe.UcDevice.MessageBox.Close(true);
				task = base.Recipe.UcDevice.MessageBox.Show(base.Name, base.Info.Args.Error900ePromptText.ToString());
			}
		}
		string arg = ((comports == null) ? "" : string.Join("\r\n", comports));
		base.Log.AddLog($"comporttask running: {comporttask}, allcomports: {arg}");
		base.Log.AddLog("usedcomports: " + string.Join("\r\n", usedcomports));
		base.Recipe.UcDevice.MessageBox.Close(true);
		return flag;
	}
}
