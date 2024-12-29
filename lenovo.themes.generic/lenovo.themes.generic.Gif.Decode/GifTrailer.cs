namespace lenovo.themes.generic.Gif.Decode;

internal class GifTrailer : GifBlock
{
	internal const int TrailerByte = 59;

	internal override GifBlockKind Kind => GifBlockKind.Other;

	private GifTrailer()
	{
	}

	internal static GifTrailer ReadTrailer()
	{
		return new GifTrailer();
	}
}
