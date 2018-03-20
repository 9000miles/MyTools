using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaiduARInternal{

	public class BaiduARBuild{
		
	private static BaiduARBuild instance;

	public static BaiduARBuild Instance{
			get{
				if (instance == null) {
					instance = new BaiduARBuild ();
				}
				return instance;
			}
		}

	#if UNITY_ANDROID
		private ARGlobalDefs.PLATFORM platform = ARGlobalDefs.PLATFORM.ANDROID;
	#elif UNITY_EDITOR
		private ARGlobalDefs.PLATFORM platform = ARGlobalDefs.PLATFORM.UNITY_EDITOR;
	#else
		private ARGlobalDefs.PLATFORM platform = ARGlobalDefs.PLATFORM.IOS;
	#endif
	// Use this for initialization
	public void Start () {
		ARGlobalDefs.platform = platform;	
	}
	
}
}
