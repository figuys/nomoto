using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.Component.Progress;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class DeviceSmsManagement
{
	public int GetCount(string address = null)
	{
		int result = -1;
		List<PropItem> receiveData = null;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return 0;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		long sequence = HostProxy.Sequence.New();
		try
		{
			if (tcpAndroidDevice != null && messageReaderAndWriter != null && messageReaderAndWriter.SendAndReceiveSync("getSmsCount", "getSmsCountResponse", new List<string> { address }, sequence, out receiveData) && receiveData != null && receiveData != null)
			{
				string text = (from m in receiveData
					where "count".Equals(m.Key)
					select m.Value).FirstOrDefault();
				if (!string.IsNullOrEmpty(text))
				{
					int.TryParse(text, out result);
				}
			}
		}
		catch
		{
		}
		return result;
	}

	public List<string> getIdListByAddress(List<string> addressList, IAsyncTaskContext taskContext)
	{
		int serviceCode = 21;
		string methodName = "getIdListByAddress";
		Header header = new Header();
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		AppServiceRequest appServiceRequest = new AppServiceRequest(tcpAndroidDevice.ExtendDataFileServiceEndPoint, tcpAndroidDevice.RsaSocketEncryptHelper);
		AppServiceResponse responseHandler = null;
		try
		{
			responseHandler = appServiceRequest.Request(serviceCode, methodName, header, null);
			if (responseHandler != null)
			{
				taskContext.AddCancelSource(delegate
				{
					responseHandler.Dispose();
				});
			}
			header.AddOrReplace("Status", "-6");
			header.AddOrReplace("addressList", JsonConvert.SerializeObject(addressList));
			responseHandler.WriteHeader(header);
			return JsonConvert.DeserializeObject<List<string>>(responseHandler.ReadHeader(null).GetString("idList"));
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Get id list by address throw ex:" + ex);
			return null;
		}
		finally
		{
			if (responseHandler != null)
			{
				try
				{
					responseHandler.Dispose();
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public void Export(List<SMSContactMerged> addressList, Action<string> itemStartCallback, Action<string, bool> itemFinishCallback, Action<string, int, long, long> itemProgressCallback, IAsyncTaskContext taskContext, string saveFileFullName)
	{
		int serviceCode = 21;
		string methodName = "exportSmsByAddress";
		Header header = new Header();
		_ = addressList.Count;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return;
		}
		AppServiceRequest appServiceRequest = new AppServiceRequest(tcpAndroidDevice.ExtendDataFileServiceEndPoint, tcpAndroidDevice.RsaSocketEncryptHelper);
		Header header2 = null;
		AppServiceResponse responseHandler = null;
		System.IO.FileInfo fileInfo = new System.IO.FileInfo(saveFileFullName);
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(fileInfo.Create());
			streamWriter.Write(" [ ");
			responseHandler = appServiceRequest.Request(serviceCode, methodName, header, delegate(int readLength, long readTotal, long lengthTotal)
			{
				itemProgressCallback(string.Empty, readLength, readTotal, lengthTotal);
			});
			if (responseHandler != null)
			{
				taskContext.AddCancelSource(delegate
				{
					responseHandler.Dispose();
				});
			}
			bool flag = true;
			foreach (SMSContactMerged address in addressList)
			{
				itemStartCallback?.Invoke(address.PhoneNumber);
				header.AddOrReplace("Status", "-6");
				header.AddOrReplace("address", address.PhoneNumber);
				header.AddOrReplace("count", address.total.ToString());
				responseHandler.WriteHeader(header);
				for (int i = 0; i < address.total; i++)
				{
					header.AddOrReplace("Status", "-6");
					responseHandler.WriteHeader(header);
					header2 = responseHandler.ReadHeader(delegate(int readLength, long readTotal, long lengthTotal)
					{
						itemProgressCallback(address.PhoneNumber, readLength, readTotal, lengthTotal);
					});
					string @string = header2.GetString("Status");
					if (!"-6".Equals(@string))
					{
						if ("-11".Equals(@string))
						{
							for (; i < address.total; i++)
							{
								itemFinishCallback?.Invoke(address.PhoneNumber, arg2: false);
							}
						}
						break;
					}
					string string2 = header2.GetString("SmsContent");
					if (!flag)
					{
						streamWriter.Write(",");
					}
					streamWriter.WriteLine(string2);
					itemFinishCallback?.Invoke(address.PhoneNumber, arg2: true);
					flag = false;
				}
			}
			streamWriter.Write(" ] ");
			header.AddOrReplace("Status", "-11");
			responseHandler.WriteHeader(header);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Backup export resource throw exception:" + ex.ToString());
			try
			{
				fileInfo.Delete();
			}
			catch (Exception)
			{
			}
			throw;
		}
		finally
		{
			if (responseHandler != null)
			{
				try
				{
					responseHandler.Dispose();
				}
				catch (Exception)
				{
				}
			}
			if (streamWriter != null)
			{
				try
				{
					streamWriter.Close();
					streamWriter.Dispose();
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public TransferResult Export(IAsyncTaskContext context, List<SMSContactMerged> smsContacts, string filepath, Action<int, bool> callBack)
	{
		bool flag = false;
		string path = Path.Combine(filepath, "SMS" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".sms");
		try
		{
			using FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
			using StreamWriter streamWriter = new StreamWriter(stream);
			new JsonSerializer();
			_ = smsContacts.Count;
			streamWriter.Write(" [ ");
			bool flag2 = false;
			foreach (SMSContactMerged smsContact in smsContacts)
			{
				List<string> addressList = smsContact.MergedList.Select((IContact m) => m.PhoneNumber).ToList();
				if (context != null && context.IsCancelCommandRequested)
				{
					break;
				}
				List<SMS> list = null;
				int num = 0;
				while (context == null || !context.IsCancelCommandRequested)
				{
					list = null;
					if (GetSmsByAddressEx(context, addressList, num, 20, "asc", out list))
					{
						if (list == null)
						{
							flag = false;
							break;
						}
						if (list.Count == 0)
						{
							break;
						}
						string text = JsonUtils.Stringify(list).Trim();
						if (string.IsNullOrEmpty(text))
						{
							flag = false;
							break;
						}
						if (flag2)
						{
							streamWriter.Write(" , ");
						}
						flag2 = true;
						text = text.Trim('[', ']');
						streamWriter.Write(text);
						num++;
						flag = true;
						callBack(list.Count, arg2: true);
						continue;
					}
					flag = false;
					callBack(list.Count, arg2: false);
					break;
				}
				if (!flag)
				{
					break;
				}
			}
			streamWriter.Write(" ] ");
		}
		catch
		{
			callBack(0, arg2: false);
			flag = false;
		}
		if (!flag)
		{
			try
			{
				File.Delete(path);
			}
			catch (Exception)
			{
			}
		}
		HostProxy.ResourcesLoggingService.RegisterFile(path);
		if (!flag)
		{
			return TransferResult.FAILD;
		}
		return TransferResult.SUCCESS;
	}

	private List<SMS> LoadSmsFile(string file)
	{
		List<SMS> result = new List<SMS>();
		if (!string.IsNullOrEmpty(file) && File.Exists(file))
		{
			using StreamReader reader = new StreamReader(file);
			try
			{
				JsonSerializer obj = new JsonSerializer
				{
					Converters = { (JsonConverter)new JavaScriptDateTimeConverter() },
					NullValueHandling = NullValueHandling.Ignore
				};
				JsonReader reader2 = new JsonTextReader(reader);
				result = obj.Deserialize<List<SMS>>(reader2);
			}
			catch
			{
			}
		}
		return result;
	}

	public bool Import(string fileName, Action<bool> callBack)
	{
		bool result = false;
		if (File.Exists(fileName))
		{
			try
			{
				List<SMS> sms = LoadSmsFile(fileName);
				if (sms == null || sms.Count == 0)
				{
					return result;
				}
				if (sms.Count > 0 && sms.Exists((SMS n) => string.IsNullOrEmpty(n.address) || string.IsNullOrEmpty(n.person) || string.IsNullOrEmpty(n.date)))
				{
					LogHelper.LogInstance.Info("Import sms file format is wrong, please check.");
					LogHelper.LogInstance.Info("address, person, date cann't be empty in import sms file");
					return result;
				}
				HostProxy.AsyncCommonProgressLoader.Progress(Context.MessageBox, delegate(IAsyncTaskContext context, CommonProgressWindowViewModel viewModel)
				{
					try
					{
						Action<NotifyTypes, object> exectingNotifyHandler = (Action<NotifyTypes, object>)context.ObjectState;
						exectingNotifyHandler(NotifyTypes.INITILIZE, new List<object>
						{
							new List<object>
							{
								ResourcesHelper.StringResources.SingleInstance.SMS_CONTENT,
								sms.Count
							},
							new List<ProgressPramater>
							{
								new ProgressPramater
								{
									Message = ResourcesHelper.StringResources.SingleInstance.SMS_IMPORT_MESSAGE
								}
							}
						});
						try
						{
							MassOpImpl(context, "importSms", "importSmsResponse", sms, delegate(bool success)
							{
								if (success)
								{
									exectingNotifyHandler(NotifyTypes.PERCENT, 1);
								}
							});
						}
						catch
						{
						}
						exectingNotifyHandler(NotifyTypes.SUCCESS, new List<object>
						{
							new List<ProgressPramater>
							{
								new ProgressPramater
								{
									Message = ResourcesHelper.StringResources.SingleInstance.IMPORT_SUCCESS_MESSAGE
								}
							},
							true
						});
					}
					finally
					{
						callBack(obj: true);
					}
				});
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("Import sms file occur an exception", exception);
				return result;
			}
		}
		return result;
	}

	public bool Delete(List<string> smsIds)
	{
		bool result = false;
		if (smsIds != null && smsIds.Count > 0)
		{
			using MessageReaderAndWriter messageReaderAndWriter = (HostProxy.deviceManager.MasterDevice as TcpAndroidDevice).MessageManager.CreateMessageReaderAndWriter();
			long sequence = HostProxy.Sequence.New();
			List<PropItem> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("deleteSms", "deleteSmsResponse", smsIds, sequence, out receiveData) && receiveData != null)
			{
				result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
		}
		return result;
	}

	public bool Delete(IAsyncTaskContext context, List<SMSContactMerged> smsContacts, Action<bool> callBack)
	{
		bool result = false;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { MessageManager: not null } tcpAndroidDevice))
		{
			return false;
		}
		MessageReaderAndWriter msgRWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		try
		{
			if (msgRWriter == null)
			{
				return false;
			}
			context.AddCancelSource(delegate
			{
				msgRWriter.Dispose();
			});
			ISequence sequence = HostProxy.Sequence;
			List<PropItem> receiveData = null;
			foreach (SMSContactMerged smsContact in smsContacts)
			{
				foreach (IContact merged in smsContact.MergedList)
				{
					if (context.IsCancelCommandRequested)
					{
						return false;
					}
					if (msgRWriter.SendAndReceiveSync("deleteSmsByAddress", "deleteSmsByAddressResponse", new List<string> { merged.PhoneNumber }, sequence.New(), out receiveData) && receiveData != null)
					{
						if (!receiveData.Exists((PropItem m) => "true".Equals(m.Value)))
						{
							result = false;
							callBack?.Invoke(obj: false);
							break;
						}
						for (int i = 0; i < merged.MsgTotal; i++)
						{
							callBack?.Invoke(obj: true);
						}
					}
				}
			}
			return result;
		}
		finally
		{
			if (msgRWriter != null)
			{
				((IDisposable)msgRWriter).Dispose();
			}
		}
	}

	private TransferResult MassOpImpl(IAsyncTaskContext context, string req, string resp, object param, Action<bool> callBack)
	{
		bool flag = false;
		_ = string.Empty;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return TransferResult.FAILD;
		}
		MessageReaderAndWriter msgRWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		try
		{
			context.AddCancelSource(delegate
			{
				msgRWriter.Dispose();
			});
			if (msgRWriter == null)
			{
				return TransferResult.FAILD;
			}
			HostProxy.Sequence.New();
			msgRWriter.TryEnterLock(100000);
			try
			{
				List<SMS> list = (List<SMS>)param;
			}
			finally
			{
				msgRWriter.ExitLock();
			}
		}
		finally
		{
			if (msgRWriter != null)
			{
				((IDisposable)msgRWriter).Dispose();
			}
		}
		if (!flag)
		{
			return TransferResult.FAILD;
		}
		return TransferResult.SUCCESS;
	}

	private bool MassOpImplEx(string req, string resp, SMS param)
	{
		bool result = false;
		_ = string.Empty;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		HostProxy.Sequence.New();
		messageReaderAndWriter.TryEnterLock(100000);
		try
		{
			return result;
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
	}

	public bool GetSmsInfoEx(out List<SMSContactMerged> smsContact, out int totalSmsCount)
	{
		smsContact = new List<SMSContactMerged>();
		totalSmsCount = 0;
		bool flag = false;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			try
			{
				List<SMSContactContainer> receiveData = null;
				long sequence = HostProxy.Sequence.New();
				flag = messageReaderAndWriter.SendAndReceiveSync<object, SMSContactContainer>("getSmsInfoEx", "getSmsInfoExResponse", null, sequence, out receiveData);
				if (flag && receiveData != null)
				{
					foreach (SMSContactContainer item in receiveData)
					{
						if (item.Contacts != null)
						{
							IEnumerable<SMSContact> source = item.Contacts.Where((SMSContact m) => !string.IsNullOrEmpty(m.address));
							totalSmsCount += source.Sum((SMSContact m) => m.total);
							List<SMSContactMerged> collection = ContactMergeHandler.Merge(((IEnumerable<IContact>)source).ToList());
							smsContact.AddRange(collection);
						}
					}
					smsContact = smsContact.OrderByDescending((SMSContactMerged n) => n.latest).ToList();
				}
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("get sms contact info failed", exception);
				flag = false;
			}
		}
		return flag;
	}

	private bool GetSmsByAddressEx(IAsyncTaskContext context, List<string> addressList, int pageIndex, int pageCount, string orderBy, out List<SMS> smsList)
	{
		bool result = false;
		smsList = new List<SMS>();
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		MessageReaderAndWriter rw = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		try
		{
			if (rw == null)
			{
				return false;
			}
			context.AddCancelSource(delegate
			{
				rw.Dispose();
			});
			long sequence = HostProxy.Sequence.New();
			List<string> list = new List<string>
			{
				pageIndex.ToString(),
				pageCount.ToString(),
				orderBy
			};
			list.AddRange(addressList);
			try
			{
				result = rw.SendAndReceiveSync("getSmsByAddressEx", "getSmsByAddressExResponse", list, sequence, out smsList);
				if ((smsList != null) & (smsList.Count > 0))
				{
					int i = 0;
					smsList.ForEach(delegate(SMS m)
					{
						m.OrderBySequence = i++;
						if (!string.IsNullOrEmpty(m.body))
						{
							try
							{
								m.body = GlobalFun.DecodeBase64(m.body);
							}
							catch (Exception ex)
							{
								LogHelper.LogInstance.Error($"Decode base64 string[{m.body}] throw exception[{ex.ToString()}]");
							}
						}
					});
				}
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("get sms info by address failed", exception);
				result = false;
			}
		}
		finally
		{
			if (rw != null)
			{
				((IDisposable)rw).Dispose();
			}
		}
		return result;
	}

	public bool GetSmsByAddressEx(IAsyncTaskContext context, SMSContactMerged smsContactMerged, int pageIndex, int pageCount, string orderBy, out List<SMS> smsList)
	{
		List<string> addressList = smsContactMerged.MergedList.Select((IContact m) => m.PhoneNumber).ToList();
		return GetSmsByAddressEx(context, addressList, pageIndex, pageCount, orderBy, out smsList);
	}

	public bool IsNeedSetSMSApplet()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: not null } tcpAndroidDevice))
		{
			return false;
		}
		return tcpAndroidDevice.Property.ApiLevel > 19;
	}

	public bool SmsAppletIsReady(TcpAndroidDevice device)
	{
		if (device == null)
		{
			return false;
		}
		if (device.ConnectType == ConnectType.Adb && device.Property.ApiLevel >= 29 && device != null)
		{
			((AdbDeviceEx)device).FocuseApp();
		}
		using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		try
		{
			List<PropItem> receiveData = null;
			long sequence = HostProxy.Sequence.New();
			if (messageReaderAndWriter.SendAndReceiveSync<object, PropItem>("smsAppletIsReady", "smsAppletIsReadyResponse", null, sequence, out receiveData) && receiveData != null)
			{
				return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
			return false;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("get sms contact info failed", exception);
			return false;
		}
	}

	public bool ResetSmsAppletToDefault(TcpAndroidDevice device)
	{
		if (device == null)
		{
			return false;
		}
		if (device.ConnectType == ConnectType.Adb && device.Property.ApiLevel >= 29)
		{
			((AdbDeviceEx)device).FocuseApp();
		}
		else
		{
			Thread.Sleep(1000);
		}
		using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		try
		{
			List<PropItem> receiveData = null;
			if (device.Property.ApiLevel >= 29 && device.ConnectType == ConnectType.Adb)
			{
				device.LockSoftStatus(autoRelease: true, DeviceSoftStateEx.Online);
			}
			long sequence = HostProxy.Sequence.New();
			if (messageReaderAndWriter.SendAndReceiveSync<object, PropItem>("resetSmsAppSetting", "resetSmsAppSettingResponse", null, sequence, out receiveData) && receiveData != null)
			{
				return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
			return false;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("Reset sms applet to default failed", exception);
			return false;
		}
	}

	public void DoProcessWithChangeSMSDefault(TcpAndroidDevice device, Action action)
	{
		bool flag = IsNeedSetSMSApplet();
		int apiLevel = device.Property.ApiLevel;
		try
		{
			if (flag && !SmsAppletIsReady(device))
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					Window win = ((apiLevel >= 29) ? ((Window)new SmsAppletIsReadyWindowForAndroidQ(device)) : ((Window)new SmsAppletIsReadyWindow(device)));
					HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
					{
						win.ShowDialog();
					});
				});
			}
			try
			{
				action?.Invoke();
			}
			catch
			{
			}
		}
		finally
		{
			if (flag && ResetSmsAppletToDefault(device))
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					Window resetTisp = ((apiLevel >= 29) ? ((Window)new ResetToDefaultSmsAppletTipsForAndroidQ(device)) : ((Window)new ResetToDefaultSmsAppletTips()));
					HostProxy.HostMaskLayerWrapper.New(resetTisp, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
					{
						resetTisp.ShowDialog();
					});
				});
			}
		}
	}

	public bool SendSms(string sms, List<string> targetContactList, Action<bool> callBack)
	{
		_ = string.Empty;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			long sequence = HostProxy.Sequence.New();
			List<PropItem> receiveData = null;
			foreach (string targetContact in targetContactList)
			{
				bool flag = false;
				if (messageReaderAndWriter.SendAndReceiveSync("SendSms", "SendSmsResponse", new List<string> { targetContact, sms }, sequence, out receiveData) && receiveData != null)
				{
					flag = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
				}
				callBack?.Invoke(flag);
				if (!flag)
				{
					break;
				}
			}
		}
		return false;
	}

	public bool SMSAppExisted(TcpAndroidDevice device)
	{
		if (device != null)
		{
			using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter != null)
			{
				long sequence = HostProxy.Sequence.New();
				List<PropItem> receiveData = null;
				if (messageReaderAndWriter.SendAndReceive<string, PropItem>("isMessengerAppExisted", null, sequence, out receiveData) && receiveData != null)
				{
					return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
				}
			}
		}
		return false;
	}
}
