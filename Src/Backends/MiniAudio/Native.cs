using System.Reflection;
using System.Runtime.InteropServices;
using SoundFlow.Backends.MiniAudio.Enums;
using SoundFlow.Enums;

namespace SoundFlow.Backends.MiniAudio;

internal static unsafe partial class Native
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AudioCallback(nint device, nint output, nint input, uint length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Result DecoderRead(nint pDecoder, nint pBufferOut, ulong bytesToRead, out uint* pBytesRead);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Result DecoderSeek(nint pDecoder, long byteOffset, SeekPoint origin);

    #region Encoder

    [LibraryImport("libminiaudio", EntryPoint = "ma_encoder_init_file", StringMarshalling = StringMarshalling.Utf8)]
    public static partial Result EncoderInitFile(string filePath, nint pConfig, nint pEncoder);

    [LibraryImport("libminiaudio", EntryPoint = "ma_encoder_uninit")]
    public static partial void EncoderUninit(nint pEncoder);

    [LibraryImport("libminiaudio", EntryPoint = "ma_encoder_write_pcm_frames")]
    public static partial Result EncoderWritePcmFrames(nint pEncoder, nint pFramesIn, ulong frameCount,
        ulong* pFramesWritten);

    #endregion

    #region Decoder

    [LibraryImport("libminiaudio", EntryPoint = "ma_decoder_init")]
    public static partial Result DecoderInit(DecoderRead onRead, DecoderSeek onSeek, nint pUserData,
        nint pConfig, nint pDecoder);

    [LibraryImport("libminiaudio", EntryPoint = "ma_decoder_uninit")]
    public static partial Result DecoderUninit(nint pDecoder);

    [LibraryImport("libminiaudio", EntryPoint = "ma_decoder_read_pcm_frames")]
    public static partial Result DecoderReadPcmFrames(nint decoder, nint framesOut, uint frameCount,
        out uint* framesRead);

    [LibraryImport("libminiaudio", EntryPoint = "ma_decoder_seek_to_pcm_frame")]
    public static partial Result DecoderSeekToPcmFrame(nint decoder, ulong frame);

    [LibraryImport("libminiaudio", EntryPoint = "ma_decoder_get_length_in_pcm_frames")]
    public static partial Result DecoderGetLengthInPcmFrames(nint decoder, out uint* length);

    #endregion

    #region Context

    [LibraryImport("libminiaudio", EntryPoint = "ma_context_init")]
    public static partial Result ContextInit(nint backends, uint backendCount, nint config, nint context);
    
    [LibraryImport("libminiaudio", EntryPoint = "ma_context_uninit")]
    public static partial void ContextUninit(nint context);

    #endregion

    #region Device

    [LibraryImport("libminiaudio", EntryPoint = "sf_get_devices")]
    public static partial Result GetDevices(nint context, out nint pPlaybackDevices, out nint pCaptureDevices, out nint playbackDeviceCount, out nint captureDeviceCount);

    [LibraryImport("libminiaudio", EntryPoint = "ma_device_init")]
    public static partial Result DeviceInit(nint context, nint config, nint device);

    [LibraryImport("libminiaudio", EntryPoint = "ma_device_uninit")]
    public static partial void DeviceUninit(nint device);

    [LibraryImport("libminiaudio", EntryPoint = "ma_device_start")]
    public static partial Result DeviceStart(nint device);

    [LibraryImport("libminiaudio", EntryPoint = "ma_device_stop")]
    public static partial Result DeviceStop(nint device);

    #endregion

    #region Allocations

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_encoder")]
    public static partial nint AllocateEncoder();

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_decoder")]
    public static partial nint AllocateDecoder();

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_context")]
    public static partial nint AllocateContext();

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_device")]
    public static partial nint AllocateDevice();

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_decoder_config")]
    public static partial nint AllocateDecoderConfig(SampleFormat format, uint channels, uint sampleRate);

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_encoder_config")]
    public static partial nint AllocateEncoderConfig(EncodingFormat encodingFormat, SampleFormat format, uint channels,
        uint sampleRate);

    [LibraryImport("libminiaudio", EntryPoint = "sf_allocate_device_config")]
    public static partial nint AllocateDeviceConfig(Capability capabilityType, SampleFormat format, uint channels,
        uint sampleRate, AudioCallback dataCallback, nint playbackDevice, nint captureDevice);

    #endregion

    #region Utils

    [LibraryImport("libminiaudio", EntryPoint = "sf_free")]
    public static partial void Free(nint ptr);

    #endregion
}