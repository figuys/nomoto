using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hardwaretest.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.hardwaretest.ViewModel;

public class MainViewModel : ViewModelBase
{
	protected enum TestItemType
	{
		AudioConnection,
		Backlight,
		Battery,
		Bluetooth,
		Display,
		EarSpeaker,
		Earphone,
		Flash,
		FrontCamera,
		GPS,
		GravitySensor,
		Mirophone,
		Multitouch,
		Proximitysensor,
		Mobilenetwork,
		Receiver,
		RearCamera,
		Touchscreen,
		Speaker,
		Volumebutton,
		Vibration,
		WiFi
	}

	protected Dictionary<TestItemType, List<string>> ItemNameMap = new Dictionary<TestItemType, List<string>>
	{
		{
			TestItemType.AudioConnection,
			new List<string> { "Audio connection" }
		},
		{
			TestItemType.Backlight,
			new List<string> { "Back light", "Backlight" }
		},
		{
			TestItemType.Battery,
			new List<string> { "Battery" }
		},
		{
			TestItemType.Bluetooth,
			new List<string> { "Bluetooth" }
		},
		{
			TestItemType.Display,
			new List<string> { "Display" }
		},
		{
			TestItemType.EarSpeaker,
			new List<string> { "Ear Speaker", "EarSpeaker" }
		},
		{
			TestItemType.Earphone,
			new List<string> { "Earphone" }
		},
		{
			TestItemType.Flash,
			new List<string> { "Flash" }
		},
		{
			TestItemType.FrontCamera,
			new List<string> { "Front Camera", "FrontCamera" }
		},
		{
			TestItemType.GPS,
			new List<string> { "GPS" }
		},
		{
			TestItemType.GravitySensor,
			new List<string> { "Gravity Sensor", "GravitySensor" }
		},
		{
			TestItemType.Mirophone,
			new List<string> { "Microphone" }
		},
		{
			TestItemType.Multitouch,
			new List<string> { "Multi touch", "Multi-touch" }
		},
		{
			TestItemType.Proximitysensor,
			new List<string> { "Proximity sensor", "ProximitySensor", "Distance Sensor", "DistanceSensor" }
		},
		{
			TestItemType.Mobilenetwork,
			new List<string> { "Mobile network" }
		},
		{
			TestItemType.Receiver,
			new List<string> { "Receiver" }
		},
		{
			TestItemType.RearCamera,
			new List<string> { "Rear Camera", "RearCamera" }
		},
		{
			TestItemType.Touchscreen,
			new List<string> { "Touchscreen", "Touch Screen", "TouchScreen" }
		},
		{
			TestItemType.Speaker,
			new List<string> { "Speaker" }
		},
		{
			TestItemType.Volumebutton,
			new List<string> { "Volume", "Volume Button" }
		},
		{
			TestItemType.Vibration,
			new List<string> { "Vibration" }
		},
		{
			TestItemType.WiFi,
			new List<string> { "Wi-Fi", "WLAN" }
		}
	};

	protected Dictionary<TestItemType, TestItemViewModel> TestItemMap = new Dictionary<TestItemType, TestItemViewModel>
	{
		{
			TestItemType.AudioConnection,
			new TestItemViewModel("K1494", "Icon-Audio connection@2x.png")
		},
		{
			TestItemType.Backlight,
			new TestItemViewModel("K1495", "Icon-Backlight@2x.png")
		},
		{
			TestItemType.Battery,
			new TestItemViewModel("K1496", "Icon-Battery@2x.png")
		},
		{
			TestItemType.Bluetooth,
			new TestItemViewModel("K1497", "Icon-Buletooth@2x.png")
		},
		{
			TestItemType.Display,
			new TestItemViewModel("K1498", "Icon-Display@2x.png")
		},
		{
			TestItemType.EarSpeaker,
			new TestItemViewModel("K1499", "Icon-Ear Speaker@2x.png")
		},
		{
			TestItemType.Earphone,
			new TestItemViewModel("K1500", "Icon-Earphone@2x.png")
		},
		{
			TestItemType.Flash,
			new TestItemViewModel("K1501", "Icon-Flash@2x.png")
		},
		{
			TestItemType.FrontCamera,
			new TestItemViewModel("K1502", "Icon-Front Camera@2x.png")
		},
		{
			TestItemType.GPS,
			new TestItemViewModel("K1503", "Icon-GPS@2x.png")
		},
		{
			TestItemType.GravitySensor,
			new TestItemViewModel("K1504", "Icon-Gravity sensor@2x.png")
		},
		{
			TestItemType.Mirophone,
			new TestItemViewModel("K1505", "Icon-Miro phone@2x.png")
		},
		{
			TestItemType.Multitouch,
			new TestItemViewModel("K1506", "Icon-Multi touch@2x.png")
		},
		{
			TestItemType.Proximitysensor,
			new TestItemViewModel("K1507", "Icon-Proximity sensor@2x.png")
		},
		{
			TestItemType.Mobilenetwork,
			new TestItemViewModel("K1508", "Icon-Mobile network@2x.png")
		},
		{
			TestItemType.Receiver,
			new TestItemViewModel("K1509", "Icon-Receiver@2x.png")
		},
		{
			TestItemType.RearCamera,
			new TestItemViewModel("K1510", "Icon-Rear Camera@2x.png")
		},
		{
			TestItemType.Touchscreen,
			new TestItemViewModel("K1511", "Icon-Touch screen@2x.png")
		},
		{
			TestItemType.Speaker,
			new TestItemViewModel("K1512", "Icon-Speaker@2x.png")
		},
		{
			TestItemType.Volumebutton,
			new TestItemViewModel("K1513", "Icon-Volume button@2x.png")
		},
		{
			TestItemType.Vibration,
			new TestItemViewModel("K1514", "Icon-Vibration@2x.png")
		},
		{
			TestItemType.WiFi,
			new TestItemViewModel("K1515", "Icon-WIfi@2x.png")
		}
	};

