using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.Encode;

public static class ImageExtensions
{
	public static UnsafeBitmapContext CreateUnsafeContext(this Image image)
	{
		return new UnsafeBitmapContext(image);
	}

	public static Rectangle DetectPadding(this Image image, Color backgroundColor = default(Color))
	{
		if (backgroundColor.IsEmpty)
		{
			backgroundColor = Color.Transparent;
		}
		int num = image.Height;
		int num2 = 0;
		int num3 = image.Width;
		int num4 = 0;
		int num5 = image.Width - 1;
		int num6 = image.Height - 1;
		using (UnsafeBitmapContext unsafeBitmapContext = new UnsafeBitmapContext(image))
		{
			for (int i = 0; i < unsafeBitmapContext.Height; i++)
			{
				for (int j = 0; j < unsafeBitmapContext.Width && (num5 - num4 > j || num3 > j || num6 - num2 > i); j++)
				{
					if (j < num3 && !unsafeBitmapContext.GetRawPixel(j, i).EqualsColor(backgroundColor))
					{
						if (i < num)
						{
							num = i;
						}
						num3 = j;
					}
					if (j < num5 - num4 && !unsafeBitmapContext.GetRawPixel(num5 - j, i).EqualsColor(backgroundColor))
					{
						if (i < num)
						{
							num = i;
						}
						num4 = num5 - j;
					}
					if (i < num6 - num2 && !unsafeBitmapContext.GetRawPixel(j, num6 - i).EqualsColor(backgroundColor))
					{
						num2 = num6 - i;
					}
				}
			}
		}
		num5 = Math.Max(0, num4 - num3);
		num6 = Math.Max(0, num2 - num);
		return new Rectangle(num3, num, num5, num6);
	}

	public static Image ScaleToFit(this Image image, Size size, ScalingMode mode = ScalingMode.FitContent)
	{
		return image.ScaleToFit(size, default(Color), dispose: true, mode);
	}

	public static Image ScaleToFit(this Image image, Size size, Color backgroundColor, ScalingMode mode = ScalingMode.FitContent)
	{
		return image.ScaleToFit(size, backgroundColor, dispose: true, mode);
	}

	public static Image ScaleToFit(this Image image, Size size, bool dispose, ScalingMode mode = ScalingMode.FitContent)
	{
		return image.ScaleToFit(size, default(Color), dispose, mode);
	}

	public static Image ScaleToFit(this Image image, Size size, Color backgroundColor, bool dispose = true, ScalingMode mode = ScalingMode.FitContent)
	{
		Bitmap bitmap = new Bitmap(size.Width, size.Height);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			if (!backgroundColor.IsEmpty)
			{
				graphics.Clear(backgroundColor);
			}
			graphics.DrawImage(image, new Rectangle(0, 0, size.Width, size.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
		}
		if (dispose)
		{
			image.Dispose();
		}
		return bitmap;
	}

	public static Image Stretch(this Image image, Size size, bool dispose = true)
	{
		Bitmap bitmap = new Bitmap(size.Width, size.Height);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.DrawImage(image, 0, 0, size.Width, size.Height);
		}
		if (dispose)
		{
			image.Dispose();
		}
		return bitmap;
	}

	public static Image Rotate(this Image image, double angle, ScalingMode mode = ScalingMode.Overflow)
	{
		return image.Rotate(angle, dispose: true, mode);
	}

	public static Image Rotate(this Image image, double angle, bool dispose, ScalingMode mode = ScalingMode.Overflow)
	{
		int width = image.Width;
		int height = image.Height;
		if (mode == ScalingMode.FitContent)
		{
			double num = angle % 180.0;
			double num2 = Math.Sqrt(Math.Pow(image.Width, 2.0) + Math.Pow(image.Height, 2.0));
			height = (int)(Math.Sin((Math.Atan((double)image.Height / (double)image.Width) * 180.0 / Math.PI + ((num > 90.0) ? (180.0 - num) : num)) * Math.PI / 180.0) * num2);
			width = (int)(Math.Cos((Math.Atan((double)(-image.Height) / (double)image.Width) * 180.0 / Math.PI + ((num > 90.0) ? (180.0 - num) : num)) * Math.PI / 180.0) * num2);
		}
		Bitmap bitmap = new Bitmap(width, height);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.TranslateTransform((float)(-image.Width) / 2f, (float)(-image.Height) / 2f, MatrixOrder.Prepend);
			graphics.RotateTransform((float)angle, MatrixOrder.Append);
			graphics.TranslateTransform((float)bitmap.Width / 2f, (float)bitmap.Height / 2f, MatrixOrder.Append);
			graphics.DrawImage(image, 0, 0);
		}
		if (dispose)
		{
			image.Dispose();
		}
		return bitmap;
	}
}
