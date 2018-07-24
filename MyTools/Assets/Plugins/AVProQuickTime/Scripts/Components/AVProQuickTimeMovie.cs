using UnityEngine;
using System;
using System.IO;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro QuickTime/Movie")]
public class AVProQuickTimeMovie : MonoBehaviour
{
	protected AVProQuickTime _moviePlayer;
	public AVProQuickTimePlugin.MovieSource _source = AVProQuickTimePlugin.MovieSource.LocalFile;
	public string _folder = "";
	public string _filename = "movie.mov";
	public bool _loop = false;
	public AVProQuickTimePlugin.PixelFormat _colourFormat = AVProQuickTimePlugin.PixelFormat.YCbCr_HD;
	public bool _loadOnStart = true;
	public bool _playOnStart = true;
	public bool _loadFirstFrame = true;
	public bool _editorPreview = false;
	public AVProQuickTime.UpdateMode _updateMode = AVProQuickTime.UpdateMode.UnityTexture;
	public float _volume = 1.0f;
	
	[NonSerializedAttribute]
	public byte[] _movieData;

	public Texture OutputTexture
	{
		get { if (_moviePlayer != null) return _moviePlayer.OutputTexture; return null; }
	}

	public AVProQuickTime MovieInstance
	{
		get { return _moviePlayer; }
	}

	public void Start()
	{
		if (null == FindObjectOfType(typeof(AVProQuickTimeManager)))
		{
			throw new Exception("You need to add AVProQuickTimeManager component to your scene.");
		}
		
		if (_loadOnStart)
		{
			LoadMovie();
		}
	}

	public void LoadMovie()
	{
		if (_moviePlayer == null)
			_moviePlayer = new AVProQuickTime();

		bool loaded = false;
		switch (_source)
		{
			case AVProQuickTimePlugin.MovieSource.LocalFile:
				loaded = _moviePlayer.StartFromFile(Path.Combine(_folder, _filename), _loop, _colourFormat, _updateMode);
				break;
			case AVProQuickTimePlugin.MovieSource.URL:
				loaded = _moviePlayer.StartFromURL(Path.Combine(_folder, _filename), _loop, _colourFormat, _updateMode);
				break;
			case AVProQuickTimePlugin.MovieSource.Memory:
				if (_movieData != null)
				{
					loaded = _moviePlayer.StartFromMemory(_movieData, _filename, _loop, _colourFormat, _updateMode);
				}
				break;
		}

		if (loaded)
		{
			_moviePlayer.Volume = _volume;
		}
		else
		{
			Debug.LogWarning("[AVProQuickTime] Couldn't load movie " + _filename);
			UnloadMovie();
		}
	}

	public void Update()
	{
		_volume = Mathf.Clamp01(_volume);

		if (_moviePlayer != null)
		{
			if (_volume != _moviePlayer.Volume)
				_moviePlayer.Volume = _volume;
			
			if (!_moviePlayer.IsPlaying)
			{
				if (_loadFirstFrame)
				{
					if (_moviePlayer.PlayState == AVProQuickTime.PlaybackState.Loaded)
					{
						_moviePlayer.Frame = 0;
						_loadFirstFrame = false;
					}
				}
				if (_playOnStart)
				{
					if ((int)_moviePlayer.PlayState >= (int)AVProQuickTime.PlaybackState.Loaded && _moviePlayer.LoadedSeconds > 0f)
					{
						_moviePlayer.Play();
						_playOnStart = false;
					}
				}			
			}
			
			_moviePlayer.Update(false);
		}
	}

	public void Play()
	{
		if (_moviePlayer != null)
			_moviePlayer.Play();
	}

	public void Pause()
	{
		if (_moviePlayer != null)
			_moviePlayer.Pause();
	}	

	public void UnloadMovie()
	{
		if (_moviePlayer != null)
		{
			_moviePlayer.Dispose();
			_moviePlayer = null;
		}
	}

	public void OnDestroy()
	{
		UnloadMovie();
	}
}