using System;
using System.Text;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

public class AVProQuickTimePlugin
{
	public enum MovieSource
	{
		LocalFile,
		URL,
		Memory,
	};
	
	public enum PlaybackState
	{
		Unknown,
		Loading,
		Loaded,
		Playing,
		Stopped,
	};
	
	public enum PixelFormat
	{
		RGBA32,
		YCbCr_SD,
		YCbCr_HD,
	}
	
	// Global Init/Deinit

    [DllImport("AVProQuickTime")]
    public static extern void Deinit();
    [DllImport("AVProQuickTime")]
    public static extern void FreeInstanceHandle(int handle);
    [DllImport("AVProQuickTime")]
    public static extern uint GetCurrentFrame(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetCurrentPosition(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetCurrentPositionSeconds(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetDurationSeconds(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetFrameCount(int handle);
    [DllImport("AVProQuickTime")]
    public static extern bool GetFramePixelsRGBA32(int handle, IntPtr data, int bufferWidth, int bufferHeight);
    [DllImport("AVProQuickTime")]
    public static extern bool GetFramePixelsYUV2(int handle, IntPtr data, int bufferWidth, int bufferHeight);
    [DllImport("AVProQuickTime")]
    public static extern float GetFrameRate(int handle);
    [DllImport("AVProQuickTime")]
    public static extern int GetHeight(int handle);
    [DllImport("AVProQuickTime")]
    public static extern int GetInstanceHandle();
    [DllImport("AVProQuickTime")]
    public static extern float GetLoadedFraction(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetNextPosition(int handle);
    [DllImport("AVProQuickTime")]
    public static extern uint GetNumFramesDrawn(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetPlaybackRate(int handle);
    [DllImport("AVProQuickTime")]
    public static extern float GetPluginVersion();
    [DllImport("AVProQuickTime")]
    public static extern int GetWidth(int handle);
    [DllImport("AVProQuickTime")]
    public static extern bool Init();
    [DllImport("AVProQuickTime")]
    public static extern bool IsMovieLoadable(int handle);
    [DllImport("AVProQuickTime")]
    public static extern bool IsMoviePropertiesLoaded(int handle);
    [DllImport("AVProQuickTime")]
    public static extern bool LoadMovieFromFile(int handle, IntPtr filename, bool loop, bool isYUV);
    [DllImport("AVProQuickTime")]
    public static extern bool LoadMovieFromMemory(int handle, IntPtr buffer, uint bufferSize, bool loop, bool isYUV);
    [DllImport("AVProQuickTime")]
    public static extern bool LoadMovieFromURL(int handle, [MarshalAs(UnmanagedType.LPStr)] string filenameURL, bool loop, bool isYUV);
    [DllImport("AVProQuickTime")]
    public static extern bool LoadMovieProperties(int handle);
    [DllImport("AVProQuickTime")]
    public static extern void Play(int handle);
    [DllImport("AVProQuickTime")]
    public static extern void SeekFrame(int handle, uint frame);
    [DllImport("AVProQuickTime")]
    public static extern void SeekSeconds(int handle, float position);
    [DllImport("AVProQuickTime")]
    public static extern void SeekUnit(int handle, float position);
    [DllImport("AVProQuickTime")]
    public static extern void SetPlaybackRate(int handle, float rate);
    [DllImport("AVProQuickTime")]
    public static extern void SetVolume(int handle, float volume);
    [DllImport("AVProQuickTime")]
    public static extern void Stop(int handle);
    [DllImport("AVProQuickTime")]
    public static extern bool Update(int handle);
    [DllImport("AVProQuickTime")]
    public static extern bool UpdateTextureGL(int handle, int textureID);
}