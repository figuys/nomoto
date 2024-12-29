using System;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using lenovo.themes.generic.Gif.Decode;

namespace lenovo.themes.generic.Gif;

public static class ImageBehavior
{
	private struct Int32Size
	{
		public int Width { get; private set; }

		public int Height { get; private set; }

		public Int32Size(int width, int height)
		{
			this = default(Int32Size);
			Width = width;
			Height = height;
		}
	}

	private class FrameMetadata
	{
		public int Left { get; set; }

		public int Top { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public TimeSpan Delay { get; set; }

		public FrameDisposalMethod DisposalMethod { get; set; }
	}

	private enum FrameDisposalMethod
	{
		None,
		DoNotDispose,
		RestoreBackground,
		RestorePrevious
	}

	public static readonly DependencyProperty AnimatedSourceProperty = DependencyProperty.RegisterAttached("AnimatedSource", typeof(ImageSource), typeof(ImageBehavior), new UIPropertyMetadata(null, AnimatedSourceChanged));

	public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.RegisterAttached("RepeatBehavior", typeof(RepeatBehavior), typeof(ImageBehavior), new UIPropertyMetadata(default(RepeatBehavior), RepeatBehaviorChanged));

	public static readonly DependencyProperty AnimateInDesignModeProperty = DependencyProperty.RegisterAttached("AnimateInDesignMode", typeof(bool), typeof(ImageBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, AnimateInDesignModeChanged));

	public static readonly DependencyProperty AutoStartProperty = DependencyProperty.RegisterAttached("AutoStart", typeof(bool), typeof(ImageBehavior), new PropertyMetadata(true));

	private static readonly DependencyPropertyKey AnimationControllerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AnimationController", typeof(ImageAnimationController), typeof(ImageBehavior), new PropertyMetadata(null));

	private static readonly DependencyPropertyKey IsAnimationLoadedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsAnimationLoaded", typeof(bool), typeof(ImageBehavior), new PropertyMetadata(false));

	public static readonly DependencyProperty IsAnimationLoadedProperty = IsAnimationLoadedPropertyKey.DependencyProperty;

