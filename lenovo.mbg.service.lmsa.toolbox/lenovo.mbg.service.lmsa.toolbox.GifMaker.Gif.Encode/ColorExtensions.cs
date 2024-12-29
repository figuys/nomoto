using System;
using System.Drawing;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.Encode;

public static class ColorExtensions
{
	public static bool EqualsColor(this UnsafeBitmapContext.Pixel pixel, Color color)
	{
		if (color.A != 0 || pixel.Alpha != 0)
		{
			if (color.A == pixel.Alpha && color.R == pixel.Red && color.G == pixel.Green)
			{
				return color.B == pixel.Blue;
			}
			return false;
		}
		return true;
	}

	public static bool EqualsPixel(this Color color, UnsafeBitmapContext.Pixel pixel)
	{
		return pixel.EqualsColor(color);
	}

	public static Color FadeTo(this Color from, Color to, float fade)
	{
		return Color.FromArgb((int)Math.Min(255f, Math.Max(0f, (float)(int)from.A + (float)(to.A - from.A) * fade)), (int)Math.Min(255f, Math.Max(0f, (float)(int)from.R + (float)(to.R - from.R) * fade)), (int)Math.Min(255f, Math.Max(0f, (float)(int)from.G + (float)(to.G - from.G) * fade)), (int)Math.Min(255f, Math.Max(0f, (float)(int)from.B + (float)(to.B - from.B) * fade)));
	}
}
