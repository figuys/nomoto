using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class ImageHandle
{
	public static void CutForSquare(string fromFile, string fileSaveUrl, int side, int quality)
	{
		string directoryName = Path.GetDirectoryName(fileSaveUrl);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		Image image = Image.FromFile(fromFile, useEmbeddedColorManagement: true);
		ImageFormat rawFormat = image.RawFormat;
		if (image.Width <= side && image.Height <= side)
		{
			image.Save(fileSaveUrl, rawFormat);
			return;
		}
		int num = image.Width;
		int num2 = image.Height;
		if (num != num2)
		{
			Image image2 = null;
			Graphics graphics = null;
			if (num > num2)
			{
				image2 = new Bitmap(num2, num2);
				graphics = Graphics.FromImage(image2);
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				Rectangle srcRect = new Rectangle((num - num2) / 2, 0, num2, num2);
				Rectangle destRect = new Rectangle(0, 0, num2, num2);
				graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
				num = num2;
			}
			else
			{
				image2 = new Bitmap(num, num);
				graphics = Graphics.FromImage(image2);
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				Rectangle srcRect2 = new Rectangle(0, (num2 - num) / 2, num, num);
				Rectangle destRect2 = new Rectangle(0, 0, num, num);
				graphics.DrawImage(image, destRect2, srcRect2, GraphicsUnit.Pixel);
				num2 = num;
			}
			image = (Image)image2.Clone();
			graphics.Dispose();
			image2.Dispose();
		}
		Image image3 = new Bitmap(side, side);
		Graphics graphics2 = Graphics.FromImage(image3);
		graphics2.InterpolationMode = InterpolationMode.HighQualityBicubic;
		graphics2.SmoothingMode = SmoothingMode.HighQuality;
		graphics2.Clear(Color.White);
		graphics2.DrawImage(image, new Rectangle(0, 0, side, side), new Rectangle(0, 0, num, num2), GraphicsUnit.Pixel);
		ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
		ImageCodecInfo encoder = null;
		ImageCodecInfo[] array = imageEncoders;
		foreach (ImageCodecInfo imageCodecInfo in array)
		{
			if (imageCodecInfo.MimeType == "image/jpeg" || imageCodecInfo.MimeType == "image/bmp" || imageCodecInfo.MimeType == "image/png" || imageCodecInfo.MimeType == "image/gif")
			{
				encoder = imageCodecInfo;
			}
		}
		EncoderParameters encoderParameters = new EncoderParameters(1);
		encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
		image3.Save(fileSaveUrl, encoder, encoderParameters);
		encoderParameters.Dispose();
		graphics2.Dispose();
		image3.Dispose();
		image.Dispose();
	}

	public static void CutForCustom(string fromFile, string fileSaveUrl, int maxWidth, int maxHeight, int quality)
	{
		Image image = Image.FromFile(fromFile, useEmbeddedColorManagement: true);
		ImageFormat rawFormat = image.RawFormat;
		if (image.Width <= maxWidth && image.Height <= maxHeight)
		{
			image.Save(fileSaveUrl, rawFormat);
		}
		else
		{
			double num = (double)maxWidth / (double)maxHeight;
			double num2 = (double)image.Width / (double)image.Height;
			if (num == num2)
			{
				Bitmap bitmap = new Bitmap(maxWidth, maxHeight);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.InterpolationMode = InterpolationMode.High;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.Clear(Color.White);
				graphics.DrawImage(image, new Rectangle(0, 0, maxWidth, maxHeight), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
				bitmap.Save(fileSaveUrl, rawFormat);
			}
			else
			{
				Image image2 = null;
				Graphics graphics2 = null;
				Rectangle srcRect = new Rectangle(0, 0, 0, 0);
				Rectangle destRect = new Rectangle(0, 0, 0, 0);
				if (num > num2)
				{
					image2 = new Bitmap(image.Width, (int)Math.Floor((double)image.Width / num));
					graphics2 = Graphics.FromImage(image2);
					srcRect.X = 0;
					srcRect.Y = (int)Math.Floor(((double)image.Height - (double)image.Width / num) / 2.0);
					srcRect.Width = image.Width;
					srcRect.Height = (int)Math.Floor((double)image.Width / num);
					destRect.X = 0;
					destRect.Y = 0;
					destRect.Width = image.Width;
					destRect.Height = (int)Math.Floor((double)image.Width / num);
				}
				else
				{
					image2 = new Bitmap((int)Math.Floor((double)image.Height * num), image.Height);
					graphics2 = Graphics.FromImage(image2);
					srcRect.X = (int)Math.Floor(((double)image.Width - (double)image.Height * num) / 2.0);
					srcRect.Y = 0;
					srcRect.Width = (int)Math.Floor((double)image.Height * num);
					srcRect.Height = image.Height;
					destRect.X = 0;
					destRect.Y = 0;
					destRect.Width = (int)Math.Floor((double)image.Height * num);
					destRect.Height = image.Height;
				}
				graphics2.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics2.SmoothingMode = SmoothingMode.HighQuality;
				graphics2.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
				Image image3 = new Bitmap(maxWidth, maxHeight);
				Graphics graphics3 = Graphics.FromImage(image3);
				graphics3.InterpolationMode = InterpolationMode.High;
				graphics3.SmoothingMode = SmoothingMode.HighQuality;
				graphics3.Clear(Color.White);
				graphics3.DrawImage(image2, new Rectangle(0, 0, maxWidth, maxHeight), new Rectangle(0, 0, image2.Width, image2.Height), GraphicsUnit.Pixel);
				ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
				ImageCodecInfo encoder = null;
				ImageCodecInfo[] array = imageEncoders;
				foreach (ImageCodecInfo imageCodecInfo in array)
				{
					if (imageCodecInfo.MimeType == "image/jpeg" || imageCodecInfo.MimeType == "image/bmp" || imageCodecInfo.MimeType == "image/png" || imageCodecInfo.MimeType == "image/gif")
					{
						encoder = imageCodecInfo;
					}
				}
				EncoderParameters encoderParameters = new EncoderParameters(1);
				encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
				image3.Save(fileSaveUrl, encoder, encoderParameters);
				graphics3.Dispose();
				image3.Dispose();
				graphics2.Dispose();
				image2.Dispose();
			}
		}
		image.Dispose();
	}

	public static void ZoomAuto(string fromFile, string savePath, double targetWidth, double targetHeight)
	{
		ZoomAuto(fromFile, savePath, targetWidth, targetHeight, "", "");
	}

	public static void ZoomAuto(string fromFile, string savePath, double targetWidth, double targetHeight, string watermarkText, string watermarkImage)
	{
		string directoryName = Path.GetDirectoryName(savePath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		Image image = Image.FromFile(fromFile, useEmbeddedColorManagement: true);
		ImageFormat rawFormat = image.RawFormat;
		if ((double)image.Width <= targetWidth && (double)image.Height <= targetHeight)
		{
			if (watermarkText != "")
			{
				using Graphics graphics = Graphics.FromImage(image);
				Font font = new Font("黑体", 10f);
				Brush brush = new SolidBrush(Color.White);
				graphics.DrawString(watermarkText, font, brush, 10f, 10f);
				graphics.Dispose();
			}
			if (watermarkImage != "" && File.Exists(watermarkImage))
			{
				using Image image2 = Image.FromFile(watermarkImage);
				if (image.Width >= image2.Width && image.Height >= image2.Height)
				{
					Graphics graphics2 = Graphics.FromImage(image);
					ImageAttributes imageAttributes = new ImageAttributes();
					ColorMap colorMap = new ColorMap();
					colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
					colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
					ColorMap[] map = new ColorMap[1] { colorMap };
					imageAttributes.SetRemapTable(map, ColorAdjustType.Bitmap);
					ColorMatrix newColorMatrix = new ColorMatrix(new float[5][]
					{
						new float[5] { 1f, 0f, 0f, 0f, 0f },
						new float[5] { 0f, 1f, 0f, 0f, 0f },
						new float[5] { 0f, 0f, 1f, 0f, 0f },
						new float[5] { 0f, 0f, 0f, 0.5f, 0f },
						new float[5] { 0f, 0f, 0f, 0f, 1f }
					});
					imageAttributes.SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
					graphics2.DrawImage(image2, new Rectangle(image.Width - image2.Width, image.Height - image2.Height, image2.Width, image2.Height), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel, imageAttributes);
					graphics2.Dispose();
				}
				image2.Dispose();
			}
			image.Save(savePath, rawFormat);
			return;
		}
		double num = image.Width;
		double num2 = image.Height;
		if (image.Width > image.Height || image.Width == image.Height)
		{
			if ((double)image.Width > targetWidth)
			{
				num = targetWidth;
				num2 = (double)image.Height * (targetWidth / (double)image.Width);
			}
		}
		else if ((double)image.Height > targetHeight)
		{
			num2 = targetHeight;
			num = (double)image.Width * (targetHeight / (double)image.Height);
		}
		Image image3 = new Bitmap((int)num, (int)num2);
		Graphics graphics3 = Graphics.FromImage(image3);
		graphics3.InterpolationMode = InterpolationMode.HighQualityBicubic;
		graphics3.SmoothingMode = SmoothingMode.HighQuality;
		graphics3.Clear(Color.White);
		graphics3.DrawImage(image, new Rectangle(0, 0, image3.Width, image3.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
		if (watermarkText != "")
		{
			using Graphics graphics4 = Graphics.FromImage(image3);
			Font font2 = new Font("宋体", 10f);
			Brush brush2 = new SolidBrush(Color.White);
			graphics4.DrawString(watermarkText, font2, brush2, 10f, 10f);
			graphics4.Dispose();
		}
		if (watermarkImage != "" && File.Exists(watermarkImage))
		{
			using Image image4 = Image.FromFile(watermarkImage);
			if (image3.Width >= image4.Width && image3.Height >= image4.Height)
			{
				Graphics graphics5 = Graphics.FromImage(image3);
				ImageAttributes imageAttributes2 = new ImageAttributes();
				ColorMap colorMap2 = new ColorMap();
				colorMap2.OldColor = Color.FromArgb(255, 0, 255, 0);
				colorMap2.NewColor = Color.FromArgb(0, 0, 0, 0);
				ColorMap[] map2 = new ColorMap[1] { colorMap2 };
				imageAttributes2.SetRemapTable(map2, ColorAdjustType.Bitmap);
				ColorMatrix newColorMatrix2 = new ColorMatrix(new float[5][]
				{
					new float[5] { 1f, 0f, 0f, 0f, 0f },
					new float[5] { 0f, 1f, 0f, 0f, 0f },
					new float[5] { 0f, 0f, 1f, 0f, 0f },
					new float[5] { 0f, 0f, 0f, 0.5f, 0f },
					new float[5] { 0f, 0f, 0f, 0f, 1f }
				});
				imageAttributes2.SetColorMatrix(newColorMatrix2, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				graphics5.DrawImage(image4, new Rectangle(image3.Width - image4.Width, image3.Height - image4.Height, image4.Width, image4.Height), 0, 0, image4.Width, image4.Height, GraphicsUnit.Pixel, imageAttributes2);
				graphics5.Dispose();
			}
			image4.Dispose();
		}
		image3.Save(savePath, rawFormat);
		graphics3.Dispose();
		image3.Dispose();
		image.Dispose();
	}

	public static bool IsWebImage(string contentType)
	{
		switch (contentType)
		{
		case "image/pjpeg":
		case "image/jpeg":
		case "image/gif":
		case "image/bmp":
		case "image/png":
			return true;
		default:
			return false;
		}
	}
}
