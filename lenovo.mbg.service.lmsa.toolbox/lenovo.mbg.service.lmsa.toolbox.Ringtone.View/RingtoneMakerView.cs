using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;
using lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.Controls.Windows;
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.View;

public partial class RingtoneMakerView : Window, IComponentConnector
{
	private RingtoneMakerModel vm;

	private WaveOut waveOut;

	private FileStream stream;

	private Mp3FileReader reader;

	private volatile bool isThreadActive;

	public RingtoneMakerView()
	{
		InitializeComponent();
		vm = new RingtoneMakerModel();
		base.CommandBindings.Add(new CommandBinding(vm.CloseCommand, delegate
		{
			OnCloseCommand();
		}));
		base.CommandBindings.Add(new CommandBinding(vm.LoadCommand, delegate
		{
			OnLoadCommand();
		}));
		base.CommandBindings.Add(new CommandBinding(vm.PlayCommand, delegate
		{
			OnPlayCommand();
		}));
		base.CommandBindings.Add(new CommandBinding(vm.SaveCommand, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			string text = e.Parameter as string;
			if (!string.IsNullOrEmpty(text))
			{
				OnSaveCommand(text);
			}
		}));
		base.CommandBindings.Add(new CommandBinding(vm.StartCommand, delegate
		{
			vm.Start = vm.Current;
		}));
		base.CommandBindings.Add(new CommandBinding(vm.SetEndCommand, delegate
		{
			vm.End = vm.Current;
		}));
		base.CommandBindings.Add(new CommandBinding(vm.RingtoneCommand, delegate
		{
			OnRingtoneCommand();
		}));
		base.CommandBindings.Add(new CommandBinding(vm.IncreaseCommand, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			string text2 = e.Parameter as string;
			if (text2 == "PART_StartText")
			{
				if (vm.Start + 1.0 < vm.End)
				{
					vm.Start += 1.0;
				}
				else
				{
					vm.Start = vm.End;
				}
			}
			else if (text2 == "PART_EndText")
			{
				if (vm.End + 1.0 < vm.Max)
				{
					vm.End += 1.0;
				}
				else
				{
					vm.End = vm.Max;
				}
			}
		}));
		base.CommandBindings.Add(new CommandBinding(vm.DecreaseCommand, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			string text3 = e.Parameter as string;
			if (text3 == "PART_StartText")
			{
				if (vm.Start - 1.0 < 0.0)
				{
					vm.Start = 0.0;
				}
				else
				{
					vm.Start -= 1.0;
				}
			}
			else if (text3 == "PART_EndText")
			{
				if (vm.End - 1.0 < vm.Start)
				{
					vm.End = vm.Start;
				}
				else
				{
					vm.End -= 1.0;
				}
			}
		}));
		base.CommandBindings.Add(new CommandBinding(vm.SelModelCommand, delegate
		{
			saveModel.BeginAnimation(Popup.IsOpenProperty, null);
			saveModel.IsOpen = true;
		}));
		base.DataContext = vm;
		player.StartChangedEvent = delegate(double param)
		{
			if (reader != null && vm.Current < param)
			{
				reader.TimeCurrent = param;
				vm.Current = param;
			}
		};
		player.EndChangedEvent = delegate(double param)
		{
			if (reader != null && vm.Current > param)
			{
				vm.Current = param;
			}
		};
		WaveCallbackInfo callbackInfo = WaveCallbackInfo.FunctionCallback();
		waveOut = new WaveOut(callbackInfo);
		waveOut.DeviceNumber = 0;
		waveOut.DesiredLatency = 300;
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		else
		{
			vm.IsDevOnLine = false;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		vm.IsDevOnLine = e == DeviceSoftStateEx.Online;
	}

	private void OnLoadCommand()
	{
		Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
		dlg.Filter = "MP3|*.mp3";
		dlg.Multiselect = false;
		if (dlg.ShowDialog() != true)
		{
			return;
		}
		StopMusic();
		vm.PlayTime = 0.0;
		vm.MediaFile = string.Empty;
		AnimationProgressWindow prg = new AnimationProgressWindow("K0235");
		Task.Factory.StartNew(delegate
		{
			try
			{
				stream = File.Open(dlg.FileName, FileMode.Open);
				reader = new Mp3FileReader(stream);
				int num = 0;
				double num2 = 0.0;
				short[] array = new short[9600];
				new List<short>();
				List<double> dataArr = new List<double>();
				reader.ReadNextFrame();
				reader.ReadFrameSample(array);
				while ((num = reader.ReadFrameSample(array)) != 0)
				{
					num2 = array.Take(num).Average((short p) => Math.Abs(p));
					dataArr.Add(num2);
				}
				vm.Max = reader.TotalTime.TotalSeconds;
				if (dataArr.Count < 100)
				{
					player.Dispatcher.Invoke(delegate
					{
						player.ItemsSource = dataArr;
					});
				}
				else
				{
					List<double> list = new List<double>();
					double num3 = (double)dataArr.Count / 100.0;
					for (int i = 0; i < 100; i++)
					{
						list.Add(dataArr.Skip((int)((double)i * num3)).Take((int)num3).Sum() / num3);
					}
					player.Dispatcher.Invoke(delegate
					{
						player.ItemsSource = list;
					});
				}
				vm.MediaFile = dlg.FileName;
			}
			catch
			{
				StopMusic();
			}
			finally
			{
				prg.Dispatcher.Invoke(delegate
				{
					prg.Close();
				});
			}
		});
		HostProxy.HostMaskLayerWrapper.New(prg, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => prg.ShowDialog());
	}

	private void OnPlayCommand()
	{
		if (vm.IsPlay)
		{
			if (WaveOut.DeviceCount <= 0 || string.IsNullOrEmpty(vm.MediaFile))
			{
				vm.IsPlay = false;
				return;
			}
			if (!vm.IsPause || vm.Current < vm.Start || vm.Current >= vm.End)
			{
				waveOut.Stop();
				reader.Seek(0L, SeekOrigin.Begin);
				MeteringSampleProvider sampleProvider = new MeteringSampleProvider(new SampleChannel(reader, forceStereo: true));
				waveOut.Init(sampleProvider);
				vm.Current = vm.Start;
			}
			vm.IsPause = false;
			reader.TimeCurrent = vm.Current;
			waveOut.Play();
			Task.Factory.StartNew(delegate
			{
				isThreadActive = true;
				while (isThreadActive && reader.Position < reader.Length && !(vm.Current > vm.End))
				{
					vm.Current = reader.CurrentTime.TotalSeconds;
					vm.PlayTime = vm.Current - vm.Start;
					Thread.Sleep(20);
				}
				vm.Current = vm.End;
				waveOut.Stop();
				vm.IsPlay = false;
				isThreadActive = false;
			});
		}
		else
		{
			waveOut.Stop();
			vm.IsPause = true;
			isThreadActive = false;
		}
	}

	private void OnSaveCommand(string param)
	{
		if (!ValidateMusicEditOk())
		{
			return;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(vm.MediaFile);
		string ringtoneFileName = GetRingtoneFileName(fileNameWithoutExtension, param);
		if (!string.IsNullOrEmpty(ringtoneFileName))
		{
			if (param == "PC")
			{
				SaveToPC(ringtoneFileName);
			}
			else
			{
				SaveToDevice(ringtoneFileName);
			}
		}
	}

	private void SaveToPC(string file)
	{
		AnimationProgressWindow prg = new AnimationProgressWindow("K0253");
		SaveRingtone(file, delegate(bool isOk)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				string msg = "K0237";
				if (!isOk)
				{
					msg = "K0236";
				}
				prg.Dispatcher.Invoke(delegate
				{
					prg.Message = msg;
					prg.IsBtnVisible = true;
				});
			});
		});
		HostProxy.HostMaskLayerWrapper.New(prg, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => prg.ShowDialog());
	}

	private void SaveToDevice(string file)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (currentDevice == null)
		{
			return;
		}
		BusinessData businessData = new BusinessData(BusinessType.RINGTONE_MAKER_SETASRINGTONE, currentDevice);
		HostProxy.PermissionService.BeginConfirmAppIsReady(currentDevice, "SetRingTone", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				AnimationProgressWindow prg = null;
				Stopwatch sw = new Stopwatch();
				sw.Start();
				SaveRingtone(file, delegate(bool isOk)
				{
					sw.Stop();
					HostProxy.BehaviorService.Collect(BusinessType.RINGTONE_MAKER_SAVE2DEVICE, businessData.Update(sw.ElapsedMilliseconds, isOk ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
					string msg = string.Empty;
					if (!isOk)
					{
						msg = "K0236";
					}
					else
					{
						TransferResult transferResult = SaveAndSetRingtone(file, "importMusicFiles", "transferStatus", MusicType.Call);
						switch (transferResult)
						{
						case TransferResult.FAILD:
							if (currentDevice.Property.AndroidVersion.Contains("6.0"))
							{
								HostProxy.CurrentDispatcher?.Invoke(delegate
								{
									RegrantAppPermissionTips win1 = new RegrantAppPermissionTips();
									HostProxy.HostMaskLayerWrapper.New(win1, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
									{
										win1.ShowDialog();
									});
								});
							}
							msg = "K0553";
							break;
						case TransferResult.NOT_ENOUGH_SPACE:
							msg = "K0238";
							break;
						default:
							msg = ((transferResult == TransferResult.SUCCESS) ? "K0237" : "K0236");
							File.Exists(file);
							break;
						}
					}
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						prg.Message = msg;
						prg.IsBtnVisible = true;
					});
				});
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					prg = new AnimationProgressWindow("K0253");
					HostProxy.HostMaskLayerWrapper.New(prg, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => prg.ShowDialog());
				});
			}
		});
	}

	private void OnCloseCommand()
	{
		StopMusic();
		waveOut?.Dispose();
		HostProxy.deviceManager.MasterDeviceChanged -= DeviceManager_MasterDeviceChanged;
		Close();
	}

	private void StopMusic()
	{
		vm.IsPause = false;
		isThreadActive = false;
		waveOut?.Stop();
		stream?.Close();
		reader?.Close();
		stream = null;
		reader = null;
	}

	private void OnRingtoneCommand()
	{
		if (!ValidateMusicEditOk())
		{
			return;
		}
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (currentDevice == null)
		{
			return;
		}
		BusinessData businessData = new BusinessData(BusinessType.RINGTONE_MAKER_SETASRINGTONE, currentDevice);
		HostProxy.PermissionService.BeginConfirmAppIsReady(currentDevice, "SetRingTone", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				AnimationProgressWindow prg = null;
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(vm.MediaFile);
				string arg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
				string file = string.Format("{0}\\{1}{2}.mp3", arg, fileNameWithoutExtension, DateTime.Now.ToString("yyyyMMddHHmmss"));
				Stopwatch sw = new Stopwatch();
				sw.Start();
				SaveRingtone(file, delegate(bool isOk)
				{
					sw.Stop();
					HostProxy.BehaviorService.Collect(BusinessType.RINGTONE_MAKER_SETASRINGTONE, businessData.Update(sw.ElapsedMilliseconds, isOk ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
					string msg = string.Empty;
					if (!isOk)
					{
						msg = "K0236";
					}
					else
					{
						TransferResult transferResult = SaveAndSetRingtone(file, "setMusicAsRingtone", "setResult", (!vm.IsMessage) ? MusicType.Call : MusicType.Notification);
						switch (transferResult)
						{
						case TransferResult.FAILD:
							if (currentDevice.Property.AndroidVersion.Contains("6.0"))
							{
								HostProxy.CurrentDispatcher?.Invoke(delegate
								{
									RegrantAppPermissionTips win1 = new RegrantAppPermissionTips();
									HostProxy.HostMaskLayerWrapper.New(win1, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
									{
										win1.ShowDialog();
									});
								});
							}
							msg = "K0553";
							break;
						case TransferResult.NOT_ENOUGH_SPACE:
							msg = "K0238";
							break;
						default:
							msg = ((transferResult == TransferResult.SUCCESS) ? "K0552" : "K0553");
							File.Exists(file);
							break;
						}
					}
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						prg.Message = msg;
						prg.IsBtnVisible = true;
					});
				});
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					prg = new AnimationProgressWindow("K0253");
					HostProxy.HostMaskLayerWrapper.New(prg, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => prg.ShowDialog());
				});
			}
		});
	}

	private void SaveRingtone(string fileName, Action<bool> callback = null)
	{
		if (string.IsNullOrEmpty(fileName) || fileName.Length >= 255)
		{
			callback?.Invoke(obj: false);
			return;
		}
		waveOut.Stop();
		Task.Factory.StartNew(delegate
		{
			try
			{
				reader.Seek(0L, SeekOrigin.Begin);
				using (FileStream fileStream = File.Open(fileName, FileMode.Create))
				{
					if (reader.Id3v2Tag != null)
					{
						fileStream.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
					}
					reader.TimeCurrent = vm.End;
					int frameIndex = reader.FrameIndex;
					long position = stream.Position;
					reader.TimeCurrent = vm.Start;
					int frameIndex2 = reader.FrameIndex;
					long num = position - stream.Position;
					if (reader.XingHeader != null)
					{
						reader.XingHeader.Frames = frameIndex - reader.FrameIndex;
						reader.XingHeader.Bytes = (int)num;
						fileStream.Write(reader.XingHeader.Mp3Frame.RawData, 0, reader.XingHeader.Mp3Frame.RawData.Length);
					}
					int num2 = 10240;
					int count = num2;
					byte[] array = new byte[num2];
					if (!vm.IsFadein && !vm.IsFadeout)
					{
						int num3 = 0;
						while (num3 < num)
						{
							if (num3 + num2 > num)
							{
								count = (int)(num - num3);
							}
							num3 += num2;
							stream.Read(array, 0, count);
							fileStream.Write(array, 0, count);
						}
					}
					else
					{
						IntPtr context = LameApi.lame_init();
						LameApi.lame_set_in_samplerate(context, reader.WaveFormat.SampleRate);
						LameApi.lame_set_VBR(context, VBRMode.MTRH);
						LameApi.lame_set_brate(context, 128);
						LameApi.lame_set_num_channels(context, reader.WaveFormat.Channels);
						LameApi.lame_set_mode(context, MPEGMode.JointStereo);
						LameApi.lame_init_params(context);
						short[] array2 = new short[9600];
						reader.TimeCurrent = vm.Start;
						int num4 = 0;
						int num5 = 0;
						int num6 = 0;
						int num7 = 0;
						if (frameIndex - frameIndex2 < 384)
						{
							num4 = (num5 = frameIndex2 + (frameIndex - frameIndex2) / 2);
							num6 = (frameIndex - frameIndex2) / 18;
						}
						else
						{
							num4 = frameIndex2 + 192;
							num5 = frameIndex - 192;
							num6 = 21;
						}
						double num8 = 0.0;
						bool flag = false;
						bool flag2 = false;
						for (int i = frameIndex2; i <= frameIndex; i++)
						{
							flag = vm.IsFadein && i <= num4;
							flag2 = vm.IsFadeout && i >= num5;
							if (flag || flag2)
							{
								num8 = ((!flag) ? (0.9 - (double)((i - num5) / num6) * 0.1) : (0.1 + (double)((i - frameIndex2) / num6) * 0.1));
								num7 = reader.ReadFrameSample(array2);
								for (int j = 0; j < num7; j++)
								{
									array2[j] = (short)((double)array2[j] * num8);
								}
								count = LameApi.lame_encode_buffer_interleaved(context, array2, num7 / reader.WaveFormat.Channels, array, num2);
								fileStream.Write(array, 0, count);
								if (i == num4 || i == frameIndex)
								{
									count = LameApi.lame_encode_flush(context, array, num2);
									fileStream.Write(array, 0, count);
								}
							}
							else
							{
								Mp3Frame mp3Frame = reader.ReadNextFrame();
								fileStream.Write(mp3Frame.RawData, 0, mp3Frame.FrameLength);
							}
						}
						LameApi.lame_close(context);
					}
					if (reader.Id3v1Tag != null)
					{
						fileStream.Write(reader.Id3v1Tag, 0, reader.Id3v1Tag.Length);
					}
					fileStream.Flush();
					fileStream.Close();
				}
				callback?.Invoke(obj: true);
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Warn("Create ringtone file fail:" + ex);
				callback?.Invoke(obj: false);
			}
		});
	}

	public bool AnalysisResult(string strJson, string valueKey)
	{
		List<PropItem> list = JsonUtils.Parse<List<PropItem>>(strJson);
		if (list == null)
		{
			return false;
		}
		return list.FirstOrDefault((PropItem p) => p.Key.Equals(valueKey))?.Value.Equals("true") ?? false;
	}

	private TransferResult SaveAndSetRingtone(string fileName, string actionName, string resultKey, MusicType type)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return TransferResult.FAILD;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return TransferResult.FAILD;
		}
		string appSaveDir = tcpAndroidDevice.Property.InternalStoragePath + "/LMSA/Audio/";
		bool importIsSuccess = false;
		new ImportAndExportWrapper().ImportFileWithNoProgress(19, new List<string> { fileName }, (string sourcePath) => appSaveDir + Path.GetFileName(sourcePath), delegate(string path, bool isSuccess)
		{
			importIsSuccess = isSuccess;
		});
		if (!importIsSuccess)
		{
			return TransferResult.FAILD;
		}
		Thread.Sleep(4000);
		DeviceMusicManagement deviceMusicManagement = new DeviceMusicManagement();
		List<MusicInfo> musicListByName = deviceMusicManagement.GetMusicListByName(Path.GetFileName(fileName));
		if (musicListByName == null)
		{
			return TransferResult.FAILD;
		}
		MusicInfo musicInfo = musicListByName.FirstOrDefault((MusicInfo m) => m.DisplayName.Equals(Path.GetFileName(fileName)));
		if (musicInfo == null)
		{
			return TransferResult.FAILD;
		}
		if (deviceMusicManagement.SetMusicAsRingtone(musicInfo, (lenovo.mbg.service.lmsa.phoneManager.Business.MusicType)type) != 0)
		{
			return TransferResult.FAILD;
		}
		return TransferResult.SUCCESS;
	}

	public long GetDeviceFreeSpace(MessageReaderAndWriter msgRWHander)
	{
		long result = 0L;
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getInternalStorageInfo", "getInternalStorageInfoResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData) && receiveData != null)
		{
			PropItem propItem = receiveData.FirstOrDefault((PropItem p) => p.Key == "free");
			if (propItem == null)
			{
				return result;
			}
			long.TryParse(propItem.Value, out result);
		}
		return result;
	}

	private string GetRingtoneFileName(string name, string param)
	{
		string arg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
		if (param == "PC")
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath = Path.GetDirectoryName(vm.MediaFile);
			folderBrowserDialog.ShowNewFolderButton = true;
			if (System.Windows.Forms.DialogResult.OK != FolderBrowserLauncher.ShowFolderBrowser(folderBrowserDialog))
			{
				return string.Empty;
			}
			arg = folderBrowserDialog.SelectedPath;
		}
		string text = string.Format("{0}//{1}{2}.mp3", arg, name, DateTime.Now.ToString("yyyyMMddHHmmss"));
		HostProxy.ResourcesLoggingService.RegisterFile(text);
		return text;
	}

	private bool ValidateMusicEditOk()
	{
		if (string.IsNullOrEmpty(vm.MediaFile))
		{
			return false;
		}
		if (vm.End - vm.Start < 4.0)
		{
			LenovoPopupWindow info = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0071", "K0240", "K0327", null);
			info.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			HostProxy.HostMaskLayerWrapper.New(info, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => info.ShowDialog());
			return false;
		}
		return true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
