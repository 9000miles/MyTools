using UnityEngine;
using System;
using System.Text;

//-----------------------------------------------------------------------------
// Copyright 2012 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro QuickTime/Manager (required)")]
public class AVProQuickTimeManager : MonoBehaviour
{
	private static AVProQuickTimeManager _instance;
	
	private bool _isOpenGL = false;
	
	// Format conversion
	private Material _mat;
	private Material _matYUV2;
	private Material _matYUV2_709;
	private Shader _shader;
	private Shader _shaderYUV2;
	private Shader _shaderYUV2_709;
	
	//-----------------------------------------------------------------------------
	
	public static AVProQuickTimeManager Instance  
	{
		get
		{
			if (_instance != null)
				return _instance;
			
			Debug.LogError("[AVProQuickTime] Trying to use component before it has started or after it has been destroyed.");
			
			return null;
		}
	}
	
	public bool IsOpenGL
	{
		get { return _isOpenGL; }
	}
	
	//-------------------------------------------------------------------------
	
	void Start()
	{
		// Make sure there's only a single instance of this component
		if (_instance != null)
		{
			Destroy(this);
			return;
		}
		
		_instance = this;
		if (!Init())
		{
			Deinit();
			this.enabled = false;
		}
	}
	
	void OnDestroy()
	{
		Deinit();
	}	
	
	protected bool Init()
	{
		try
		{
			if (!AVProQuickTimePlugin.Init())
			{
				Debug.LogError("[AVProQuickTime] failed to initialise.  Check QuickTime is installed.");
				return false;
			}
		}
		catch (DllNotFoundException e)
		{
			Debug.Log("Unity couldn't find the DLL, did you move the 'Plugins' folder to the root of your project?");
			throw e;
		}
		if (!LoadShader("Hidden/AVProQuickTime_CompositeSwapChannelsRB", out _shader))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProQuickTime_CompositeYUV2RGB", out _shaderYUV2))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProQuickTime_CompositeYUV2709RGB", out _shaderYUV2_709))
		{
			return false;
		}
		
		_mat = new Material(_shader);
		_matYUV2 = new Material(_shaderYUV2);
		_matYUV2_709 = new Material(_shaderYUV2_709);

		_isOpenGL = SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
	
		return true;
	}
	
	public void Deinit()
	{
		Material.Destroy(_matYUV2_709);
		Material.Destroy(_matYUV2);
		Material.Destroy(_mat);
		
		_matYUV2_709 = null;
		_matYUV2 = null;
		_mat = null;
		
		_shaderYUV2_709 = null;
		_shaderYUV2 = null;
		_shader = null;

		// Clean up any open movies
		AVProQuickTimeMovie[] movies = (AVProQuickTimeMovie[])FindObjectsOfType(typeof(AVProQuickTimeMovie));
		if (movies != null && movies.Length > 0)
		{
			for (int i = 0; i < movies.Length; i++)
			{
				movies[i].UnloadMovie();
			}
		}

		_instance = null;
		
		AVProQuickTimePlugin.Deinit();
	}
	
	public Material GetConversionMaterial(AVProQuickTimePlugin.PixelFormat format)
	{
		Material result = null;
		switch (format)
		{
		case AVProQuickTimePlugin.PixelFormat.RGBA32:
			result = _mat;
			break;
		case AVProQuickTimePlugin.PixelFormat.YCbCr_SD:
			result = _matYUV2;
			break;
		case AVProQuickTimePlugin.PixelFormat.YCbCr_HD:
			result = _matYUV2_709;
			break;
		default:
			Debug.LogError("[AVProQuickTime] Unknown video format '" + format);
			break;
		}
		return result;
	}

	//-----------------------------------------------------------------------------
	
	private static bool LoadShader(string name, out Shader result)
	{
		result = Shader.Find(name);
		if (!result || !result.isSupported)
		{
			result = null;
			Debug.LogError("[AVProQuickTime] shader '" + name + "' not found or supported");
		}
		
		return (result != null);
	}	
}