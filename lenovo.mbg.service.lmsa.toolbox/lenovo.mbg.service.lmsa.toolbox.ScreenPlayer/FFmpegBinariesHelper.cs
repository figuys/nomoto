using System;
using System.IO;
using FFmpeg.AutoGen;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class FFmpegBinariesHelper
{
	internal static void RegisterFFmpegBinaries()
	{
		string text = Environment.CurrentDirectory;
		string path = "FFmpeg";
		while (text != null)
		{
			string text2 = Path.Combine(text, path);
			if (Directory.Exists(text2))
			{
				ffmpeg.RootPath = text2;
				break;
			}
			text = Directory.GetParent(text)?.FullName;
		}
	}
}
