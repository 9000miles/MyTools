using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AVProQuickTimePlayVideoDemo : MonoBehaviour
{
	public AVProQuickTimeMovie _movie;
	public AVProQuickTimeGUIDisplay _display;
	
	private int _focusedWindow;
	
	public void Update()
	{
		_focusedWindow = FocusedWindow;
	}	
	
	static public int FocusedWindow
	{
		get
		{
			System.Reflection.FieldInfo field =	typeof(GUI).GetField("focusedWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			return (int)field.GetValue(null);
		}
	}	

	public void OnGUI()
	{
		if (_focusedWindow != 0)
		{
			GUI.Window(0, new Rect(0, 0, 660, 48), ControlWindow, "Demo Controls");
		}
		else
		{
			GUI.Window(0, new Rect(0, 0, 660, 280), ControlWindow, "Demo Controls");
		}
	}

	private void ControlWindow(int id)
	{
		if (_focusedWindow != 0)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("Click to open...");
			GUILayout.EndVertical();
			return;
		}
		
		if (_movie == null)
			return;
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Folder: ", GUILayout.Width(48));
		_movie._folder = GUILayout.TextField(_movie._folder, 128);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("File: ");
		_movie._filename = GUILayout.TextField(_movie._filename, 128, GUILayout.Width(350));
		if (GUILayout.Button("Load File", GUILayout.Width(75)))
		{
			_movie._source = AVProQuickTimePlugin.MovieSource.LocalFile;
			Debug.Log("" + _movie._colourFormat.ToString());
			_movie.LoadMovie();
		}
		if (GUILayout.Button("Load URL", GUILayout.Width(75)))
		{
			_movie._source = AVProQuickTimePlugin.MovieSource.URL;
			_movie.LoadMovie();
		}
		if (GUILayout.Button("Load Memory", GUILayout.Width(100)))
		{
			_movie._source = AVProQuickTimePlugin.MovieSource.Memory;
			if (System.IO.File.Exists(_movie._filename))
			{
				_movie._movieData = System.IO.File.ReadAllBytes(_movie._filename);
				_movie.LoadMovie();
			}
		}
		GUILayout.EndHorizontal();
		
		if (_display != null)
		{
			_display._alphaBlend = GUILayout.Toggle(_display._alphaBlend, "Use Transparency (requires reload)");
			if (_display._alphaBlend)
			{
				_movie._colourFormat = AVProQuickTimePlugin.PixelFormat.RGBA32;
			}
			else
			{
				_movie._colourFormat = AVProQuickTimePlugin.PixelFormat.YCbCr_HD;
			}
		}
		
		AVProQuickTime moviePlayer = _movie.MovieInstance;
		if (moviePlayer != null)
		{	
			GUILayout.BeginHorizontal();
			GUILayout.Label("Loaded ", GUILayout.Width(80));
			GUILayout.HorizontalSlider(moviePlayer.LoadedSeconds, 0.0f, moviePlayer.DurationSeconds, GUILayout.Width(200));
			if (moviePlayer.DurationSeconds > 0f)
				GUILayout.Label(((moviePlayer.LoadedSeconds * 100f) / moviePlayer.DurationSeconds) + "%");
			else
				GUILayout.Label("0%");
			GUILayout.EndHorizontal();

			
			if (moviePlayer.LoadedSeconds > 0f)
			{
				GUILayout.Label("Resolution: " + moviePlayer.Width + "x" + moviePlayer.Height + " @ " + moviePlayer.FrameRate.ToString("F2") + "hz");
			
			
				GUILayout.BeginHorizontal();
				GUILayout.Label("Volume ", GUILayout.Width(80));
				float volume = _movie._volume;
				float newVolume = GUILayout.HorizontalSlider(volume, 0.0f, 1.0f, GUILayout.Width(200));
				if (volume != newVolume)
				{
					_movie._volume = newVolume;
				}
				GUILayout.Label(_movie._volume.ToString("F1"));
				GUILayout.EndHorizontal();
				
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Time ", GUILayout.Width(80));
				float position = moviePlayer.PositionSeconds;
				float newPosition = GUILayout.HorizontalSlider(position, 0.0f, moviePlayer.DurationSeconds, GUILayout.Width(200));
				if (position != newPosition)
				{
					moviePlayer.PositionSeconds = newPosition;
					moviePlayer.Play();
					moviePlayer.Update(true);
				}
				GUILayout.Label(moviePlayer.PositionSeconds.ToString("F1") + " / " + moviePlayer.DurationSeconds.ToString("F1") + "s");
				
				if (GUILayout.Button("Play"))
				{
					moviePlayer.Play();
				}
				if (GUILayout.Button("Pause"))
				{
					moviePlayer.Pause();
				}
			
				GUILayout.EndHorizontal();
				
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Frame " + moviePlayer.Frame.ToString() + " / " + moviePlayer.FrameCount.ToString());
		
				if (GUILayout.Button("<", GUILayout.Width(50)))
				{
					moviePlayer.Pause();
					if (moviePlayer.Frame > 0)
						moviePlayer.Frame--;
				}
				if (GUILayout.Button(">", GUILayout.Width(50)))
				{
					moviePlayer.Pause();
					moviePlayer.Frame++;
				}
				
				GUILayout.EndHorizontal();
				
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Rate: " + moviePlayer.PlaybackRate.ToString("F2") + "x");
				if (GUILayout.Button("-", GUILayout.Width(50)))
				{
					moviePlayer.PlaybackRate = moviePlayer.PlaybackRate - 1.0f;
				}
		
				if (GUILayout.Button("+", GUILayout.Width(50)))
				{
					moviePlayer.PlaybackRate = moviePlayer.PlaybackRate + 1.0f;
				}
				GUILayout.EndHorizontal();
			}
		}
		
		
		GUILayout.Space(10.0f);
		if (GUILayout.Button("Quit"))
		{
			Application.Quit();
		}		

		GUILayout.EndVertical();
	}
}
