using System;
using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace lenovo.mbg.service.common.utilities;

public static class QrCodeUtility
{
	public static MemoryStream GenerateQrCodeImageStream(string content)
	{
		MemoryStream memoryStream = null;
		if (!string.IsNullOrEmpty(content))
		{
			try
			{
				QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
				QrCode qrCode = new QrCode();
				qrEncoder.TryEncode(content, out qrCode);
				GraphicsRenderer graphicsRenderer = new GraphicsRenderer(new FixedModuleSize(12, QuietZoneModules.Two));
				memoryStream = new MemoryStream();
				graphicsRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
			}
			catch
			{
				memoryStream = null;
			}
		}
		return memoryStream;
	}

	public static DateTime? ConvertDateTime(string value)
	{
		try
		{
			if (value == null)
			{
				return null;
			}
			long num = long.Parse(value);
			return new DateTime(1970, 1, 1).AddMilliseconds(num).ToLocalTime();
		}
		catch (Exception)
		{
			return null;
		}
	}
}
