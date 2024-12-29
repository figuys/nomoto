using System;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

internal static class FFmpegHelper
{
	public unsafe static string av_strerror(int error)
	{
		int num = 1024;
		byte* ptr = stackalloc byte[(int)(uint)num];
		ffmpeg.av_strerror(error, ptr, (ulong)num);
		return Marshal.PtrToStringAnsi((IntPtr)ptr);
	}

	public static string GetErrorMessage(int error)
	{
		if (error < 0)
		{
			return "Unknown error occured!";
		}
		return av_strerror(error);
	}
}
