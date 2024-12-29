using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicInfoViewModelV7 : ViewModelBase
{
	private bool _isSeleted;

	private ServerPicInfo _RawPicInfo;

	private static ImageSource _defaultThumbImage;

	private ImageSource _picThumbImageSource;

	public int DataVersion { get; set; }

	public string Artist { get; set; }

	public string Album { get; set; }

	public long FileSize { get; set; }

	public string Size { get; set; }

	public DateTime PhotoOP { get; set; }

	public string GroupKey { get; set; }

	public bool IsSelected
	{
		get
		{
			return _isSeleted;
		}
		set
		{
			if (_isSeleted != value)
			{
				_isSeleted = value;
				if (!IsNotUserClick)
				{
					SingleMouseClickCommandHandler();
				}
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public bool IsNotUserClick { get; set; }

	private PicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<PicMgtViewModelV7>(typeof(PICMgtViewV7));

	public ServerPicInfo RawPicInfo
	{
		get
		{
			return _RawPicInfo;
		}
		set
		{
			_RawPicInfo = value;
			OnPropertyChanged("RawPicInfo");
		}
	}

	public ImageSource PhotoImageSource
	{
		get
		{
			return _picThumbImageSource;
		}
		private set
		{
			_picThumbImageSource = value;
			OnPropertyChanged("PhotoImageSource");
		}
	}

	public bool IsImageLoaded { get; private set; }

	public ReplayCommand DoubleMouseClickCommand { get; set; }

	public ReplayCommand DeletePicCommand { get; set; }

	public PicInfoViewModelV7()
	{
		DeletePicCommand = new ReplayCommand(DeletePicCommandHandler);
		DoubleMouseClickCommand = new ReplayCommand(DoubleMouseClickCommandHandler);
		IsImageLoaded = false;
	}

	public void SetImage(ImageSource image)
	{
		IsImageLoaded = true;
		if (image == null)
		{
			PhotoImageSource = _defaultThumbImage;
		}
		else
		{
			PhotoImageSource = image;
		}
	}

	private void DoubleMouseClickCommandHandler(object parameter)
	{
		PicAlbumViewModelV7 focusedAlbum = GetCurrentViewModel.FocusedAlbum;
		if (focusedAlbum != null)
		{
			PicOriginalHolder picOriginalHolder = new PicOriginalHolder();
			PicOriginalHolderViewModelV7 picOriginalHolderViewModelV = new PicOriginalHolderViewModelV7(focusedAlbum.CachedPicList, this);
			picOriginalHolderViewModelV.ShowPic(this);
			picOriginalHolder.DataContext = picOriginalHolderViewModelV;
			picOriginalHolder.ShowDialog();
		}
	}

	private void DeletePicCommandHandler(object parameter)
	{
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0585", "K0562", "K0208", "K0583", new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Assets/Images/PicPopup/delete.png")));
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		if (!win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
		{
			return;
		}
		AsyncDataLoader.Loading(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			Dictionary<string, int> result = new Dictionary<string, int>();
			BusinessData businessData = new BusinessData(BusinessType.PICTURE_DELETE, Context.CurrentDevice);
			stopwatch.Start();
			bool num = new DeviceCommonManagement().DeleteDevFilesWithConfirm("deletePicturesById", new List<string> { RawPicInfo.Id }, ref result);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.PICTURE_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, (result["success"] > 0) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, result));
			if (num)
			{
				GetCurrentViewModel.PicToolRefreshCommandHandler(null);
			}
		});
	}

	private void SingleMouseClickCommandHandler()
	{
		ObservableCollection<PicInfoViewModelV7> cachedAllPics = GetCurrentViewModel.FocusedAlbum.CachedAllPics;
		PicInfoListViewModelV7 picInfoListViewModelV = GetCurrentViewModel.FocusedAlbum.CachedPicList.FirstOrDefault((PicInfoListViewModelV7 m) => m.GroupKey == GroupKey);
		if (IsSelected)
		{
			if (picInfoListViewModelV != null)
			{
				if (picInfoListViewModelV.Pics.Count((PicInfoViewModelV7 m) => !m.IsSelected) > 0)
				{
					picInfoListViewModelV.IsListSelected = null;
				}
				else
				{
					picInfoListViewModelV.IsListSelected = true;
				}
			}
			if (cachedAllPics.Count((PicInfoViewModelV7 m) => !m.IsSelected) > 0)
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllPic = null;
				GetCurrentViewModel.IsSelectedAllAlbumPic = null;
			}
			else
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllPic = true;
				if (GetCurrentViewModel.Albums.Count((PicAlbumViewModelV7 m) => m.IsSelectedAllPic != true) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumPic = true;
				}
			}
		}
		else
		{
			if (picInfoListViewModelV != null)
			{
				if (picInfoListViewModelV.Pics.Count((PicInfoViewModelV7 m) => m.IsSelected) > 0)
				{
					picInfoListViewModelV.IsListSelected = null;
				}
				else
				{
					picInfoListViewModelV.IsListSelected = false;
				}
			}
			if (cachedAllPics.Count((PicInfoViewModelV7 m) => m.IsSelected) > 0)
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllPic = null;
				GetCurrentViewModel.IsSelectedAllAlbumPic = null;
			}
			else
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllPic = false;
				if (GetCurrentViewModel.Albums.Count((PicAlbumViewModelV7 m) => m.IsSelectedAllPic != false) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumPic = false;
				}
			}
		}
		GetCurrentViewModel.RefreshAllAlbumPicSelectedCount();
	}

	public void ResetCloneImageUri()
	{
		PhotoImageSource = ImageHandleHelper.LoadBitmap(RawPicInfo?.LocalFilePath);
	}
}
