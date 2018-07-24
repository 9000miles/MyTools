using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

public class AVProQuickTimeFormatConverter : System.IDisposable
{
	private int _movieHandle;
	
	// Format conversion and texture output
	private Texture2D _texture;
	private RenderTexture _target;
	private Material _conversionMaterial;
	
	// For DirectX texture updates
	private GCHandle _frameHandle;
	private Color32[] _frameData;
	
	// Conversion params
	private int _width;
	private int _height;
	private bool _flipX;
	private bool _flipY;
	private AVProQuickTimePlugin.PixelFormat _sourceVideoFormat;
	
	public Texture OutputTexture
	{
		get { return _target; }
	}

	public bool	ValidPicture { get; private set; }

	public bool Build(int movieHandle, int width, int height, AVProQuickTimePlugin.PixelFormat format, bool flipX, bool flipY)
	{
		ValidPicture = false;
		_movieHandle = movieHandle;
		
		_width = width;
		_height = height;
		_sourceVideoFormat = format;
		_flipX = flipX;
		_flipY = flipY;

		_conversionMaterial = AVProQuickTimeManager.Instance.GetConversionMaterial(_sourceVideoFormat);
		if (_conversionMaterial == null)
			return false;

		CreateTexture();
		CreateRenderTexture();
		CreateBuffer();
		
		return true;
	}
	
	public bool Update()
	{
		bool result = UpdateTexture();
		if (result)
		{
			DoFormatConversion();
		}
		return result;
	}
	
	private bool UpdateTexture()
	{
		bool result = false;
		if (AVProQuickTimeManager.Instance.IsOpenGL)
		{
			// OpenGL
#if UNITY_3_5
			GL.IssuePluginEvent( (_texture.GetNativeTextureID() << 8) | (_movieHandle&255) );
			result = true;
#else
			result = AVProQuickTimePlugin.UpdateTextureGL(_movieHandle, _texture.GetNativeTextureID());
#endif			
			GL.InvalidateState();
		}
		else
		{
			// Non-OpenGL
			bool formatIs422 = (_sourceVideoFormat != AVProQuickTimePlugin.PixelFormat.RGBA32);
			if (formatIs422)
			{
				result = AVProQuickTimePlugin.GetFramePixelsYUV2(_movieHandle, _frameHandle.AddrOfPinnedObject(), _texture.width, _texture.height);
			}
			else
			{
				result = AVProQuickTimePlugin.GetFramePixelsRGBA32(_movieHandle, _frameHandle.AddrOfPinnedObject(), _texture.width, _texture.height);
			}
			
			if (result)
			{
				_texture.SetPixels32(_frameData);
				_texture.Apply(false, false);
			}
		}
		
		return result;
	}

	public void Dispose()
	{
		_width = _height = 0;
		if (_conversionMaterial != null)
		{
			_conversionMaterial.SetTexture("_MainTex", null);
			_conversionMaterial = null;
		}
		
		if (_target != null)
		{
			_target.Release();
			RenderTexture.Destroy(_target);
			_target = null;
		}
		
		if (_texture != null)
		{
			Texture2D.Destroy(_texture);
			_texture = null;
		}
		
		if (_frameHandle.IsAllocated)
		{
			_frameHandle.Free();
			_frameData = null;
		}
	}

	private void CreateTexture()
	{
		// Calculate texture size
		int textureWidth = _width;
		int textureHeight = _height;
		bool formatIs422 = (_sourceVideoFormat != AVProQuickTimePlugin.PixelFormat.RGBA32);
		if (formatIs422)
			textureWidth /= 2;	// YCbCr422 modes need half width
		
		// If the texture isn't a power of 2
		if (!Mathf.IsPowerOfTwo(_width) || !Mathf.IsPowerOfTwo(_height))
		{
			// If we're running in DirectX mode or our format is YCbCr422
			//if (!AVProQuickTimeManager.Instance.IsOpenGL || formatIs422)
			{
				// We use a power of 2 texture as Unity is MUCH faster at updating these
				textureWidth = Mathf.NextPowerOfTwo(textureWidth);
				textureHeight = Mathf.NextPowerOfTwo(textureHeight);
				Debug.Log("[AVProQuickTime] Using texture " + textureWidth + "x" + textureHeight + " for stream " + _width + "x" + _height + " " + _sourceVideoFormat.ToString());
			}
		}
		
		// Create texture that stores the initial raw frame
		// If there is already a texture, only destroy it if it isn't of desired size
		if (_texture != null)
		{
			if (_texture.width != textureWidth || _texture.height != textureHeight)
			{
				Texture2D.Destroy(_texture);
				_texture = null;
			}
		}
		if (_texture == null)
		{
			_texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
			_texture.hideFlags = HideFlags.HideAndDontSave;
			_texture.wrapMode = TextureWrapMode.Clamp;
			_texture.filterMode = FilterMode.Point;
			_texture.anisoLevel = 0;
			_texture.Apply(false, false);
		}	
	}
	
