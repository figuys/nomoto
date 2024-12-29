using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PicInfoViewModel : ViewModelBase
{
	private bool _isSeleted;

	private Action<bool> _selectionHandler;

	private static ImageSource _defaultThumbImage;

	private ImageSource _picThumbImageSource;

	public int DataVersion { get; set; }

	public string Artist { get; set; }

	public string Album { get; set; }

	public long FileSize { get; set; }

	public string Size { get; set; }

	public DateTime PhotoOP { get; set; }

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
				_selectionHandler?.BeginInvoke(_isSeleted, null, null);
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public ServerPicInfo RawPicInfo { get; set; }

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

	public PicInfoViewModel(ImageSource defaultImage, Action<bool> selectionHandler)
	{
		DeletePicCommand = new ReplayCommand(DeletePicCommandHandler);
		DoubleMouseClickCommand = new ReplayCommand(DoubleMouseClickCommandHandler);
		_selectionHandler = selectionHandler;
		IsImageLoaded = false;
	}

	public void AddSelectionHandler(Action<bool> selectionHandler)
	{
		_selectionHandler = selectionHandler;
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
		PicAlbumViewModel focusedAlbum = PicMgtViewModel.SingleInstance.FocusedAlbum;
		if (focusedAlbum != null)
		{
			PicOriginalHolder picOriginalHolder = new PicOriginalHolder();
			PicOriginalHolderViewModel picOriginalHolderViewModel = new PicOriginalHolderViewModel(focusedAlbum.CachedPicList, this);
			picOriginalHolderViewModel.ShowPic(this);
			picOriginalHolder.DataContext = picOriginalHolderViewModel;
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
			HostProxy.BehaviorService.Collect(BusinessType.PICTURE_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, BusinessStatus.SUCCESS, result));
			if (num)
			{
				PicMgtViewModel.SingleInstance.PicToolRefreshCommandHandler(null);
			}
		});
	}
}
