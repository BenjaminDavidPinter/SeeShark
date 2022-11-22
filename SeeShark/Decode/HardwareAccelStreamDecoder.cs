// Copyright (c) Speykious
// This file is part of SeeShark.
// SeeShark is licensed under the BSD 3-Clause License. See LICENSE for details.

using FFmpeg.AutoGen;
using SeeShark.FFmpeg;

namespace SeeShark.Decode;

/// <summary>
/// Decodes a video stream using hardware acceleration.
/// This may not be needed at all for the library, we will have to investigate that later.
/// </summary>
public unsafe class HardwareAccelVideoStreamDecoder : VideoStreamDecoder
{
    protected readonly Frame HwFrame;

    public HardwareAccelVideoStreamDecoder(string url,
        HardwareAccelDevice hwAccelDevice, AVInputFormat* inputFormat = null)
    : base(url, inputFormat)
    {
        HwFrame = new Frame();

        ffmpeg.av_hwdevice_ctx_create(
            &CodecContext->hw_device_ctx,
            (AVHWDeviceType)hwAccelDevice,
            null, null, 0
        ).ThrowExceptionIfError();
    }

    public new DecodeStatus TryDecodeNextFrame(out Frame nextFrame)
    {
        DecodeStatus ret = base.TryDecodeNextFrame(out var frame);

        frame.HardwareAccelCopyTo(HwFrame).ThrowExceptionIfError();
        nextFrame = HwFrame;

        return ret;
    }

    protected override void DisposeManaged()
    {
        base.DisposeManaged();

        HwFrame.Dispose();
    }
}
