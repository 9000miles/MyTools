using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class AVProQuickTime : IDisposable
{
    private AVProQuickTimeFormatConverter _formatConverter;
    private uint _lastFrameDrawn;
    private int _movieHandle = -1;
    private GCHandle _movieMemoryHandle;
    private uint _movieMemoryLength;
    private IntPtr _movieMemoryPtr;
    private AVProQuickTimePlugin.MovieSource _movieSource;
    private UpdateMode _updateMode;
    private float _volume = 1f;

    public void Close()
    {
        Pause();
        IsVisual = false;
        int num = 0;
        Height = num;
        Width = num;
        IsPrepared = false;
        IsPaused = true;
        IsPlaying = false;
        PlayState = PlaybackState.Unknown;
        if (_movieMemoryHandle.IsAllocated)
        {
            _movieMemoryHandle.Free();
        }
        _movieMemoryPtr = IntPtr.Zero;
        _movieMemoryLength = 0;
        if (_movieHandle >= 0)
        {
            AVProQuickTimePlugin.FreeInstanceHandle(_movieHandle);
            _movieHandle = -1;
        }
    }

    public void Dispose()
    {
        Close();
        if (_formatConverter != null)
        {
            _formatConverter.Dispose();
            _formatConverter = null;
        }
    }

    public void Pause()
    {
        if ((_movieHandle >= 0) && !IsPaused)
        {
            AVProQuickTimePlugin.Stop(_movieHandle);
            IsPaused = true;
            PlayState = PlaybackState.Stopped;
            IsPlaying = false;
        }
    }

    public void Play()
    {
        if ((_movieHandle >= 0) && IsPaused)
        {
            AVProQuickTimePlugin.Play(_movieHandle);
            IsPaused = false;
            PlayState = PlaybackState.Playing;
            IsPlaying = true;
        }
    }

    private bool PrepareMovie()
    {
        int num;
        if (!AVProQuickTimePlugin.LoadMovieProperties(_movieHandle))
        {
            Debug.LogWarning("[AVProQuickTime] Failed loading movie properties");
            Close();
            return false;
        }
        AVProQuickTimePlugin.SetVolume(_movieHandle, _volume);
        Width = AVProQuickTimePlugin.GetWidth(_movieHandle);
        Height = AVProQuickTimePlugin.GetHeight(_movieHandle);
        FrameCount = (uint) AVProQuickTimePlugin.GetFrameCount(_movieHandle);
        DurationSeconds = AVProQuickTimePlugin.GetDurationSeconds(_movieHandle);
        FrameRate = AVProQuickTimePlugin.GetFrameRate(_movieHandle);
        IsPrepared = true;
        PlayState = PlaybackState.Loaded;
        Debug.Log(string.Concat(new object[] { "[AVProQuickTime] loaded movie ", Filename, "[", Width, "x", Height, " @ ", FrameRate, "hz] ", DurationSeconds, " sec ", FrameCount, " frames" }));
        if ((Width > 0) && (Height > 0))
        {
            if ((Width <= 0x1000) && (Height <= 0x1000))
            {
                IsVisual = true;
                if (_formatConverter == null)
                {
                    _formatConverter = new AVProQuickTimeFormatConverter();
                }
                if (_updateMode == UpdateMode.UnityTexture)
                {
                    if (!_formatConverter.Build(_movieHandle, Width, Height, PixelFormat, false, true))
                    {
                        Debug.LogWarning("[AVProQuickTime] unable to convert camera format");
                        num = 0;
                        Height = num;
                        Width = num;
                        if (_formatConverter != null)
                        {
                            _formatConverter.Dispose();
                            _formatConverter = null;
                        }
                    }
                }
                else if (_formatConverter != null)
                {
                    _formatConverter.Dispose();
                    _formatConverter = null;
                }
            }
            else
            {
                Debug.LogError("[AVProQuickTime] Movie resolution is too large");
                num = 0;
                Height = num;
                Width = num;
                if (_formatConverter != null)
                {
                    _formatConverter.Dispose();
                    _formatConverter = null;
                }
            }
        }
        else
        {
            Debug.LogError("[AVProQuickTime] Movie resolution is too small");
            num = 0;
            Height = num;
            Width = num;
            if (_formatConverter != null)
            {
                _formatConverter.Dispose();
                _formatConverter = null;
            }
        }
        return true;
    }

    public bool StartFromFile(string filename, bool loop, AVProQuickTimePlugin.PixelFormat format, UpdateMode updateMode)
    {
        Close();
        Filename = filename.Trim();
        _movieSource = AVProQuickTimePlugin.MovieSource.LocalFile;
        _updateMode = updateMode;
        return StartMovie(loop, format);
    }

    public bool StartFromMemory(byte[] movieData, string filename, bool loop, AVProQuickTimePlugin.PixelFormat format, UpdateMode updateMode)
    {
        Close();
        if ((movieData == null) || (movieData.Length < 8))
        {
            return false;
        }
        Filename = filename.Trim();
        _movieSource = AVProQuickTimePlugin.MovieSource.Memory;
        _movieMemoryHandle = GCHandle.Alloc(movieData, GCHandleType.Pinned);
        _movieMemoryPtr = _movieMemoryHandle.AddrOfPinnedObject();
        _movieMemoryLength = (uint) movieData.Length;
        _updateMode = updateMode;
        return StartMovie(loop, format);
    }

    public bool StartFromURL(string url, bool loop, AVProQuickTimePlugin.PixelFormat format, UpdateMode updateMode)
    {
        Close();
        Filename = url.Trim();
        _movieSource = AVProQuickTimePlugin.MovieSource.URL;
        _updateMode = updateMode;
        return StartMovie(loop, format);
    }

    private bool StartMovie(bool loop, AVProQuickTimePlugin.PixelFormat format)
    {
        Loop = loop;
        PixelFormat = format;
        if (_movieHandle < 0)
        {
            _movieHandle = AVProQuickTimePlugin.GetInstanceHandle();
        }
        bool isYUV = PixelFormat != AVProQuickTimePlugin.PixelFormat.RGBA32;
        bool flag2 = false;
        bool flag3 = false;
        switch (_movieSource)
        {
            case AVProQuickTimePlugin.MovieSource.LocalFile:
                if (!string.IsNullOrEmpty(Filename))
                {
                    flag2 = true;
                    byte[] bytes = Encoding.UTF8.GetBytes(Filename);
                    int cb = Marshal.SizeOf(typeof(byte)) * (bytes.Length + 1);
                    IntPtr destination = Marshal.AllocHGlobal(cb);
                    Marshal.Copy(bytes, 0, destination, bytes.Length);
                    Marshal.WriteByte(destination, bytes.Length, 0);
                    flag3 = AVProQuickTimePlugin.LoadMovieFromFile(_movieHandle, destination, Loop, isYUV);
                    Marshal.FreeHGlobal(destination);
                }
                break;

            case AVProQuickTimePlugin.MovieSource.URL:
                if (!string.IsNullOrEmpty(Filename))
                {
                    flag2 = true;
                    flag3 = AVProQuickTimePlugin.LoadMovieFromURL(_movieHandle, Filename, Loop, isYUV);
                }
                break;

            case AVProQuickTimePlugin.MovieSource.Memory:
                flag2 = true;
                flag3 = AVProQuickTimePlugin.LoadMovieFromMemory(_movieHandle, _movieMemoryPtr, _movieMemoryLength, Loop, isYUV);
                break;
        }
        if (flag2)
        {
            if (flag3)
            {
                PlayState = PlaybackState.Loading;
            }
            else
            {
                Debug.LogWarning("[AVProQuickTime] Movie failed to load");
                Close();
            }
        }
        else
        {
            Debug.LogWarning("[AVProQuickTime] Invalid movie file specified");
            Close();
        }
        return (_movieHandle >= 0);
    }

    public bool Update(bool force)
    {
        bool flag = false;
        if (_movieHandle >= 0)
        {
            if (!AVProQuickTimePlugin.IsMovieLoadable(_movieHandle))
            {
                Debug.LogWarning("[AVProQuickTime] Unable to load movie: " + Filename);
                Close();
                return false;
            }
            if (IsPrepared)
            {
                AVProQuickTimePlugin.Update(_movieHandle);
                uint numFramesDrawn = AVProQuickTimePlugin.GetNumFramesDrawn(_movieHandle);
                bool flag2 = true;
                if (!force && IsVisual)
                {
                }
                if (_lastFrameDrawn != numFramesDrawn)
                {
                    flag2 = true;
                    _lastFrameDrawn = numFramesDrawn;
                }
                if (!flag2 || !IsVisual)
                {
                    return flag;
                }
                if (_updateMode == UpdateMode.UnityTexture)
                {
                    if (_formatConverter == null)
                    {
                        return flag;
                    }
                    _formatConverter.Update();
                    return true;
                }
                return true;
            }
            if (AVProQuickTimePlugin.IsMoviePropertiesLoaded(_movieHandle))
            {
                PrepareMovie();
            }
        }
        return flag;
    }

    public float AspectRatio
    {
        get
        {
            return (((float) Width) / ((float) Height));
        }
    }

    public float DurationSeconds { get; private set; }

    public string Filename { get; private set; }

    public uint Frame
    {
        get
        {
            return AVProQuickTimePlugin.GetCurrentFrame(_movieHandle);
        }
        set
        {
            AVProQuickTimePlugin.SeekFrame(_movieHandle, value);
        }
    }

    public uint FrameCount { get; private set; }

    public float FrameRate { get; private set; }

    public int Handle
    {
        get
        {
            return _movieHandle;
        }
    }

    public int Height { get; private set; }

    public bool IsPaused { get; private set; }

    public bool IsPlaying { get; private set; }

    protected bool IsPrepared { get; private set; }

    private bool IsVisual { get; set; }

    public float LoadedSeconds
    {
        get
        {
            return (AVProQuickTimePlugin.GetLoadedFraction(_movieHandle) * DurationSeconds);
        }
    }

    public bool Loop { get; private set; }

    public Texture OutputTexture
    {
        get
        {
            if ((_formatConverter != null) && _formatConverter.ValidPicture)
            {
                return _formatConverter.OutputTexture;
            }
            return null;
        }
    }

    protected AVProQuickTimePlugin.PixelFormat PixelFormat { get; private set; }

    public float PlaybackRate
    {
        get
        {
            return AVProQuickTimePlugin.GetPlaybackRate(_movieHandle);
        }
        set
        {
            AVProQuickTimePlugin.SetPlaybackRate(_movieHandle, value);
        }
    }

    public PlaybackState PlayState { get; private set; }

    public float PositionSeconds
    {
        get
        {
            return AVProQuickTimePlugin.GetCurrentPositionSeconds(_movieHandle);
        }
        set
        {
            if (value <= LoadedSeconds)
            {
                AVProQuickTimePlugin.SeekSeconds(_movieHandle, value);
            }
        }
    }

    public float Volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
            AVProQuickTimePlugin.SetVolume(_movieHandle, _volume);
        }
    }

    public int Width { get; private set; }

    public enum PlaybackState
    {
        Unknown,
        Loading,
        Loaded,
        Playing,
        Stopped
    }

    public enum UpdateMode
    {
        None,
        UnityTexture,
        Overlay
    }
}

