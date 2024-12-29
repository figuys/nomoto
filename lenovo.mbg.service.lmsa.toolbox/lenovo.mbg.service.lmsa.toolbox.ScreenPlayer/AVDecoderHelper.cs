using System;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class AVDecoderHelper
{
	private int_array4 stride;

	private byte_ptrArray4 render_data;

	private unsafe AVPacket* packet = null;

	private unsafe AVFrame* encode_frame = null;

	private unsafe SwsContext* swcontext = null;

	private unsafe AVCodecContext* decoderCtx = null;

	public unsafe AVDecoderHelper()
	{
		stride = default(int_array4);
		render_data = default(byte_ptrArray4);
		AVCodec* codec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);
		decoderCtx = ffmpeg.avcodec_alloc_context3(codec);
		ffmpeg.avcodec_open2(decoderCtx, codec, null);
		packet = (AVPacket*)ffmpeg.av_malloc((ulong)sizeof(AVPacket));
		encode_frame = ffmpeg.av_frame_alloc();
	}

	public unsafe void SizeChanged(int width, int height, byte* buff, int srcWidth, int srcHeight)
	{
		ffmpeg.av_image_fill_arrays(ref render_data, ref stride, buff, AVPixelFormat.AV_PIX_FMT_BGRA, width, height, 1);
		if (swcontext != null)
		{
			ffmpeg.sws_freeContext(swcontext);
		}
		swcontext = ffmpeg.sws_getContext(srcWidth, srcHeight, AVPixelFormat.AV_PIX_FMT_YUV420P, width, height, AVPixelFormat.AV_PIX_FMT_BGRA, 4, null, null, null);
	}

	public unsafe bool DecodeFrame(byte[] buff, int len, int offset = 4)
	{
		if (packet == null || decoderCtx == null || swcontext == null)
		{
			return false;
		}
		try
		{
			ffmpeg.av_new_packet(packet, len);
			Marshal.Copy(buff, offset, (IntPtr)packet->data, len);
			ffmpeg.avcodec_send_packet(decoderCtx, packet);
			ffmpeg.av_packet_unref(packet);
			if (ffmpeg.avcodec_receive_frame(decoderCtx, encode_frame) < 0)
			{
				return false;
			}
			return ffmpeg.sws_scale(swcontext, encode_frame->data, encode_frame->linesize, 0, encode_frame->height, render_data, stride) > 0;
		}
		catch
		{
			return false;
		}
	}

	public unsafe void Release()
	{
		ffmpeg.avcodec_close(decoderCtx);
		fixed (AVCodecContext** avctx = &decoderCtx)
		{
			ffmpeg.avcodec_free_context(avctx);
		}
		ffmpeg.av_free(packet);
		fixed (AVFrame** frame = &encode_frame)
		{
			ffmpeg.av_frame_free(frame);
		}
		ffmpeg.sws_freeContext(swcontext);
	}
}