	private void CreateRenderTexture()
	{
		// Create RenderTexture for post transformed frames
		// If there is already a renderTexture, only destroy it smaller than desired size
		if (_target != null)
		{
			if (_target.width != _width || _target.height != _height)
			{
				_target.Release();
				RenderTexture.Destroy(_target);
				_target = null;
			}
		}
		
		if (_target == null)
		{
			ValidPicture = false;
			_target = new RenderTexture(_width, _height, 0);
			_target.wrapMode = TextureWrapMode.Clamp;
			_target.useMipMap = false;
			_target.hideFlags = HideFlags.HideAndDontSave;
			_target.format = RenderTextureFormat.ARGB32;
			_target.filterMode = FilterMode.Bilinear;
			_target.Create();
		}
	}
	
	private void CreateBuffer()
	{
		// Allocate buffer for non-opengl updates
		if (!AVProQuickTimeManager.Instance.IsOpenGL)
		{
			if (_frameHandle.IsAllocated && _frameData != null)
			{
				if (_frameData.Length < _texture.width * _texture.height)
				{
					_frameHandle.Free();
					_frameData = null;
				}
			}
			if (_frameData == null)
			{
				_frameData = new Color32[_texture.width * _texture.height];
				_frameHandle = GCHandle.Alloc(_frameData, GCHandleType.Pinned);
			}	
		}
	}
	
	private void DoFormatConversion()
	{
		if (_texture == null)
			return;
		
		RenderTexture prev = RenderTexture.active;
		RenderTexture.active = _target;

		bool formatIs422 = (_sourceVideoFormat != AVProQuickTimePlugin.PixelFormat.RGBA32);
		if (formatIs422)
		{
			_conversionMaterial.SetFloat("_TextureWidth", _texture.width);
		}
		
		_conversionMaterial.SetTexture("_MainTex", _texture);
		_conversionMaterial.SetPass(0);

		GL.PushMatrix();
		GL.LoadOrtho();
		DrawQuad(_flipX, _flipY);
		GL.PopMatrix();
		
		RenderTexture.active = prev;
		ValidPicture = true;
	}
	
	private void DrawQuad(bool invertX, bool invertY)
	{
		GL.Begin(GL.QUADS);
		float x1, x2;
		float y1, y2;
		if (invertX)
		{
			x1 = 1.0f; x2 = 0.0f;
		}
		else
		{
			x1 = 0.0f; x2 = 1.0f;
		}
		if (invertY)
		{
			y1 = 1.0f; y2 = 0.0f;
		}
		else
		{
			y1 = 0.0f; y2 = 1.0f;
		}
		
		// Alter UVs if we're only using a portion of the texture
		if (_width != _texture.width)
		{
			float xd = _width / (float)_texture.width;
			x1 *= xd; x2 *= xd;
		}
		if (_height != _texture.height)
		{
			float yd = _height / (float)_texture.height;
			y1 *= yd; y2 *= yd;
		}

		GL.TexCoord2(x1, y1); GL.Vertex3(0.0f, 0.0f, 0.1f);
		GL.TexCoord2(x2, y1); GL.Vertex3(1.0f, 0.0f, 0.1f);
		GL.TexCoord2(x2, y2); GL.Vertex3(1.0f, 1.0f, 0.1f);
		GL.TexCoord2(x1, y2); GL.Vertex3(0.0f, 1.0f, 0.1f);
		GL.End();
	}	
}