	protected long CurTimestemp;

	private long readlocker;

	private static bool LoopRunning = true;

	private bool _IsEnabled;

	protected BusinessData businessData;

	protected List<HwTestResultModel> TestResult;

	protected List<string> TestItems;

	protected Stopwatch sw = new Stopwatch();

	private bool _Started;

	private Uri _StarttingUri;

	public ObservableCollection<TestItemViewModel> Items { get; set; }

	public ReplayCommand ReadResultCommand { get; }

	public ReplayCommand StartTestCommand { get; }

	public bool IsEnabled
	{
		get
		{
			return _IsEnabled;
		}
		set
		{
			_IsEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public bool Started
	{
		get
		{
			return _Started;
		}
		set
		{
			_Started = value;
			OnPropertyChanged("Started");
		}
	}

	public Uri StarttingUri
	{
		get
		{
			return _StarttingUri;
		}
		set
		{
			_StarttingUri = value;
			OnPropertyChanged("StarttingUri");
		}
	}

	public MainViewModel()
	{
		Items = new ObservableCollection<TestItemViewModel>();
		StartTestCommand = new ReplayCommand(StartTestCommandHandler);
		ReadResultCommand = new ReplayCommand(ReadResultCommandHandler);
	}

	public override void LoadData(object data)
	{
		StopLoop();
		LoadTestItem();
		base.LoadData(data);
	}

	public void StartLoop()
	{
		Task task = LoopReadResultAsync();
		Task.Run(() => task.Wait(1800000)).ContinueWith(delegate(Task<bool> ar)
		{
			if (!ar.Result)
			{
				if (!Started || FireTimeout() == true)
				{
					StopLoop();
				}
				else
				{
					LoopRunning = false;
					StartLoop();
				}
			}
		});
	}

	private bool? FireTimeout()
	{
		if (HostProxy.HostNavigation.CurrentPluginID != "985c66acdde2483ed96844a6b5ea4337")
		{
			return false;
		}
		return Context.MessageBox.ShowMessage("K0071", "K1522", "K0571", "K0570", isCloseBtn: false, null, MessageBoxImage.Exclamation);
	}

	public void StopLoop()
	{
		LoopRunning = false;
		readlocker = 0L;
		IsEnabled = false;
		Started = false;
	}

	private void LoadTestItem()
	{
		Task.Run(delegate
		{
			using MessageReaderAndWriter messageReaderAndWriter = (Context.CurrentDevice as TcpAndroidDevice).MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return (List<string>)null;
			}
			long sequence = HostProxy.Sequence.New();
			List<string> receiveData = null;
			messageReaderAndWriter.SendAndReceiveSync("hardwareTestList", "hardwareTestListResponse", new List<string>(), sequence, out receiveData);
			return receiveData;
		}).ContinueWith(delegate(Task<List<string>> ar)
		{
			Context.Dispatcher.Invoke(delegate
			{
				TestItems = ar.Result;
				Items.Clear();
				if (ar.Result != null && ar.Result.Count > 0)
				{
					ar.Result.ForEach(delegate(string n)
					{
						KeyValuePair<TestItemType, List<string>> keyValuePair = ItemNameMap.FirstOrDefault((KeyValuePair<TestItemType, List<string>> s) => s.Value.Contains(n, StringComparer.CurrentCultureIgnoreCase));
						if (keyValuePair.Value != null && TestItemMap.ContainsKey(keyValuePair.Key))
						{
							TestItemViewModel testItemViewModel = TestItemMap[keyValuePair.Key];
							testItemViewModel.Test = n;
							testItemViewModel.Status = -1;
							testItemViewModel.Idx = Items.Count;
							Items.Add(testItemViewModel);
						}
					});
				}
			});
		});
	}

	private void StartTestCommandHandler(object data)
	{
		StarttingUri = null;
		if (!Started)
		{
			Task.Run(delegate
			{
				sw.Reset();
				sw.Start();
				TestResult = null;
				TcpAndroidDevice tcpAndroidDevice = Context.CurrentDevice as TcpAndroidDevice;
				businessData = new BusinessData(BusinessType.HARDWARETEST_START, tcpAndroidDevice);
				using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
				if (messageReaderAndWriter == null)
				{
					return (List<JObject>)null;
				}
				long sequence = HostProxy.Sequence.New();
				List<JObject> receiveData = null;
				messageReaderAndWriter.SendAndReceiveSync("hardwareTestHomePage", "hardwareTestHomePageResponse", new List<string>(), sequence, out receiveData);
				return receiveData;
			}).ContinueWith(delegate(Task<List<JObject>> ar)
			{
				if (ar.Result != null && ar.Result.Count > 0)
				{
					if (ar.Result[0].Value<bool>("value"))
					{
						CurTimestemp = GetStartTimeSpan();
						LogHelper.LogInstance.Debug($"Click start hwtest timestemp: {CurTimestemp}");
						Context.Dispatcher.Invoke(delegate
						{
							Started = true;
							IsEnabled = true;
							StarttingUri = new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.hardwaretest;component/Resource/Hardware testing.gif");
						});
						HostProxy.BehaviorService.Collect(BusinessType.HARDWARETEST_START, businessData.Update(sw.ElapsedMilliseconds, BusinessStatus.CLICK, TestItems));
						StartLoop();
					}
					else
					{
						string msg = string.Format(LangTranslation.Translate("K1567"), "Mobile Assistant");
						if (Context.CurrentDevice.ConnectedAppType == "Moto")
						{
							msg = string.Format(LangTranslation.Translate("K1567"), "Device Help");
						}
						Context.MessageBox.ShowMessage("K0711", msg, "K0327", null, isCloseBtn: true);
					}
				}
			});
		}
		else
		{
			StopLoop();
			Task.Run(delegate
			{
				sw.Stop();
				long elapsedMilliseconds = sw.ElapsedMilliseconds;
				ReadResultAsync().Wait();
				BusinessData businessData = BusinessData.Clone(this.businessData);
				businessData.useCaseStep = BusinessType.HARDWARETEST_FINISHED.ToString();
				HostProxy.BehaviorService.Collect(BusinessType.HARDWARETEST_FINISHED, businessData.Update(elapsedMilliseconds, BusinessStatus.SUCCESS, TestResult));
			});
		}
	}

	private void ReadResultCommandHandler(object data)
	{
		ReadResultAsync();
	}

	private Task LoopReadResultAsync()
	{
		return Task.Run(delegate
		{
			lock (this)
			{
				LoopRunning = true;
				do
				{
					Thread.Sleep(60000);
					if (!LoopRunning)
					{
						break;
					}
					ReadResultAsync()?.Wait();
				}
				while (LoopRunning && Started);
				Thread.Sleep(10000);
			}
		});
	}

	private Task<List<HwTestResultModel>> ReadResultAsync()
	{
		if (Interlocked.Read(ref readlocker) != 0L)
		{
			return null;
		}
		Interlocked.Exchange(ref readlocker, 1L);
		return Task.Run(delegate
		{
			using MessageReaderAndWriter messageReaderAndWriter = (Context.CurrentDevice as TcpAndroidDevice).MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return (List<HwTestResultModel>)null;
			}
			long sequence = HostProxy.Sequence.New();
			List<HwTestResultModel> receiveData = null;
			messageReaderAndWriter.SendAndReceiveSync("readHardwareTestResult", "readHardwareTestResultResponse", new List<string>(), sequence, out receiveData);
			return receiveData;
		}).ContinueWith(delegate(Task<List<HwTestResultModel>> ar)
		{
			if (ar.Result != null && ar.Result.Count > 0)
			{
				TestResult = ar.Result;
				LogHelper.LogInstance.Info("Revice readHardwareTestResultResponse: " + JsonHelper.SerializeObject2Json(ar.Result));
				Context.Dispatcher.Invoke(delegate
				{
					ar.Result.ForEach(delegate(HwTestResultModel n)
					{
						TestItemViewModel testItemViewModel = Items.FirstOrDefault((TestItemViewModel m) => m.Test.Equals(n.hardwareTest, StringComparison.CurrentCultureIgnoreCase));
						if (testItemViewModel != null && n.lastRun >= CurTimestemp)
						{
							testItemViewModel.Status = n.result;
						}
					});
				});
			}
			else
			{
				LogHelper.LogInstance.Info("Revice readHardwareTestResultResponse: null");
			}
			Interlocked.Exchange(ref readlocker, 0L);
			return ar.Result;
		});
	}

	private long GetStartTimeSpan()
	{
		DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		DateTime dateTime2 = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now).AddDays(-2.0);
		return (long)(dateTime2 - dateTime).TotalMilliseconds;
	}
}
