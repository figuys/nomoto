using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.Encode;

public sealed class UnsafeBitmapContext : IDisposable
{
	public struct Pixel
	{
		public byte Blue;

		public byte Green;

		public byte Red;

		public byte Alpha;
	}

	private Stream _originalStream;

	private Bitmap _bitmap;

	private BitmapData _lockData;

	private unsafe byte* _ptrBase;

	private int _pixelWidth;

	public int Width { get; private set; }

	public int Height { get; private set; }

	public UnsafeBitmapContext(Bitmap bitmap)
	{
		_bitmap = bitmap;
		LockBits();
	}

	public UnsafeBitmapContext(Image image)
	{
		if (!(image is Bitmap))
		{
			throw new ArgumentException("Image must be convertable to a bitmap.");
		}
		_bitmap = (Bitmap)image;
		LockBits();
	}

	public UnsafeBitmapContext(Stream stream)
	{
		try
		{
			_originalStream = stream;
			stream.Position = 0L;
			_bitmap = (Bitmap)Image.FromStream(stream);
		}
		catch
		{
			throw new ArgumentException("Stream did not contain a valid image format.");
		}
		LockBits();
	}

	private unsafe void LockBits()
	{
		Width = _bitmap.Width;
		Height = _bitmap.Height;
		_pixelWidth = sizeof(Pixel);
		_lockData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
		_ptrBase = (byte*)_lockData.Scan0.ToPointer();
	}

	public void Dispose()
	{
		_bitmap.UnlockBits(_lockData);
		_lockData = null;
		if (_originalStream != null)
		{
			_originalStream.SetLength(0L);
			_originalStream.Position = 0L;
			_bitmap.Save(_originalStream, _bitmap.RawFormat);
			_bitmap.Dispose();
			_originalStream.Position = 0L;
		}
		_originalStream = null;
		_bitmap = null;
	}

	public Color GetPixel(int x, int y)
	{
		Pixel rawPixel = GetRawPixel(x, y);
		return Color.FromArgb(rawPixel.Alpha, rawPixel.Red, rawPixel.Green, rawPixel.Blue);
	}

	public unsafe Pixel GetRawPixel(int x, int y)
	{
		return *Pointer(x, y);
	}

	public void SetPixel(int x, int y, Color color)
	{
		SetPixel(x, y, color.A, color.R, color.G, color.B);
	}

	public unsafe void SetPixel(int x, int y, byte alpha, byte red, byte green, byte blue)
	{
		Pixel* intPtr = Pointer(x, y);
		intPtr->Alpha = alpha;
		intPtr->Red = red;
		intPtr->Green = green;
		intPtr->Blue = blue;
	}

	private unsafe Pixel* Pointer(int x, int y)
	{
		if (x >= Width || x < 0 || y >= Height || y < 0)
		{
			Dispose();
			throw new ArgumentException("The X and Y parameters must be within the scope of the image pixel ranges.");
		}
		return (Pixel*)(_ptrBase + y * _lockData.Stride + x * _pixelWidth);
	}
}
