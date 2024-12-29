using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicOriginalHolderViewModelV7 : ViewModelBase
{
	private PicInfoViewModelV7 curPicInfoVM;

	private ObservableCollection<PicInfoListViewModelV7> groupArr;

	private readonly PicExportWorkerV7 m_picExportWorker;

	private Cursor _defaultCursor;

	private Cursor _currentCursor;

	private ImageSource _displayingImageSource;

	private double _rotateAngle;

	private volatile string m_currentPicId = string.Empty;

	private readonly object m_currentPicIdSync = new object();

	private BitmapImage m_defaultImg;

	private Stream m_imageFileStream;

	private Visibility _imageBorderVisibility;

	private Visibility _errorTipsBorderVisibility = Visibility.Collapsed;

	private string _tips = string.Empty;

	private bool m_prevPicButtonEnabled;

	private bool m_nextPicButtonEnabled;

	private PicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<PicMgtViewModelV7>(typeof(PICMgtViewV7));

	public Cursor CurrentCursor
	{
		get
		{
			return _currentCursor;
		}
		set
		{
			if (_currentCursor != value)
			{
				_currentCursor = value;
				OnPropertyChanged("CurrentCursor");
			}
		}
	}

	public ImageSource DisplayingImageSource
	{
		get
		{
			return _displayingImageSource;
		}
		set
		{
			if (_displayingImageSource != value)
			{
				_displayingImageSource = value;
				OnPropertyChanged("DisplayingImageSource");
			}
		}
	}

	public double RotateAngle
	{
		get
		{
			return _rotateAngle;
		}
		set
		{
			if (_rotateAngle != value)
			{
				_rotateAngle = value;
				OnPropertyChanged("RotateAngle");
			}
		}
	}

	public Visibility ImageBorderVisibility
	{
		get
		{
			return _imageBorderVisibility;
		}
		set
		{
			if (_imageBorderVisibility != value)
			{
				_imageBorderVisibility = value;
				OnPropertyChanged("ImageBorderVisibility");
			}
		}
	}

	public Visibility ErrorTipsBorderVisibility
	{
		get
		{
			return _errorTipsBorderVisibility;
		}
		set
		{
			if (_errorTipsBorderVisibility != value)
			{
				_errorTipsBorderVisibility = value;
				OnPropertyChanged("ErrorTipsBorderVisibility");
			}
		}
	}

	public string Tips
	{
		get
		{
			return _tips;
		}
		set
		{
			if (!(_tips == value))
			{
				_tips = value;
				OnPropertyChanged("Tips");
			}
		}
	}

	public ReplayCommand RotatePicAnticlockwiseCommand { get; set; }

	public ReplayCommand RotatePicClockwiseCommand { get; set; }

	public ReplayCommand DeletePicCommand { get; set; }

	public ReplayCommand CloseWindowCommand { get; set; }

	public ReplayCommand ShowPrevPicCommand { get; set; }

	public ReplayCommand ShowNextPicCommand { get; set; }

	public bool PrevPicButtonEnabled
	{
		get
		{
			return m_prevPicButtonEnabled;
		}
		set
		{
			if (m_prevPicButtonEnabled != value)
			{
				m_prevPicButtonEnabled = value;
				OnPropertyChanged("PrevPicButtonEnabled");
			}
		}
	}

	public bool NextPicButtonEnabled
	{
		get
		{
			return m_nextPicButtonEnabled;
		}
		set
		{
			if (m_nextPicButtonEnabled != value)
			{
				m_nextPicButtonEnabled = value;
				OnPropertyChanged("NextPicButtonEnabled");
			}
		}
	}

	public event EventHandler Closing;

	public PicOriginalHolderViewModelV7(ObservableCollection<PicInfoListViewModelV7> picList, PicInfoViewModelV7 vm)
	{
		if (Thread.CurrentThread.IsBackground)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				m_defaultImg = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/pic_default.png", UriKind.RelativeOrAbsolute));
			});
		}
		else
		{
			m_defaultImg = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/pic_default.png", UriKind.RelativeOrAbsolute));
		}
		groupArr = picList;
		curPicInfoVM = vm;
		SetPicButtonEnable(vm);
		CloseWindowCommand = new ReplayCommand(CloseWindowCommandHandler);
		DeletePicCommand = new ReplayCommand(DeletePicCommandHandler);
		RotatePicAnticlockwiseCommand = new ReplayCommand(RotatePicAnticlockwiseCommandHandler);
		RotatePicClockwiseCommand = new ReplayCommand(RotatePicClockwiseCommandHandler);
		ShowPrevPicCommand = new ReplayCommand(ShowPrevPicCommandHandler);
		ShowNextPicCommand = new ReplayCommand(ShowNextPicCommandHandler);
		_defaultCursor = Application.Current.MainWindow.Cursor;
		CurrentCursor = _defaultCursor;
		m_picExportWorker = new PicExportWorkerV7(ExportImgResultHandler);
	}

	private PicInfoViewModelV7 GetPicInfoVM(bool isNext = true, bool isDelete = false)
	{
		for (int i = 0; i < groupArr.Count; i++)
		{
			for (int j = 0; j < groupArr[i].Pics.Count; j++)
			{
				if (!(groupArr[i].Pics[j].RawPicInfo.Id == curPicInfoVM.RawPicInfo.Id))
				{
					continue;
				}
				if (isNext)
				{
					if (i == groupArr.Count - 1 && j == groupArr[i].Pics.Count - 1)
					{
						PrevPicButtonEnabled = false;
						NextPicButtonEnabled = false;
						return null;
					}
					if (j == groupArr[i].PicCount - 1)
					{
						if (isDelete)
						{
							PrevPicButtonEnabled = ((i != 0 || j != 0) ? true : false);
						}
						else
						{
							PrevPicButtonEnabled = true;
						}
						NextPicButtonEnabled = ((i != groupArr.Count - 2 || groupArr[i + 1].Pics.Count != 1) ? true : false);
						return groupArr[i + 1].Pics[0];
					}
					if (isDelete)
					{
						PrevPicButtonEnabled = ((i != 0 || j != 0) ? true : false);
					}
					else
					{
						PrevPicButtonEnabled = true;
					}
					NextPicButtonEnabled = ((i != groupArr.Count - 1 || j != groupArr[i].Pics.Count - 2) ? true : false);
					return groupArr[i].Pics[j + 1];
				}
				if (i == 0 && j == 0)
				{
					PrevPicButtonEnabled = false;
					NextPicButtonEnabled = false;
					return null;
				}
				if (j == 0)
				{
					int num = groupArr[i - 1].Pics.Count - 1;
					PrevPicButtonEnabled = i != 1 || num != 0;
					if (isDelete)
					{
						NextPicButtonEnabled = ((i != groupArr.Count - 1 || j != groupArr[i].Pics.Count - 1) ? true : false);
					}
					else
					{
						NextPicButtonEnabled = true;
					}
					return groupArr[i - 1].Pics[num];
				}
				PrevPicButtonEnabled = i != 0 || j != 1;
				if (isDelete)
				{
					NextPicButtonEnabled = ((i != groupArr.Count - 1 || j != groupArr[i].Pics.Count - 1) ? true : false);
				}
				else
				{
					NextPicButtonEnabled = true;
				}
				return groupArr[i].Pics[j - 1];
			}
		}
		return null;
	}

	private void EndLoading()
	{
		CurrentCursor = _defaultCursor;
	}

	private void ResetCurrentPic(string picId)
	{
		lock (m_currentPicIdSync)
		{
			m_currentPicId = picId;
		}
	}

	private bool CurrentPicEquals(string targetPicId)
	{
		return m_currentPicId.Equals(targetPicId);
	}

	public void ShowPic(PicInfoViewModelV7 pic)
	{
		curPicInfoVM = pic;
		ServerPicInfo rawPicInfo = pic.RawPicInfo;
		if (rawPicInfo == null || string.IsNullOrEmpty(rawPicInfo.Id))
		{
			return;
		}
		ImageBorderVisibility = Visibility.Visible;
		ErrorTipsBorderVisibility = Visibility.Collapsed;
		ResetCurrentPic(pic.RawPicInfo.Id);
		LogHelper.LogInstance.Info("ready to Load picture id " + rawPicInfo.Id + "!");
		try
		{
			CurrentCursor = Cursors.Wait;
			string text = m_picExportWorker.BeginExport(pic);
			if (!string.IsNullOrEmpty(text))
			{
				EndLoading();
				RotateAngle = 0.0;
				DisplayingImageSource = CreateBitImageAndDisposePrevImage(text);
			}
		}
		catch
		{
			CurrentCursor = _defaultCursor;
			ImageBorderVisibility = Visibility.Collapsed;
			ErrorTipsBorderVisibility = Visibility.Visible;
			Tips = "K0565";
		}
	}

	private void ExportImgResultHandler(string id, TransferResult result, string imagPath)
	{
		if (!CurrentPicEquals(id))
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			EndLoading();
			if (result == TransferResult.SUCCESS)
			{
				RotateAngle = 0.0;
				DisplayingImageSource = CreateBitImageAndDisposePrevImage(imagPath);
			}
			else if (TransferResult.FAILD_FILE_NOT_EXISTS.Equals(result))
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					ImageBorderVisibility = Visibility.Collapsed;
					ErrorTipsBorderVisibility = Visibility.Visible;
					Tips = "K0564";
				});
			}
			else
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					ImageBorderVisibility = Visibility.Collapsed;
					ErrorTipsBorderVisibility = Visibility.Visible;
					Tips = "K0565";
				});
			}
		});
	}

	private BitmapImage CreateBitImageAndDisposePrevImage(string path)
	{
		if (m_imageFileStream != null)
		{
			try
			{
				m_imageFileStream.Close();
				m_imageFileStream.Dispose();
				m_imageFileStream = null;
			}
			catch (Exception)
			{
			}
		}
		BitmapImage bitmapImage = new BitmapImage();
		try
		{
			m_imageFileStream = File.OpenRead(path);
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = m_imageFileStream;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
		}
		catch
		{
			bitmapImage = null;
			bitmapImage = m_defaultImg;
		}
		return bitmapImage;
	}

	private void RotatePicAnticlockwiseCommandHandler(object parameter)
	{
		RotateAngle -= 90.0;
	}

	private void RotatePicClockwiseCommandHandler(object parameter)
	{
		RotateAngle += 90.0;
	}

	private void DeletePicCommandHandler(object parameter)
	{
		Window win = parameter as Window;
		if (Context.MessageBox.ShowMessage("K0585", "K0562", "K0583", "K0208", isCloseBtn: false, null, MessageBoxImage.Asterisk, null, win) != true || !new DeviceCommonManagement().DeleteDevFilesWithConfirmEx(win, "deletePicturesById", new List<string> { curPicInfoVM.RawPicInfo.Id }))
		{
			return;
		}
		PicInfoViewModelV7 picInfoVM = GetPicInfoVM(isNext: false, isDelete: true);
		if (picInfoVM == null)
		{
			picInfoVM = GetPicInfoVM(isNext: true, isDelete: true);
		}
		if (picInfoVM == null)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				win.Close();
			});
		}
		else
		{
			ShowPic(picInfoVM);
		}
		GetCurrentViewModel.PicToolRefreshCommandHandler(null);
	}

	private void CloseWindowCommandHandler(object parameter)
	{
		(parameter as Window).Close();
		m_picExportWorker?.Dispose();
	}

	public override void Dispose()
	{
		if (this.Closing != null)
		{
			Delegate[] invocationList = this.Closing.GetInvocationList();
			foreach (Delegate value in invocationList)
			{
				Delegate.RemoveAll(this.Closing, value);
			}
		}
	}

	private void ShowPrevPicCommandHandler(object e)
	{
		PicInfoViewModelV7 picInfoVM = GetPicInfoVM(isNext: false);
		if (picInfoVM != null)
		{
			ShowPic(picInfoVM);
		}
	}

	private void ShowNextPicCommandHandler(object e)
	{
		PicInfoViewModelV7 picInfoVM = GetPicInfoVM();
		if (picInfoVM != null)
		{
			ShowPic(picInfoVM);
		}
	}

	private void SetPicButtonEnable(PicInfoViewModelV7 vm)
	{
		for (int i = 0; i < groupArr.Count; i++)
		{
			for (int j = 0; j < groupArr[i].Pics.Count; j++)
			{
				if (groupArr[i].Pics[j].RawPicInfo.Id == vm.RawPicInfo.Id)
				{
					if (j == 0 && i == 0)
					{
						PrevPicButtonEnabled = false;
						NextPicButtonEnabled = i != groupArr.Count - 1 || groupArr[i].Pics.Count != 1;
					}
					else if (i == groupArr.Count - 1 && j >= groupArr[i].Pics.Count - 1)
					{
						PrevPicButtonEnabled = true;
						NextPicButtonEnabled = false;
					}
					else
					{
						PrevPicButtonEnabled = true;
						NextPicButtonEnabled = true;
					}
				}
			}
		}
	}
}
