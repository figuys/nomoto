using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using lenovo.themes.generic.Gif;

namespace lenovo.themes.generic.AttachedProperty;

public class ImageDownloader
{
	public interface IUrlConverter
	{
		string Convert(string originalUrl);
	}

	public static readonly DependencyProperty UrlConverterProperty = DependencyProperty.RegisterAttached("UrlConverter", typeof(IUrlConverter), typeof(ImageDownloader), new PropertyMetadata(null));

	public static readonly DependencyProperty UrlProperty = DependencyProperty.RegisterAttached("Url", typeof(string), typeof(ImageDownloader), new PropertyMetadata(string.Empty, UrlPropertyChangedCallback));

	public static IUrlConverter GetUrlConverter(DependencyObject obj)
	{
		return (IUrlConverter)obj.GetValue(UrlConverterProperty);
	}

	public static void SetUrlConverter(DependencyObject obj, IUrlConverter value)
	{
		obj.SetValue(UrlConverterProperty, value);
	}

	public static string GetUrl(DependencyObject obj)
	{
		return (string)obj.GetValue(UrlProperty);
	}

	public static void SetUrl(DependencyObject obj, string value)
	{
		obj.SetValue(UrlProperty, value);
	}

	private static void UrlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue != null)
		{
			string url = e.NewValue.ToString();
			Image img = d as Image;
			BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/images/imgloading.gif"));
			double prevWidth = img.Width;
			Stretch prevStretch = img.Stretch;
			img.Width = 100.0;
			img.Stretch = Stretch.Uniform;
			if (bitmapImage != null)
			{
				ImageBehavior.SetAutoStart(img, value: true);
				ImageBehavior.SetRepeatBehavior(img, RepeatBehavior.Forever);
				ImageBehavior.SetAnimatedSource(img, bitmapImage);
			}
			Task.Run(async delegate
			{
				await DownloadAndBindImageSourceAsync(img, url, prevWidth, prevStretch);
			});
		}
	}

	private static async Task<int> DownloadAndBindImageSourceAsync(Image img, string url, double prevImgWidth, Stretch prevStretch)
	{
		IUrlConverter urlConverter = null;
		string text = url;
		try
		{
			img.Dispatcher.Invoke(delegate
			{
				urlConverter = GetUrlConverter(img);
			});
			if (urlConverter != null)
			{
				text = urlConverter.Convert(url);
			}
		}
		catch (Exception)
		{
			return 0;
		}
		HttpClient httpClient = null;
		HttpResponseMessage response = null;
		Stream stream = null;
		try
		{
			httpClient = new HttpClient();
			response = await httpClient.GetAsync(text);
			stream = await response.Content.ReadAsStreamAsync();
			if (stream != null)
			{
				long length = stream.Length;
				if (length > int.MaxValue)
				{
					return 0;
				}
				int num = (int)length;
				int num2 = 1024;
				byte[] buffer = new byte[num2];
				int num3 = 0;
				int num4 = 0;
				MemoryStream ms = new MemoryStream(num);
				while (num3 < num && num4 < 5)
				{
					int num5;
					if ((num5 = stream.Read(buffer, 0, num2)) > 0)
					{
						num3 += num5;
						ms.Write(buffer, 0, num5);
					}
					else
					{
						Thread.Sleep(1000);
						num4++;
					}
				}
				string path = Path.Combine(Path.GetTempPath(), "feedback_img_" + Path.GetFileName(url));
				ms.Seek(0L, SeekOrigin.Begin);
				using (FileStream destination = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					ms.CopyTo(destination);
				}
				ms.Seek(0L, SeekOrigin.Begin);
				if (num3 == num)
				{
					img.Dispatcher.Invoke(delegate
					{
						try
						{
							BitmapImage bitmapImage = new BitmapImage();
							bitmapImage.BeginInit();
							bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
							bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
							bitmapImage.UriSource = null;
							bitmapImage.StreamSource = ms;
							bitmapImage.EndInit();
							img.Width = prevImgWidth;
							img.Stretch = prevStretch;
							ImageBehavior.SetAutoStart(img, value: true);
							ImageBehavior.SetRepeatBehavior(img, RepeatBehavior.Forever);
							ImageBehavior.SetAnimatedSource(img, bitmapImage);
						}
						catch (Exception)
						{
						}
					});
				}
			}
			return 1;
		}
		catch (Exception)
		{
			return 0;
		}
		finally
		{
			if (httpClient != null)
			{
				try
				{
					((HttpMessageInvoker)httpClient).Dispose();
				}
				catch (Exception)
				{
				}
			}
			if (response != null)
			{
				try
				{
					response.Dispose();
				}
				catch (Exception)
				{
				}
			}
			if (stream != null)
			{
				try
				{
					stream.Dispose();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
