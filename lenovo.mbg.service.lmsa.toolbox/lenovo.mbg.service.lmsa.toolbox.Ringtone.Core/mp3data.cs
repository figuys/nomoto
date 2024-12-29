using System.Runtime.InteropServices;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;

public struct mp3data
{
	[MarshalAs(UnmanagedType.Bool)]
	public bool header_parsed;

	public int stereo;

	public int samplerate;

	public int bitrate;

	public int mode;

	public int mode_ext;

	public int framesize;

	public ulong nsamp;

	public int totalframes;

	public int framenum;
}
