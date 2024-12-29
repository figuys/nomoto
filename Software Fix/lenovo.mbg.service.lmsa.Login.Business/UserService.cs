using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.LenovoId;
using lenovo.mbg.service.lmsa.Login.Model;
using lenovo.mbg.service.lmsa.Login.Protocol;
using lenovo.themes.generic.Controls;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class UserService : IDisposable
{
	private static UserService single;

	private volatile bool mIsOnlie = false;

	private readonly object mCurrentOnlineUserChangingEventArgsLock = new object();

	private OnlineUserChangingEventArgs mCurrentOnlineUserChangingEventArgs;

	private OnlineUserChangedEventArgs mCurrentOnlineUserChangedEventArgs;

	private readonly object mCurrentOnlineUserChangedEventArgsLock = new object();

	public static UserService Single
	{
		get
		{
			if (single == null)
			{
				single = new UserService();
			}
			return single;
		}
	}

	public string UserSource { get; protected set; }

	public bool IsOnline => mIsOnlie;

	public OnlineUserInfo CurrentLoggedInUser { get; set; }

	private event EventHandler<OnlineUserChangingEventArgs> mOnlineUserChanging;

	public event EventHandler<OnlineUserChangingEventArgs> OnlineUserChanging
	{
		add
		{
			mOnlineUserChanging += value;
			lock (mCurrentOnlineUserChangingEventArgsLock)
			{
				if (mCurrentOnlineUserChangedEventArgs != null)
				{
					value.BeginInvoke(this, mCurrentOnlineUserChangingEventArgs, null, null);
				}
			}
		}
		remove
		{
			mOnlineUserChanging -= value;
		}
	}

	private event EventHandler<OnlineUserChangedEventArgs> mOnlineUserChanged;

	public event EventHandler<OnlineUserChangedEventArgs> OnlineUserChanged
	{
		add
		{
			mOnlineUserChanged += value;
			lock (mCurrentOnlineUserChangedEventArgsLock)
			{
				if (mCurrentOnlineUserChangedEventArgs != null)
				{
					value.BeginInvoke(this, mCurrentOnlineUserChangedEventArgs, null, null);
				}
			}
		}
		remove
		{
			mOnlineUserChanged -= value;
		}
	}

	private UserService()
	{
	}

	public ResponseData<LoggingInResponseData> Login(UserLoginFormData data)
	{
		LogHelper.LogInstance.Info("rsa login method entered");
		UserSource = data.UserSource;
		FireOnlineUserChangingEvent(this, new OnlineUserChangingEventArgs(data.UserSource, isLoggingIn: true, DateTime.Now));
		IUserLoginHandler userLoginHandler = LoginHandlerFacory.CreateLoginHandler(data.UserSource);
		ResponseData<LoggingInResponseData> responseData = userLoginHandler.Login(data);
		if (mIsOnlie = responseData != null && string.Compare("0000", responseData.Code) == 0)
		{
			AppContext.IsLogIn = true;
			LogHelper.LogInstance.Info("rsa login success");
			OnlineUserInfo onlineUserInfo = new OnlineUserInfo();
			onlineUserInfo.UserName = responseData.Data.Name;
			onlineUserInfo.EmailAddress = responseData.Data.EmailAddress;
			onlineUserInfo.UserId = responseData.Data.UserId;
			onlineUserInfo.Country = responseData.Data.Country;
			onlineUserInfo.FullName = responseData.Data.FullName;
			onlineUserInfo.PhoneNumber = responseData.Data.PhoneNumber;
			onlineUserInfo.UserSource = data.UserSource;
			onlineUserInfo.quitSurvey = responseData.Data.quitSurvey;
			onlineUserInfo.failureNum = responseData.Data.failureNum;
			onlineUserInfo.config = responseData.Data.dictionary;
			if (responseData.Data.dictionary.ContainsKey("TURN_ON_NOTICE"))
			{
				onlineUserInfo.IsRtNotify = responseData.Data.dictionary["TURN_ON_NOTICE"].ToString() == "Y";
			}
			else
			{
				onlineUserInfo.IsRtNotify = false;
			}
			onlineUserInfo.IsB2BSupportMultDev = responseData.Data.B2bUsrInfo?.IsMultiDev ?? false;
			onlineUserInfo.B2BBuyNowDisplay = responseData.Data.B2bCountry && GetB2BBuyNowButtonDisplay(onlineUserInfo.UserId);
			onlineUserInfo.B2BEntranceEnable = responseData.Data.B2bUsrInfo?.B2bButtonDisplay ?? false;
			CurrentLoggedInUser = onlineUserInfo;
			global::Smart.BehaviorService.InitUser(onlineUserInfo.UserName);
			FireOnlineUserChangedEvent(this, new OnlineUserChangedEventArgs(CurrentLoggedInUser, data, mIsOnlie, DateTime.Now));
			Task.Run(delegate
			{
				GetB2bPrice();
			});
		}
		else
		{
			if (data.UserSource == "guest")
			{
				GuestLoginExpire();
			}
			LogHelper.LogInstance.Info("rsa login failed");
			UserSource = string.Empty;
			OnlineUserInfo userInfo = (CurrentLoggedInUser = null);
			FireOnlineUserChangedEvent(this, new OnlineUserChangedEventArgs(userInfo, data, mIsOnlie, DateTime.Now));
		}
		Single.OnlineUserChanged += Temp_OnlineUserChanged;
		if (!string.IsNullOrEmpty(responseData?.Data?.RSANotification))
		{
			string notification = Regex.Unescape(responseData.Data.RSANotification);
			LoginNotificationDialog(notification);
		}
		return responseData;
	}

	public void GetB2bPrice()
	{
		string empty = string.Empty;
		List<PriceInfo> list = AppContext.WebApi.RequestContent<List<PriceInfo>>(WebApiUrl.CALL_B2B_GET_PRICE_URL, null, 3, null, HttpMethod.GET);
		if (list != null && list.Count >= 3)
		{
			CurrentLoggedInUser.PriceUnit = list[0].monetaryUnit;
			CurrentLoggedInUser.Price1 = list[0].price;
			CurrentLoggedInUser.Price2 = list[1].price;
			CurrentLoggedInUser.Price3 = list[2].price;
			CurrentLoggedInUser.SkuName1 = list[0].sku;
			CurrentLoggedInUser.SkuName2 = list[1].sku;
			CurrentLoggedInUser.SkuName3 = list[2].sku;
		}
		else
		{
			CurrentLoggedInUser.PriceUnit = "$";
			CurrentLoggedInUser.Price1 = 1.99f;
			CurrentLoggedInUser.Price2 = 17.99f;
			CurrentLoggedInUser.Price3 = 49.99f;
			CurrentLoggedInUser.SkuName1 = "sku1";
			CurrentLoggedInUser.SkuName2 = "sku2";
			CurrentLoggedInUser.SkuName3 = "sku3";
		}
	}

	private void Temp_OnlineUserChanged(object sender, OnlineUserChangedEventArgs e)
	{
		if (e.IsOnline)
		{
			LatestLoginUserInfo latestLoginUserInfo = new LatestLoginUserInfo();
			latestLoginUserInfo.UserName = e.UserInfo.UserName;
			latestLoginUserInfo.Password = string.Empty;
			latestLoginUserInfo.UserID = e.UserInfo.UserId;
			latestLoginUserInfo.LoginFormData = e.UserLoginFormData;
			latestLoginUserInfo.email = e.UserInfo.EmailAddress;
			latestLoginUserInfo.phone = e.UserInfo.PhoneNumber;
			latestLoginUserInfo.config = e.UserInfo.config;
			string value = JsonConvert.SerializeObject(latestLoginUserInfo);
			Single.AddOrUpdateLoginSetting(LatestLoginUserInfo.TAG, value);
		}
	}

	public void AutoLoginIfNeed(Action<Window> callBack)
	{
		if ("TRUE".Equals(GetLoginSetting("AutoLogin")))
		{
			LogHelper.LogInstance.Info("Curent user will auto login by lenovo_id.");
			HostProxy.CurrentDispatcher.BeginInvoke((Action)delegate
			{
				LenovoIdWindow.ShowDialogEx(isRegister: false, callBack);
			});
		}
		else
		{
			callBack(null);
			LogHelper.LogInstance.Info("System not set auto login.");
		}
	}

	private void LoginNotificationDialog(string _notification)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			LenovoPopupWindow okwin = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0711", _notification, "K0327", null);
			HostProxy.HostMaskLayerWrapper.New(okwin, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				okwin.ShowDialog();
			});
			LenovoIdWindow.ShowDialogEx();
		});
	}

	private void GuestLoginExpire()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			LenovoPopupWindow okwin = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0711", "Guest user expire", "K0327", null);
			HostProxy.HostMaskLayerWrapper.New(okwin, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				okwin.ShowDialog();
			});
		});
	}

	public ResponseData<LoggingInResponseData> VerifyPassword(LmsaUserLoginFormData data)
	{
		return WebServiceProxy.SingleInstance.PasswordVerify(data);
	}

	public void Logout(Action AfterCallback)
	{
		mIsOnlie = false;
		FireOnlineUserChangingEvent(this, new OnlineUserChangingEventArgs(CurrentLoggedInUser.UserSource, isLoggingIn: false, DateTime.Now));
		IUserLoginHandler userLoginHandler = LoginHandlerFacory.CreateLoginHandler(CurrentLoggedInUser.UserSource);
		userLoginHandler.Logout(AfterCallback);
		OnlineUserInfo currentLoggedInUser = CurrentLoggedInUser;
		if (CurrentLoggedInUser.UserId != null)
		{
			Task.Run(delegate
			{
				LogoutReport(null);
			});
		}
		CurrentLoggedInUser = null;
		global::Smart.BehaviorService.InitUser(null);
		FireOnlineUserChangedEvent(this, new OnlineUserChangedEventArgs(currentLoggedInUser, null, mIsOnlie, DateTime.Now));
		FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, "AutoLogin", false);
	}

	public ResponseData<ChangePasswordResponseData> ChangePassword(ChangePasswordFormData data)
	{
		return WebServiceProxy.SingleInstance.ChangePassowrd(data);
	}

	public ResponseData<ForgotPasswordResponseData> ForgotPassowrd(ForgotPasswordFormData data)
	{
		return WebServiceProxy.SingleInstance.ForgotPassowrd(data);
	}

	public ResponseData<LogoutReportResponseData> LogoutReport(LogoutReportFormData data)
	{
		return WebServiceProxy.SingleInstance.LogoutReport(data);
	}

	private void FireOnlineUserChangingEvent(object sender, OnlineUserChangingEventArgs e)
	{
		lock (mCurrentOnlineUserChangingEventArgsLock)
		{
			mCurrentOnlineUserChangingEventArgs = e;
		}
		if (this.mOnlineUserChanging != null)
		{
			Delegate[] invocationList = this.mOnlineUserChanging.GetInvocationList();
			for (int num = invocationList.Count() - 1; num >= 0; num--)
			{
				((EventHandler<OnlineUserChangingEventArgs>)invocationList[num]).BeginInvoke(sender, e, null, null);
			}
		}
	}

	private void FireOnlineUserChangedEvent(object sender, OnlineUserChangedEventArgs e)
	{
		lock (mCurrentOnlineUserChangedEventArgsLock)
		{
			mCurrentOnlineUserChangedEventArgs = e;
		}
		UserInfo userInfo = null;
		if (e.IsOnline && e.UserInfo != null)
		{
			userInfo = new UserInfo();
			userInfo.UserName = e.UserInfo.UserName;
			userInfo.EmailAddress = e.UserInfo.EmailAddress;
			userInfo.Country = e.UserInfo.Country;
			userInfo.FullName = e.UserInfo.FullName;
			userInfo.PhoneNumber = e.UserInfo.PhoneNumber;
			userInfo.UserId = e.UserInfo.UserId;
			userInfo.Config = e.UserInfo.config;
			userInfo.IsB2BSupportMultDev = e.UserInfo.IsB2BSupportMultDev;
		}
		global::Smart.User.FireUserChanged(new UserInfoArgs(userInfo, e.IsOnline));
		if (this.mOnlineUserChanged != null)
		{
			Delegate[] invocationList = this.mOnlineUserChanged.GetInvocationList();
			for (int num = invocationList.Count() - 1; num >= 0; num--)
			{
				((EventHandler<OnlineUserChangedEventArgs>)invocationList[num]).BeginInvoke(sender, e, null, null);
			}
		}
	}

	public void Dispose()
	{
		EventHandler<OnlineUserChangedEventArgs> eventHandler = this.mOnlineUserChanged;
		if (this.mOnlineUserChanged != null)
		{
			Delegate[] invocationList = this.mOnlineUserChanged.GetInvocationList();
			Delegate[] array = invocationList;
			for (int i = 0; i < array.Length; i++)
			{
				EventHandler<OnlineUserChangedEventArgs> value = (EventHandler<OnlineUserChangedEventArgs>)array[i];
				mOnlineUserChanged -= value;
			}
		}
	}

	public bool AddOrUpdateLoginSetting(string key, object value)
	{
		try
		{
			FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, key, value);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool AddOrUpdateAgreePolicySetting(string key, object value)
	{
		try
		{
			FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, key, value);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public string GetLoginSetting(string key)
	{
		string empty = string.Empty;
		switch (key)
		{
		case "AutoLogin":
			empty = FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "AutoLogin");
			if (!string.IsNullOrEmpty(empty))
			{
				return empty.ToUpper();
			}
			return "FALSE";
		case "SavePassword":
			empty = FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "SavePassword");
			if (!string.IsNullOrEmpty(empty))
			{
				return empty.ToUpper();
			}
			return "FALSE";
		case "AgreePolicy":
			empty = FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "AgreePolicy");
			if (!string.IsNullOrEmpty(empty))
			{
				return empty.ToUpper();
			}
			return "FALSE";
		default:
			return "UNKNOWN";
		}
	}

	public bool GetB2BBuyNowButtonDisplay(string _userId)
	{
		string value = FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "B2BBuyNowButtonDisplay_" + _userId);
		if (!string.IsNullOrEmpty(value))
		{
			return Convert.ToBoolean(value);
		}
		return false;
	}

	public void SaveB2BBuyNowButtonDisplay()
	{
		CurrentLoggedInUser.B2BBuyNowDisplay = true;
		FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, "B2BBuyNowButtonDisplay_" + CurrentLoggedInUser.UserId, true);
	}
}