	public static readonly RoutedEvent AnimationLoadedEvent = EventManager.RegisterRoutedEvent("AnimationLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageBehavior));

	public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent("AnimationCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageBehavior));

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ImageSource GetAnimatedSource(Image obj)
	{
		return (ImageSource)obj.GetValue(AnimatedSourceProperty);
	}

	public static void SetAnimatedSource(Image obj, ImageSource value)
	{
		obj.SetValue(AnimatedSourceProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static RepeatBehavior GetRepeatBehavior(Image obj)
	{
		return (RepeatBehavior)obj.GetValue(RepeatBehaviorProperty);
	}

	public static void SetRepeatBehavior(Image obj, RepeatBehavior value)
	{
		obj.SetValue(RepeatBehaviorProperty, value);
	}

	public static bool GetAnimateInDesignMode(DependencyObject obj)
	{
		return (bool)obj.GetValue(AnimateInDesignModeProperty);
	}

	public static void SetAnimateInDesignMode(DependencyObject obj, bool value)
	{
		obj.SetValue(AnimateInDesignModeProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static bool GetAutoStart(Image obj)
	{
		return (bool)obj.GetValue(AutoStartProperty);
	}

	public static void SetAutoStart(Image obj, bool value)
	{
		obj.SetValue(AutoStartProperty, value);
	}

	public static ImageAnimationController GetAnimationController(Image imageControl)
	{
		return (ImageAnimationController)imageControl.GetValue(AnimationControllerPropertyKey.DependencyProperty);
	}

	private static void SetAnimationController(DependencyObject obj, ImageAnimationController value)
	{
		obj.SetValue(AnimationControllerPropertyKey, value);
	}

	public static bool GetIsAnimationLoaded(Image image)
	{
		return (bool)image.GetValue(IsAnimationLoadedProperty);
	}

	private static void SetIsAnimationLoaded(Image image, bool value)
	{
		image.SetValue(IsAnimationLoadedPropertyKey, value);
	}

	public static void AddAnimationLoadedHandler(Image image, RoutedEventHandler handler)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		image.AddHandler(AnimationLoadedEvent, handler);
	}

	public static void RemoveAnimationLoadedHandler(Image image, RoutedEventHandler handler)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		image.RemoveHandler(AnimationLoadedEvent, handler);
	}

	public static void AddAnimationCompletedHandler(Image d, RoutedEventHandler handler)
	{
		d?.AddHandler(AnimationCompletedEvent, handler);
	}

	public static void RemoveAnimationCompletedHandler(Image d, RoutedEventHandler handler)
	{
		d?.RemoveHandler(AnimationCompletedEvent, handler);
	}

	private static void AnimatedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
	{
		if (!(o is Image image))
		{
			return;
		}
		ImageSource imageSource = e.OldValue as ImageSource;
		ImageSource imageSource2 = e.NewValue as ImageSource;
		if (imageSource == imageSource2)
		{
			return;
		}
		if (imageSource != null)
		{
			image.Loaded -= ImageControlLoaded;
			image.Unloaded -= ImageControlUnloaded;
			AnimationCache.DecrementReferenceCount(imageSource, GetRepeatBehavior(image));
			GetAnimationController(image)?.Dispose();
			image.Source = null;
		}
		if (imageSource2 != null)
		{
			image.Loaded += ImageControlLoaded;
			image.Unloaded += ImageControlUnloaded;
			if (image.IsLoaded)
			{
				InitAnimationOrImage(image);
			}
		}
	}

	private static void ImageControlLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is Image imageControl)
		{
			InitAnimationOrImage(imageControl);
		}
	}

	private static void ImageControlUnloaded(object sender, RoutedEventArgs e)
	{
		if (sender is Image image)
		{
			ImageSource animatedSource = GetAnimatedSource(image);
			if (animatedSource != null)
			{
				AnimationCache.DecrementReferenceCount(animatedSource, GetRepeatBehavior(image));
			}
			GetAnimationController(image)?.Dispose();
		}
	}

	private static void RepeatBehaviorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
	{
		if (!(o is Image image))
		{
			return;
		}
		ImageSource animatedSource = GetAnimatedSource(image);
		if (animatedSource != null)
		{
			if (!object.Equals(e.OldValue, e.NewValue))
			{
				AnimationCache.DecrementReferenceCount(animatedSource, (RepeatBehavior)e.OldValue);
			}
			if (image.IsLoaded)
			{
				InitAnimationOrImage(image);
			}
		}
	}

	private static void AnimateInDesignModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
	{
		if (!(o is Image image))
		{
			return;
		}
		bool flag = (bool)e.NewValue;
		if (GetAnimatedSource(image) != null && image.IsLoaded)
		{
			if (flag)
			{
				InitAnimationOrImage(image);
			}
			else
			{
				image.BeginAnimation(Image.SourceProperty, null);
			}
		}
	}

	private static void InitAnimationOrImage(Image imageControl)
	{
		GetAnimationController(imageControl)?.Dispose();
		SetAnimationController(imageControl, null);
		SetIsAnimationLoaded(imageControl, value: false);
		BitmapSource source = GetAnimatedSource(imageControl) as BitmapSource;
		bool isInDesignMode = DesignerProperties.GetIsInDesignMode(imageControl);
		bool animateInDesignMode = GetAnimateInDesignMode(imageControl);
		bool flag = !isInDesignMode || animateInDesignMode;
		bool flag2 = IsLoadingDeferred(source);
		if (source != null && flag && !flag2)
		{
			if (source.IsDownloading)
			{
				EventHandler handler = null;
				handler = delegate
				{
					source.DownloadCompleted -= handler;
					InitAnimationOrImage(imageControl);
				};
				source.DownloadCompleted += handler;
				imageControl.Source = source;
				return;
			}
			ObjectAnimationUsingKeyFrames animation = GetAnimation(imageControl, source);
			if (animation != null)
			{
				if (animation.KeyFrames.Count > 0)
				{
					TryTwice(delegate
					{
						imageControl.Source = (ImageSource)animation.KeyFrames[0].Value;
					});
				}
				else
				{
					imageControl.Source = source;
				}
				ImageAnimationController value = new ImageAnimationController(imageControl, animation, GetAutoStart(imageControl));
				SetAnimationController(imageControl, value);
				SetIsAnimationLoaded(imageControl, value: true);
				imageControl.RaiseEvent(new RoutedEventArgs(AnimationLoadedEvent, imageControl));
				return;
			}
		}
		imageControl.Source = source;
		if (source != null)
		{
			SetIsAnimationLoaded(imageControl, value: true);
			imageControl.RaiseEvent(new RoutedEventArgs(AnimationLoadedEvent, imageControl));
		}
	}

	private static ObjectAnimationUsingKeyFrames GetAnimation(Image imageControl, BitmapSource source)
	{
		ObjectAnimationUsingKeyFrames animation = AnimationCache.GetAnimation(source, GetRepeatBehavior(imageControl));
		if (animation != null)
		{
			return animation;
		}
		if (GetDecoder(source, out var gifFile) is GifBitmapDecoder gifBitmapDecoder && gifBitmapDecoder.Frames.Count > 1)
		{
			Int32Size fullSize = GetFullSize(gifBitmapDecoder, gifFile);
			int num = 0;
			animation = new ObjectAnimationUsingKeyFrames();
			TimeSpan zero = TimeSpan.Zero;
			BitmapSource baseFrame = null;
			foreach (BitmapFrame frame in gifBitmapDecoder.Frames)
			{
				FrameMetadata frameMetadata = GetFrameMetadata(gifBitmapDecoder, gifFile, num);
				BitmapSource bitmapSource = MakeFrame(fullSize, frame, frameMetadata, baseFrame);
				DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame(bitmapSource, zero);
				animation.KeyFrames.Add(keyFrame);
				zero += frameMetadata.Delay;
				switch (frameMetadata.DisposalMethod)
				{
				case FrameDisposalMethod.None:
				case FrameDisposalMethod.DoNotDispose:
					baseFrame = bitmapSource;
					break;
				case FrameDisposalMethod.RestoreBackground:
					baseFrame = ((!IsFullFrame(frameMetadata, fullSize)) ? ClearArea(bitmapSource, frameMetadata) : null);
					break;
				}
				num++;
			}
			animation.Duration = zero;
			animation.RepeatBehavior = GetActualRepeatBehavior(imageControl, gifBitmapDecoder, gifFile);
			AnimationCache.AddAnimation(source, GetRepeatBehavior(imageControl), animation);
			AnimationCache.IncrementReferenceCount(source, GetRepeatBehavior(imageControl));
			return animation;
		}
		return null;
	}

	private static BitmapSource ClearArea(BitmapSource frame, FrameMetadata metadata)
	{
		DrawingVisual drawingVisual = new DrawingVisual();
		using (DrawingContext drawingContext = drawingVisual.RenderOpen())
		{
			Rect rect = new Rect(0.0, 0.0, frame.PixelWidth, frame.PixelHeight);
			PathGeometry clipGeometry = Geometry.Combine(geometry2: new RectangleGeometry(new Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height)), geometry1: new RectangleGeometry(rect), mode: GeometryCombineMode.Exclude, transform: null);
			drawingContext.PushClip(clipGeometry);
			drawingContext.DrawImage(frame, rect);
		}
		RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(frame.PixelWidth, frame.PixelHeight, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
		renderTargetBitmap.Render(drawingVisual);
		WriteableBitmap writeableBitmap = new WriteableBitmap(renderTargetBitmap);
		if (writeableBitmap.CanFreeze && !writeableBitmap.IsFrozen)
		{
			writeableBitmap.Freeze();
		}
		return writeableBitmap;
	}

	private static void TryTwice(Action action)
	{
		try
		{
			action();
		}
		catch (Exception)
		{
			action();
		}
	}

	private static bool IsLoadingDeferred(BitmapSource source)
	{
		if (!(source is BitmapImage bitmapImage))
		{
			return false;
		}
		if (bitmapImage.UriSource != null && !bitmapImage.UriSource.IsAbsoluteUri)
		{
			return bitmapImage.BaseUri == null;
		}
		return false;
	}

	private static BitmapDecoder GetDecoder(BitmapSource image, out GifFile gifFile)
	{
		gifFile = null;
		BitmapDecoder bitmapDecoder = null;
		Stream stream = null;
		Uri result = null;
		BitmapCreateOptions createOptions = BitmapCreateOptions.None;
		if (image is BitmapImage bitmapImage)
		{
			createOptions = bitmapImage.CreateOptions;
			if (bitmapImage.StreamSource != null)
			{
				stream = bitmapImage.StreamSource;
			}
			else if (bitmapImage.UriSource != null)
			{
				result = bitmapImage.UriSource;
				if (bitmapImage.BaseUri != null && !result.IsAbsoluteUri)
				{
					result = new Uri(bitmapImage.BaseUri, result);
				}
			}
		}
		else if (image is BitmapFrame bitmapFrame)
		{
			bitmapDecoder = bitmapFrame.Decoder;
			Uri.TryCreate(bitmapFrame.BaseUri, bitmapFrame.ToString(), out result);
		}
		if (bitmapDecoder == null)
		{
			if (stream != null && stream.CanRead)
			{
				stream.Position = 0L;
				bitmapDecoder = BitmapDecoder.Create(stream, createOptions, BitmapCacheOption.OnLoad);
				stream.Close();
			}
			else if (result != null && result.IsAbsoluteUri)
			{
				bitmapDecoder = BitmapDecoder.Create(result, createOptions, BitmapCacheOption.OnLoad);
			}
		}
		if (bitmapDecoder is GifBitmapDecoder && !CanReadNativeMetadata(bitmapDecoder))
		{
			if (stream != null && stream.CanRead)
			{
				stream.Position = 0L;
				gifFile = GifFile.ReadGifFile(stream, metadataOnly: true);
				stream.Close();
			}
			else
			{
				if (!(result != null))
				{
					throw new InvalidOperationException("Can't get URI or Stream from the source. AnimatedSource should be either a BitmapImage, or a BitmapFrame constructed from a URI.");
				}
				gifFile = DecodeGifFile(result);
			}
		}
		if (bitmapDecoder == null)
		{
			throw new InvalidOperationException("Can't get a decoder from the source. AnimatedSource should be either a BitmapImage or a BitmapFrame.");
		}
		return bitmapDecoder;
	}

	private static bool CanReadNativeMetadata(BitmapDecoder decoder)
	{
		try
		{
			return decoder.Metadata != null;
		}
		catch
		{
			return false;
		}
	}

	private static GifFile DecodeGifFile(Uri uri)
	{
		Stream stream = null;
		if (uri.Scheme == PackUriHelper.UriSchemePack)
		{
			StreamResourceInfo streamResourceInfo = ((!(uri.Authority == "siteoforigin:,,,")) ? Application.GetResourceStream(uri) : Application.GetRemoteStream(uri));
			if (streamResourceInfo != null)
			{
				stream = streamResourceInfo.Stream;
			}
		}
		else
		{
			stream = new WebClient().OpenRead(uri);
		}
		if (stream != null)
		{
			using (stream)
			{
				return GifFile.ReadGifFile(stream, metadataOnly: true);
			}
		}
		return null;
	}

	private static bool IsFullFrame(FrameMetadata metadata, Int32Size fullSize)
	{
		if (metadata.Left == 0 && metadata.Top == 0 && metadata.Width == fullSize.Width)
		{
			return metadata.Height == fullSize.Height;
		}
		return false;
	}

	private static BitmapSource MakeFrame(Int32Size fullSize, BitmapSource rawFrame, FrameMetadata metadata, BitmapSource baseFrame)
	{
		if (baseFrame == null && IsFullFrame(metadata, fullSize))
		{
			return rawFrame;
		}
		DrawingVisual drawingVisual = new DrawingVisual();
		using (DrawingContext drawingContext = drawingVisual.RenderOpen())
		{
			if (baseFrame != null)
			{
				Rect rectangle = new Rect(0.0, 0.0, fullSize.Width, fullSize.Height);
				drawingContext.DrawImage(baseFrame, rectangle);
			}
			Rect rectangle2 = new Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height);
			drawingContext.DrawImage(rawFrame, rectangle2);
		}
		RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(fullSize.Width, fullSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
		renderTargetBitmap.Render(drawingVisual);
		WriteableBitmap writeableBitmap = new WriteableBitmap(renderTargetBitmap);
		if (writeableBitmap.CanFreeze && !writeableBitmap.IsFrozen)
		{
			writeableBitmap.Freeze();
		}
		return writeableBitmap;
	}

	private static RepeatBehavior GetActualRepeatBehavior(Image imageControl, BitmapDecoder decoder, GifFile gifMetadata)
	{
		RepeatBehavior repeatBehavior = GetRepeatBehavior(imageControl);
		if (repeatBehavior != default(RepeatBehavior))
		{
			return repeatBehavior;
		}
		int num = ((int?)gifMetadata?.RepeatCount) ?? GetRepeatCount(decoder);
		if (num == 0)
		{
			return RepeatBehavior.Forever;
		}
		return new RepeatBehavior(num);
	}

	private static int GetRepeatCount(BitmapDecoder decoder)
	{
		BitmapMetadata applicationExtension = GetApplicationExtension(decoder, "NETSCAPE2.0");
		if (applicationExtension != null)
		{
			byte[] queryOrNull = applicationExtension.GetQueryOrNull<byte[]>("/Data");
			if (queryOrNull != null && queryOrNull.Length >= 4)
			{
				return BitConverter.ToUInt16(queryOrNull, 2);
			}
		}
		return 1;
	}

	private static BitmapMetadata GetApplicationExtension(BitmapDecoder decoder, string application)
	{
		int num = 0;
		string query = "/appext";
		for (BitmapMetadata queryOrNull = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(query); queryOrNull != null; queryOrNull = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(query))
		{
			byte[] queryOrNull2 = queryOrNull.GetQueryOrNull<byte[]>("/Application");
			if (queryOrNull2 != null && Encoding.ASCII.GetString(queryOrNull2) == application)
			{
				return queryOrNull;
			}
			query = $"/[{++num}]appext";
		}
		return null;
	}

	private static FrameMetadata GetFrameMetadata(BitmapDecoder decoder, GifFile gifMetadata, int frameIndex)
	{
		if (gifMetadata != null && gifMetadata.Frames.Count > frameIndex)
		{
			return GetFrameMetadata(gifMetadata.Frames[frameIndex]);
		}
		return GetFrameMetadata(decoder.Frames[frameIndex]);
	}

	private static FrameMetadata GetFrameMetadata(BitmapFrame frame)
	{
		BitmapMetadata metadata = (BitmapMetadata)frame.Metadata;
		TimeSpan delay = TimeSpan.FromMilliseconds(100.0);
		int queryOrDefault = metadata.GetQueryOrDefault("/grctlext/Delay", 10);
		if (queryOrDefault != 0)
		{
			delay = TimeSpan.FromMilliseconds(queryOrDefault * 10);
		}
		FrameDisposalMethod queryOrDefault2 = (FrameDisposalMethod)metadata.GetQueryOrDefault("/grctlext/Disposal", 0);
		return new FrameMetadata
		{
			Left = metadata.GetQueryOrDefault("/imgdesc/Left", 0),
			Top = metadata.GetQueryOrDefault("/imgdesc/Top", 0),
			Width = metadata.GetQueryOrDefault("/imgdesc/Width", frame.PixelWidth),
			Height = metadata.GetQueryOrDefault("/imgdesc/Height", frame.PixelHeight),
			Delay = delay,
			DisposalMethod = queryOrDefault2
		};
	}

	private static FrameMetadata GetFrameMetadata(GifFrame gifMetadata)
	{
		GifImageDescriptor descriptor = gifMetadata.Descriptor;
		FrameMetadata frameMetadata = new FrameMetadata
		{
			Left = descriptor.Left,
			Top = descriptor.Top,
			Width = descriptor.Width,
			Height = descriptor.Height,
			Delay = TimeSpan.FromMilliseconds(100.0),
			DisposalMethod = FrameDisposalMethod.None
		};
		GifGraphicControlExtension gifGraphicControlExtension = gifMetadata.Extensions.OfType<GifGraphicControlExtension>().FirstOrDefault();
		if (gifGraphicControlExtension != null)
		{
			if (gifGraphicControlExtension.Delay != 0)
			{
				frameMetadata.Delay = TimeSpan.FromMilliseconds(gifGraphicControlExtension.Delay);
			}
			frameMetadata.DisposalMethod = (FrameDisposalMethod)gifGraphicControlExtension.DisposalMethod;
		}
		return frameMetadata;
	}

	private static Int32Size GetFullSize(BitmapDecoder decoder, GifFile gifMetadata)
	{
		if (gifMetadata != null)
		{
			GifLogicalScreenDescriptor logicalScreenDescriptor = gifMetadata.Header.LogicalScreenDescriptor;
			return new Int32Size(logicalScreenDescriptor.Width, logicalScreenDescriptor.Height);
		}
		int queryOrDefault = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Width", 0);
		int queryOrDefault2 = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Height", 0);
		return new Int32Size(queryOrDefault, queryOrDefault2);
	}

	private static T GetQueryOrDefault<T>(this BitmapMetadata metadata, string query, T defaultValue)
	{
		if (metadata.ContainsQuery(query))
		{
			return (T)Convert.ChangeType(metadata.GetQuery(query), typeof(T));
		}
		return defaultValue;
	}

	private static T GetQueryOrNull<T>(this BitmapMetadata metadata, string query) where T : class
	{
		if (metadata.ContainsQuery(query))
		{
			return metadata.GetQuery(query) as T;
		}
		return null;
	}
}
