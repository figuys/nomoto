using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.Encode;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.View;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.ViewModel;

public class GifMakerViewModel : ViewModelBase
{
	private GifMakerView wnd;

	public static ICommand ImagePlotCmd = new RoutedCommand();

	public static ICommand AddImgCmd = new RoutedCommand();

	public static ICommand DelImgCmd = new RoutedCommand();

	public ObservableCollection<ImageEntitiy> ImageArr { get; set; }

	public ICommand CloseCmd { get; set; }

	public ICommand AddDelayCmd { get; set; }

	public ICommand DelDelayCmd { get; set; }

	public ICommand SelDirPathCmd { get; set; }

	public ICommand PlayerCmd { get; set; }

	public ICommand CreateGifCmd { get; set; }

	public GifMakerModel Model { get; set; }

	public GifMakerViewModel(GifMakerView view)
	{
		wnd = view;
		Model = new GifMakerModel();
		CloseCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(CloseCmd, delegate
		{
			ImageArr.Clear();
			Model.GifImage = null;
			wnd.Close();
			wnd = null;
			GC.Collect();
		}));
		wnd.CommandBindings.Add(new CommandBinding(AddImgCmd, delegate
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Filter = "Image|*.jpg;*.jpeg;*.png;*.bmp";
			dlg.Multiselect = true;
			if (dlg.ShowDialog() == true)
			{
				Task.Factory.StartNew(delegate
				{
					string[] fileNames = dlg.FileNames;
					foreach (string filePath in fileNames)
					{
						ImageEntitiy entity = new ImageEntitiy
						{
							IsImage = true,
							FilePath = filePath,
							Index = Guid.NewGuid().ToString()
						};
						if (entity.BitmapImage != null)
						{
							wnd.Dispatcher.BeginInvoke((Action)delegate
							{
								ImageArr.Insert(ImageArr.Count - 1, entity);
							});
						}
					}
				});
			}
		}));
		wnd.CommandBindings.Add(new CommandBinding(DelImgCmd, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			ImageEntitiy imageEntitiy = ImageArr.FirstOrDefault((ImageEntitiy p) => p.Index == (string)e.Parameter);
			imageEntitiy.BitmapImage = null;
			ImageArr.Remove(imageEntitiy);
		}));
		AddDelayCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(AddDelayCmd, delegate
		{
			if (Model.GifDelay < 30)
			{
				GifMakerModel model = Model;
				int gifDelay = model.GifDelay + 1;
				model.GifDelay = gifDelay;
			}
		}));
		DelDelayCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(DelDelayCmd, delegate
		{
			if (Model.GifDelay > 1)
			{
				GifMakerModel model2 = Model;
				int gifDelay2 = model2.GifDelay - 1;
				model2.GifDelay = gifDelay2;
			}
		}));
		SelDirPathCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(SelDirPathCmd, delegate
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = Model.GifPath,
				ShowNewFolderButton = true
			};
			if (DialogResult.OK == FolderBrowserLauncher.ShowFolderBrowser(folderBrowserDialog))
			{
				Model.GifPath = folderBrowserDialog.SelectedPath;
			}
		}));
		PlayerCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(PlayerCmd, delegate
		{
			if (Model.IsAnimation)
			{
				if (ImageArr.Count > 1)
				{
					int num = 0;
					int num2 = 0;
					if (Model.GifWidth > Model.GifHeight && Model.GifWidth > 200)
					{
						num = 200;
						num2 = 200 * Model.GifHeight / Model.GifWidth;
					}
					else if (Model.GifHeight > Model.GifWidth && Model.GifHeight > 200)
					{
						num2 = 200;
						num = 200 * Model.GifWidth / Model.GifHeight;
					}
					else
					{
						num = Model.GifWidth;
						num2 = Model.GifHeight;
					}
					if (Model.IsChanged)
					{
						Model.IsChanged = false;
						Model.GifImage = null;
						ImageToGif(Model.GifFile, num, num2);
					}
					else
					{
						wnd.ResumeAnimation();
					}
					Model.IsGifMakeOk = true;
				}
			}
			else
			{
				Model.IsGifMakeOk = false;
				wnd.StopAnimation();
			}
		}));
		CreateGifCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(CreateGifCmd, delegate
		{
			if (ImageArr.Count > 1)
			{
				if (!GlobalFun.Exists(Model.GifPath))
				{
					FolderBrowserDialog folderBrowserDialog2 = new FolderBrowserDialog
					{
						Description = HostProxy.LanguageService.Translate("Please select a new save path"),
						SelectedPath = Model.GifPath,
						ShowNewFolderButton = true
					};
					if (DialogResult.OK != FolderBrowserLauncher.ShowFolderBrowser(folderBrowserDialog2))
					{
						return;
					}
					Model.GifPath = folderBrowserDialog2.SelectedPath;
				}
				HostProxy.BehaviorService.Collect(BusinessType.GIF_MAKER_CREATEGIF, null);
				string giffile = Path.Combine(Model.GifPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".gif");
				ImageToGif(giffile, Model.GifWidth, Model.GifHeight);
			}
		}));
		wnd.CommandBindings.Add(new CommandBinding(ImagePlotCmd, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			string index = (string)e.Parameter;
			ImageEntitiy imageEntitiy2 = ImageArr.FirstOrDefault((ImageEntitiy p) => p.Index == index);
			BitmapImage displayImage = LoadImage(imageEntitiy2.FilePath);
			PlotView plotView = new PlotView();
			plotView.Title = LangTranslation.Translate("K0200");
			plotView.ViewModel.Model.DisplayImage = displayImage;
			plotView.ViewModel.Model.TempDir = Model.TempDir;
			plotView.ShowInTaskbar = false;
			plotView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			string uid = Guid.NewGuid().ToString("N");
			IntPtr owner = HostProxy.HostOperationService.ShowMaskLayer(uid);
			new WindowInteropHelper(plotView).Owner = owner;
			if (true == plotView.ShowDialog())
			{
				Model.IsAnimation = false;
				Model.IsChanged = true;
				imageEntitiy2.FilePath = plotView.ViewModel.Model.NewFile;
			}
			plotView.ViewModel.Model = null;
			plotView.ViewModel = null;
			HostProxy.HostOperationService.CloseMaskLayer(uid);
		}));
		ImageArr = new ObservableCollection<ImageEntitiy>();
		ImageArr.Add(new ImageEntitiy
		{
			IsImage = false,
			FilePath = string.Empty,
			Index = Guid.NewGuid().ToString()
		});
		ImageArr.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
			{
				Model.IsChanged = true;
				Model.IsAnimation = false;
			}
		};
		Model.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsAnimation" && !Model.IsAnimation)
			{
				wnd.StopAnimation();
			}
		};
	}

	private void ImageToGif(string giffile, int width, int height, bool repeat = true)
	{
		ProgressView prg = new ProgressView();
		Task.Factory.StartNew(delegate
		{
			GifEncoder gifEncoder = new GifEncoder();
			gifEncoder.Start(giffile);
			gifEncoder.SetDelay(1000 / Model.GifDelay);
			gifEncoder.SetRepeat((!repeat) ? (-1) : 0);
			int num = 0;
			int num2 = ImageArr.Count - 1;
			foreach (ImageEntitiy item in ImageArr)
			{
				if (item.IsImage)
				{
					gifEncoder.AddFrame(Image.FromFile(item.FilePath).ScaleToFit(new System.Drawing.Size(width, height), dispose: true, ScalingMode.Overflow));
					prg.Model.Percentage = ++num * 100 / num2;
				}
			}
			gifEncoder.Finish();
			Thread.Sleep(500);
			prg.Dispatcher.Invoke(delegate
			{
				prg.Close();
			});
			Model.LoadBitmap();
		});
		string uid = Guid.NewGuid().ToString("N");
		IntPtr owner = HostProxy.HostOperationService.ShowMaskLayer(uid);
		new WindowInteropHelper(prg).Owner = owner;
		prg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		prg.ShowDialog();
		HostProxy.HostOperationService.CloseMaskLayer(uid);
	}

	public BitmapImage LoadImage(string file)
	{
		if (string.IsNullOrEmpty(file) || !File.Exists(file))
		{
			return null;
		}
		try
		{
			Image image = Image.FromFile(file);
			double num = image.Width;
			double num2 = image.Height;
			image.Dispose();
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			if (num > 1366.0 || num2 > 768.0)
			{
				if (num2 / num > 0.0)
				{
					bitmapImage.DecodePixelHeight = 768;
				}
				else
				{
					bitmapImage.DecodePixelWidth = 1366;
				}
			}
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.UriSource = new Uri(file);
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
