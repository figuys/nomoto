using System;
using System.Runtime.InteropServices;
using System.Text;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;

public class LameApi
{
	private const string libname = "libmp3lame.32.dll";

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lame_init();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_close(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern uint lame_get_lametag_frame(IntPtr context, [In][Out] byte[] buffer, [In] uint size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern string get_lame_version();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern string get_lame_short_version();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern string get_lame_very_short_version();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern string get_psy_version();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern string get_lame_url();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern string get_lame_os_bitness();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void get_lame_version_numerical([Out] LAMEVersion ver);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_num_samples(IntPtr context, ulong num_samples);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern ulong lame_get_num_samples(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_in_samplerate(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_in_samplerate(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_num_channels(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_num_channels(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_scale(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_scale(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_scale_left(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_scale_left(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_scale_right(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_scale_right(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_out_samplerate(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_out_samplerate(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_analysis(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_analysis(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_bWriteVbrTag(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern bool lame_get_bWriteVbrTag(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_decode_only(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern bool lame_get_decode_only(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_quality(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_quality(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_mode(IntPtr context, MPEGMode value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern MPEGMode lame_get_mode(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_force_ms(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_force_ms(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_free_format(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_free_format(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_findReplayGain(IntPtr context, [In][MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_findReplayGain(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_decode_on_the_fly(IntPtr context, [In][MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_decode_on_the_fly(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_nogap_total(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_nogap_total(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_nogap_currentindex(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_nogap_currentindex(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_brate(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_brate(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_compression_ratio(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_compression_ratio(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_preset(IntPtr context, LAMEPreset value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_asm_optimizations(IntPtr context, ASMOptimizations opt, [MarshalAs(UnmanagedType.Bool)] bool val);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_copyright(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_copyright(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_original(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_original(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_error_protection(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_error_protection(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_extension(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_extension(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_strict_ISO(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_strict_ISO(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_disable_reservoir(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_disable_reservoir(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_quant_comp(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_quant_comp(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_quant_comp_short(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_quant_comp_short(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_experimentalX(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_experimentalX(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_experimentalY(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_experimentalY(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_experimentalZ(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_experimentalZ(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_exp_nspsytune(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_exp_nspsytune(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_msfix(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_msfix(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR(IntPtr context, VBRMode value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern VBRMode lame_get_VBR(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR_q(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_VBR_q(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR_quality(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_VBR_quality(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR_mean_bitrate_kbps(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_VBR_mean_bitrate_kbps(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR_min_bitrate_kbps(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_VBR_min_bitrate_kbps(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR_max_bitrate_kbps(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_VBR_max_bitrate_kbps(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_VBR_hard_min(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_VBR_hard_min(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_lowpassfreq(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_lowpassfreq(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_lowpasswidth(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_lowpasswidth(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_highpassfreq(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_highpassfreq(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_highpasswidth(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_highpasswidth(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_ATHonly(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_ATHonly(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_ATHshort(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_ATHshort(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_noATH(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_noATH(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_ATHtype(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_ATHtype(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_ATHlower(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_ATHlower(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_athaa_type(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_athaa_type(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_athaa_sensitivity(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_athaa_sensitivity(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_allow_diff_short(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_allow_diff_short(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_useTemporal(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_useTemporal(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_interChRatio(IntPtr context, float value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_interChRatio(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_no_short_blocks(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_no_short_blocks(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_force_short_blocks(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_force_short_blocks(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_emphasis(IntPtr context, int value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_emphasis(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern MPEGVersion lame_get_version(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_encoder_delay(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_encoder_padding(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_framesize(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_mf_samples_to_encode(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_size_mp3buffer(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_frameNum(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_totalframes(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_RadioGain(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_AudiophileGain(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_PeakSample(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_noclipGainChange(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern float lame_get_noclipScale(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_init_params(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer(IntPtr context, [In] short[] buffer_l, [In] short[] buffer_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_interleaved(IntPtr context, [In] short[] pcm, int num_samples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_float(IntPtr context, [In] float[] pcm_l, [In] float[] pcm_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_ieee_float(IntPtr context, [In] float[] pcm_l, [In] float[] pcm_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_interleaved_ieee_float(IntPtr context, [In] float[] pcm, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_ieee_double(IntPtr context, [In] double[] pcm_l, [In] double[] pcm_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_interleaved_ieee_double(IntPtr context, [In] double[] pcm, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_long(IntPtr context, [In] long[] buffer_l, [In] long[] buffer_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_long2(IntPtr context, [In] long[] buffer_l, [In] long[] buffer_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_buffer_int(IntPtr context, [In] int[] buffer_l, [In] int[] buffer_r, int nSamples, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_flush(IntPtr context, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_encode_flush_nogap(IntPtr context, [In][Out] byte[] mp3buf, int mp3buf_size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_init_bitstream(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_errorf(IntPtr context, delReportFunction fn);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_debugf(IntPtr context, delReportFunction fn);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_set_msgf(IntPtr context, delReportFunction fn);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lame_print_config(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lame_print_internals(IntPtr context);

	[DllImport("msvcrt.dll", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
	internal static extern int _vsnprintf_s([In][Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder str, int sizeOfBuffer, int count, [In][MarshalAs(UnmanagedType.LPStr)] string format, [In] IntPtr va_args);

	internal static string printf(string format, IntPtr va_args)
	{
		StringBuilder stringBuilder = new StringBuilder(4096);
		_vsnprintf_s(stringBuilder, stringBuilder.Capacity, stringBuilder.Capacity - 2, format.Replace("\t", "ÿ"), va_args);
		return stringBuilder.ToString().Replace("ÿ", "\t");
	}

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr hip_decode_init();

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int hip_decode_exit(IntPtr decContext);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void hip_set_errorf(IntPtr decContext, delReportFunction f);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void hip_set_debugf(IntPtr decContext, delReportFunction f);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void hip_set_msgf(IntPtr decContext, delReportFunction f);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int hip_decode(IntPtr decContext, [In] byte[] mp3buf, int len, [In][Out] short[] pcm_l, [In][Out] short[] pcm_r);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int hip_decode_headers(IntPtr decContext, [In] byte[] mp3buf, int len, IntPtr pcm_l, IntPtr pcm_r, [Out] mp3data mp3data);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int hip_decode1(IntPtr decContext, [In] byte[] mp3buf, int len, [In][Out] short[] pcm_l, [In][Out] short[] pcm_r);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int hip_decode1_headers(IntPtr decContext, [In] byte[] mp3buf, int len, [In][Out] short[] pcm_l, [In][Out] short[] pcm_r, [Out] mp3data mp3data);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int hip_decode(IntPtr decContext, [In] byte[] mp3buf, int len, [In][Out] short[] pcm_l, [In][Out] short[] pcm_r, [Out] mp3data mp3data, out int enc_delay, out int enc_padding);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_genre_list(delGenreCallback handler, IntPtr cookie);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_init(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_add_v2(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_v1_only(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_v2_only(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_space_v1(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_pad_v2(IntPtr context);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_set_pad(IntPtr context, int n);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_set_title(IntPtr context, string title);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_set_artist(IntPtr context, string artist);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_set_album(IntPtr context, string album);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_set_year(IntPtr context, string year);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void id3tag_set_comment(IntPtr context, string comment);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool id3tag_set_track(IntPtr context, string track);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int id3tag_set_genre(IntPtr context, string genre);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool id3tag_set_fieldvalue(IntPtr context, string value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool id3tag_set_albumart(IntPtr context, [In] byte[] image, int size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_id3v1_tag(IntPtr context, [In][Out] byte[] buffer, int size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lame_get_id3v2_tag(IntPtr context, [In][Out] byte[] buffer, int size);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lame_set_write_id3tag_automatic(IntPtr context, bool value);

	[DllImport("libmp3lame.32.dll", CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool lame_get_write_id3tag_automatic(IntPtr context);
}
